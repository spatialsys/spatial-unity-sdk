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

        private static bool _clearedWhileFetching = false;

        [InitializeOnLoadMethod]
        private static void OnScriptsReloaded()
        {
            // Only fetch badges if a world id exists on this project
            if (AuthUtility.isAuthenticated && string.IsNullOrEmpty(ProjectConfig.defaultWorldID))
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
            if (!AuthUtility.isAuthenticated)
            {
                ClearWorlds();
                return Promise.Resolved();
            }

            initialFetchComplete = true;
            isFetchingWorlds = true;
            _clearedWhileFetching = false;
            return SpatialAPI.GetWorlds()
                .Then(resp => {
                    if (!_clearedWhileFetching)
                        worlds = resp.worlds;
                })
                .Finally(() => {
                    isFetchingWorlds = false;
                    _clearedWhileFetching = false;
                });
        }

        public static void ClearWorlds()
        {
            worlds = new SpatialAPI.World[0];
            _clearedWhileFetching = true;
        }
    }
}