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
        static Toolbar()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        private static int _selectedTarget = 0;
        private static string[] _targetOptions = new string[]
        {
            "WebGL", "OSX", "Windows",
        };

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            GUI.color = Color.white * 0.75f;
            GUI.contentColor = Color.white * 1.15f;

            string cannotTestReason = GetTestButtonErrorString();
            using (new EditorGUI.DisabledScope(disabled: !string.IsNullOrEmpty(cannotTestReason)))
            {
                PackageConfig activeConfig = ProjectConfig.activePackageConfig;
                string buttonText, buttonTooltipText;

#if SPATIAL_UNITYSDK_STAGING
                _selectedTarget = EditorGUILayout.Popup(_selectedTarget, _targetOptions, GUILayout.Width(80));
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
                                    target = BuildTarget.StandaloneOSX;
                                    break;
                                case 2:
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

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_SettingsIcon")))
            {
                SpatialSDKConfigWindow.OpenWindow(SpatialSDKConfigWindow.CONFIG_TAB_NAME);
            }

            GUI.color = Color.white;
            GUILayout.Space(15);
        }

        private static string GetTestButtonErrorString()
        {
            if (!EditorUtility.isUsingSupportedUnityVersion)
                return $"Unity version must be between {EditorUtility.MIN_UNITY_VERSION_STR} and {EditorUtility.MAX_UNITY_VERSION_STR} to test in Spatial (currently using {Application.unityVersion})";
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return "Feature disabled while in play mode";
            if (ProjectConfig.activePackageConfig == null)
                return "There is no active package selected. You can create and select a package through the configuration window.";

            return null;
        }
    }
}