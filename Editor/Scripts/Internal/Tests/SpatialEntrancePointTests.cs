using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialEntrancePointTests
    {
        [ComponentTest(typeof(SpatialEntrancePoint))]
        public static void CheckForColliderBelow(SpatialEntrancePoint target)
        {
            if (!Physics.Raycast(target.transform.position, Vector3.down, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
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
    }
}
