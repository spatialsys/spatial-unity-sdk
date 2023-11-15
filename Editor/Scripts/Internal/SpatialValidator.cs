using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using RSG;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// From where was the currently running validation initiated?
    /// </summary>
    public enum ValidationRunContext
    {
        UploadingToSandbox,
        PublishingPackage,
        ManualRun
    }

    public static class SpatialValidator
    {
        public static List<SpatialTestResponse> allResponses { get; private set; } = new List<SpatialTestResponse>();
        public static ValidationRunContext runContext { get; private set; } = ValidationRunContext.ManualRun;

        private static bool _initialized;
        private static Dictionary<Type, List<MethodInfo>> _componentTests;
        private static List<MethodInfo> _sceneTests;
        private static List<MethodInfo> _packageTests;

        private static Scene? _currentTestScene = null;

        /// <summary>
        /// Returns a summary of the run, otherwise null if tests failed to run.
        /// </summary>
        public static IPromise<SpatialValidationSummary> RunTestsOnPackage(ValidationRunContext context)
        {
            PackageConfig config = ProjectConfig.activePackageConfig;
            if (config == null)
            {
                Debug.LogError("No config found.");
                return Promise<SpatialValidationSummary>.Resolved(null);
            }

            runContext = context;
            LoadTestsIfNecessary();
            allResponses.Clear();

            return RunPackageTests(config)
                .Then(() => {
                    if (config is SpaceConfig spaceConfig)
                    {
                        return RunTestsOnSpacePackageScenes(new SceneAsset[] { spaceConfig.scene });
                    }
                    else if (config is SpaceTemplateConfig spaceTemplateConfig)
                    {
                        return RunTestsOnSpacePackageScenes(spaceTemplateConfig.variants.Select(v => v.scene));
                    }
                    return Promise.Resolved();
                })
                .Catch(HandleInternalTestException)
                .Then(() => Promise<SpatialValidationSummary>.Resolved(CreateValidationSummary(config)));
        }

        /// <summary>
        /// Returns a summary of the run, otherwise null if tests failed to run.
        /// </summary>
        public static IPromise<SpatialValidationSummary> RunTestsOnActiveScene(ValidationRunContext context)
        {
            PackageConfig config = ProjectConfig.activePackageConfig;
            if (config == null)
            {
                Debug.LogError("No config found.");
                return Promise<SpatialValidationSummary>.Resolved(null);
            }

            runContext = context;
            LoadTestsIfNecessary();
            allResponses.Clear();

            return RunPackageTests(config)
                .Then(() => RunSceneTests(SceneManager.GetActiveScene()))
                .Catch(HandleInternalTestException)
                .Then(() => Promise<SpatialValidationSummary>.Resolved(CreateValidationSummary(config)));
        }

        public static IPromise<SpatialValidationSummary> RunTestsOnComponent(ValidationRunContext context, Component target)
        {
            runContext = context;
            LoadTestsIfNecessary();
            allResponses.Clear();

            return RunComponentTests(target)
                .Catch(HandleInternalTestException)
                .Then(() => Promise<SpatialValidationSummary>.Resolved(CreateValidationSummary(null)));
        }

        public static void AddResponse(SpatialTestResponse response)
        {
            if (_currentTestScene.HasValue)
                response.scenePath = _currentTestScene.Value.path;

            allResponses.Add(response);
        }

        public static void ClearResponses()
        {
            allResponses.Clear();
        }

        // ======================================================
        // INTERNAL HELPERS IMPLEMENTATION
        // ======================================================

        private static IPromise RunTestsOnSpacePackageScenes(IEnumerable<SceneAsset> sceneAssets)
        {
            if (!Application.isBatchMode)
                EditorSceneManager.SaveOpenScenes(); // We are going to swap scenes and will lose changes without this

            Scene previousScene = EditorSceneManager.GetActiveScene();
            string originalScenePath = previousScene.path;
            var testedScenePaths = new HashSet<string>();

            return BuildSequencePromise(sceneAssets, (SceneAsset sceneAsset) => {
                if (sceneAsset == null)
                    return Promise.Resolved();

                string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                if (!testedScenePaths.Add(scenePath))
                    return Promise.Resolved(); // We already tested this scene, although this shouldn't happen since each scene should be assigned to at most one variant.

                Scene scene;
                if (previousScene.path != scenePath)
                {
                    scene = EditorSceneManager.OpenScene(scenePath);
                }
                else
                {
                    scene = EditorSceneManager.GetActiveScene();
                }

                previousScene = scene;
                return RunSceneTests(scene);
            }).Then(() => {
                if (previousScene.path != originalScenePath && !string.IsNullOrEmpty(originalScenePath))
                    EditorSceneManager.OpenScene(originalScenePath);
            });
        }

        private static IPromise RunComponentTestsOnObjectRecursively(GameObject g)
        {
            // Ignore object and children if marked editor only
            if (g.CompareTag("EditorOnly"))
                return Promise.Resolved();

            return BuildParallelPromise(g.GetComponents<Component>(), component => {
                // Null components are missing scripts
                if (component == null)
                {
                    // We only need to warn on build machines, because we remove all null components in the SceneProcessor build step
                    var resp = new SpatialTestResponse(
                        g,
                        Application.isBatchMode ? TestResponseType.Warning : TestResponseType.Fail, // Don't fail on build machines
                        $"GameObject {g.GetPath()} has a missing script",
                        "Missing scripts should be removed so that they don't cause errors or unexpected behavior."
                    );
                    resp.SetAutoFix(true, "Remove component", (g) => {
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(g as GameObject);
                    });
                    SpatialValidator.AddResponse(resp);
                    return Promise.Resolved();
                }
                else
                {
                    return RunComponentTests(component);
                }
            })
            .Then(() => BuildParallelPromise(GetTransformChildrenAsList(g.transform), (Transform child) => RunComponentTestsOnObjectRecursively(child.gameObject)));
        }

        private static List<Transform> GetTransformChildrenAsList(Transform transform)
        {
            List<Transform> list = new List<Transform>(transform.childCount);
            foreach (Transform child in transform)
                list.Add(child);
            return list;
        }

        private static IPromise RunComponentTests(Component target)
        {
            object[] targetParam = new object[] { target };
            Type targetType = target.GetType();

            return BuildParallelPromise(_componentTests, (KeyValuePair<Type, List<MethodInfo>> pair) => {
                Type type = pair.Key;
                if (type.IsAssignableFrom(targetType))
                {
                    List<MethodInfo> tests = pair.Value;
                    return BuildParallelPromise(tests, (MethodInfo method) => Invoke(method, targetParam));
                }
                return Promise.Resolved();
            });
        }

        private static IPromise Invoke(MethodInfo method, object[] param)
        {
            try
            {
                if (method.ReturnType == typeof(IPromise))
                {
                    return (IPromise)method.Invoke(null, param);
                }
                else
                {
                    method.Invoke(null, param);
                    return Promise.Resolved();
                }
            }
            catch (Exception exc)
            {
                return Promise.Rejected(exc.InnerException ?? exc);
            }
        }

        private static IPromise BuildSequencePromise<T>(IEnumerable<T> list, Func<T, IPromise> selector)
        {
            var promiseCallbackList = list.Select(element => (Func<IPromise>)(() => selector(element)));
            return Promise.Sequence(promiseCallbackList);
        }

        private static IPromise BuildParallelPromise<T>(IEnumerable<T> list, Func<T, IPromise> selector)
        {
            var promiseCallbackList = list.Select(element => selector(element));
            return Promise.All(promiseCallbackList);
        }

        private static IPromise RunPackageTests(PackageConfig config)
        {
            _currentTestScene = null;
            object[] configParam = new object[] { config };

            return BuildParallelPromise(_packageTests,
                (MethodInfo method) => {
                    var attr = method.GetCustomAttribute<PackageTest>();
                    if (attr.TestAffectsPackageType(config.packageType))
                    {
                        return Invoke(method, configParam);
                    }
                    return Promise.Resolved();
                })
                .Then(() => BuildParallelPromise(config.gameObjectAssets, (GameObject go) => RunComponentTestsOnObjectRecursively(go)));
        }

        private static IPromise RunSceneTests(Scene scene)
        {
            _currentTestScene = scene;
            object[] sceneParam = new object[] { scene };
            return BuildParallelPromise(_sceneTests, (MethodInfo method) => Invoke(method, sceneParam))
                .Then(() => BuildParallelPromise(scene.GetRootGameObjects(), (GameObject go) => RunComponentTestsOnObjectRecursively(go)))
                .Finally(() => _currentTestScene = null);
        }

        private static void HandleInternalTestException(Exception exception)
        {
            Debug.LogException(exception);

            string responseDescription = exception.Message;
            const int MAX_STACKTRACE_LENGTH = 5000; // Including too many characters in output can cause GUI errors.
            if (exception.StackTrace.Length > MAX_STACKTRACE_LENGTH)
            {
                string truncatedStacktrace = exception.StackTrace.Substring(0, MAX_STACKTRACE_LENGTH);
                responseDescription += $"\n\n{truncatedStacktrace}\n...\n\nStacktrace truncated - Check the console window for the full stacktrace.";
            }
            else
            {
                responseDescription += $"\n\n{exception.StackTrace}";
            }

            AddResponse(new SpatialTestResponse(
                targetObject: null,
                TestResponseType.Fail,
                "An unhandled internal exception occurred while running validation tests",
                responseDescription
            ));
        }

        private static void LoadTestsIfNecessary()
        {
            if (_initialized)
            {
                return;
            }

            Assembly assembly = Assembly.GetAssembly(typeof(SpatialValidator));
            IEnumerable<MethodInfo> allPublicStaticMethods = assembly.GetExportedTypesCached()
                .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Where(method => !method.IsSpecialName); // exclude property getters/setters

            _componentTests = new Dictionary<Type, List<MethodInfo>>();
            _sceneTests = new List<MethodInfo>();
            _packageTests = new List<MethodInfo>();

            foreach (MethodInfo method in allPublicStaticMethods)
            {
                if (method.TryGetCustomAttribute<ComponentTest>(out var componentAttr))
                {
                    if (!method.ValidateParameterTypes(typeof(Component)))
                    {
                        Debug.LogError("Component test does not match signature: (Component). " + method.Name);
                        continue;
                    }
                    Type targetType = componentAttr.targetType ?? typeof(Component);
                    if (!_componentTests.ContainsKey(targetType))
                    {
                        _componentTests.Add(targetType, new List<MethodInfo>());
                    }
                    _componentTests[targetType].Add(method);
                }
                else if (method.TryGetCustomAttribute<SceneTest>(out var sceneAttr))
                {
                    if (!method.ValidateParameterTypes(typeof(Scene)))
                    {
                        Debug.LogError("Scene test does not match signature: (Scene). " + method.Name);
                        continue;
                    }
                    _sceneTests.Add(method);
                }
                else if (method.TryGetCustomAttribute<PackageTest>(out var packageAttr))
                {
                    if (!method.ValidateParameterTypes(typeof(PackageConfig)))
                    {
                        Debug.LogError("Package test does not match signature (PackageConfig). " + method.Name);
                        continue;
                    }
                    _packageTests.Add(method);
                }
            }

            _initialized = true;
        }

        private static SpatialValidationSummary CreateValidationSummary(PackageConfig targetPackage)
        {
            SpatialTestResponse[] warnings = allResponses
                .Where(resp => resp.responseType == TestResponseType.Warning)
                .ToArray();
            SpatialTestResponse[] errors = allResponses
                .Where(resp => resp.responseType == TestResponseType.Fail)
                .ToArray();

            return new SpatialValidationSummary() {
                targetPackage = targetPackage,
                warnings = warnings,
                errors = errors
            };
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ComponentTest : Attribute
    {
        public Type targetType;
        public ComponentTest(Type targetType)
        {
            this.targetType = targetType;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class SceneTest : Attribute { }
    [AttributeUsage(AttributeTargets.Method)]
    public class PackageTest : Attribute
    {
        public readonly PackageType[] targetTypes;

        /// <summary>
        /// Targets all package types.
        /// </summary>
        public PackageTest()
        {
            targetTypes = null;
        }
        public PackageTest(params PackageType[] targetTypes)
        {
            this.targetTypes = targetTypes;
        }

        public bool TestAffectsPackageType(PackageType type)
        {
            return targetTypes == null || targetTypes.Contains(type);
        }
    }
}
