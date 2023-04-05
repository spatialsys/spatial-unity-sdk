using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public static class EditorUtility
    {
        private const string AUTH_TOKEN_KEY = "SpatialSDK_Token";

        public const string MIN_UNITY_VERSION_STR = "2021.3.8f1";
        public const string MAX_UNITY_VERSION_STR = "2021.3.21f1";
        private static readonly Version MIN_UNITY_VERSION = GetParsedUnityVersion(MIN_UNITY_VERSION_STR);
        private static readonly Version MAX_UNITY_VERSION = GetParsedUnityVersion(MAX_UNITY_VERSION_STR);
        private static readonly Version CURRENT_UNITY_VERSION = GetParsedUnityVersion(Application.unityVersion);

        public static bool isUsingSupportedUnityVersion => CURRENT_UNITY_VERSION != null && CURRENT_UNITY_VERSION >= MIN_UNITY_VERSION && CURRENT_UNITY_VERSION <= MAX_UNITY_VERSION;

        public static Version GetParsedUnityVersion(string versionString)
        {
            try
            {
                return new Version(versionString.Replace('f', '.'));
            }
            catch
            {
                Debug.LogError($"Failed to parse Unity version string '{versionString}'; Expected format: X.X.XfX");
                return null;
            }
        }

        public static string GetSavedAuthToken()
        {
            return EditorPrefs.GetString(AUTH_TOKEN_KEY);
        }

        public static void SaveAuthToken(string token)
        {
            EditorPrefs.SetString(AUTH_TOKEN_KEY, token);
        }

        public static bool TryGetDateTimeFromEditorPrefs(string key, out DateTime result)
        {
            string dateTicks = EditorPrefs.GetString(key, defaultValue: null);
            if (long.TryParse(dateTicks, out long dateTicksLong))
            {
                result = new DateTime(dateTicksLong);
                return true;
            }

            result = DateTime.UnixEpoch;
            return false;
        }

        public static void SetDateTimeToEditorPrefs(string key, DateTime value)
        {
            EditorPrefs.SetString(key, value.Ticks.ToString());
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

        public static string GetAssetBundleName(UnityEngine.Object asset)
        {
            if (asset == null)
                return null;

            string assetPath = AssetDatabase.GetAssetPath(asset);
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
                return null;

            return importer.assetBundleName;
        }

        /// <summary>
        /// Returns true if it successfully set the asset's bundle name.
        /// </summary>
        public static bool SetAssetBundleName(UnityEngine.Object asset, string bundleName)
        {
            if (asset == null)
                return false;

            string assetPath = AssetDatabase.GetAssetPath(asset);
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
                return false;

            importer.assetBundleName = bundleName;
            importer.SaveAndReimport();
            return true;
        }

        public static void OpenSandboxInBrowser()
        {
            Application.OpenURL($"https://{SpatialAPI.SPATIAL_ORIGIN}/sandbox");
        }

        public static IEnumerable<T> FindAssetsByType<T>() where T : UnityEngine.Object
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

        /// <summary>
        /// Returns true if the method's parameter signature matches up with the parameter `targetTypes` list:
        /// <para>1. The amount of parameters must be the same length as `targetTypes`</para>
        /// <para>2. The order of parameters must match</para>
        /// <para>3. The type must be assignable to each type in `targetTypes`</para>
        /// </summary>
        public static bool ValidateParameterTypes(this MethodInfo method, params System.Type[] targetTypes)
        {
            if (method == null || targetTypes == null)
                return false;

            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length != targetTypes.Length)
                return false;

            for (int i = 0; i < targetTypes.Length; i++)
            {
                if (!targetTypes[i].IsAssignableFrom(parameters[i].ParameterType))
                    return false;
            }

            return true;
        }

        public static string FormatNumber(this int num)
        {
            // Add comma-separators (e.g. 1,000,000)
            return num.ToString("N0");
        }

        public static string FormatNumber(this long num)
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

        public static string GetComponentNamesWithInstanceCountString(IEnumerable<Component> components)
        {
            IEnumerable<Type> componentTypes = components.Select(cmp => cmp.GetType());

            return string.Join(
                '\n',
                componentTypes
                    .Distinct()
                    .Select(type => {
                        int typeInstanceCount = componentTypes.Count(t => t == type);
                        return $"- {typeInstanceCount} instance(s) of {type.Name}";
                    })
            );
        }

        public static bool IsPlatformModuleInstalled(BuildTarget targetPlatform)
        {
            // Internal editor class must be accessed through reflection
            var moduleManager = Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
            MethodInfo isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget", BindingFlags.Static | BindingFlags.NonPublic);
            string moduleName = (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { targetPlatform });
            return (bool)isPlatformSupportLoaded.Invoke(null, new object[] { moduleName });
        }
    }
}
