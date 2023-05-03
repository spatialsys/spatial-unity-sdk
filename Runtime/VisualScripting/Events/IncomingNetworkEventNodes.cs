using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    public struct IncomingNetworkEventArgs
    {
        public string stringID;
        public byte byteID;
        public int senderActor;
        public object[] args;

        public IncomingNetworkEventArgs(string stringID, int sendActor, params object[] args)
        {
            byteID = 0;
            this.stringID = stringID;
            this.senderActor = sendActor;
            this.args = args;
        }

        public IncomingNetworkEventArgs(byte byteID, int sendActor, params object[] args)
        {
            this.byteID = byteID;
            stringID = "";
            this.senderActor = sendActor;
            this.args = args;
        }
    }

    [UnitTitle("Spatial Sync: Receive Event (RPC)")]
    [UnitSurtitle("Spatial Sync")]
    [UnitShortTitle("Receive Event (RPC)")]
    [UnitCategory("Events\\Spatial")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class IncomingNetworkEventByteNode : EventUnit<IncomingNetworkEventArgs>
    {
        public static string eventName = "OnSDKNetEventByte";
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
            return new EventHook(eventName);
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

        protected override bool ShouldTrigger(Flow flow, IncomingNetworkEventArgs args)
        {
            return flow.GetValue<byte>(eventID) == args.byteID;
        }

        protected override void AssignArguments(Flow flow, IncomingNetworkEventArgs arg)
        {
            for (var i = 0; i < argumentCount; i++)
            {
                flow.SetValue(argumentPorts[i], arg.args[i]);
            }
            flow.SetValue(senderActor, arg.senderActor);
        }
    }
}
