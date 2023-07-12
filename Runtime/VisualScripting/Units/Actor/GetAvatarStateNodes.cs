using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Actor: Avatar State")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Avatar State")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetAvatarStateNode : Unit
    {
        [DoNotSerialize]
        [NullMeansSelf]
        public ValueInput actor { get; private set; }
        [DoNotSerialize]
        [PortLabel("Avatar Exists")]
        public ValueOutput avatarExists { get; private set; }
        [DoNotSerialize]
        [PortLabel("Position")]
        public ValueOutput avatarPosition { get; private set; }
        [DoNotSerialize]
        [PortLabel("Rotation")]
        public ValueOutput avatarRotation { get; private set; }
        [DoNotSerialize]
        [PortLabel("Velocity")]
        public ValueOutput avatarVelocity { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);

            avatarExists = ValueOutput<bool>(nameof(avatarExists), (f) => ClientBridge.GetAvatarExists.Invoke(f.GetValue<int>(actor)));
            avatarPosition = ValueOutput<Vector3>(nameof(avatarPosition), (f) => ClientBridge.GetAvatarPositionWithActor.Invoke(f.GetValue<int>(actor)));
            avatarRotation = ValueOutput<Quaternion>(nameof(avatarRotation), (f) => ClientBridge.GetAvatarRotationWithActor.Invoke(f.GetValue<int>(actor)));
            avatarVelocity = ValueOutput<Vector3>(nameof(avatarVelocity), (f) => ClientBridge.GetAvatarVelocityWithActor.Invoke(f.GetValue<int>(actor)));
        }
    }

    [UnitTitle("Local Actor: Avatar State")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Avatar State")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarStateNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Position")]
        public ValueOutput avatarPosition { get; private set; }
        [DoNotSerialize]
        [PortLabel("Rotation")]
        public ValueOutput avatarRotation { get; private set; }
        [DoNotSerialize]
        [PortLabel("Velocity")]
        public ValueOutput avatarVelocity { get; private set; }

        protected override void Definition()
        {
            avatarPosition = ValueOutput<Vector3>(nameof(avatarPosition), (f) => ClientBridge.GetLocalAvatarPosition.Invoke());
            avatarRotation = ValueOutput<Quaternion>(nameof(avatarRotation), (f) => ClientBridge.GetLocalAvatarRotation.Invoke());
            avatarVelocity = ValueOutput<Vector3>(nameof(avatarVelocity), (f) => ClientBridge.GetLocalAvatarVelocity.Invoke());
        }
    }

    [UnitTitle("Local Actor: Is Grounded")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Is Grounded")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorGroundedNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput isGrounded { get; private set; }

        protected override void Definition()
        {
            isGrounded = ValueOutput<bool>(nameof(isGrounded), (f) => ClientBridge.GetLocalAvatarGrounded.Invoke());
        }
    }

    [UnitTitle("Actor: Get Name")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get Name")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorNameNode : Unit
    {
        [DoNotSerialize]
        [NullMeansSelf]
        public ValueInput actor { get; private set; }

        [DoNotSerialize]
        [PortLabel("Name")]
        public ValueOutput actorName { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);

            actorName = ValueOutput<string>(nameof(actorName), (f) => ClientBridge.GetActorName.Invoke(f.GetValue<int>(actor)));
        }
    }
}
