using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Input: Set Overrides")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("Set Overrides")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(InputIcon))]
    public class SetInputOverrides : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput movement { get; private set; }
        [DoNotSerialize]
        public ValueInput jump { get; private set; }
        [DoNotSerialize]
        public ValueInput sprint { get; private set; }
        [DoNotSerialize]
        public ValueInput actionButton { get; private set; }

        protected override void Definition()
        {
            movement = ValueInput<bool>(nameof(movement), true);
            jump = ValueInput<bool>(nameof(jump), true);
            sprint = ValueInput<bool>(nameof(sprint), true);
            actionButton = ValueInput<bool>(nameof(actionButton), true);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetInputOverrides.Invoke(
                    f.GetValue<bool>(movement),
                    f.GetValue<bool>(jump),
                    f.GetValue<bool>(sprint),
                    f.GetValue<bool>(actionButton),
                    f.stack.self
                );
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        public override void Uninstantiate(GraphReference instance)
        {
            base.Uninstantiate(instance);
            ClientBridge.OnInputGraphRootObjectDestroyed.Invoke(instance.gameObject);
        }
    }
}