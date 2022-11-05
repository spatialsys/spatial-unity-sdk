using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public static class MenuItems
    {
        private const int HELP_PRIORITY = 100;
        private const int WINDOW_PRIORITY = 200;

        [MenuItem("Spatial SDK/Help/Documentation", false, HELP_PRIORITY)]
        public static void OpenDocumentation()
        {
            EditorUtility.OpenDocumentationPage();
        }

        [MenuItem("Spatial SDK/Getting Started", false, WINDOW_PRIORITY)]
        public static void OpenGettingStartedWindow()
        {
            ConfigWindow.Open(ConfigWindow.TabType.GettingStarted);
        }

        [MenuItem("Spatial SDK/Authentication", false, WINDOW_PRIORITY)]
        public static void OpenAuthenticationWindow()
        {
            ConfigWindow.Open(ConfigWindow.TabType.Authentication);
        }

        [MenuItem("Spatial SDK/Configuration", false, WINDOW_PRIORITY)]
        public static void OpenConfigWindow()
        {
            ConfigWindow.Open(ConfigWindow.TabType.Configuration);
        }

        [MenuItem("Spatial SDK/Check for updates...")]
        public static void UpgradeToLatest()
        {
            UpgradeUtility.CheckForUpgrade(UpgradeUtility.UpgradeCheckType.ForceFetch)
                .Then(upgradeRequired => {
                    if (upgradeRequired)
                    {
                        UpgradeUtility.ShowUpgradeDialog();
                    }
                    else
                    {
                        UnityEditor.EditorUtility.DisplayDialog("Spatial SDK", "You are up to date!", "OK");
                    }
                })
                .Catch(err => {
                    UnityEditor.EditorUtility.DisplayDialog("Spatial SDK", "Error checking for upgrade: " + err.Message, "OK");
                    Debug.LogException(err);
                });
        }
    }
}
