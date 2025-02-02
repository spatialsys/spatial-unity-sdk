using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public static class EditorUtility
    {
        public const string STORAGE_DIRECTORY = "Assets/Spatial";
        public const string AUTOGENERATED_ASSETS_DIRECTORY = STORAGE_DIRECTORY + "/Generated";

        public static readonly (Version, Version)[] SUPPORTED_UNITY_VERSION_RANGES = {
            (new Version(2021, 3, 8), new Version(2021, 3, 21)),
            (new Version(2021, 3, 44), new Version(2021, 3, 44)) // There may have been some crash introduced in between 2021.3.21 and 2021.3.44, so we don't want to risk breaking changes.
        };

        public static bool isUsingSupportedUnityVersion
        {
            get
            {
                Version currentUnityVersion = GetParsedUnityVersion(Application.unityVersion);
                if (currentUnityVersion == null)
                    return false;

                foreach ((Version min, Version max) in SUPPORTED_UNITY_VERSION_RANGES)
                {
                    if (currentUnityVersion >= min && currentUnityVersion <= max)
                        return true;
                }

                return false;
            }
        }

        public static readonly HashSet<string> defaultTags = new HashSet<string> {
            "Untagged",
            "Respawn",
            "Finish",
            "EditorOnly",
            "MainCamera",
            "Player",
            "GameController",
        };

        public static readonly HashSet<string> strippableEditorComponents = new HashSet<string> {
            "ProBuilderMesh",
            "ProBuilderShape",
            "BezierShape",
            "PolyShape",
            "Entity",
        };

        public static string GetSemanticUnityVersion(string versionString)
        {
            int revisionIndex = versionString.IndexOf('f');
            return revisionIndex > -1 ? versionString.Substring(0, revisionIndex) : versionString;
        }

        public static Version GetParsedUnityVersion(string versionString)
        {
            try
            {
                return new Version(GetSemanticUnityVersion(versionString));
            }
            catch
            {
                Debug.LogError($"Failed to parse Unity version string '{versionString}'; Expected format: X.X.X");
                return null;
            }
        }

        private static MethodInfo _getImportedAssetImportDependenciesAsGUIDs;
        private static MethodInfo _getSourceAssetImportDependenciesAsGUIDs;

        static EditorUtility()
        {
            // Get references to internal AssetDatabase methods
            // string[] AssetDatabase.GetSourceAssetImportDependenciesAsGUIDs(string path)
            // string[] AssetDatabase.GetImportedAssetImportDependenciesAsGUIDs(string path)
            _getImportedAssetImportDependenciesAsGUIDs = typeof(AssetDatabase).GetMethod("GetImportedAssetImportDependenciesAsGUIDs", BindingFlags.NonPublic | BindingFlags.Static);
            _getSourceAssetImportDependenciesAsGUIDs = typeof(AssetDatabase).GetMethod("GetSourceAssetImportDependenciesAsGUIDs", BindingFlags.NonPublic | BindingFlags.Static);
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

        public static T LoadAssetFromPackagePath<T>(string relativePath) where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath<T>($"{PackageManagerUtility.PACKAGE_DIRECTORY_PATH}/{relativePath}");
        }

        public static void OpenDocumentationPage()
        {
            Help.BrowseURL(PackageManagerUtility.documentationUrl);
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

        public static string TruncateTextForUI(string text, bool fast = false)
        {
            // Text elements are limited to 49152 vertices (weirdly specific).
            // Each non-whitespace character (space, new lines, etc) uses 2 triangles, or 4 vertices to form a rect.
            // So only 12288 non-whitespace characters are allowed. Truncate earlier to add a little buffer.
            const int MAX_LENGTH_NO_WHITESPACE = 12200;

            bool doTruncate = false;
            int substrLen = 0;
            if (text?.Length > MAX_LENGTH_NO_WHITESPACE)
            {
                if (fast)
                {
                    // Truncate without accounting for whitespace characters. This may cause the string to render less than what's actually supported.
                    doTruncate = true;
                    substrLen = MAX_LENGTH_NO_WHITESPACE;
                }
                else
                {
                    // NOTE: Not slow if you're only doing this once. Consider the "fast" option if you're processing many UI elements.
                    int nonWhiteSpaceCount = 0;

                    foreach (char c in text)
                    {
                        if (!char.IsWhiteSpace(c))
                        {
                            nonWhiteSpaceCount++;

                            if (nonWhiteSpaceCount > MAX_LENGTH_NO_WHITESPACE)
                            {
                                // This character will cause renderer to exceed the vertex limit, so don't include it and break out.
                                doTruncate = true;
                                break;
                            }
                        }

                        substrLen++;
                    }
                }
            }

            if (doTruncate)
            {
                const string TRUNCATED_MSG = "...<truncated>";
                return text.Substring(0, substrLen - TRUNCATED_MSG.Length) + TRUNCATED_MSG;
            }

            return text;
        }

        /// <summary>
        /// Replaces the middle part of the string with ellipsis if it exceeds max length.
        /// </summary>
        public static string TruncateFromMiddle(string text, int maxLength)
        {
            // Max length includes the ellipsis when truncating, so clamp value to avoid index out of range.
            maxLength = Mathf.Max(7, maxLength);
            if (text.Length <= maxLength)
                return text;

            // Get start and end index of the middle segment to remove.
            int startIndex = (maxLength / 2) - 2;
            int endIndex = startIndex + text.Length - maxLength + 5;
            return text.Substring(0, startIndex) + "[...]" + text.Substring(endIndex);
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

        public static bool IsEditorOnlyType(this Type type)
        {
            return type != null && type.GetCustomAttributes(typeof(EditorOnlyAttribute), inherit: true).Length > 0;
        }

        public static bool IsStrippableEditorOnlyType(this Type type)
        {
            return strippableEditorComponents.Contains(type.Name);
        }

        /// <summary>
        /// Removes the `target` component and any dependent components on the game object (if necessary) recursively.
        /// </summary>
        public static void RemoveComponentAndDependents(Component target)
        {
            if (target == null)
                return; // Component or script is missing. Use RemoveMonoBehavioursWithMissingScript instead.

            Type targetType = target.GetType();
            GameObject attachedGO = target.gameObject;
            bool isLastComponent = attachedGO.GetComponents(targetType).Length == 1;

            // Remove any dependent components before removing the target component.
            // We only need to do this when it's the last component instance on the object.
            if (isLastComponent)
            {
                IEnumerable<Type> otherAttachedComponentTypes = attachedGO.GetComponents<Component>()
                    .Select(comp => comp.GetType())
                    .Distinct()
                    .Where(type => type != targetType);

                foreach (Type t in otherAttachedComponentTypes)
                {
                    if (t.RequiresComponentType(targetType))
                    {
                        Component[] dependentComps = target.GetComponents(t);
                        foreach (Component c in dependentComps)
                            RemoveComponentAndDependents(c);
                    }
                }
            }

            UnityEngine.Object.DestroyImmediate(target);
        }

        /// <summary>
        /// Returns true if this type requires `componentTypeToCheck` type (via the RequireComponent attribute)
        /// </summary>
        public static bool RequiresComponentType(this Type targetComponentType, Type componentTypeToCheck)
        {
            if (targetComponentType == null || componentTypeToCheck == null)
                return false;

            IEnumerable<RequireComponent> allRequireComponentAttributes = targetComponentType.GetCustomAttributes<RequireComponent>(inherit: true);
            foreach (RequireComponent requireAttr in allRequireComponentAttributes)
            {
                if (requireAttr.m_Type0 == componentTypeToCheck || requireAttr.m_Type1 == componentTypeToCheck || requireAttr.m_Type2 == componentTypeToCheck)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a backup file of the specified asset, which will allow for completely reverting all changes easily.
        /// Use RestoreAssetFromBackup to revert the asset to the backed up state.
        /// </summary>
        public static void CreateAssetBackup(UnityEngine.Object asset)
        {
            string origPath = AssetDatabase.GetAssetPath(asset);
            File.Copy(origPath, $"{origPath}.backup~", overwrite: true);
            File.Copy($"{origPath}.meta", $"{origPath}.meta.backup~", overwrite: true);
        }

        /// <summary>
        /// Deletes the backup file of the specified asset without restoring, if there is one found. Does nothing if there was no backup created.
        /// </summary>
        public static void DeleteAssetBackup(UnityEngine.Object asset)
        {
            string origPath = AssetDatabase.GetAssetPath(asset);
            string backupPath = origPath + ".backup~";
            string backupMetaPath = $"{origPath}.meta.backup~";
            if (File.Exists(backupPath))
                File.Delete(backupPath);
            if (File.Exists(backupMetaPath))
                File.Delete(backupMetaPath);
        }

        /// <summary>
        /// Tries to restore the asset from the backup file and deletes the backup afterwards. Does nothing if there was no backup created.
        /// </summary>
        public static void RestoreAssetFromBackup(UnityEngine.Object asset)
        {
            string origPath = AssetDatabase.GetAssetPath(asset);

            string backupPath = origPath + ".backup~";
            if (!File.Exists(backupPath))
                return;

            // Deleting the current asset seems to be the only way we can effectively get unity editor to re-import the asset.
            // SetDirty doesn't do it
            AssetDatabase.DeleteAsset(origPath);

            // Restore
            File.Copy(backupPath, origPath, overwrite: true);
            File.Copy($"{origPath}.meta.backup~", $"{origPath}.meta", overwrite: true);
            AssetDatabase.ImportAsset(origPath, ImportAssetOptions.ForceSynchronousImport);

            DeleteAssetBackup(asset);
        }

        /// <summary>
        /// Gets all paths to the asset's dependencies and attempts to add them to an existing HashSet
        /// GetDependencies doesn't include all "import time" dependencies, so we need to manually add them
        /// See: https://forum.unity.com/threads/discrepancy-between-assetdatabase-getdependencies-and-results-of-exportpackage.1295025/
        /// </summary>
        public static void UnionWithAssetDependenciesPaths(HashSet<string> assetPaths, UnityEngine.Object asset)
        {
            if (asset == null)
                return;

            string[] dependencies = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(asset));
            assetPaths.UnionWith(dependencies);

            foreach (string dep in dependencies)
            {
                string[] importTimeDependencies = _getImportedAssetImportDependenciesAsGUIDs.Invoke(null, new object[] { dep }) as string[];
                if (importTimeDependencies.Length > 0)
                    assetPaths.UnionWith(importTimeDependencies.Select(d => AssetDatabase.GUIDToAssetPath(d)));

                string[] sourceAssetImportTimeDependencies = _getSourceAssetImportDependenciesAsGUIDs.Invoke(null, new object[] { dep }) as string[];
                if (sourceAssetImportTimeDependencies.Length > 0)
                    assetPaths.UnionWith(sourceAssetImportTimeDependencies.Select(d => AssetDatabase.GUIDToAssetPath(d)));
            }
        }

        /// <summary>
        /// Saves the asset immediately, regardless if it was dirty or not.
        /// </summary>
        public static void SaveAssetImmediately(UnityEngine.Object asset)
        {
            UnityEditor.EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
        }
    }
}
