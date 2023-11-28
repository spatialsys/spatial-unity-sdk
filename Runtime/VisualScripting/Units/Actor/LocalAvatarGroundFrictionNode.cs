using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Ground Friction")]

    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Set Ground Friction")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetLocalAvatarGroundFrictionNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("Friction")]
        public ValueInput friction { get; private set; }

        protected override void Definition()
        {
            friction = ValueInput<float>(nameof(friction), 1); // This default should be matched with AvatarController
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.SetLocalAvatarGroundFriction?.Invoke(f.GetValue<float>(friction));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Ground Friction")]

    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Get Ground Friction")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarGroundFrictionNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput friction { get; private set; }

        protected override void Definition()
        {
            friction = ValueOutput<float>(nameof(friction), (f) => SpatialBridge.GetLocalAvatarGroundFriction?.Invoke() ?? 1);
        }
    }
}
