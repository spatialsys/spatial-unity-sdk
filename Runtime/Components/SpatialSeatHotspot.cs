using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialSeatHotspot : SpatialComponentBase
    {
        public override string prettyName => "Seat Hotspot";
        public override string tooltip => "This component makes it possible for an avatar to sit down in this spot";

        [Tooltip("When checked, avatars will always face a specified direction along the blue axis. Disable this for chairs that you can sit on in any orientation like stools")]
        public bool forceAvatarOrientation = true;
        [Tooltip("Controls the size of the clickable area. Clicking this area will put an avatar in the seat if its not occupied. Shown with the Green gizmo")]
        public float clickableRadius = .3f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, clickableRadius);
            if (forceAvatarOrientation)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + transform.forward);
            }
        }
    }
}