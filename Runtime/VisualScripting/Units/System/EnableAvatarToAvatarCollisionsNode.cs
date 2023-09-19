using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\System")]
    [UnitTitle("Spatial System: Enable Avatar to Avatar Collisions")]

    [UnitSurtitle("Spatial System")]
    [UnitShortTitle("Enable Avatar to Avatar Collisions")]

    [TypeIcon(typeof(SpatialAvatarAnimation))]
    public class EnableAvatarToAvatarCollisionsNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput enabled { get; private set; }

        protected override void Definition()
        {
            enabled = ValueInput<bool>(nameof(enabled), true);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) =>
            {
                ClientBridge.EnableAvatarToAvatarCollisions?.Invoke(f.GetValue<bool>(enabled));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
