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
        public static List<SpatialTestResponse> projectResponses { get; private set; } = new List<SpatialTestResponse>();
        public static Dictionary<string, List<SpatialTestResponse>> sceneResponses { get; private set; } = new Dictionary<string, List<SpatialTestResponse>>();
        public static ValidationContext validationContext { get; private set; } = ValidationContext.ManualRun;

        private static bool _initialized;
        private static Dictionary<Type, List<MethodInfo>> _componentTests;
        private static List<MethodInfo> _sceneTests;
        private static List<MethodInfo> _projectTests;

        private static Scene? _currentTestScene = null;

        private static void LoadTestsIfNecessary()
        {
            if (_initialized)
            {
                return;
            }
            Assembly assembly = Assembly.GetAssembly(typeof(SpatialValidator));
            _componentTests = new Dictionary<Type, List<MethodInfo>>();
            _sceneTests = new List<MethodInfo>();
            _projectTests = new List<MethodInfo>();

            foreach (MethodInfo method in assembly.GetTypes().SelectMany(x => x.GetMethods()))
            {
                ComponentTest att = method.GetCustomAttribute(typeof(ComponentTest)) as ComponentTest;
                if (att != null)
                {
                    if (!_componentTests.ContainsKey(att.targetType))
                    {
                        _componentTests.Add(att.targetType, new List<MethodInfo>());
                    }
                    if (method.GetParameters().Length != 1 || !typeof(UnityEngine.Object).IsAssignableFrom(method.GetParameters()[0].ParameterType))
                    {
                        Debug.LogError("Component tests should have a single parameter of type unity object. " + method.Name);
                        continue;
                    }
                    _componentTests[att.targetType].Add(method);
                }
                else if (method.GetCustomAttribute(typeof(SceneTest)) != null)
                {
                    if (method.GetParameters().Length != 1 || !(method.GetParameters()[0].ParameterType == typeof(UnityEngine.SceneManagement.Scene)))
                    {
                        Debug.LogError("Scene tests should have a single parameter of type Scene. " + method.Name);
                        continue;
                    }
                    _sceneTests.Add(method);
                }
                else if (method.GetCustomAttribute(typeof(ProjectTest)) != null)
                {
                    if (method.GetParameters().Length != 0)
                    {
                        Debug.LogError("Project tests should not have any parameters. " + method.Name);
                        continue;
                    }
                    _projectTests.Add(method);
                }
            }
            _initialized = true;
        }

        /// <summary>
        /// Returns true if no tests FAILED. There could be warnings though.
        /// </summary>
        /// <returns></returns>
        public static bool RunTestsOnProject(ValidationContext context)
        {
            validationContext = context;

            LoadTestsIfNecessary();
            if (!Application.isBatchMode)
                EditorSceneManager.SaveOpenScenes();//we are going to swap scenes and will lose changes without this
            _currentTestScene = null;
            projectResponses.Clear();
            sceneResponses.Clear();

            foreach (MethodInfo method in _projectTests)
            {
                method.Invoke(null, null);
            }

            PackageConfig config = PackageConfig.instance;
            Scene previousScene = EditorSceneManager.GetActiveScene();
            string originalScenePath = previousScene.path;

            foreach (PackageConfig.Environment.Variant variant in config.environment.variants)
            {
                if (variant.scene == null)
                {
                    continue;
                }
                string testScenePath = AssetDatabase.GetAssetPath(variant.scene);
                sceneResponses.Add(testScenePath, new List<SpatialTestResponse>());
                Scene testScene;
                if (previousScene.path != testScenePath)
                {
                    testScene = EditorSceneManager.OpenScene(testScenePath);
                }
                else
                {
                    testScene = EditorSceneManager.GetActiveScene();
                }

                _currentTestScene = testScene;
                previousScene = testScene;

                object[] targetParam = new object[] { testScene };
                foreach (MethodInfo method in _sceneTests)
                {
                    method.Invoke(null, targetParam);
                }

                // Run component tests
                List<Component> components = new List<Component>();
                foreach (GameObject g in testScene.GetRootGameObjects())
                {
                    FindComponentsRecursive(g, components);
                }
                foreach (Component component in components)
                {
                    RunTestsOnObject(component);
                }
            }

            if (previousScene.path != originalScenePath)
            {
                if (!string.IsNullOrEmpty(originalScenePath))
                    EditorSceneManager.OpenScene(originalScenePath);
            }

            RefreshAllResponses();

            foreach (SpatialTestResponse response in allResponses)
            {
                if (response.responseType == TestResponseType.Fail)
                {
                    return false;
                }
            }
            return true;
        }

        private static void FindComponentsRecursive(GameObject g, List<Component> components)
        {
            // Ignore object and children if marked editor only
            if (g.tag == "EditorOnly")
                return;

            foreach (var component in g.GetComponents<Component>())
            {
                // null-check here because components can be null if script is missing
                if (component != null)
                    components.Add(component);
            }

            foreach (Transform child in g.transform)
            {
                FindComponentsRecursive(child.gameObject, components);
            }
        }

        private static void RunTestsOnObject(UnityEngine.Object target)
        {
            object[] targetParam = new object[] { target };

            foreach (Type type in _componentTests.Keys)
            {
                if (target.GetType() == type || type.IsAssignableFrom(target.GetType()))
                {
                    foreach (MethodInfo method in _componentTests[type])
                    {
                        method.Invoke(null, targetParam);
                    }
                }
            }
        }

        public static bool RunTestsOnActiveScene(ValidationContext context)
        {
            validationContext = context;

            LoadTestsIfNecessary();
            Scene scene = SceneManager.GetActiveScene();
            projectResponses.Clear();
            sceneResponses.Clear();
            sceneResponses.Add(scene.path, new List<SpatialTestResponse>());

            object[] targetParam = new object[] { scene };

            _currentTestScene = null;
            foreach (MethodInfo method in _projectTests)
            {
                method.Invoke(null, null);
            }

            _currentTestScene = scene;

            foreach (MethodInfo method in _sceneTests)
            {
                method.Invoke(null, targetParam);
            }

            List<Component> components = new List<Component>();

            foreach (GameObject g in scene.GetRootGameObjects())
            {
                components.AddRange(g.GetComponentsInChildren(typeof(Component), true));
            }

            foreach (Component component in components)
            {
                RunTestsOnObject(component);
            }

            RefreshAllResponses();

            foreach (SpatialTestResponse response in allResponses)
            {
                if (response.responseType == TestResponseType.Fail)
                {
                    return false;
                }
            }
            return true;
        }

        public static void AddResponse(SpatialTestResponse response)
        {
            if (!_currentTestScene.HasValue)
            {
                projectResponses.Add(response);
            }
            else
            {
                response.scenePath = _currentTestScene.Value.path;
                sceneResponses[_currentTestScene.Value.path].Add(response);
            }
        }

        public static void RefreshAllResponses()
        {
            allResponses.Clear();
            allResponses.AddRange(projectResponses);

            foreach (string key in sceneResponses.Keys)
            {
                allResponses.AddRange(sceneResponses[key]);
            }
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
    public class ProjectTest : Attribute { }
}
