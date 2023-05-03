using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialWearable : SpatialPackageAsset
    {
        public enum Type
        {
            Aura = 0,
        }

        public override string prettyName => "Wearable";
        public override string tooltip => "This component is used to define an object that can be attached or equipped by an avatar";
        public override string documentationURL => "https://docs.spatial.io/wearables";

        public Type type = Type.Aura;
    }
}