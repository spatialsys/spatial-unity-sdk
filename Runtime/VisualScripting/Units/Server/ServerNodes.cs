using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Get Space Participant Count")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get Space Participant Count")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetSpaceParticipantCountNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput participantCount { get; private set; }

        protected override void Definition()
        {
            participantCount = ValueOutput<int>(nameof(participantCount), (f) => ClientBridge.GetSpaceParticipantCount?.Invoke() ?? 1);
        }
    }

    [UnitTitle("Spatial: Get Server Participant Count")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get Server Participant Count")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetServerParticipantCountNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput participantCount { get; private set; }

        protected override void Definition()
        {
            participantCount = ValueOutput<int>(nameof(participantCount), (f) => ClientBridge.GetServerParticipantCount?.Invoke() ?? 1);
        }
    }

    [UnitTitle("Spatial: Get Total Servers Count")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get Total Servers Count")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetTotalServersCountNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput totalServersCount { get; private set; }

        protected override void Definition()
        {
            totalServersCount = ValueOutput<int>(nameof(totalServersCount), (f) => ClientBridge.GetTotalServersCount?.Invoke() ?? 1);
        }
    }

    [UnitTitle("Spatial: Get Server Open")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get Server Open")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetServerOpenNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput isOpen { get; private set; }

        protected override void Definition()
        {
            isOpen = ValueOutput<bool>(nameof(isOpen), (f) => ClientBridge.GetServerOpen?.Invoke() ?? true);
        }
    }

    [UnitTitle("Spatial: Set Server Open")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Server Open")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetServerOpenNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput isOpen { get; private set; }

        protected override void Definition()
        {
            isOpen = ValueInput<bool>(nameof(isOpen), true);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetServerOpen?.Invoke(f.GetValue<bool>(isOpen));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Get Server Visible")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get Server Visible")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetServerVisibleNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput isVisible { get; private set; }

        protected override void Definition()
        {
            isVisible = ValueOutput<bool>(nameof(isVisible), (f) => ClientBridge.GetServerVisible?.Invoke() ?? true);
        }
    }

    [UnitTitle("Spatial: Set Server Visible")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Server Visible")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetServerVisibleNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput isVisible { get; private set; }

        protected override void Definition()
        {
            isVisible = ValueInput<bool>(nameof(isVisible), true);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetServerVisible?.Invoke(f.GetValue<bool>(isVisible));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Get Server Max Participants")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get Server Max Participants")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetServerMaxParticipantsNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput maxParticipants { get; private set; }

        protected override void Definition()
        {
            maxParticipants = ValueOutput<int>(nameof(maxParticipants), (f) => ClientBridge.GetServerMaxParticipants?.Invoke() ?? 50);
        }
    }

    [UnitTitle("Spatial: Set Server Max Participants")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Server Max Participants")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetServerMaxParticipantsNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput maxParticipants { get; private set; }

        protected override void Definition()
        {
            maxParticipants = ValueInput<int>(nameof(maxParticipants));

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetServerMaxParticipants?.Invoke(f.GetValue<int>(maxParticipants));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Get Server Properties")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get Server Properties")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetServerPropertiesNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput properties { get; private set; }

        protected override void Definition()
        {
            properties = ValueOutput<AotDictionary>(nameof(properties), (f) => ClientBridge.GetServerProperties?.Invoke() ?? null);
        }
    }

    [UnitTitle("Spatial: Set Server Properties")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Server Properties")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetServerPropertiesNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput properties { get; private set; }

        protected override void Definition()
        {
            properties = ValueInput<AotDictionary>(nameof(properties));

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetServerProperties?.Invoke(f.GetValue<AotDictionary>(properties));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Teleport To New Server")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Teleport To New Server")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class TeleportToNewServerNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput maxParticipants { get; private set; }
        [DoNotSerialize]
        public ValueInput isOpen { get; private set; }
        [DoNotSerialize]
        public ValueInput isVisible { get; private set; }
        [DoNotSerialize]
        [AllowsNull]
        public ValueInput serverProperties { get; private set; }
        [DoNotSerialize]
        [AllowsNull]
        public ValueInput matchProperties { get; private set; }

        protected override void Definition()
        {
            maxParticipants = ValueInput<int>(nameof(maxParticipants), 0);
            isOpen = ValueInput<bool>(nameof(isOpen), true);
            isVisible = ValueInput<bool>(nameof(isVisible), true);
            serverProperties = ValueInput<AotDictionary>(nameof(serverProperties), null);
            matchProperties = ValueInput<AotList>(nameof(matchProperties), null);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.TeleportToNewServer?.Invoke(f.GetValue<int>(maxParticipants), f.GetValue<bool>(isOpen), f.GetValue<bool>(isVisible), f.GetValue<AotDictionary>(serverProperties), f.GetValue<AotList>(matchProperties));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Teleport Actors To New Server")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Teleport Actors To New Server")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class TeleportActorsToNewServerNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput actors { get; private set; }
        [DoNotSerialize]
        public ValueInput maxParticipants { get; private set; }
        [DoNotSerialize]
        public ValueInput isVisible { get; private set; }
        [DoNotSerialize]
        [AllowsNull]
        public ValueInput serverProperties { get; private set; }
        [DoNotSerialize]
        [AllowsNull]
        public ValueInput matchProperties { get; private set; }

        protected override void Definition()
        {
            actors = ValueInput<AotList>(nameof(actors));
            maxParticipants = ValueInput<int>(nameof(maxParticipants), 0);
            isVisible = ValueInput<bool>(nameof(isVisible), true);
            serverProperties = ValueInput<AotDictionary>(nameof(serverProperties), null);
            matchProperties = ValueInput<AotList>(nameof(matchProperties), null);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.TeleportActorsToNewServer?.Invoke(f.GetValue<AotList>(actors), f.GetValue<int>(maxParticipants), f.GetValue<bool>(isVisible), f.GetValue<AotDictionary>(serverProperties), f.GetValue<AotList>(matchProperties));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Teleport To Best Match Server")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Teleport To Best Match Server")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class TeleportToBestMatchServerNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput maxParticipants { get; private set; }
        [DoNotSerialize]
        [AllowsNull]
        public ValueInput serverProperties { get; private set; }
        [DoNotSerialize]
        [AllowsNull]
        public ValueInput serverPropertiesToMatch { get; private set; }

        protected override void Definition()
        {
            maxParticipants = ValueInput<int>(nameof(maxParticipants), 0);
            serverProperties = ValueInput<AotDictionary>(nameof(serverProperties), null);
            serverPropertiesToMatch = ValueInput<AotList>(nameof(serverPropertiesToMatch), null);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.TeleportToBestMatchServer?.Invoke(f.GetValue<int>(maxParticipants), f.GetValue<AotDictionary>(serverProperties), f.GetValue<AotList>(serverPropertiesToMatch));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}