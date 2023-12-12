using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Sync: Receive Remote Event (RPC)")]
    [UnitSurtitle("Spatial Sync")]
    [UnitShortTitle("Receive Event (RPC)")]
    [UnitCategory("Events\\Spatial\\Network")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class IncomingNetworkEventByteNode : EventUnit<NetworkingRemoteEventArgs>
    {
        private const string EVENT_HOOK_ID = "OnSDKNetEventByte";

        protected override bool register => true;

        [SerializeAs(nameof(argumentCount))]
        private int _argumentCount;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Arguments")]
        public int argumentCount
        {
            get => _argumentCount;
            set => _argumentCount = Mathf.Clamp(value, 0, 5);
        }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput eventID { get; private set; }

        [DoNotSerialize]
        public ValueOutput senderActor { get; private set; }
        [DoNotSerialize]
        public List<ValueOutput> argumentPorts { get; } = new List<ValueOutput>();

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(NetworkingRemoteEventArgs args)
        {
            EventBus.Trigger(EVENT_HOOK_ID, args);
        }

        protected override void Definition()
        {
            base.Definition();

            eventID = ValueInput<byte>(nameof(eventID), 0);
            senderActor = ValueOutput<int>(nameof(senderActor));
            argumentPorts.Clear();

            for (var i = 0; i < argumentCount; i++)
            {
                argumentPorts.Add(ValueOutput<object>("Arg_" + i));
            }
        }

        protected override bool ShouldTrigger(Flow flow, NetworkingRemoteEventArgs args)
        {
            return flow.GetValue<byte>(eventID) == args.eventID;
        }

        protected override void AssignArguments(Flow flow, NetworkingRemoteEventArgs args)
        {
            for (var i = 0; i < argumentCount; i++)
                flow.SetValue(argumentPorts[i], args.eventArgs[i]);
            flow.SetValue(senderActor, args.senderActor);
        }
    }
}
