using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Local Actor: Get User Data")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Get User Data")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorUserDataNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput userID { get; private set; }
        [DoNotSerialize]
        public ValueOutput displayName { get; private set; }
        [DoNotSerialize]
        public ValueOutput username { get; private set; }
        [DoNotSerialize]
        [PortLabel(nameof(IActor.isRegistered))]
        public ValueOutput isSignedIn { get; private set; }
        [DoNotSerialize]
        public ValueOutput isSpaceAdmin { get; private set; }
        [DoNotSerialize]
        public ValueOutput platform { get; private set; }

        protected override void Definition()
        {
            userID = ValueOutput<string>(nameof(userID), (f) => SpatialBridge.actorService.localActor.userID);
            displayName = ValueOutput<string>(nameof(displayName), (f) => SpatialBridge.actorService.localActor.displayName);
            username = ValueOutput<string>(nameof(username), (f) => SpatialBridge.actorService.localActor.username);
            isSignedIn = ValueOutput<bool>(nameof(isSignedIn), (f) => SpatialBridge.actorService.localActor.isRegistered);
            isSpaceAdmin = ValueOutput<bool>(nameof(isSpaceAdmin), (f) => SpatialBridge.actorService.localActor.isSpaceAdministrator);
#pragma warning disable CS0618 // Type or member is obsolete
            platform = ValueOutput<SpatialPlatform>(nameof(platform), (f) => (SpatialPlatform)(int)SpatialBridge.actorService.localActor.platform);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    [UnitTitle("Actor: Get User Data")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get User Data")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorUserDataNode : Unit
    {
        [DoNotSerialize]
        public ValueInput actor { get; private set; }

        [DoNotSerialize]
        public ValueOutput exists { get; private set; }
        [DoNotSerialize]
        public ValueOutput userID { get; private set; }
        [DoNotSerialize]
        public ValueOutput displayName { get; private set; }
        [DoNotSerialize]
        public ValueOutput username { get; private set; }
        [DoNotSerialize]
        [PortLabel(nameof(IActor.isRegistered))]
        public ValueOutput isSignedIn { get; private set; }
        [DoNotSerialize]
        public ValueOutput isSpaceAdmin { get; private set; }
        [DoNotSerialize]
        public ValueOutput platform { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);

            exists = ValueOutput<bool>(nameof(exists), (f) => GetActor(f) != null);
            userID = ValueOutput<string>(nameof(userID), (f) => GetActor(f)?.userID);
            displayName = ValueOutput<string>(nameof(displayName), (f) => GetActor(f)?.displayName);
            username = ValueOutput<string>(nameof(username), (f) => GetActor(f)?.username);
            isSignedIn = ValueOutput<bool>(nameof(isSignedIn), (f) => GetActor(f)?.isRegistered ?? false);
            isSpaceAdmin = ValueOutput<bool>(nameof(isSpaceAdmin), (f) => GetActor(f)?.isSpaceAdministrator ?? false);
#pragma warning disable CS0618 // Type or member is obsolete
            platform = ValueOutput<SpatialPlatform>(nameof(platform), (f) => {
                IActor actor = GetActor(f);
                return (actor != null) ? (SpatialPlatform)(int)actor.platform : SpatialPlatform.Unknown;
            });
#pragma warning restore CS0618 // Type or member is obsolete
        }

        private IActor GetActor(Flow f)
        {
            if (SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a))
                return a;
            return null;
        }
    }

    [UnitTitle("Local Actor: Get User ID")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Get User ID")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorUserID : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput userID { get; private set; }

        protected override void Definition()
        {
            userID = ValueOutput<string>(nameof(userID), (f) => SpatialBridge.actorService.localActor.userID);
        }
    }

    [UnitTitle("Actor: Get User ID")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get User ID")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorUserID : Unit
    {
        [DoNotSerialize]
        public ValueInput actor { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput userID { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);

            userID = ValueOutput<string>(nameof(userID), (f) => {
                return SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a) ? a.userID : null;
            });
        }
    }

    [UnitTitle("Local Actor: Get Display Name")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Get Display Name")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorNameNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput actorName { get; private set; }

        protected override void Definition()
        {
            actorName = ValueOutput<string>(nameof(actorName), (f) => SpatialBridge.actorService.localActor.displayName);
        }
    }

    [UnitTitle("Actor: Get Display Name")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get Display Name")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorNameNode : Unit
    {
        [DoNotSerialize]
        public ValueInput actor { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput actorName { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);

            actorName = ValueOutput<string>(nameof(actorName), (f) => {
                return SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a) ? a.displayName : null;
            });
        }
    }

    [UnitTitle("Local Actor: Get Username")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Get Username")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorUsernameNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput username { get; private set; }

        protected override void Definition()
        {
            username = ValueOutput<string>(nameof(username), (f) => SpatialBridge.actorService.localActor.username);
        }
    }

    [UnitTitle("Actor: Get Username")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get Username")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorUsernameNode : Unit
    {
        [DoNotSerialize]
        public ValueInput actor { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput username { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);

            username = ValueOutput<string>(nameof(username), (f) => {
                return SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a) ? a.username : null;
            });
        }
    }

    [UnitTitle("Local Actor: Is Registered")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Is Registered")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorIsSignedInNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput isSignedIn { get; private set; }

        protected override void Definition()
        {
            isSignedIn = ValueOutput<bool>(nameof(isSignedIn), (f) => SpatialBridge.actorService.localActor.isRegistered);
        }
    }

    [UnitTitle("Actor: Is Registered")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Is Registered")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorIsSignedInNode : Unit
    {
        [DoNotSerialize]
        public ValueInput actor { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput isSignedIn { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);

            isSignedIn = ValueOutput<bool>(nameof(isSignedIn), (f) => {
                return SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a) && a.isRegistered;
            });
        }
    }

    [UnitTitle("Local Actor: Is Space Admin")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Is Space Admin")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorIsSpaceAdminNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput isSpaceAdmin { get; private set; }

        protected override void Definition()
        {
            isSpaceAdmin = ValueOutput<bool>(nameof(isSpaceAdmin), (f) => SpatialBridge.actorService.localActor.isSpaceAdministrator);
        }
    }

    [UnitTitle("Actor: Is Space Admin")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Is Space Admin")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorIsSpaceAdminNode : Unit
    {
        [DoNotSerialize]
        public ValueInput actor { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput isSpaceAdmin { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);

            isSpaceAdmin = ValueOutput<bool>(nameof(isSpaceAdmin), (f) => {
                return SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a) && a.isSpaceAdministrator;
            });
        }
    }

    [UnitTitle("Local Actor: Get Platform")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Get Platform")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorPlatformNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput actorPlatform { get; private set; }

        protected override void Definition()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            actorPlatform = ValueOutput<SpatialPlatform>(nameof(actorPlatform), (f) => {
                return (SpatialPlatform)(int)SpatialBridge.actorService.localActor.platform;
            });
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    [UnitTitle("Actor: Get Platform")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get Platform")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
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

#pragma warning disable CS0618 // Type or member is obsolete
            actorPlatform = ValueOutput<SpatialPlatform>(nameof(actorPlatform), (f) => {
                if (SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a))
                    return (SpatialPlatform)(int)a.platform;
                return SpatialPlatform.Unknown;
            });
#pragma warning restore CS0618 // Type or member is obsolete
            actorExists = ValueOutput<bool>(nameof(actorExists), (f) => SpatialBridge.actorService.actors.ContainsKey(f.GetValue<int>(actor)));
        }
    }

    [UnitTitle("Local Actor: Is Talking")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Is Talking")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorIsTalkingNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput isTalking { get; private set; }

        protected override void Definition()
        {
            isTalking = ValueOutput<bool>(nameof(isTalking), (f) => SpatialBridge.actorService.localActor.isTalking);
        }
    }

    [UnitTitle("Actor: Is Talking")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Is Talking")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorIsTalkingNode : Unit
    {
        [DoNotSerialize]
        public ValueInput actor { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput isTalking { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);

            isTalking = ValueOutput<bool>(nameof(isTalking), (f) => {
                return SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a) && a.isTalking;
            });
        }
    }
}
