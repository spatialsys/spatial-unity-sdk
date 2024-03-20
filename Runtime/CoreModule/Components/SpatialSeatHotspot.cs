using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Components")]
    public class SpatialSeatHotspot : SpatialComponentBase
    {
        public override string prettyName => "Seat Hotspot";
        public override string tooltip => "This component makes it possible for an avatar to sit down in this spot";
        public override string documentationURL => "https://docs.spatial.io/components/seat-hotspot";

        [Tooltip("When checked, avatars will always face a specified direction along the blue axis. Disable this for chairs that you can sit on in any orientation like stools")]
        public bool forceAvatarOrientation = true;

        private void Start()
        {
            SpatialBridge.spatialComponentService.InitializeSeatHotspot(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, .3f);
            if (forceAvatarOrientation)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + transform.forward);
            }
        }
    }
}