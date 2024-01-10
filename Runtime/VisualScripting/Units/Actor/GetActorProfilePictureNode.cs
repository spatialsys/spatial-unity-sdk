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
            int actorNumber = flow.GetValue<int>(actor);
            IActor sdkActor = null;
            if (actorNumber == SpatialBridge.actorService.localActorNumber)
            {
                sdkActor = SpatialBridge.actorService.localActor;
            }
            else
            {
                SpatialBridge.actorService.actors.TryGetValue(actorNumber, out sdkActor);
            }
            if (sdkActor != null)
            {
                ActorProfilePictureRequest request = sdkActor.GetProfilePicture();
                yield return request;
                flow.SetValue(actorTexture, request.texture);
            }
            else
            {
                SpatialBridge.LogError?.Invoke($"{nameof(GetActorProfilePictureNode)}: Actor with actor number '{actorNumber}' does not exist");
                flow.SetValue(actorTexture, null);
            }
            yield return outputTrigger;
        }
    }
}