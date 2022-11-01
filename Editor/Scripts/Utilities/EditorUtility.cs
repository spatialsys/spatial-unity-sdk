using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public static class EditorUtility
    {
        private const string AUTH_TOKEN_KEY = "SpatialSDK_Token";

        public static EditorConfig CreateDefaultConfigurationFile()
        {
            string directory = CreateFolderHierarchy("Spatial SDK");
            string filePath = directory + "/SpatialSDK_Config.asset";
            EditorConfig result = AssetDatabase.LoadAssetAtPath<EditorConfig>(filePath);

            if (result == null)
            {
                EditorConfig config = ScriptableObject.CreateInstance<EditorConfig>();
                config.environmentVariants = new EditorConfig.EnvironmentVariant[1];
                config.environmentVariants[0] = new EditorConfig.EnvironmentVariant();
                AssetDatabase.CreateAsset(config, filePath);
                AssetDatabase.Refresh();
                result = AssetDatabase.LoadAssetAtPath<EditorConfig>(filePath);
            }

            return result;
        }

        public static string GetSavedAuthToken()
        {
            return EditorPrefs.GetString(AUTH_TOKEN_KEY);
        }

        public static void SaveAuthToken(string token)
        {
            EditorPrefs.SetString(AUTH_TOKEN_KEY, token);
        }

        public static bool CheckStringDistance(string a, string b, int distanceThreshold)
        {
            // Check how many characters are different between the two strings, and see if it exceeds the threshold.
            int lengthDifference = Mathf.Abs(a.Length - b.Length);

            if (lengthDifference >= distanceThreshold)
                return true;

            int minLength = Mathf.Min(a.Length, b.Length);
            int currDiff = lengthDifference;

            for (int i = 0; i < minLength; i++)
            {
                if (a[i] != b[i])
                {
                    currDiff++;

                    if (currDiff >= distanceThreshold)
                        return true;
                }
            }

            return false;
        }

        public static void OpenDocumentationPage()
        {
            Help.BrowseURL("https://www.notion.so/spatialxr/Spatial-Unity-Creator-Toolset-Documentation-73cc001642764c8bab70722485af5dfc");
        }

        public static string CreateFolderHierarchy(params string[] folders)
        {
            string path = "Assets";
            foreach (string folder in folders)
            {
                string newPath = path + $"/{folder}";

                if (!AssetDatabase.IsValidFolder(newPath))
                    AssetDatabase.CreateFolder(path, folder);

                path = newPath;
            }

            return path;
        }

        public static void OpenSandboxInBrowser()
        {
            Application.OpenURL("https://staging.spatial.io/sandbox"); // TODO: change to prod
        }
    }
}
