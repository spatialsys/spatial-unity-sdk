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

        public class PackageComponentStatusCollection
        {
            public ComponentStatus defaultStatus = ComponentStatus.Blocked;
            public Dictionary<string, ComponentStatus> validatorIDToComponentStatus;

            public ComponentStatus GetComponentStatusForPackageType(PackageConfig packageConfig)
            {
                if (validatorIDToComponentStatus != null && validatorIDToComponentStatus.TryGetValue(packageConfig.validatorID, out ComponentStatus status))
                    return status;

                return defaultStatus;
            }

            public static PackageComponentStatusCollection AllowAll()
            {
                return new PackageComponentStatusCollection() { defaultStatus = ComponentStatus.Allowed };
            }

            public static PackageComponentStatusCollection Allow(params string[] validatorIDs)
            {
                var statusCollection = new PackageComponentStatusCollection();
                statusCollection.validatorIDToComponentStatus = new Dictionary<string, ComponentStatus>();
                foreach (string id in validatorIDs)
                    statusCollection.validatorIDToComponentStatus.Add(id, ComponentStatus.Allowed);
                return statusCollection;
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

            for (int row = 0; row < table.rowCount; row++)
            {
                string typeFullName = table[row][0];
                Type type = ReflectionCacheUtility.GetTypeFromFullNameCached(typeFullName);
                if (type == null)
                {
                    Debug.LogWarning($"Could not find type {typeFullName} in the reflection cache. The component type allowlist might be outdated.");
                    continue;
                }

                var statusCollection = new PackageComponentStatusCollection();
                statusCollection.validatorIDToComponentStatus = new Dictionary<string, ComponentStatus>();

                // Column 0 is the component type name. Every column after should be validator IDs.
                for (int col = 1; col < table.columnCount; col++)
                {
                    string validatorID = table.GetColumnName(col);
                    ComponentStatus compStatus = table.GetComponentStatusAtCell(row, col);
                    statusCollection.validatorIDToComponentStatus.Add(validatorID, compStatus);
                }

                componentTypeStatuses[type] = statusCollection;
            }
        }

        private static ComponentStatus GetComponentStatusAtCell(this CsvTable table, int row, int column)
        {
            string cellValue = table[row][column];
            return (cellValue == "Y" || cellValue == "y") ? ComponentStatus.Allowed : ComponentStatus.Blocked;
        }
    }
}
