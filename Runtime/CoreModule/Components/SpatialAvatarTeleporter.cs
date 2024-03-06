using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Spatial Components")]
    [RequireComponent(typeof(Collider))]
    public class SpatialAvatarTeleporter : SpatialComponentBase
    {
        public override string prettyName => "Avatar Teleporter";
        public override string tooltip => "When an avatar enters the trigger area teleport them to the target location.";
        public override string documentationURL => "https://docs.spatial.io/avatar-teleporter";

        public Transform targetLocation;

        private void Start()
        {
            SpatialBridge.spatialComponentService.InitializeAvatarTeleporter(this);
        }

        private void OnDrawGizmos()
        {
            if (targetLocation != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, targetLocation.position);
            }
        }
    }
}
