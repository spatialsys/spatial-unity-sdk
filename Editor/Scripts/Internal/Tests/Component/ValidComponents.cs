using CsvLib;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class ValidComponents
    {
        public enum ComponentStatus
        {
            Blocked = 0,
            Allowed = 1
        }

        public struct PackageComponentStatusCollection
        {
            public ComponentStatus spaceStatus;
            public ComponentStatus spaceTemplateStatus;
            public ComponentStatus globalAvatarStatus;
            public ComponentStatus worldAvatarStatus;
            public ComponentStatus avatarAnimationStatus;
            public ComponentStatus prefabObjectStatus;

            public ComponentStatus GetComponentStatusForPackageType(PackageConfig packageConfig)
            {
                switch (packageConfig.packageType)
                {
                    case PackageType.Space:
                        return spaceStatus;
                    case PackageType.SpaceTemplate:
                        return spaceTemplateStatus;
                    case PackageType.Avatar:
                        AvatarConfig avatarConfig = packageConfig as AvatarConfig;
                        return avatarConfig.usageContext == AvatarConfig.Scope.Global ? globalAvatarStatus : worldAvatarStatus;
                    case PackageType.AvatarAnimation:
                        return avatarAnimationStatus;
                    case PackageType.PrefabObject:
                        return prefabObjectStatus;
                }

                // Unhandled package types allows all component by default for convenience.
                return ComponentStatus.Allowed;
            }

            public static PackageComponentStatusCollection AllowAll()
            {
                return new PackageComponentStatusCollection() {
                    spaceStatus = ComponentStatus.Allowed,
                    spaceTemplateStatus = ComponentStatus.Allowed,
                    globalAvatarStatus = ComponentStatus.Allowed,
                    worldAvatarStatus = ComponentStatus.Allowed,
                    avatarAnimationStatus = ComponentStatus.Allowed,
                    prefabObjectStatus = ComponentStatus.Allowed
                };
            }
        }

        private static bool _initialized = false;

        public static readonly Dictionary<Type, PackageComponentStatusCollection> componentTypeStatuses = new();

        public static bool IsComponentTypeAllowedForPackageType(PackageConfig packageConfig, Type componentType)
        {
            if (componentType == null)
                return false;

            InitializeIfNecessary();

            // Prioritize subclasses over their base classes.
            Type currentTargetType = null;
            ComponentStatus currentTargetStatus = ComponentStatus.Allowed;

            foreach (KeyValuePair<Type, PackageComponentStatusCollection> pair in componentTypeStatuses)
            {
                Type targetTypeCandidate = pair.Key;
                ComponentStatus targetStatusCandidate = pair.Value.GetComponentStatusForPackageType(packageConfig);

                if (targetTypeCandidate == componentType)
                {
                    // The candidate is the component, so we can evaluate the status directly and stop searching.
                    currentTargetType = targetTypeCandidate;
                    currentTargetStatus = targetStatusCandidate;
                    break;
                }

                if (currentTargetType == null && targetTypeCandidate.IsAssignableFrom(componentType))
                {
                    // The candidate is an interface that the component implements or a base class of the component type.
                    // This is also the first type we encountered that satisfies our condition, so immediately set the "baseline" target.
                    currentTargetType = targetTypeCandidate;
                    currentTargetStatus = targetStatusCandidate;
                }
                else if (currentTargetType != null && targetTypeCandidate != currentTargetType && currentTargetType.IsAssignableFrom(targetTypeCandidate))
                {
                    // We already selected a candidate before, so we should only override the component status if one of the conditions below is true:
                    // 1) The current type is a class, and this candidate type is a subclass of it.
                    // 2) The current type is an interface, which this candidate type implements.
                    currentTargetType = targetTypeCandidate;
                    currentTargetStatus = targetStatusCandidate;
                }
            }

            return currentTargetType != null && currentTargetStatus == ComponentStatus.Allowed;
        }

        private static void InitializeIfNecessary()
        {
            if (_initialized)
                return;

            UnityEngine.Object csvFileAssetRef = Resources.Load("component_types_list", typeof(TextAsset));
            string csvFilePath = AssetDatabase.GetAssetPath(csvFileAssetRef);
            var table = new CsvTable(csvFilePath);
            _initialized = true;

            int spaceColumn = table.GetColumnIndex(PackageType.Space.ToString());
            int spaceTemplateColumn = table.GetColumnIndex(PackageType.SpaceTemplate.ToString());
            int globalAvatarColumn = table.GetColumnIndex("GlobalAvatar");
            int worldAvatarColumn = table.GetColumnIndex("WorldAvatar");
            int avatarAnimationColumn = table.GetColumnIndex(PackageType.AvatarAnimation.ToString());
            int prefabObjectColumn = table.GetColumnIndex(PackageType.PrefabObject.ToString());

            for (int row = 0; row < table.rowCount; row++)
            {
                string typeFullName = table[row][0];
                Type type = ReflectionCacheUtility.GetTypeFromFullNameCached(typeFullName);
                if (type == null)
                {
                    Debug.LogWarning($"Could not find type {typeFullName} in the reflection cache. The component type allowlist might be outdated.");
                    continue;
                }
                componentTypeStatuses[type] = new PackageComponentStatusCollection() {
                    spaceStatus = table.GetComponentStatusAtCell(row, spaceColumn),
                    spaceTemplateStatus = table.GetComponentStatusAtCell(row, spaceTemplateColumn),
                    globalAvatarStatus = table.GetComponentStatusAtCell(row, globalAvatarColumn),
                    worldAvatarStatus = table.GetComponentStatusAtCell(row, worldAvatarColumn),
                    avatarAnimationStatus = table.GetComponentStatusAtCell(row, avatarAnimationColumn),
                    prefabObjectStatus = table.GetComponentStatusAtCell(row, prefabObjectColumn)
                };
            }
        }

        private static ComponentStatus GetComponentStatusAtCell(this CsvTable table, int row, int column)
        {
            string cellValue = table[row][column];
            return (cellValue == "Y" || cellValue == "y") ? ComponentStatus.Allowed : ComponentStatus.Blocked;
        }
    }
}
