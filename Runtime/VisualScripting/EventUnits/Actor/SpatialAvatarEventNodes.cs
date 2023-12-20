using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Local Avatar: On Jump")]
    [UnitSurtitle("Spatial Local Avatar")]
    [UnitShortTitle("On Jump")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnLocalAvatarJumpNode : EventUnit<bool>
    {
        private const string EVENT_HOOK_ID = "OnLocalAvatarJump";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput isGrounded { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(bool isGrounded)
        {
            EventBus.Trigger(EVENT_HOOK_ID, isGrounded);
        }

        protected override void Definition()
        {
            base.Definition();
            isGrounded = ValueOutput<bool>(nameof(isGrounded));
        }

        protected override bool ShouldTrigger(Flow flow, bool isGrounded)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, bool isGrounded)
        {
            flow.SetValue(this.isGrounded, isGrounded);
        }
    }

    [UnitTitle("Spatial Local Avatar: On Emote")]
    [UnitSurtitle("Spatial Local Avatar")]
    [UnitShortTitle("On Emote")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnLocalAvatarEmoteNode : EventUnit<EmptyEventArgs>
    {
        private const string EVENT_HOOK_ID = "OnLocalAvatarEmote";

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent()
        {
            EventBus.Trigger(EVENT_HOOK_ID);
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            return true;
        }
    }

    [UnitTitle("Spatial Local Avatar: On Land")]
    [UnitSurtitle("Spatial Local Avatar")]
    [UnitShortTitle("On Land")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnLocalAvatarLandNode : EventUnit<EmptyEventArgs>
    {
        private const string EVENT_HOOK_ID = "OnLocalAvatarLand";

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent()
        {
            EventBus.Trigger(EVENT_HOOK_ID);
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            return true;
        }
    }

    [UnitTitle("Spatial Local Avatar: On Collider Hit")]
    [UnitSurtitle("Spatial Local Avatar")]
    [UnitShortTitle("On Collider Hit")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnLocalAvatarColliderHitNode : EventUnit<(ControllerColliderHit, Vector3)>
    {
        private const string EVENT_HOOK_ID = "OnLocalAvatarColliderHit";

        [DoNotSerialize]
        public ValueOutput collider { get; private set; }
        [DoNotSerialize]
        public ValueOutput moveDirection { get; private set; }
        [DoNotSerialize]
        public ValueOutput moveLength { get; private set; }
        [DoNotSerialize]
        public ValueOutput normal { get; private set; }
        [DoNotSerialize]
        public ValueOutput point { get; private set; }
        [DoNotSerialize]
        public ValueOutput avatarVelocity { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(ControllerColliderHit hit, Vector3 avatarVelocity)
        {
            EventBus.Trigger(EVENT_HOOK_ID, (hit, avatarVelocity));
        }

        protected override void Definition()
        {
            base.Definition();
            collider = ValueOutput<Collider>(nameof(collider));
            moveDirection = ValueOutput<Vector3>(nameof(moveDirection));
            moveLength = ValueOutput<float>(nameof(moveLength));
            normal = ValueOutput<Vector3>(nameof(normal));
            point = ValueOutput<Vector3>(nameof(point));
            avatarVelocity = ValueOutput<Vector3>(nameof(avatarVelocity));
        }

        protected override bool ShouldTrigger(Flow flow, (ControllerColliderHit, Vector3) args)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, (ControllerColliderHit, Vector3) args)
        {
            ControllerColliderHit hit = args.Item1;
            flow.SetValue(collider, hit.collider);
            flow.SetValue(moveDirection, hit.moveDirection);
            flow.SetValue(moveLength, hit.moveLength);
            flow.SetValue(normal, hit.normal);
            flow.SetValue(point, hit.point);
            flow.SetValue(avatarVelocity, args.Item2);
        }
    }

    [UnitTitle("Spatial Local Avatar: On Is Grounded Changed")]
    [UnitSurtitle("Spatial Local Avatar")]
    [UnitShortTitle("On Is Grounded Changed")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnLocalAvatarIsGroundedChangedNode : EventUnit<bool>
    {
        private const string EVENT_HOOK_ID = "OnLocalAvatarGroundedChanged";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput isGrounded { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(bool isGrounded)
        {
            EventBus.Trigger(EVENT_HOOK_ID, isGrounded);
        }

        protected override void Definition()
        {
            base.Definition();
            isGrounded = ValueOutput<bool>(nameof(isGrounded));
        }

        protected override bool ShouldTrigger(Flow flow, bool isGrounded)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, bool isGrounded)
        {
            flow.SetValue(this.isGrounded, isGrounded);
        }
    }
}