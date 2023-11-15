using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialCameraPassthrough : SpatialComponentBase
    {
        public override string prettyName => "Camera Passthrough";
        public override string tooltip => "Lets the camera ignore collision with this object.";
        public override string documentationURL => null;

        [Tooltip("When checked, the camera will also pass through all children.")]
        public bool applyToChildren;

        private void Start()
        {
            ClientBridge.InitializeSpatialCameraPassthrough?.Invoke(this);
        }
    }
}
