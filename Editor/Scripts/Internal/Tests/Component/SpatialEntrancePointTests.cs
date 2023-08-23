using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class SpatialEntrancePointTests
    {
        [ComponentTest(typeof(SpatialEntrancePoint))]
        public static void CheckForColliderBelow(SpatialEntrancePoint target)
        {
            if (!TryGetSpawnPositionForEntrancePoint(target, out Vector3 _))
            {
                // Don't fail this test on build servers because it happens too often
                // Better to let it succeed but have users report issues to us
                var responseType = (Application.isBatchMode) ? TestResponseType.Warning : TestResponseType.Fail;
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    target,
                    responseType,
                    "Entrance point is not above solid ground.",
                    "Make sure that the entrance point is placed above an object with an active collider."
                ));
            }
        }

        [ComponentTest(typeof(SpatialEntrancePoint))]
        public static void CheckSpawnPointIsAboveEnvironmentRespawnLevel(SpatialEntrancePoint target)
        {
            bool hasValidSpawnPosition = TryGetSpawnPositionForEntrancePoint(target, out Vector3 spawnPosition);
            if (!hasValidSpawnPosition)
                return; // Test above handles this.

            float respawnLevelY = EnvironmentSettings.DEFAULT_RESPAWN_LEVEL_Y;
            var envSettingsOverride = Object.FindObjectOfType<SpatialEnvironmentSettingsOverrides>();
            bool hasRespawnLevelOverride = envSettingsOverride != null && envSettingsOverride.environmentSettings != null;
            if (hasRespawnLevelOverride)
                respawnLevelY = envSettingsOverride.environmentSettings.respawnLevelY;

            // Fail if this spawn point is under the respawn level since it will cause avatars to constantly respawn out of their control.
            if (spawnPosition.y < respawnLevelY)
            {
                string sourceStr = hasRespawnLevelOverride ? "(from override)" : "(default value)";
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    target,
                    TestResponseType.Fail,
                    "Entrance point cannot be below the environment's respawn level",
                    "You can either raise the entrance point, or decrease the environment respawn level through the Environment Settings Override component." +
                        $"\nThe current environment's respawn level is {respawnLevelY} meter(s) high {sourceStr}." +
                        $"\nThis entrance point's respawn position is {spawnPosition.y} meter(s) high."
                ));
            }
        }

        private static bool TryGetSpawnPositionForEntrancePoint(SpatialEntrancePoint entrancePoint, out Vector3 spawnPosition)
        {
            // Avatar can't collide with trigger colliders. Ideally, we wouldn't be using default raycast layers here and use the physics collision matrix, but this is fine for now.
            if (Physics.Raycast(entrancePoint.transform.position, Vector3.down, out RaycastHit hit, maxDistance: Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                spawnPosition = hit.point;
                return true;
            }
            else
            {
                spawnPosition = Vector3.zero;
                return false;
            }
        }
    }
}
