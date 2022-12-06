using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialEntrancePointTests
    {
        [ComponentTest(typeof(SpatialEntrancePoint))]
        public static void CheckForColliderBelow(SpatialEntrancePoint target)
        {
            if (!Physics.Raycast(target.transform.position, Vector3.down))
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    target,
                    TestResponseType.Fail,
                    "Entrance point is not above solid ground.",
                    "Make sure that the entrance point is placed above an object with an active collider."
                ));
            }
        }
    }
}
