using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// What is the context of the current validation run?
    /// </summary>
    public enum ValidationContext
    {
        Testing,
        Publishing,
        ManualRun
    }

    public static class SpatialValidator
    {
        public static List<SpatialTestResponse> allResponses { get; private set; } = new List<SpatialTestResponse>();
        public static ValidationContext validationContext { get; private set; } = ValidationContext.ManualRun;

        private static bool _initialized;
        private static Dictionary<Type, List<MethodInfo>> _componentTests;
        private static List<MethodInfo> _sceneTests;
        private static List<MethodInfo> _packageTests;

        private static Scene? _currentTestScene = null;

        /// <summary>
        /// Returns true if there's at least one failed test response. Warning responses are excluded.
        /// </summary>
        private static bool _hasFailedTestResponse
        {
            get
            {
                foreach (SpatialTestResponse response in allResponses)
                {
                    if (response.responseType == TestResponseType.Fail)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Returns true if no tests FAILED. There could be warnings though.
        /// </summary>
        public static bool RunTestsOnPackage(ValidationContext context)
        {
            PackageConfig config = ProjectConfig.activePackage;
            if (config == null)
            {
                Debug.LogError("No config found.");
                return false;
            }

            validationContext = context;
            LoadTestsIfNecessary();
            allResponses.Clear();

            RunPackageTests(config);

            if (config is EnvironmentConfig envConfig)
            {
                RunTestsOnEnvironmentPackage(envConfig);
            }

            return !_hasFailedTestResponse;
        }

        public static bool RunTestsOnActiveScene(ValidationContext context)
        {
            PackageConfig config = ProjectConfig.activePackage;
            if (config == null)
            {
                Debug.LogError("No config found.");
                return false;
            }

            validationContext = context;
            LoadTestsIfNecessary();
            allResponses.Clear();

            RunPackageTests(config);
            RunSceneTests(SceneManager.GetActiveScene());
            return !_hasFailedTestResponse;
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

        private static void RunTestsOnEnvironmentPackage(EnvironmentConfig config)
        {
            if (!Application.isBatchMode)
                EditorSceneManager.SaveOpenScenes(); // We are going to swap scenes and will lose changes without this

            Scene previousScene = EditorSceneManager.GetActiveScene();
            string originalScenePath = previousScene.path;
            var testedScenePaths = new HashSet<string>();

            foreach (EnvironmentConfig.Variant variant in config.variants)
            {
                if (variant.scene == null)
                    continue;

                string scenePath = AssetDatabase.GetAssetPath(variant.scene);
                if (!testedScenePaths.Add(scenePath))
                    continue; // We already tested this scene, although this shouldn't happen since each scene should be assigned to at most one variant.

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
                RunSceneTests(scene);
            }

            if (previousScene.path != originalScenePath && !string.IsNullOrEmpty(originalScenePath))
                EditorSceneManager.OpenScene(originalScenePath);
        }

        private static void RunComponentTestsOnObjectRecursively(GameObject g)
        {
            // Ignore object and children if marked editor only
            if (g.CompareTag("EditorOnly"))
                return;

            foreach (var component in g.GetComponents<Component>())
            {
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
                }
                else
                {
                    RunComponentTests(component);
                }
            }

            foreach (Transform child in g.transform)
                RunComponentTestsOnObjectRecursively(child.gameObject);
        }

        private static void RunComponentTests(Component target)
        {
            object[] targetParam = new object[] { target };
            Type targetType = target.GetType();

            foreach (KeyValuePair<Type, List<MethodInfo>> pair in _componentTests)
            {
                Type type = pair.Key;
                if (type.IsAssignableFrom(targetType))
                {
                    List<MethodInfo> tests = pair.Value;
                    foreach (MethodInfo method in tests)
                    {
                        method.Invoke(null, targetParam);
                    }
                }
            }
        }

        private static void RunPackageTests(PackageConfig config)
        {
            _currentTestScene = null;
            object[] configParam = new object[] { config };
            foreach (MethodInfo method in _packageTests)
            {
                var attr = method.GetCustomAttribute<PackageTest>();
                if (attr.TestAffectsPackageType(config.packageType))
                    method.Invoke(null, configParam);
            }
        }

        private static void RunSceneTests(Scene scene)
        {
            _currentTestScene = scene;
            object[] sceneParam = new object[] { scene };
            foreach (MethodInfo method in _sceneTests)
            {
                method.Invoke(null, sceneParam);
            }

            foreach (GameObject g in scene.GetRootGameObjects())
                RunComponentTestsOnObjectRecursively(g);

            _currentTestScene = null;
        }

        private static void LoadTestsIfNecessary()
        {
            if (_initialized)
            {
                return;
            }

            Assembly assembly = Assembly.GetAssembly(typeof(SpatialValidator));
            IEnumerable<MethodInfo> allPublicStaticMethods = assembly.GetExportedTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Where(method => !method.IsSpecialName); // exclude property getters/setters

            _componentTests = new Dictionary<Type, List<MethodInfo>>();
            _sceneTests = new List<MethodInfo>();
            _packageTests = new List<MethodInfo>();

            foreach (MethodInfo method in allPublicStaticMethods)
            {
                ParameterInfo[] methodParams = method.GetParameters();
                if (method.TryGetCustomAttribute<ComponentTest>(out var componentAttr))
                {
                    if (methodParams.Length != 1 || !typeof(Component).IsAssignableFrom(methodParams[0].ParameterType))
                    {
                        Debug.LogError("Component tests should have a single parameter of type Component. " + method.Name);
                        continue;
                    }
                    if (!_componentTests.ContainsKey(componentAttr.targetType))
                    {
                        _componentTests.Add(componentAttr.targetType, new List<MethodInfo>());
                    }
                    _componentTests[componentAttr.targetType].Add(method);
                }
                else if (method.TryGetCustomAttribute<SceneTest>(out var sceneAttr))
                {
                    if (methodParams.Length != 1 || methodParams[0].ParameterType != typeof(Scene))
                    {
                        Debug.LogError("Scene tests should have a single parameter of type Scene. " + method.Name);
                        continue;
                    }
                    _sceneTests.Add(method);
                }
                else if (method.TryGetCustomAttribute<PackageTest>(out var packageAttr))
                {
                    if (methodParams.Length != 1 || !typeof(PackageConfig).IsAssignableFrom(methodParams[0].ParameterType))
                    {
                        Debug.LogError("Package tests should have a single parameter of type PackageConfig. " + method.Name);
                        continue;
                    }
                    _packageTests.Add(method);
                }
            }

            _initialized = true;
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
