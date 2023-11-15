using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpatialSys.UnitySDK.Editor
{
    public static class PrefabObjectComponentTests
    {
        [ComponentTest(typeof(SpatialPrefabObject))]
        public static void ValidatePrefabColliders(SpatialPrefabObject prefab)
        {
            Scene previewScene = new Scene();

            try
            {
                bool isAssetObject = AssetDatabase.Contains(prefab) && PrefabUtility.GetPrefabAssetType(prefab) != PrefabAssetType.NotAPrefab;
                SpatialPrefabObject prefabInstance;

                if (isAssetObject)
                {
                    // Need to temporarily instantiate a prefab instance in order to access collider world-space bounds and activeInHierarchy state
                    previewScene = EditorSceneManager.NewPreviewScene();
                    prefabInstance = PrefabUtility.InstantiatePrefab(prefab, previewScene) as SpatialPrefabObject;
                }
                else
                {
                    // Assume that it's already an instance if it's not an asset.
                    prefabInstance = prefab;
                }

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
                        isAssetObject ? prefab : prefabInstance,
                        TestResponseType.Fail,
                        "The prefab does not have a valid collider setup",
                        message
                    ));
                }
            }
            finally
            {
                if (previewScene.IsValid())
                    EditorSceneManager.ClosePreviewScene(previewScene);
            }
        }
    }
}
