using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class TerrainTests
    {
        [ComponentTest(typeof(Terrain))]
        public static void WarnAboutTerrainResolution(Component target)
        {
            Terrain terrain = target as Terrain;
            if (terrain.terrainData == null)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    terrain,
                    TestResponseType.Fail,
                    "Terrain has no terrain data",
                    "Terrains must have terrain data"
                ));
                return;
            }

            if (terrain.terrainData.heightmapResolution > 2048)
            {
                var resp = new SpatialTestResponse(
                    terrain,
                    TestResponseType.Tip,
                    "Terrain has very high resolution heightmap",
                    "Consider reducing the heightmap resolution of your terrain to 2048 or less"
                );
                SpatialValidator.AddResponse(resp);
            }
        }
    }
}
