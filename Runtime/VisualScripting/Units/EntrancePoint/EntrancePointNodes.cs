using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Entrance Point")]
    [UnitTitle("Entrance Point: Set Radius")]
    [UnitSurtitle("Entrance Point")]
    [UnitShortTitle("Set Radius")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class EntrancePointSetRadiusNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput entrancePoint { get; private set; }

        [DoNotSerialize]
        public ValueInput radius { get; private set; }

        protected override void Definition()
        {
            entrancePoint = ValueInput<SpatialEntrancePoint>(nameof(entrancePoint), null).NullMeansSelf();
            radius = ValueInput<float>(nameof(radius), 1.0f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialEntrancePoint pt = f.GetValue<SpatialEntrancePoint>(entrancePoint);
                if (pt != null)
                    pt.radius = f.GetValue<float>(radius);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}
