using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [RequireComponent(typeof(Animator))]
    public class SpatialAvatar : SpatialComponentBase
    {
        public override string prettyName => "Custom Avatar";
        public override string tooltip => "This component is used to define a custom avatar for Spatial";
        public override string documentationURL => null;
    }
}