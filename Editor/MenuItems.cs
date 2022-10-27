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
    }
}
