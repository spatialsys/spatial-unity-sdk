using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public class TerrainTests
    {
        [ComponentTest(typeof(Terrain))]
        public static void WarnAboutTerrainResolution(UnityEngine.Object target)
        {
            Terrain terrain = target as Terrain;

            if (terrain.terrainData.heightmapResolution > 2048)
            {
                var resp = new SpatialTestResponse(
                    terrain,
                    TestResponseType.Warning,
                    "Terrain has very high resolution heightmap",
                    "Consider reducing the heightmap resolution of your terrain to 2048 or less"
                );
                SpatialValidator.AddResponse(resp);
            }
        }
    }
}
