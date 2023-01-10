using UnityEngine;
using UnityEditor;

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
                // TODO: Add a mini window previewing what's about to be built, which will act as a confirmation as well so the dialog won't be necessary.
                if (GUILayout.Button(new GUIContent(
                        "▶️ Test Current Scene",
                        !string.IsNullOrEmpty(cannotTestReason) ? cannotTestReason : $"Builds the current scene to test in the Spatial web app"
                    )) &&
                    UnityEditor.EditorUtility.DisplayDialog("Testing Space", $"You are about to export the current scene to the Spatial sandbox.", "Continue", "Cancel"))
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

            return null;
        }
    }
}