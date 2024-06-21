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
        private static bool styleInitialized;
        private static Texture2D _helpButtonTexture;
        private static Texture2D _helpButtonHoveredTexture;

        private static GUIStyle _helpButtonStyle;

        static Button playmodeWarning;
        static ScriptableObject m_currentToolbar;
        static VisualElement toolbarRoot;
        static VisualElement toolbarElement;
        static Type m_toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");

        static Toolbar()
        {
            styleInitialized = false;
            EditorApplication.update -= OnUpdate;

            // Disable toolbar in certain environments
#if !SPATIAL_UNITYSDK_INTERNAL
            EditorApplication.update += OnUpdate;
#endif
        }

        private static void OnUpdate()
        {
            if (playmodeWarning != null)
                playmodeWarning.style.display = EditorApplication.isPlaying ? DisplayStyle.Flex : DisplayStyle.None;

            // Catches a bug where our toolbar gets lost when changing OS resolution while unity is open.
            bool missingParent = toolbarRoot == null || toolbarRoot.parent == null || toolbarRoot.parent.parent == null || toolbarRoot.parent.parent.parent == null || toolbarRoot.parent.parent.parent.parent == null;

            // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes 
            if (m_currentToolbar == null || toolbarElement == null || missingParent)
            {
                // Find toolbar
                var toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
                m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                if (m_currentToolbar != null)
                {
                    var root = m_currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    var rawRoot = root.GetValue(m_currentToolbar);
                    VisualElement mRoot = rawRoot as VisualElement;
                    var toolbarZone = mRoot.Q("ToolbarZoneRightAlign");

                    var uiAsset = EditorUtility.LoadAssetFromPackagePath<VisualTreeAsset>("Editor/Scripts/GUI/Toolbar/SpatialToolbar.uxml");
                    VisualElement ui = uiAsset.Instantiate();
                    toolbarElement = ui;
                    toolbarRoot = toolbarZone;
                    toolbarZone.Add(ui);
                    ui.Q<IMGUIContainer>("IMGUI").onGUIHandler = () => OnToolbarGUI();

                    playmodeWarning = ui.Q<Button>("PlaymodeWarning");

                    playmodeWarning.clicked += () => {
                        PlaymodePopup.InitWindow(false);
                    };

                    ui.Q<Button>("HelpButton").clicked += () => {
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

                        AddSeparator("Ask for Help");
                        menu.AddItem(new GUIContent("Help and Discussion Forum"), false, () => Application.OpenURL("https://github.com/spatialsys/spatial-unity-sdk/discussions"));

                        AddSeparator("Ask the Community");
                        menu.AddItem(new GUIContent("Community Discord"), false, () => Application.OpenURL("https://discord.gg/spatial"));
                        menu.ShowAsContext();
                    };
                }
            }
        }

        private static void InitStyle()
        {
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
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.color = Color.white * 0.75f;
            GUI.contentColor = Color.white * 1.15f;

            string disabledReason = GetBuildDisabledReason();
            using (new EditorGUI.DisabledScope(disabled: !string.IsNullOrEmpty(disabledReason)))
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
                        !string.IsNullOrEmpty(disabledReason) ? disabledReason : buttonTooltipText
                    )))
                {
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
                            target = BuildTarget.StandaloneWindows64;
                            break;
                    }

                    BuildUtility.BuildAndUploadForSandbox(target)
                        .Catch(ex => Debug.LogException(ex)); // Catch all other unhandled exceptions
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

            GUI.color = Color.white;
            GUILayout.EndHorizontal();
        }

        private static string GetBuildDisabledReason()
        {
            if (!EditorUtility.isUsingSupportedUnityVersion)
                return $"Unity version must be between {EditorUtility.MIN_UNITY_VERSION_STR} and {EditorUtility.MAX_UNITY_VERSION_STR} to test in Spatial (currently using {Application.unityVersion})";
            return BuildUtility.GetBuildDisabledReason();
        }
    }
}