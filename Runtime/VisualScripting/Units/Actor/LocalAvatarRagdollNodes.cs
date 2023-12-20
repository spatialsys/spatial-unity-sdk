using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Ragdoll Physics Active")]
    [UnitSurtitle("Local Actor Ragdoll")]
    [UnitShortTitle("Get Ragdoll Physics Active")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarRagdollPhysicsActiveNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput active { get; private set; }

        protected override void Definition()
        {
            active = ValueOutput<bool>(nameof(active), (f) => SpatialBridge.actorService.localActor.avatar.ragdollPhysicsActive);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Ragdoll Physics Active")]
    [UnitSurtitle("Local Actor Ragdoll")]
    [UnitShortTitle("Set Ragdoll Physics Active")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetLocalAvatarRagdollPhysicsActiveNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput active { get; private set; }
        [DoNotSerialize]
        public ValueInput initialVelocity { get; private set; }

        protected override void Definition()
        {
            active = ValueInput<bool>(nameof(active), @default: true);
            initialVelocity = ValueInput<Vector3>(nameof(initialVelocity), @default: Vector3.zero);
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.SetRagdollPhysicsActive(f.GetValue<bool>(active), f.GetValue<Vector3>(initialVelocity));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Ragdoll Velocity")]
    [UnitSurtitle("Local Actor Ragdoll")]
    [UnitShortTitle("Get Ragdoll Velocity")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarRagdollVelocityNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput velocity { get; private set; }

        protected override void Definition()
        {
            velocity = ValueOutput<Vector3>(nameof(velocity), (f) => SpatialBridge.actorService.localActor.avatar.ragdollVelocity);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Ragdoll Velocity")]
    [UnitSurtitle("Local Actor Ragdoll")]
    [UnitShortTitle("Set Ragdoll Velocity")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetLocalAvatarRagdollVelocityNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput velocity { get; private set; }

        protected override void Definition()
        {
            velocity = ValueInput<Vector3>(nameof(velocity), @default: Vector3.zero);
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.ragdollVelocity = f.GetValue<Vector3>(velocity);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Add Force To Ragdoll")]
    [UnitSurtitle("Local Actor Ragdoll")]
    [UnitShortTitle("Add Force")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class AddForceToLocalAvatarRagdollNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput force { get; private set; }
        [DoNotSerialize]
        public ValueInput ignoreMass { get; private set; }

        protected override void Definition()
        {
            force = ValueInput<Vector3>(nameof(force), @default: Vector3.zero);
            ignoreMass = ValueInput<bool>(nameof(ignoreMass), @default: false);
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.AddRagdollForce(f.GetValue<Vector3>(force), f.GetValue<bool>(ignoreMass));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}
