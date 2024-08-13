using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using System.Reflection;

namespace SpatialSys.UnitySDK.Editor
{
    [InitializeOnLoad]
    public static class Toolbar
    {
        private const string SANDBOX_TARGET_BUILD_PLATFORM_KEY = "SpatialSDK_TargetBuildPlatform";

        private static Button _playmodeWarning;
        private static ScriptableObject _currentToolbar;
        private static VisualElement _toolbarRoot;
        private static VisualElement _toolbarElement;
        private static Type _toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");

        private enum TargetPlatform
        {
            Web,
            iOS,
            Android,
#if SPATIAL_UNITYSDK_INTERNAL
            Windows
#endif
        }

        private static TargetPlatform _selectedTarget = TargetPlatform.Web;


        static Toolbar()
        {
            EditorApplication.update -= OnUpdate;

            // Disable toolbar in certain environments
#if !SPATIAL_UNITYSDK_INTERNAL
            EditorApplication.update += OnUpdate;
#endif
        }

        private static void OnUpdate()
        {
            if (_playmodeWarning != null)
                _playmodeWarning.style.display = EditorApplication.isPlaying ? DisplayStyle.Flex : DisplayStyle.None;

            // Catches a bug where our toolbar gets lost when changing OS resolution while unity is open.
            bool missingParent = _toolbarRoot == null || _toolbarRoot.parent == null || _toolbarRoot.parent.parent == null || _toolbarRoot.parent.parent.parent == null || _toolbarRoot.parent.parent.parent.parent == null;

            // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes 
            if (_currentToolbar == null || _toolbarElement == null || missingParent)
            {
                // Find toolbar
                var toolbars = Resources.FindObjectsOfTypeAll(_toolbarType);
                _currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                if (_currentToolbar != null)
                {
                    FieldInfo root = _currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    VisualElement mRoot = root.GetValue(_currentToolbar) as VisualElement;
                    VisualElement toolbarZone = mRoot.Q("ToolbarZoneRightAlign");

                    var uiAsset = EditorUtility.LoadAssetFromPackagePath<VisualTreeAsset>("Editor/Scripts/GUI/Toolbar/SpatialToolbar.uxml");
                    VisualElement ui = uiAsset.Instantiate();
                    _toolbarElement = ui;
                    _toolbarRoot = toolbarZone;
                    toolbarZone.Add(ui);

                    _playmodeWarning = ui.Q<Button>("PlaymodeWarning");

                    _playmodeWarning.clicked += HandlePlayModeWarningClicked;

                    ui.Q<Button>("HelpButton").clicked += HandleHelpButtonClicked;

                    ui.Q<Button>("TestButton").clicked += HandleTestButtonClicked;

                    ui.Q<Button>("ConfigButton").clicked += HandleConfigButtonClicked;

                    ui.Q<Button>("PlatformDropdown").clicked += HandlePlatformDropdownClicked;

                    ui.Q<Button>("PackageDropdown").clicked += HandlePackageDropdownClicked;
                }
            }
            else
            {
                // Update Test button text and tooltip
                string disabledReason = GetBuildDisabledReason();
                bool disabled = !string.IsNullOrEmpty(disabledReason);

                string buttonText, buttonTooltipText;
                PackageConfig activeConfig = ProjectConfig.activePackageConfig;
                if (activeConfig == null || activeConfig.isSpaceBasedPackage)
                {
                    Scene scene = EditorSceneManager.GetActiveScene();
                    buttonText = "▶ Test Scene";
                    buttonTooltipText = $"Builds the active scene ({scene.name}) for testing in the Spatial web app";
                }
                else
                {
                    buttonText = "▶ Test Package";
                    buttonTooltipText = $"Builds the active package ({activeConfig?.packageName}) for testing in the Spatial web app";
                }

                Button testButton = _toolbarElement.Q<Button>("TestButton");

                testButton.text = buttonText;
                testButton.tooltip = disabled ? disabledReason : buttonTooltipText;
                testButton.SetEnabled(!disabled);

                Button platformDropdown = _toolbarElement.Q<Button>("PlatformDropdown");
                platformDropdown.tooltip = disabled ? disabledReason : "Change Spatial platform";
                platformDropdown.SetEnabled(!disabled);

                Button packageDropdown = _toolbarElement.Q<Button>("PackageDropdown");
                packageDropdown.tooltip = disabled ? disabledReason : "Change active package";
                packageDropdown.SetEnabled(!disabled);

                // Update platform icon
                VisualElement platformIcon = _toolbarElement.Q<VisualElement>("PlatformIcon");
                platformIcon.style.backgroundImage = _selectedTarget switch {
                    TargetPlatform.Web => (Texture2D)EditorGUIUtility.IconContent("BuildSettings.Web.Small").image,
                    TargetPlatform.iOS => (Texture2D)EditorGUIUtility.IconContent("BuildSettings.iPhone On").image,
                    TargetPlatform.Android => (Texture2D)EditorGUIUtility.IconContent("BuildSettings.Android On").image,
#if SPATIAL_UNITYSDK_INTERNAL
                    TargetPlatform.Windows => (Texture2D)EditorGUIUtility.IconContent("BuildSettings.Standalone On").image,
#endif
                    _ => (Texture2D)EditorGUIUtility.IconContent("BuildSettings.WebGL On").image,

                };
            }
        }

        private static void HandlePlayModeWarningClicked()
        {
            PlaymodePopup.InitWindow(false);
        }

        private static void HandleHelpButtonClicked()
        {
            GenericMenu menu = new GenericMenu();
            // The separator with text is styles identically to Items on windows so we don't use them there. On Mac they look nice.
            void AddSeparator(string header)
            {
                menu.AddSeparator("");
                if (Application.platform == RuntimePlatform.OSXEditor)
                    menu.AddSeparator(header);
            }

            AddSeparator("Read Docs & Tutorials");
            menu.AddItem(new GUIContent("Documentation"), false, () => Application.OpenURL("https://toolkit.spatial.io/"));
            menu.AddItem(new GUIContent("Scripting API"), false, () => Application.OpenURL("https://cs.spatial.io/reference"));
            menu.AddItem(new GUIContent("Templates"), false, () => Application.OpenURL("https://toolkit.spatial.io/templates"));

            AddSeparator("Ask for Help");
            menu.AddItem(new GUIContent("Help and Discussion Forum"), false, () => Application.OpenURL("https://github.com/spatialsys/spatial-unity-sdk/discussions"));

            AddSeparator("Ask the Community");
            menu.AddItem(new GUIContent("Community Discord"), false, () => Application.OpenURL("https://discord.gg/spatial"));
            menu.ShowAsContext();
        }

        private static void HandleTestButtonClicked()
        {
            BuildTarget target = BuildTarget.WebGL;
            switch (_selectedTarget)
            {
                case TargetPlatform.iOS:
                    target = BuildTarget.iOS; // iOS bundle can be run on OSX
                    break;
                case TargetPlatform.Android:
                    target = BuildTarget.Android;
                    break;
#if SPATIAL_UNITYSDK_INTERNAL
                case TargetPlatform.Windows:
                    target = BuildTarget.StandaloneWindows64;
                    break;
#endif
            }


            BuildUtility.BuildAndUploadForSandbox(target)
                .Catch(ex => Debug.LogException(ex)); // Catch all other unhandled exceptions
        }

        private static void HandleConfigButtonClicked()
        {
            SpatialSDKConfigWindow.OpenWindow(SpatialSDKConfigWindow.CONFIG_TAB_NAME);
        }

        private static void HandlePlatformDropdownClicked()
        {
            GenericMenu menu = new GenericMenu();
            void SetTarget(TargetPlatform target)
            {
                _selectedTarget = target;
                SessionState.SetInt(SANDBOX_TARGET_BUILD_PLATFORM_KEY, (int)_selectedTarget);
            }

            if (Application.platform == RuntimePlatform.OSXEditor)
                menu.AddSeparator("Target Platform");

            foreach (TargetPlatform target in Enum.GetValues(typeof(TargetPlatform)))
            {
                menu.AddItem(new GUIContent(target.ToString()), _selectedTarget == target, () => SetTarget(target));
            }
            menu.ShowAsContext();
        }

        private static void HandlePackageDropdownClicked()
        {
            GenericMenu menu = new GenericMenu();

            if (Application.platform == RuntimePlatform.OSXEditor)
                menu.AddSeparator("Active package");

            foreach (PackageConfig package in ProjectConfig.packages)
            {
                menu.AddItem(new GUIContent($"{package.packageType} - {package.packageName}"), ProjectConfig.activePackageConfig == package, () => ProjectConfig.SetActivePackage(package));
            }
            menu.ShowAsContext();
        }

        private static string GetBuildDisabledReason()
        {
            if (!EditorUtility.isUsingSupportedUnityVersion)
                return $"Unity version must be between {EditorUtility.MIN_UNITY_VERSION_STR} and {EditorUtility.MAX_UNITY_VERSION_STR} to test in Spatial (currently using {Application.unityVersion})";
            return BuildUtility.GetBuildDisabledReason();
        }
    }
}