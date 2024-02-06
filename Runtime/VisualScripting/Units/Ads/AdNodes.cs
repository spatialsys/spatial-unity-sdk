using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Ads")]
    [UnitTitle("Ad Service: Is Supported")]
    [UnitShortTitle("Is Supported")]
    [UnitSurtitle("Ad Service")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class AdServiceIsSupportedNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput isSupported { get; private set; }

        protected override void Definition()
        {
            isSupported = ValueOutput<bool>(nameof(isSupported), (f) => {
                return SpatialBridge.adService.isSupported;
            });
        }
    }

    [UnitCategory("Spatial\\Ads")]
    [UnitTitle("Ad Service: Request Ad")]
    [UnitShortTitle("Ad Service: Request Ad")]
    [UnitSurtitle("Ad Service")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class AdServiceRequestAdNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        public ControlOutput start { get; private set; }
        [DoNotSerialize]
        public ControlOutput error { get; private set; }
        [DoNotSerialize]
        public ControlOutput finished { get; private set; }

        [DoNotSerialize]
        public ValueInput adType { get; private set; }

        protected override void Definition()
        {
            adType = ValueInput<SpatialAdType>(nameof(adType), SpatialAdType.MidGame);

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            start = ControlOutput(nameof(start));
            error = ControlOutput(nameof(error));
            finished = ControlOutput(nameof(finished));
            Succession(inputTrigger, start);
            Succession(inputTrigger, error);
            Succession(inputTrigger, finished);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool started = false;
            bool finalized = false;
            bool succeeded = false;
            AdRequest request = SpatialBridge.adService.RequestAd(flow.GetValue<SpatialAdType>(adType));
            request.started += _ => {
                started = true;
            };
            request.completed += _ => {
                finalized = true;
                succeeded = request.succeeded;
            };

            // Wait for start
            yield return new WaitUntil(() => started || finalized);
            if (started)
                yield return start;

            // Wait for finish
            yield return new WaitUntil(() => finalized);
            if (succeeded)
            {
                yield return finished;
            }
            else
            {
                yield return error;
            }
        }
    }
}
