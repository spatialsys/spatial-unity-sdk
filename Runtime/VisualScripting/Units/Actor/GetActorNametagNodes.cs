using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Actor: Get Nametag State")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get Nametag State")]
    [UnitCategory("Spatial\\Actor")]
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

            displayName = ValueOutput<string>(nameof(displayName), (f) => SpatialBridge.GetActorNametagDisplayName.Invoke(f.GetValue<int>(actor)));
            subtext = ValueOutput<string>(nameof(subtext), (f) => SpatialBridge.GetActorNametagSubtext.Invoke(f.GetValue<int>(actor)));
            barVisible = ValueOutput<bool>(nameof(barVisible), (f) => SpatialBridge.GetActorNametagBarVisible.Invoke(f.GetValue<int>(actor)));
            barValue = ValueOutput<float>(nameof(barValue), (f) => SpatialBridge.GetActorNametagBarValue.Invoke(f.GetValue<int>(actor)));
        }
    }
}
