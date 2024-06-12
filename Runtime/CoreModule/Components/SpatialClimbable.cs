using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Core/Components")]
    [RequireComponent(typeof(Collider))]
    public class SpatialClimbable : SpatialComponentBase
    {
        public override string prettyName => "Climbable";
        public override string tooltip => "Allows an avatar to climb this object";
        public override string documentationURL => "https://toolkit.spatial.io/docs/components/climbable";
        public override bool isExperimental => true;
        private void Start()
        {
            SpatialBridge.spatialComponentService.InitializeClimbable(this);
        }
    }
}
