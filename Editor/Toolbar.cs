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
                    BuildUtility.BuildAndUploadForSandbox();
                }
            }

            if (GUILayout.Button(new GUIContent("▲ Publish Space", "Uploads all the source assets to Spatial where it will be compiled for all platforms")) &&
                UnityEditor.EditorUtility.DisplayDialog(
                    "Publishing Space",
                    "You are about to publish this environment to the public. This will take a while and you can only do this limited number of times per day.",
                    "Continue",
                    "Cancel"
                ))
            {
                UnityEditor.EditorUtility.DisplayDialog("Notice", "This feature is not yet ready!", "OK");
                BuildUtility.PackageForPublishing();
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_SettingsIcon")))
            {
                ConfigWindow.Open();
            }

            GUI.color = Color.white;
            GUILayout.Space(15);
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