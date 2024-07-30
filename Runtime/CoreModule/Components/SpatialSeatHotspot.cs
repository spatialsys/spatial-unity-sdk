using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Core/Components")]
    public class SpatialSeatHotspot : SpatialComponentBase
    {
        public override string prettyName => "Seat Hotspot";
        public override string tooltip => "This component makes it possible for an avatar to sit down in this spot";
        public override string documentationURL => "https://toolkit.spatial.io/docs/components/seat-hotspot";

        [Tooltip("When checked, avatars will always face a specified direction along the blue axis. Disable this for chairs that you can sit on in any orientation like stools")]
        public bool forceAvatarOrientation = true;

        [Tooltip("The priority of this seat hotspot. Lower priority seats will be filled by avatars when they are automatically assigned seating.")]
        public int priority;

        private void Start()
        {
            SpatialBridge.spatialComponentService.InitializeSeatHotspot(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            const float RADIUS = 0.3f;
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireSphere(Vector3.zero, RADIUS);

            if (forceAvatarOrientation)
            {
                Vector3 arrowPos = transform.position + (transform.forward * RADIUS);
                Quaternion arrowRot = Quaternion.LookRotation(transform.forward);
                const float ARROW_SIZE = 0.4f;
                Color arrowColor = Color.cyan;

                UnityEditor.Handles.color = arrowColor;
                UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                UnityEditor.Handles.ArrowHandleCap(-1, arrowPos, arrowRot, ARROW_SIZE, EventType.Repaint);
                // Lower opacity when occluded, to match the wire sphere gizmo.
                arrowColor.a = 0.2f;
                UnityEditor.Handles.color = arrowColor;
                UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
                UnityEditor.Handles.ArrowHandleCap(-1, arrowPos, arrowRot, ARROW_SIZE, EventType.Repaint);
            }
        }
#endif
    }
}