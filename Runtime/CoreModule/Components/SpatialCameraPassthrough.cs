using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Spatial Components")]
    public class SpatialCameraPassthrough : SpatialComponentBase
    {
        public override string prettyName => "Camera Passthrough";
        public override string tooltip => "Lets the camera ignore collision with this object.";
        public override string documentationURL => null;

        [Tooltip("When checked, the camera will also pass through all children.")]
        public bool applyToChildren;

        private void Start()
        {
            SpatialBridge.spatialComponentService.InitializeCameraPassthrough(this);
        }
    }
}
