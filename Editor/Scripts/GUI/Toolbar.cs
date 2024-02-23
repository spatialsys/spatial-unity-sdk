using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityToolbarExtender;

namespace SpatialSys.UnitySDK.Editor
{
    [InitializeOnLoad]
    public static class Toolbar
    {
        private const string SANDBOX_TARGET_BUILD_PLATFORM_KEY = "SpatialSDK_TargetBuildPlatform";
        private static bool styleInitialized;
        private static Texture2D _helpTextTexture;
        private static Texture2D _helpButtonTexture;
        private static Texture2D _helpButtonHoveredTexture;

        private static GUIStyle _helpButtonStyle;

        static Toolbar()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
            styleInitialized = false;
        }

        private static void InitStyle()
        {
            _helpTextTexture = SpatialGUIUtility.LoadGUITexture("GUI/HelpText.png");
            _helpButtonTexture = SpatialGUIUtility.LoadGUITexture("GUI/HelpButtonTexture.png");
            _helpButtonHoveredTexture = SpatialGUIUtility.LoadGUITexture("GUI/HelpButtonTextureSelected.png");

            _helpButtonStyle = new GUIStyle(GUI.skin.button) {
                border = new RectOffset(3, 3, 3, 3),
                //Something internally messes with padding on dark vs light skin...
                padding = new RectOffset(6, 6, EditorGUIUtility.isProSkin ? 4 : 3, EditorGUIUtility.isProSkin ? 0 : 1),
            };
            _helpButtonStyle.normal.background = _helpButtonTexture;
            _helpButtonStyle.hover.background = _helpButtonHoveredTexture;
            if (ColorUtility.TryParseHtmlString("#00FF77", out Color color))
            {
                _helpButtonStyle.normal.textColor = color;
                _helpButtonStyle.active.textColor = color;
                _helpButtonStyle.hover.textColor = color;
            }
        }

        private static int _selectedTarget = 0;
        private static string[] _targetOptions = new string[]
        {
            "WebGL", "iOS", "Android", "Windows",
        };

        static void OnToolbarGUI()
        {
            if (!styleInitialized)
            {
                styleInitialized = true;
                InitStyle();
            }
            GUILayout.FlexibleSpace();

            GUI.color = Color.white * 0.75f;
            GUI.contentColor = Color.white * 1.15f;

            string cannotTestReason = GetTestButtonErrorString();
            using (new EditorGUI.DisabledScope(disabled: !string.IsNullOrEmpty(cannotTestReason)))
            {
                PackageConfig activeConfig = ProjectConfig.activePackageConfig;
                string buttonText, buttonTooltipText;

#if SPATIAL_UNITYSDK_STAGING
                _selectedTarget = SessionState.GetInt(SANDBOX_TARGET_BUILD_PLATFORM_KEY, 0);
                int newSelection = EditorGUILayout.Popup(_selectedTarget, _targetOptions, GUILayout.Width(80));
                if (_selectedTarget != newSelection)
                {
                    _selectedTarget = newSelection;
                    SessionState.SetInt(SANDBOX_TARGET_BUILD_PLATFORM_KEY, _selectedTarget);
                }
#endif
                if (activeConfig == null || activeConfig.isSpaceBasedPackage)
                {
                    Scene scene = EditorSceneManager.GetActiveScene();
                    buttonText = "Test Active Scene";
                    buttonTooltipText = $"Builds the active scene ({scene.name}) for testing in the Spatial web app";
                }
                else
                {
                    buttonText = "Test Active Package";
                    buttonTooltipText = $"Builds the active package ({activeConfig?.packageName}) for testing in the Spatial web app";
                }

                if (GUILayout.Button(new GUIContent(
                        $"▶️ {buttonText}",
                        !string.IsNullOrEmpty(cannotTestReason) ? cannotTestReason : buttonTooltipText
                    )))
                {
                    UpgradeUtility.PerformUpgradeIfNecessaryForTestOrPublish()
                        .Then(() => {
                            BuildTarget target = BuildTarget.WebGL;
                            switch (_selectedTarget)
                            {
                                case 1:
                                    target = BuildTarget.iOS; // iOS bundle can be run on OSX
                                    break;
                                case 2:
                                    target = BuildTarget.Android;
                                    break;
                                case 3:
                                    target = BuildTarget.StandaloneWindows;
                                    break;
                            }
                            return BuildUtility.BuildAndUploadForSandbox(target);
                        })
                        .Catch(exc => {
                            if (exc is RSG.PromiseCancelledException)
                                return;

                            UnityEditor.EditorUtility.DisplayDialog("Sandbox Error", $"An unexpected error occurred while preparing your package for the sandbox.\n\n{exc.Message}", "OK");
                            Debug.LogException(exc);
                        });
                }
            }

            if (GUILayout.Button("Publishing"))
            {
                SpatialSDKConfigWindow.OpenWindow(SpatialSDKConfigWindow.CONFIG_TAB_NAME);
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_SettingsIcon" : "SettingsIcon")))
            {
                SpatialSDKConfigWindow.OpenWindow(SpatialSDKConfigWindow.CONFIG_TAB_NAME);
            }

            GUILayout.Space(8);

            if (GUILayout.Button(new GUIContent(_helpTextTexture), _helpButtonStyle))
            {
                GenericMenu menu = new GenericMenu();
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    menu.AddSeparator("Read Docs & Tutorials");
                    menu.AddItem(new GUIContent("Documentation"), false, () => Application.OpenURL("https://docs.spatial.io/"));
                    menu.AddItem(new GUIContent("Scripting API"), false, () => Application.OpenURL("https://cs.spatial.io/api/"));
                    menu.AddSeparator("");
                    menu.AddSeparator("Ask for Help");
                    menu.AddItem(new GUIContent("Help and Discussion Forum"), false, () => Application.OpenURL("https://github.com/spatialsys/spatial-unity-sdk/discussions"));
                    menu.AddSeparator("");
                    menu.AddSeparator("Ask the Community");
                    menu.AddItem(new GUIContent("Community Discord"), false, () => Application.OpenURL("https://discord.gg/spatial"));
                }
                else
                {
                    // windows doesnt render the separators nicely...idk why
                    menu.AddItem(new GUIContent("Documentation"), false, () => Application.OpenURL("https://docs.spatial.io/"));
                    menu.AddItem(new GUIContent("Scripting API"), false, () => Application.OpenURL("https://cs.spatial.io/api/"));
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Help and Discussion Forum"), false, () => Application.OpenURL("https://github.com/spatialsys/spatial-unity-sdk/discussions"));
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Community Discord"), false, () => Application.OpenURL("https://discord.gg/spatial"));
                }
                menu.ShowAsContext();
            }

            GUI.color = Color.white;
            GUILayout.Space(8);
        }

        private static string GetTestButtonErrorString()
        {
            if (!EditorUtility.isUsingSupportedUnityVersion)
                return $"Unity version must be between {EditorUtility.MIN_UNITY_VERSION_STR} and {EditorUtility.MAX_UNITY_VERSION_STR} to test in Spatial (currently using {Application.unityVersion})";
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return "Feature disabled while in play mode";
            if (ProjectConfig.activePackageConfig == null)
                return "There is no active package selected. You can create and select a package through the configuration window.";
            if (!AuthUtility.isAuthenticated)
                return "Must be logged into a Spatial account";
            return null;
        }
    }
}