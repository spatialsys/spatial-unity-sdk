using UnityEngine;
using UnityEditor;
using RSG;

namespace SpatialSys.UnitySDK.Editor
{
    public static class WorldUtility
    {
        public static SpatialAPI.World[] worlds { get; private set; } = new SpatialAPI.World[0];
        public static bool isFetchingWorlds { get; private set; }
        public static bool initialFetchComplete { get; private set; }

        [InitializeOnLoadMethod]
        private static void OnScriptsReloaded()
        {
            // Only fetch badges if a world id exists on this project
            if (EditorUtility.isAuthenticated && string.IsNullOrEmpty(ProjectConfig.defaultWorldID))
            {
                FetchWorlds();
            }
        }

        public static IPromise AssignDefaultWorldToProjectIfNecessary()
        {
            IPromise createWorldPromise = Promise.Resolved();
            if (string.IsNullOrEmpty(ProjectConfig.defaultWorldID))
            {
                createWorldPromise = SpatialAPI.CreateWorld()
                    .Then(resp => {
                        ProjectConfig.defaultWorldID = resp.id;
                    });
            }
            return createWorldPromise;
        }

        public static IPromise FetchWorlds()
        {
            initialFetchComplete = true;
            isFetchingWorlds = true;
            return SpatialAPI.GetWorlds()
                .Then(resp => {
                    worlds = resp.worlds;
                })
                .Finally(() => {
                    isFetchingWorlds = false;
                });
        }
    }
}