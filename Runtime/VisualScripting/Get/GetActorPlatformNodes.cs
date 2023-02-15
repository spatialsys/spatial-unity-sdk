using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    public enum SpatialPlatform
    {
        Unknown = 0,
        Web = 1,
        Mobile = 2,
        MetaQuest = 3,
    }

    [UnitTitle("Get Actor Platform")]
    public class GetActorPlatformNode : Unit
    {
        [DoNotSerialize]
        public ValueInput actor { get; private set; }
        [DoNotSerialize]
        public ValueOutput actorPlatform { get; private set; }
        [DoNotSerialize]
        public ValueOutput actorExists { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);
            actorPlatform = ValueOutput<SpatialPlatform>(nameof(actorPlatform),
                (f) => ConvertPlatformToSpatialPlatform(ClientBridge.GetActorPlatform.Invoke(f.GetValue<int>(actor)).Item2)
                );
            actorExists = ValueOutput<bool>(nameof(actorExists), (f) => ClientBridge.GetActorPlatform.Invoke(f.GetValue<int>(actor)).Item1);
        }

        //convert the plaform int we get from SpatialAPI to the SpatialPlatform enum
        public SpatialPlatform ConvertPlatformToSpatialPlatform(int platform)
        {
            switch (platform)
            {
                case 0:
                    return SpatialPlatform.Web;
                case 2:
                case 3:
                    return SpatialPlatform.Mobile;
                case 4:
                    return SpatialPlatform.MetaQuest;
                default:
                    return SpatialPlatform.Unknown;
            }
        }
    }

    [UnitTitle("Get Local Platform")]
    public class GetLocalActorPlatformNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput actorPlatform { get; private set; }

        protected override void Definition()
        {
            actorPlatform = ValueOutput<SpatialPlatform>(nameof(actorPlatform),
                (f) => ConvertPlatformToSpatialPlatform(ClientBridge.GetLocalActorPlatform.Invoke())
                );
        }

        //convert the plaform int we get from SpatialAPI to the SpatialPlatform enum
        public SpatialPlatform ConvertPlatformToSpatialPlatform(int platform)
        {
            switch (platform)
            {
                case 0:
                    return SpatialPlatform.Web;
                case 2:
                case 3:
                    return SpatialPlatform.Mobile;
                case 4:
                    return SpatialPlatform.MetaQuest;
                default:
                    return SpatialPlatform.Unknown;
            }
        }
    }

}
