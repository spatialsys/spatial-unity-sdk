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
        private static string _testBundleName;

        static Toolbar()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
            EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosed += OnSceneClosed;
            UpdateTestBundleName();
        }

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            GUI.color = Color.white * 0.75f;
            GUI.contentColor = Color.white * 1.15f;

            bool validBundle = !string.IsNullOrEmpty(_testBundleName);

            using (new EditorGUI.DisabledScope(disabled: !validBundle))
            {
                // TODO: Add a mini window previewing what's about to be built, which will act as a confirmation as well so the dialog won't be necessary.
                if (GUILayout.Button(new GUIContent(
                        "▶️ Test Current Space",
                        validBundle ? $"Builds the bundle ({_testBundleName}) for testing in the Spatial web app" : "No open scenes are tagged as an asset bundle"
                    )) &&
                    UnityEditor.EditorUtility.DisplayDialog("Testing Space", $"You are about to export this bundle {_testBundleName} to the Spatial sandbox.", "Continue", "Cancel"))
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

            bool isPublishingAvailable = false;

            using (new EditorGUI.DisabledScope(disabled: !isPublishingAvailable))
            {
                if (GUILayout.Button(new GUIContent(
                        "▲ Publish Space",
                        isPublishingAvailable ? "Uploads all the source assets to Spatial where it will be compiled for all platforms" : "This feature is coming soon"
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
                            BuildUtility.PackageForPublishing();
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
                ConfigWindow.Open();
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
                        if (UnityEditor.EditorUtility.DisplayDialog("Upgrade to latest version?", "A new version of the Spatial SDK is available. You will need to upgrade to the latest version to publish or test your environment. Would you like to upgrade now?", "Yes", "No"))
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

        private static void OnActiveSceneChanged(Scene oldScene, Scene newScene) => UpdateTestBundleName();
        private static void OnSceneOpened(Scene scene, OpenSceneMode openMode) => UpdateTestBundleName();
        private static void OnSceneClosed(Scene scene) => UpdateTestBundleName();

        private static void UpdateTestBundleName()
        {
            _testBundleName = BuildUtility.GetAssetBundleNameForOpenedScene();
        }
    }
}