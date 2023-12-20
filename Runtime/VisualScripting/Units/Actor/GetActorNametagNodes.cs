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

            displayName = ValueOutput<string>(nameof(displayName), (f) => GetActor(f)?.avatar?.displayName);
            subtext = ValueOutput<string>(nameof(subtext), (f) => GetActor(f)?.avatar?.nametagSubtext);
            barVisible = ValueOutput<bool>(nameof(barVisible), (f) => GetActor(f)?.avatar?.nametagBarVisible ?? false);
            barValue = ValueOutput<float>(nameof(barValue), (f) => GetActor(f)?.avatar?.nametagBarValue ?? 0f);
        }

        private IActor GetActor(Flow f)
        {
            if (SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a))
                return a;
            return null;
        }
    }
}
