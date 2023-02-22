using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

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

        public static string GetAssetBundleName(Object asset)
        {
            if (asset == null)
                return null;

            string assetPath = AssetDatabase.GetAssetPath(asset);
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
                return null;

            return importer.assetBundleName;
        }

        public static void OpenSandboxInBrowser()
        {
            Application.OpenURL($"https://{SpatialAPI.SPATIAL_ORIGIN}/sandbox");
        }

        public static IEnumerable<T> FindAssetsByType<T>() where T : Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            foreach (var t in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(t);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    yield return asset;
                }
            }
        }

        public static bool TryGetCustomAttribute<T>(this MethodInfo method, out T attribute) where T : System.Attribute
        {
            attribute = method.GetCustomAttribute<T>();
            return attribute != null;
        }

        public static string FormatInteger(this int num)
        {
            // Add comma-separators (e.g. 1,000,000)
            return num.ToString("N0");
        }

        public static string AbbreviateNumber(int number)
        {
            if (number < 1000)
            {
                return number.ToString();
            }
            else if (number < 10000)
            {
                return (number / 1000f).ToString("0.#") + "K";
            }
            else if (number < 1000000)
            {
                return (number / 1000).ToString() + "K";
            }
            else if (number < 10000000)
            {
                return (number / 1000000f).ToString("0.#") + "M";
            }
            else
            {
                return (number / 1000000).ToString() + "M";
            }
        }
    }
}
