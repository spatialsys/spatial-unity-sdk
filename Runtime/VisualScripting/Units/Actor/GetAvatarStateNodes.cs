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
            avatarExists = ValueOutput<bool>(nameof(avatarExists), (f) => GetActor(f)?.avatar?.isBodyLoaded ?? false);
            avatarPosition = ValueOutput<Vector3>(nameof(avatarPosition), (f) => GetActor(f)?.avatar?.position ?? Vector3.zero);
            avatarRotation = ValueOutput<Quaternion>(nameof(avatarRotation), (f) => GetActor(f)?.avatar?.rotation ?? Quaternion.identity);
            avatarVelocity = ValueOutput<Vector3>(nameof(avatarVelocity), (f) => GetActor(f)?.avatar?.velocity ?? Vector3.zero);
        }

        private IActor GetActor(Flow f)
        {
            if (SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a))
                return a;
            return null;
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
            avatarPosition = ValueOutput<Vector3>(nameof(avatarPosition), (f) => SpatialBridge.actorService.localActor.avatar.position);
            avatarRotation = ValueOutput<Quaternion>(nameof(avatarRotation), (f) => SpatialBridge.actorService.localActor.avatar.rotation);
            avatarVelocity = ValueOutput<Vector3>(nameof(avatarVelocity), (f) => SpatialBridge.actorService.localActor.avatar.velocity);
        }
    }

    [UnitTitle("Local Actor: Avatar Is Grounded")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Avatar Is Grounded")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorGroundedNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput isGrounded { get; private set; }

        protected override void Definition()
        {
            isGrounded = ValueOutput<bool>(nameof(isGrounded), (f) => SpatialBridge.actorService.localActor.avatar.isGrounded);
        }
    }
}
