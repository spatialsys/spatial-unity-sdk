using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public static class MenuItems
    {
        private const int HELP_PRIORITY = 10;

        [MenuItem("Spatial SDK/Configuration")]
        public static void OpenConfigWindow()
        {
            ConfigWindow.Open();
        }

        [MenuItem("Spatial SDK/Help/Documentation", false, HELP_PRIORITY)]
        public static void OpenDocumentation()
        {
            EditorUtility.OpenDocumentationPage();
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
