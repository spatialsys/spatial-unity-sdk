using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    [InitializeOnLoad]
    public static class AddressablesUtility
    {
        private const string DEFAULT_ADDRESSABLES_BUILD_PATH = BuildUtility.SANDBOX_ADDRESSABLES_BUILD_PATH;
        private const string INIT_ADDRESSABLES_MENU_ITEM_PATH = "Spatial SDK/Utilities/Initialize Addressables for Project";
        private const string HAS_BUILT_ADDRESSABLES_SESSION_STATE_KEY = "SpatialSDK_HasBuiltAddressables";

        // This folder might be moved in the future... but for now you can't change it from the default.
        public const string DATA_FOLDER_PATH = AddressableAssetSettingsDefaultObject.kDefaultConfigFolder;
        public const string CATALOG_FILE_SUFFIX = "spatial";

        public static bool hasBuiltAddressables
        {
            get => SessionState.GetBool(HAS_BUILT_ADDRESSABLES_SESSION_STATE_KEY, defaultValue: false);
            private set => SessionState.SetBool(HAS_BUILT_ADDRESSABLES_SESSION_STATE_KEY, value);
        }

        /// <summary>
        /// Returns true if the project has Addressables set up.
        /// </summary>
        public static bool isActiveInProject => Directory.Exists(DATA_FOLDER_PATH) &&
            AddressableAssetSettingsDefaultObject.SettingsExists &&
            Directory.Exists(AddressableAssetSettingsDefaultObject.Settings.GroupFolder);

        static AddressablesUtility()
        {
            SetEditorPlayModeDataBuilder();

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
                SetEditorPlayModeDataBuilder();
        }

        /// <summary>
        /// Builds Addressables for the active build target.
        /// Returns false if the Addressables build failed with an error. Otherwise, true.
        /// To determine whether any Addressable output has been created, use `hasBuiltAddressables`.
        /// </summary>
        public static bool BuildAddressablesIfNecessary(string buildPath, bool useBrotliCompression)
        {
            hasBuiltAddressables = false;

            if (!isActiveInProject || !ProjectConfig.activePackageConfig.supportsAddressables)
                return true;

            if (!HasContentToBuild())
            {
                Debug.LogWarning("Skipping Addressables build because there was no content detected. Ensure you have added at least one asset to a group.");
                return true;
            }

            SetUpAddressablesConfig(buildPath, useBrotliCompression);

            // Delete build directory to avoid uploading bundles from a previous unrelated build
            if (Directory.Exists(buildPath))
                Directory.Delete(buildPath, recursive: true);

            AddressableAssetSettings.CleanPlayerContent();
            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult buildResult);
            bool success = string.IsNullOrEmpty(buildResult.Error);
            hasBuiltAddressables = success && buildResult.AssetBundleBuildResults.Count > 0;
            return success;
        }

        [MenuItem(INIT_ADDRESSABLES_MENU_ITEM_PATH, true)]
        private static bool InitializeNewProject_ValidateMenuItem()
        {
            return !isActiveInProject;
        }

        /// <summary>
        /// Initializes Addressable configs and assets within the current project.
        /// </summary>
        [MenuItem(INIT_ADDRESSABLES_MENU_ITEM_PATH, priority = 100)]
        public static void InitializeNewProject()
        {
            if (isActiveInProject)
            {
                Debug.LogError("Addressables is already initialized in this project");
                return;
            }

            // Delete pre-existing settings asset if there's any, otherwise the setup below won't do anything.
            if (AddressableAssetSettingsDefaultObject.Settings != null)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(AddressableAssetSettingsDefaultObject.Settings));

            // Initializes Addressables data folder hierarchy
            AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create(
                AddressableAssetSettingsDefaultObject.kDefaultConfigFolder,
                AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName,
                createDefaultGroups: true,
                isPersisted: true
            );

            // Writes enforced settings to the default assets to minimize tracked changes in version control.
            SetUpAddressablesConfig(DEFAULT_ADDRESSABLES_BUILD_PATH, useBrotliCompression: false);
        }

        /// <summary>
        /// Sets up the addressable configs to be compatible with Spatial
        /// </summary>
        public static void SetUpAddressablesConfig(string buildPath, bool useBrotliCompression)
        {
            if (!isActiveInProject)
                throw new System.InvalidOperationException("Addressables should be initialized in the project first");

            BuildTarget targetPlatform = EditorUserBuildSettings.activeBuildTarget;

            AddressableAssetSettings addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetProfileSettings profileSettings = addressableSettings.profileSettings;

            // Outputs catalog file to a consistent location, instead of appending a timestamp.
            addressableSettings.OverridePlayerVersion = CATALOG_FILE_SUFFIX;
            // MonoScript bundle build needs to be enabled for C# scripting to work.
            addressableSettings.MonoScriptBundleNaming = MonoScriptBundleNaming.DefaultGroupGuid;
            // Also avoid shaders from conflicting with internal shader types.
            addressableSettings.ShaderBundleNaming = ShaderBundleNaming.DefaultGroupGuid;
            // Helps with build times and runtime memory.
            addressableSettings.NonRecursiveBuilding = true;

            // Enforce scriptable build defaults.
            int targetBuilderIndex = addressableSettings.DataBuilders.FindIndex(x => x is BuildScriptPackedMode);
            if (targetBuilderIndex <= -1)
                throw new System.Exception($"Cannot find any valid data builders. Only {nameof(BuildScriptPackedMode)} is supported currently.");
            addressableSettings.ActivePlayerDataBuilderIndex = targetBuilderIndex;
            addressableSettings.InitializationObjects.Clear();

            // Debug settings
            if (ProjectConfigData.PostProfilerEvents)
                ProjectConfigData.PostProfilerEvents = false;
            addressableSettings.buildSettings.LogResourceManagerExceptions = true;

            // Enforce build and load paths
            const string BUILD_PATH_PROFILE_KEY = "SpatialBuildPath";
            const string LOAD_PATH_PROFILE_KEY = "SpatialLoadPath";
            const string LOAD_PATH = "{SpatialSys.Client.AddressablesManager.spaceBundlesURLPrefix}";
            profileSettings.CreateValue(BUILD_PATH_PROFILE_KEY, buildPath);
            profileSettings.CreateValue(LOAD_PATH_PROFILE_KEY, LOAD_PATH);
            profileSettings.SetValue(addressableSettings.activeProfileId, BUILD_PATH_PROFILE_KEY, buildPath);
            profileSettings.SetValue(addressableSettings.activeProfileId, LOAD_PATH_PROFILE_KEY, LOAD_PATH);

            // Remote catalog is the only type of usable catalog.
            addressableSettings.BuildRemoteCatalog = true;
            addressableSettings.RemoteCatalogBuildPath.SetVariableByName(addressableSettings, BUILD_PATH_PROFILE_KEY);
            addressableSettings.RemoteCatalogLoadPath.SetVariableByName(addressableSettings, LOAD_PATH_PROFILE_KEY);

            foreach (AddressableAssetGroup group in addressableSettings.groups)
            {
                if (group == null)
                    continue;

                var bundleSchema = group.GetSchema<BundledAssetGroupSchema>();
                if (bundleSchema != null)
                {
                    // Only concerned with enforcing settings on groups that we will be building.
                    if (!bundleSchema.IncludeInBuild)
                        continue;

                    bundleSchema.BuildPath.SetVariableByName(addressableSettings, BUILD_PATH_PROFILE_KEY);
                    bundleSchema.LoadPath.SetVariableByName(addressableSettings, LOAD_PATH_PROFILE_KEY);

                    bundleSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.AppendHash;
                    bundleSchema.UseAssetBundleCache = true;
                    bundleSchema.UseAssetBundleCrc = !useBrotliCompression; // CRC breaks when compressing with an unknown format (when downloaded, it's expecting uncompressed CRC).
                    bundleSchema.UseAssetBundleCrcForCachedBundles = false; // Disabled for performance.

                    if (targetPlatform == BuildTarget.WebGL)
                    {
                        if (useBrotliCompression)
                        {
                            // Uncompressed so that Brotli compression can be performed as a post-step.
                            bundleSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.Uncompressed;
                        }
                        else
                        {
                            // LZMA isn't supported on web, so enforce LZ4.
                            bundleSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
                        }
                    }
                    else
                    {
                        bundleSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZMA;
                    }
                }

                var playerDataSchema = group.GetSchema<PlayerDataGroupSchema>();
                if (playerDataSchema != null)
                    playerDataSchema.IncludeBuildSettingsScenes = false;
            }

            // Apply modified settings
            UnityEditor.EditorUtility.SetDirty(addressableSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        /// <summary>
        /// Populates the list of registered asset groups to all supported group types that are found under the AssetGroups folder.
        /// </summary>
        public static void AutoAssignAddressableAssetGroups()
        {
            if (!isActiveInProject)
                throw new System.InvalidOperationException("Addressables should be initialized in the project first");

            AddressableAssetSettings addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            addressableSettings.groups.Clear();

            string[] groupAssetGUIDs = AssetDatabase.FindAssets($"t:{nameof(AddressableAssetGroup)}", new string[] { addressableSettings.GroupFolder });
            foreach (string guid in groupAssetGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var groupAsset = AssetDatabase.LoadAssetAtPath<AddressableAssetGroup>(path);
                if (groupAsset == null)
                    continue;
                addressableSettings.groups.Add(groupAsset);
                addressableSettings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupAdded, groupAsset, postEvent: true);
            }

            if (addressableSettings.groups.Count > 0)
            {
                // Assign default group to the first group that contains the keyword "default".
                AddressableAssetGroup defaultGroup = addressableSettings.FindGroup(
                    group => group.Name.ToLowerInvariant().Contains("default") && group.CanBeSetAsDefault()
                );

                // Otherwise, just assign to the first group.
                if (defaultGroup == null)
                    defaultGroup = addressableSettings.FindGroup(group => group.CanBeSetAsDefault());

                if (defaultGroup != null)
                    addressableSettings.DefaultGroup = defaultGroup;
            }

            // Apply modified settings
            UnityEditor.EditorUtility.SetDirty(addressableSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        public static void AddAssetPathsForExportedPackage(HashSet<string> assetPaths)
        {
            if (!isActiveInProject || !ProjectConfig.activePackageConfig.supportsAddressables)
                return;

            IReadOnlyList<Object> activeAddressableAssets = GetAllActiveAddressableAssets();
            if (activeAddressableAssets == null || activeAddressableAssets.Count == 0)
            {
                Debug.LogWarning("Skipping package export for Addressable assets because there was no content detected. Ensure you have added at least one asset to a group.");
                return;
            }

            AddressableAssetSettings addressableSettings = AddressableAssetSettingsDefaultObject.Settings;

            // Only add the asset groups and their schemas. All other config assets do not need to be packaged.
            foreach (AddressableAssetGroup group in addressableSettings.groups)
            {
                if (group == null)
                    continue;

                // Skip groups with bundle schemas that aren't included in build.
                var bundleSchema = group.GetSchema<BundledAssetGroupSchema>();
                if (bundleSchema != null && !bundleSchema.IncludeInBuild)
                    continue;

                assetPaths.Add(AssetDatabase.GetAssetPath(group));
                foreach (AddressableAssetGroupSchema schema in group.Schemas.Where(schema => schema != null))
                    assetPaths.Add(AssetDatabase.GetAssetPath(schema));
            }

            // Add dependencies of all Addressable asset entries.
            foreach (Object asset in activeAddressableAssets)
                EditorUtility.UnionWithAssetDependenciesPaths(assetPaths, asset);
        }

        /// <summary>
        /// Returns true if there is at least one Addressable entry active in the configuration.
        /// </summary>
        public static bool HasContentToBuild()
        {
            if (!isActiveInProject)
                return false;

            AddressableAssetSettings addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            bool hasSomeContent = false;

            foreach (AddressableAssetGroup group in addressableSettings.groups)
            {
                if (group == null)
                    continue;

                BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
                if (schema == null)
                    continue;

                bool groupHasContent = schema.IncludeInBuild && group.entries.Count > 0;
                hasSomeContent |= groupHasContent;
            }

            return hasSomeContent;
        }

        /// <summary>
        /// Returns a list of all Addressable asset entries that are in groups that will be included in build.
        /// </summary>
        public static IReadOnlyList<Object> GetAllActiveAddressableAssets()
        {
            if (!isActiveInProject)
                return null;

            AddressableAssetSettings addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            List<AddressableAssetEntry> entries = new();
            List<Object> assets = new();
            foreach (AddressableAssetGroup group in addressableSettings.groups)
            {
                if (group == null)
                    continue;

                var bundleSchema = group.GetSchema<BundledAssetGroupSchema>();
                if (bundleSchema == null || !bundleSchema.IncludeInBuild)
                    continue;

                entries.Clear();
                group.GatherAllAssets(entries, includeSelf: true, recurseAll: true, includeSubObjects: false,
                    entryFilter: (entry) => entry.MainAsset != null && !entry.IsScene
                );

                foreach (AddressableAssetEntry entry in entries)
                    assets.Add(entry.MainAsset);
            }

            return assets;
        }

        private static void SetEditorPlayModeDataBuilder()
        {
            if (!isActiveInProject)
                return;

            // Enforce "Asset Database" to be used during editor play mode, otherwise there can be loading issues.
            AddressableAssetSettings addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            int targetPlayModeBuilderIndex = addressableSettings.DataBuilders.FindIndex(x => x is BuildScriptFastMode);
            if (targetPlayModeBuilderIndex >= 0)
            {
                // This isn't stored or serialized to the asset, so no need to set dirty or refresh asset DB.
                addressableSettings.ActivePlayModeDataBuilderIndex = targetPlayModeBuilderIndex;
            }
            else
            {
                Debug.LogError($"Failed to find {nameof(BuildScriptFastMode)} in data builders list. Addressables may fail to load in editor.");
            }
        }
    }
}