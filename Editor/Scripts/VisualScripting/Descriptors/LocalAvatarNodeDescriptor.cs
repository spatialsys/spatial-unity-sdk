using System;
using Unity.VisualScripting;
using UnityEngine;
using SpatialSys.UnitySDK.VisualScripting;

namespace SpatialSys.UnitySDK.Editor
{
    [Descriptor(typeof(SetLocalAvatarGroundFrictionNode))]
    public class SetLocalAvatarGroundFrictionNodeDescriptor : UnitDescriptor<SetLocalAvatarGroundFrictionNode>
    {
        public SetLocalAvatarGroundFrictionNodeDescriptor(SetLocalAvatarGroundFrictionNode unit) : base(unit) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);
            switch (port.key)
            {
                case "friction":
                    description.summary = "Contribution of how much ground friction to apply to the character. This should be a value from 0 to 1.";
                    break;
            }
        }
    }

    [Descriptor(typeof(GetLocalAvatarGroundFrictionNode))]
    public class GetLocalAvatarGroundFrictionNodeDescriptor : UnitDescriptor<GetLocalAvatarGroundFrictionNode>
    {
        public GetLocalAvatarGroundFrictionNodeDescriptor(GetLocalAvatarGroundFrictionNode unit) : base(unit) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);
            switch (port.key)
            {
                case "friction":
                    description.summary = "Contribution of how much ground friction to apply to the character. This is a value from 0 to 1.";
                    break;
            }
        }
    }
}