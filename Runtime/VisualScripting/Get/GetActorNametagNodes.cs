using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actions")]
    [UnitTitle("Spatial Sync: Get Nametag State")]
    [UnitSurtitle("Spatial Sync")]
    [UnitShortTitle("Get Nametag State")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorNametagSubtext : Unit
    {
        [DoNotSerialize]
        [NullMeansSelf]
        public ValueInput actor { get; private set; }
        [DoNotSerialize]
        public ValueOutput displayName { get; private set; }
        [DoNotSerialize]
        public ValueOutput subtext { get; private set; }
        [DoNotSerialize]
        public ValueOutput barVisible { get; private set; }
        [DoNotSerialize]
        public ValueOutput barValue { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);

            displayName = ValueOutput<string>(nameof(displayName), (f) => ClientBridge.GetActorNametagDisplayName.Invoke(f.GetValue<int>(actor)));
            subtext = ValueOutput<string>(nameof(subtext), (f) => ClientBridge.GetActorNametagSubtext.Invoke(f.GetValue<int>(actor)));
            barVisible = ValueOutput<bool>(nameof(barVisible), (f) => ClientBridge.GetActorNametagBarVisible.Invoke(f.GetValue<int>(actor)));
            barValue = ValueOutput<float>(nameof(barValue), (f) => ClientBridge.GetActorNametagBarValue.Invoke(f.GetValue<int>(actor)));
        }
    }
}
