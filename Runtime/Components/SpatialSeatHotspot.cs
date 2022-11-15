using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialSeatHotspot : SpatialComponentBase
    {
        public override string prettyName => "Seat Hotspot";
        public override string tooltip => "This component makes it possible for an avatar to sit down in this spot";
        public override string documentationURL => "https://spatialxr.notion.site/Seat-Hotspot-6e2cf27bad66490395b6827adc0a9613";

        [Tooltip("When checked, avatars will always face a specified direction along the blue axis. Disable this for chairs that you can sit on in any orientation like stools")]
        public bool forceAvatarOrientation = true;

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