using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

using UnityToolbarExtender;
using RSG;

namespace SpatialSys.UnitySDK.Editor
{
    [InitializeOnLoad]
    public static class Toolbar
    {
        static Toolbar()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            GUI.color = Color.white * 0.75f;
            GUI.contentColor = Color.white * 1.15f;

            string cannotTestReason = GetTestButtonErrorString();

            using (new EditorGUI.DisabledScope(disabled: !string.IsNullOrEmpty(cannotTestReason)))
            {
                PackageConfig activeConfig = ProjectConfig.activePackage;
                string buttonText, buttonTooltipText;

                if (activeConfig is EnvironmentConfig)
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
                    )) &&
                    UnityEditor.EditorUtility.DisplayDialog("Testing Package", $"You are about to upload and test your package in the Spatial sandbox.", "Continue", "Cancel"))
                {
                    UpgradeUtility.PerformUpgradeIfNecessaryForTestOrPublish()
                        .Then(() => {
                            return BuildUtility.BuildAndUploadForSandbox();
                        })
                        .Catch(exc => {
                            if (exc is RSG.PromiseCancelledException)
                                return;

                            UnityEditor.EditorUtility.DisplayDialog("Sandbox Error", $"There was an unexpected error while preparing your space for sandbox testing.\n\n{exc.Message}", "OK");
                            Debug.LogException(exc);
                        });
                }
            }

            if (GUILayout.Button("Publishing"))
            {
                SpatialSDKConfigWindow.OpenWindow("config");
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_SettingsIcon")))
            {
                SpatialSDKConfigWindow.OpenWindow("config");
            }

            GUI.color = Color.white;
            GUILayout.Space(15);
        }

        private static string GetTestButtonErrorString()
        {
            if (!EditorUtility.isUsingSupportedUnityVersion)
                return $"Requires Unity {EditorUtility.CLIENT_UNITY_VERSION} to test in Spatial (currently using {Application.unityVersion})";
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return "Feature disabled while in play mode";
            if (ProjectConfig.activePackage == null)
                return "There is no active package selected. You can create and select a package through the configuration window.";

            return null;
        }
    }
}