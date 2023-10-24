using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Get Local Actor User ID")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorUserID : Unit
    {
        [DoNotSerialize]
        public ValueOutput userID { get; private set; }

        protected override void Definition()
        {
            userID = ValueOutput<string>(nameof(userID), (f) => ClientBridge.GetLocalActorUserID.Invoke());
        }
    }

    [UnitTitle("Get Actor User ID")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorUserID : Unit
    {
        [DoNotSerialize]
        public ValueInput actor { get; private set; }
        [DoNotSerialize]
        public ValueOutput userID { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);
            userID = ValueOutput<string>(nameof(userID), (f) => ClientBridge.GetActorUserID.Invoke(f.GetValue<int>(actor)));
        }
    }
}
