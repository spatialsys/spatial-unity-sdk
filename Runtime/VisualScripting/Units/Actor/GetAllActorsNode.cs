using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Get All Actors")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetAllActorsNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput actors { get; private set; }

        protected override void Definition()
        {
            actors = ValueOutput<List<int>>(nameof(actors), (f) => SpatialBridge.actorService.actors.Keys.ToList());
        }
    }
}
