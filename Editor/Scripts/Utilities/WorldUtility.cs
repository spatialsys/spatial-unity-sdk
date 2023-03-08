using UnityEngine;
using UnityEditor;
using RSG;

namespace SpatialSys.UnitySDK.Editor
{
    public static class WorldUtility
    {
        public static IPromise ValidateWorldExists()
        {
            IPromise createWorldPromise = Promise.Resolved();
            if (string.IsNullOrEmpty(ProjectConfig.worldID))
            {
                createWorldPromise = SpatialAPI.CreateWorld()
                    .Then(resp => {
                        ProjectConfig.worldID = resp.id;
                    });
            }
            return createWorldPromise;
        }

    }
}