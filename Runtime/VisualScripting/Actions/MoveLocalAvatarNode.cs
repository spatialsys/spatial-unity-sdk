using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actions")]
    [UnitTitle("Local Actor: Move")]

    [UnitSurtitle("Local Actor Control")]
    [UnitShortTitle("Move")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class MoveLocalAvatarNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("MoveInput")]
        public ValueInput moveInput { get; private set; }

        [DoNotSerialize]
        [PortLabel("Sprint")]
        public ValueInput sprint { get; private set; }

        protected override void Definition()
        {
            moveInput = ValueInput<Vector2>(nameof(moveInput), Vector2.zero);
            sprint = ValueInput<bool>(nameof(sprint), false);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.MoveLocalAvatar.Invoke(f.GetValue<Vector2>(moveInput), f.GetValue<bool>(sprint));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}
