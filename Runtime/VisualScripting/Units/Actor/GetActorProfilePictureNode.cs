using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Actor: Get Profile Picture")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get Profile Picture")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorProfilePictureNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput actor { get; private set; }
       
        [DoNotSerialize]
        [PortLabel("Texture")]
        public ValueOutput actorTexture { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);
            actorTexture = ValueOutput<Texture2D>(nameof(actorTexture));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            int actorId = flow.GetValue<int>(actor);
            if (actorId >= 0)
            {
                bool completed = false;
                ClientBridge.GetActorProfilePicture.Invoke(actorId, texture => {
                    completed = true;
                    flow.SetValue(actorTexture, texture);
                });
                yield return new WaitUntil(() => completed);
            }
            else
            {
                flow.SetValue(actorTexture, null);
            }
            yield return outputTrigger;
        }
    }
}