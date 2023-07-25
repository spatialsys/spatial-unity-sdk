using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    public static class PrefabObjectPackageTests
    {
        [PackageTest(PackageType.PrefabObject)]
        public static void EnsurePrefabIsAssigned(PrefabObjectConfig config)
        {
            if (config.prefab == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Fail, "A prefab with the Prefab Object component must be assigned in the config.")
                );
            }
        }

        [PackageTest(PackageType.PrefabObject)]
        public static void ValidatePrefabColliders(PrefabObjectConfig config)
        {
            if (config.prefab == null)
                return;

            GameObject prefabInstance = null;
            try
            {
                // Need to temporarily instantiate a prefab instance in order to access collider world-space bounds and activeInHierarchy state
                prefabInstance = ((SpatialPrefabObject)PrefabUtility.InstantiatePrefab(config.prefab)).gameObject;
                var colliders = prefabInstance.GetComponentsInChildren<Collider>(includeInactive: true);

                // NOTE: It's ok if there are issues with some colliders, but there must be at least one collider with no issues. This list is to provide info to the creator on what's failing.
                var issues = new List<string>();
                bool hasAtLeastOneValidCollider = false;
                foreach (Collider c in colliders)
                {
                    string colliderTypeName = c.GetType().Name;
                    if (!c.gameObject.activeInHierarchy || !c.enabled)
                    {
                        issues.Add($"The {colliderTypeName} attached to '{c.name}' is either disabled or on a disabled object");
                        continue;
                    }

                    if (c.isTrigger)
                    {
                        issues.Add($"The {colliderTypeName} attached to '{c.name}' is a trigger, which can't be used to move the object");
                        continue;
                    }

                    // "Flat" 2D colliders should technically be allowed since they're still interactable. Not using bounds volume on purpose.
                    float colliderSize = c.bounds.size.magnitude;
                    const float COLLIDER_SIZE_THRESHOLD = 0.002f;
                    if (colliderSize < COLLIDER_SIZE_THRESHOLD)
                    {
                        float percentOfThreshold = Mathf.Round((colliderSize / COLLIDER_SIZE_THRESHOLD) * 1000f) / 10f; // round to nearest 0.1%
                        issues.Add($"The {colliderTypeName} attached to '{c.name}' is too small ({percentOfThreshold}% of minimum size) to be interacted with");
                        continue;
                    }

                    hasAtLeastOneValidCollider = true;
                }

                if (!hasAtLeastOneValidCollider)
                {
                    string message = "Users will not be able to move this object without a valid collider setup. Add any type of collider that encompasses the object to fix this issue.";

                    if (issues.Count > 0)
                    {
                        message += "\n\nThere were some issues found with existing colliders on this prefab:";
                        foreach (string issue in issues)
                            message += $"\n- {issue}";
                    }

                    SpatialValidator.AddResponse(new SpatialTestResponse(
                        config.prefab,
                        TestResponseType.Fail,
                        "The prefab does not have a valid collider setup",
                        message
                    ));
                }
            }
            finally
            {
                if (prefabInstance != null)
                    Object.DestroyImmediate(prefabInstance);
            }
        }
    }
}
