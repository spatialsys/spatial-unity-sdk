using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Interactable: Get Text")]
    [UnitSurtitle("Spatial Interactable")]
    [UnitShortTitle("Get Text")]
    [UnitCategory("Spatial\\Interactable")]
    [TypeIcon(typeof(SpatialInteractable))]
    public class GetInteractableText : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [NullMeansSelf]
        [PortLabelHidden]
        public ValueInput interactable { get; private set; }
        [DoNotSerialize]
        public ValueOutput text { get; private set; }

        private string result = "";

        protected override void Definition()
        {
            interactable = ValueInput<SpatialInteractable>(nameof(interactable), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialInteractable i = f.GetValue<SpatialInteractable>(interactable);
                if (i != null)
                {
                    result = i.interactText;
                }
                else
                {
                    result = "";
                }
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);

            text = ValueOutput<string>(nameof(text), (f) => result);
        }
    }
}
