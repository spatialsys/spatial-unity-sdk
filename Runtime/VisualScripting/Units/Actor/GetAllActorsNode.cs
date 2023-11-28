using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

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
            actors = ValueOutput<List<int>>(nameof(actors), (f) => SpatialBridge.GetActors.Invoke());
        }
    }
}
