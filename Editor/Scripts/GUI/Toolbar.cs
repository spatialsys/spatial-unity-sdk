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
                        "▶️ Test Current Space",
                        !string.IsNullOrEmpty(cannotTestReason) ? cannotTestReason : $"Builds the current scene to test in the Spatial web app"
                    )) &&
                    UnityEditor.EditorUtility.DisplayDialog("Testing Space", $"You are about to export the current scene to the Spatial sandbox.", "Continue", "Cancel"))
                {
                    PerformUpgradeFlowIfNecessary()
                        .Then(() => {
                            return BuildUtility.BuildAndUploadForSandbox();
                        })
                        .Catch(exc => {
                            if (exc is RSG.PromiseCancelledException)
                                return;

                            UnityEditor.EditorUtility.DisplayDialog("Sandbox Error", $"There was an unexpected error while preparing your space for sandbox testing.\n\n{exc.Message}", "OK");
                        });
                }
            }

            string cannotPublishReason = GetPublishButtonErrorString();

            using (new EditorGUI.DisabledScope(disabled: !string.IsNullOrEmpty(cannotPublishReason)))
            {
                if (GUILayout.Button(new GUIContent(
                        "▲ Publish Space",
                        !string.IsNullOrEmpty(cannotPublishReason) ? cannotPublishReason : "Uploads all the source assets to Spatial where it will be compiled for all platforms"
                    )) &&
                    UnityEditor.EditorUtility.DisplayDialog(
                        "Publishing Space",
                        "You are about to publish this environment to the public. This will take a while and you can only do this limited number of times per day.",
                        "Continue",
                        "Cancel"
                    ))
                {
                    PerformUpgradeFlowIfNecessary()
                        .Then(() => {
                            return BuildUtility.PackageForPublishing();
                        })
                        .Catch(exc => {
                            if (exc is RSG.PromiseCancelledException)
                                return;

                            UnityEditor.EditorUtility.DisplayDialog("Publishing Error", $"There was an unexpected error while publishing your space.\n\n{exc.Message}", "OK");
                        });
                }
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_SettingsIcon")))
            {
                SpatialSDKConfigWindow.OpenWindow("config");
            }

            GUI.color = Color.white;
            GUILayout.Space(15);
        }

        private static IPromise PerformUpgradeFlowIfNecessary()
        {
            return UpgradeUtility.CheckForUpgrade()
                .Then(upgradeRequired => {
                    if (upgradeRequired)
                    {
                        string dialogMessage = "A new version of the Spatial SDK is available. You will need to upgrade to the latest version to publish or test your environment. Would you like to upgrade now?";
                        if (UnityEditor.EditorUtility.DisplayDialog("Upgrade to latest version?", dialogMessage, "Yes", "No"))
                        {
                            return UpgradeUtility.UpgradeToLatest();
                        }
                        else
                        {
                            UnityEditor.EditorUtility.DisplayDialog("Upgrade required to continue", "You will need to upgrade to the latest Spatial SDK to continue. You can do this by pressing the menu item under \"Spatial SDK/Check for updates...\".", "OK");
                            throw new RSG.PromiseCancelledException();
                        }
                    }

                    return Promise<bool>.Resolved(false);
                })
                .Then(upgradePerformed => {
                    if (upgradePerformed)
                        UnityEditor.EditorUtility.DisplayDialog("Upgrade successful", "The Spatial SDK has been upgraded to the latest version.", "OK");
                })
                .Catch(err => {
                    if (err is RSG.PromiseCancelledException)
                        throw err; // Cancel downstream promises

                    Debug.LogException(err);
                    UnityEditor.EditorUtility.DisplayDialog("Upgrade failed", "Failed to upgrade to latest version of the Spatial SDK. Check out the console panel for details.", "OK");
                    throw new System.Exception("Failed to perform necessary upgrade");
                });
        }

        private static string GetTestButtonErrorString()
        {
            if (!EditorUtility.isUsingSupportedUnityVersion)
                return $"Requires Unity {EditorUtility.CLIENT_UNITY_VERSION} to test in Spatial (currently using {Application.unityVersion})";
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return "Feature disabled while in play mode";

            return null;
        }

        private static string GetPublishButtonErrorString()
        {
            if (PackageConfig.instance == null)
                return "Unable to locate SDK configuration";
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return "Feature disabled while in play mode";

            return null;
        }
    }
}