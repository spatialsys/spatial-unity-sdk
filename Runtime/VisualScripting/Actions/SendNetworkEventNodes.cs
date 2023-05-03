using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    public enum NetworkEventTargets
    {
        Others,
        Everyone,
    }

    [UnitTitle("Spatial Sync: Send Event (RPC)")]
    [UnitSurtitle("Spatial Sync")]
    [UnitShortTitle("Send Event (RPC)")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SendNetworkEventByte : Unit
    {
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
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("ID")]
        public ValueInput eventID { get; private set; }
        [DoNotSerialize]
        public ValueInput sendTo { get; private set; }
        [DoNotSerialize]
        public List<ValueInput> argumentPorts { get; } = new List<ValueInput>();

        protected override void Definition()
        {
            eventID = ValueInput<byte>(nameof(eventID), 0);
            sendTo = ValueInput<NetworkEventTargets>(nameof(sendTo), NetworkEventTargets.Others);

            for (var i = 0; i < argumentCount; i++)
            {
                argumentPorts.Add(ValueInput<object>("Arg_" + i));
            }

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SendSDKNetworkEventByte?.Invoke(
                    everyone: f.GetValue<NetworkEventTargets>(sendTo) == NetworkEventTargets.Everyone,
                    f.GetValue<byte>(eventID),
                    argumentPorts.Select(f.GetConvertedValue).ToArray()
                );
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial Sync: Send Event To Actor (RPC)")]
    [UnitSurtitle("Spatial Sync")]
    [UnitShortTitle("Send Event To Actor (RPC)")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SendNetworkEventToActorByteNode : Unit
    {
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
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("ID")]
        public ValueInput eventID { get; private set; }
        [DoNotSerialize]
        public ValueInput sendTo { get; private set; }
        [DoNotSerialize]
        public List<ValueInput> argumentPorts { get; } = new List<ValueInput>();

        protected override void Definition()
        {
            eventID = ValueInput<byte>(nameof(eventID), 0);
            sendTo = ValueInput<int>(nameof(sendTo), -1);

            for (var i = 0; i < argumentCount; i++)
            {
                argumentPorts.Add(ValueInput<object>("Arg_" + i));
            }

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SendSDKNetworkEventToActorByte?.Invoke(
                    f.GetValue<int>(sendTo),
                    f.GetValue<byte>(eventID),
                    argumentPorts.Select(f.GetConvertedValue).ToArray()
                );
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
