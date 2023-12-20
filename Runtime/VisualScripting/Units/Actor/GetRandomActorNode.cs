using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Get Random Actor")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetRandomActorNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput actor { get; private set; }

        protected override void Definition()
        {
            actor = ValueOutput<int>(nameof(actor), (f) => {
                if (SpatialBridge.actorService.TryGetRandomActor(includeLocalActor: true, out IActor a))
                    return a.actorNumber;
                return -1;
            });
        }
    }
}
