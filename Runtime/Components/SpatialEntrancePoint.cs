namespace SpatialSys.UnitySDK
{
    public class SpatialEntrancePoint : SpatialComponentBase
    {
        public override string prettyName => "Entrance Point";
        public override string tooltip => "Specify the area in which users will be placed when entering this space. Multiple entrance points can be used in a single scene";
        public override string documentationURL => "https://docs.spatial.io/components/entrance-point";

        public float radius = 1f;
    }
}
