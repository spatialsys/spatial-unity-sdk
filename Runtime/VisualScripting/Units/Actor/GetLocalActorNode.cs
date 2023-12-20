using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Get Local Actor")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput actor { get; private set; }

        protected override void Definition()
        {
            actor = ValueOutput<int>(nameof(actor), (f) => SpatialBridge.actorService.localActor.actorNumber);
        }
    }
}
