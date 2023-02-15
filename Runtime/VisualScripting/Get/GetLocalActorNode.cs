using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Get Local Actor")]
    public class GetLocalActorNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput actor { get; private set; }

        protected override void Definition()
        {
            actor = ValueOutput<int>(nameof(actor), (f) => ClientBridge.GetLocalActor.Invoke());
        }
    }
}
