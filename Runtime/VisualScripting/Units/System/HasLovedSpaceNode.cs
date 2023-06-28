using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Has Loved Space")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class HasLovedSpaceNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput hasLoved { get; private set; }

        protected override void Definition()
        {
            hasLoved = ValueOutput<bool>(nameof(hasLoved), (f) => ClientBridge.HasLocalLovedSpace.Invoke());
        }
    }
}
