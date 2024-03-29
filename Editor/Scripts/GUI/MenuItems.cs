using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public static class MenuItems
    {
        [MenuItem("Spatial SDK/Account")]
        public static void OpenAccount()
        {
            SpatialSDKConfigWindow.OpenWindow(SpatialSDKConfigWindow.ACCOUNT_TAB_NAME);
        }

        [MenuItem("Spatial SDK/Configuration")]
        public static void OpenConfig()
        {
            SpatialSDKConfigWindow.OpenWindow(SpatialSDKConfigWindow.CONFIG_TAB_NAME);
        }

        [MenuItem("Spatial SDK/Help")]
        public static void OpenHelp()
        {
            SpatialSDKConfigWindow.OpenWindow(SpatialSDKConfigWindow.HELP_TAB_NAME);
        }

        [MenuItem("Spatial SDK/Visual Scripting/Regenerate Nodes")]
        public static void RegenerateNodes()
        {
            NodeGeneration.SetTypesAndAssemblies();
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
