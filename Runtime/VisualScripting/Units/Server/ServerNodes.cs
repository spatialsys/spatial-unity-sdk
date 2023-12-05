using System.Collections.Generic;
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
            participantCount = ValueOutput<int>(nameof(participantCount), (f) => SpatialBridge.networkingService.spaceParticipantCount);
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
            participantCount = ValueOutput<int>(nameof(participantCount), (f) => SpatialBridge.networkingService.serverParticipantCount);
        }
    }

    [UnitTitle("Spatial: Get Server Unique Users Count")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get Server Unique Users Count")]
    [UnitCategory("Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetServerUniqueUsersCountNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput uniqueUsersCount { get; private set; }

        protected override void Definition()
        {
            uniqueUsersCount = ValueOutput<int>(nameof(uniqueUsersCount), (f) => SpatialBridge.networkingService.serverParticipantCountUnique);
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
            totalServersCount = ValueOutput<int>(nameof(totalServersCount), (f) => SpatialBridge.networkingService.serverCount);
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
            isOpen = ValueOutput<bool>(nameof(isOpen), (f) => SpatialBridge.networkingService.isServerOpen);
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
                SpatialBridge.networkingService.SetServerOpen(f.GetValue<bool>(isOpen));
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
            isVisible = ValueOutput<bool>(nameof(isVisible), (f) => SpatialBridge.networkingService.isServerVisible);
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
                SpatialBridge.networkingService.SetServerVisible(f.GetValue<bool>(isVisible));
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
            maxParticipants = ValueOutput<int>(nameof(maxParticipants), (f) => SpatialBridge.networkingService.serverMaxParticipantCount);
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
                SpatialBridge.networkingService.SetServerMaxParticipantCount(f.GetValue<int>(maxParticipants));
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
            properties = ValueOutput<AotDictionary>(nameof(properties), (f) => {
                var props = SpatialBridge.networkingService.GetServerProperties();
                AotDictionary aotDict = new();
                foreach (KeyValuePair<string, object> kvp in props)
                    aotDict.Add(kvp.Key, kvp.Value);
                return aotDict;
            });
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
                Dictionary<string, object> props = new();
                foreach (KeyValuePair<object, object> kvp in f.GetValue<AotDictionary>(properties))
                {
                    if (kvp.Key is not string)
                        throw new System.Exception($"{nameof(SetServerPropertiesNode)}: all server property keys must be strings, but was {kvp.Key?.GetType()?.Name ?? "null"}");

                    props.Add((string)kvp.Key, kvp.Value);
                }
                SpatialBridge.networkingService.SetServerProperties(props);
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
                AotDictionary serverProps = f.GetValue<AotDictionary>(serverProperties);
                Dictionary<string, object> serverPropsDict = null;
                if (serverProps != null)
                {
                    serverPropsDict = new(serverProps.Count);
                    foreach (KeyValuePair<string, object> kvp in serverProps)
                        serverPropsDict.Add(kvp.Key, kvp.Value);
                }

                AotList matchProps = f.GetValue<AotList>(matchProperties);
                List<string> matchPropsList = null;
                if (matchProps != null)
                {
                    matchPropsList = new(matchProps.Count);
                    foreach (string s in matchProps)
                        matchProps.Add(s);
                }

                SpatialBridge.networkingService.TeleportToNewServer(
                    f.GetValue<int>(maxParticipants),
                    f.GetValue<bool>(isOpen),
                    f.GetValue<bool>(isVisible),
                    serverPropsDict,
                    matchPropsList
                );

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
                List<int> actorIDs = new();
                foreach (int actorID in f.GetValue<AotList>(actors))
                    actorIDs.Add(actorID);

                AotDictionary serverProps = f.GetValue<AotDictionary>(serverProperties);
                Dictionary<string, object> serverPropsDict = null;
                if (serverProps != null)
                {
                    serverPropsDict = new(serverProps.Count);
                    foreach (KeyValuePair<string, object> kvp in serverProps)
                        serverPropsDict.Add(kvp.Key, kvp.Value);
                }

                AotList matchProps = f.GetValue<AotList>(matchProperties);
                List<string> matchPropsList = null;
                if (matchProps != null)
                {
                    matchPropsList = new(matchProps.Count);
                    foreach (string s in matchProps)
                        matchProps.Add(s);
                }

                SpatialBridge.networkingService.TeleportActorsToNewServer(
                    actorIDs,
                    f.GetValue<int>(maxParticipants),
                    f.GetValue<bool>(isVisible),
                    serverPropsDict,
                    matchPropsList
                );

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
                AotDictionary serverProps = f.GetValue<AotDictionary>(serverProperties);
                Dictionary<string, object> serverPropsDict = null;
                if (serverProps != null)
                {
                    serverPropsDict = new(serverProps.Count);
                    foreach (KeyValuePair<string, object> kvp in serverProps)
                        serverPropsDict.Add(kvp.Key, kvp.Value);
                }

                AotList matchProps = f.GetValue<AotList>(serverPropertiesToMatch);
                List<string> matchPropsList = null;
                if (matchProps != null)
                {
                    matchPropsList = new(matchProps.Count);
                    foreach (string s in matchProps)
                        matchProps.Add(s);
                }

                SpatialBridge.networkingService.TeleportToBestMatchServer(
                    f.GetValue<int>(maxParticipants),
                    serverPropsDict,
                    matchPropsList
                );

                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}