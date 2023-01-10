using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public static class EditorUtility
    {
        public const string CLIENT_UNITY_VERSION = "2021.3.8f1";
        private const string AUTH_TOKEN_KEY = "SpatialSDK_Token";

        public static bool isUsingSupportedUnityVersion => Application.unityVersion == CLIENT_UNITY_VERSION;

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
            Help.BrowseURL(UpgradeUtility.packageInfo.documentationUrl);
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
            Application.OpenURL($"https://{SpatialAPI.SPATIAL_ORIGIN}/sandbox");
        }
    }
}
