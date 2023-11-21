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
        public ValueOutput isSignedIn { get; private set; }
        [DoNotSerialize]
        public ValueOutput isSpaceAdmin { get; private set; }
        [DoNotSerialize]
        public ValueOutput platform { get; private set; }

        protected override void Definition()
        {
            userID = ValueOutput<string>(nameof(userID), (f) => ClientBridge.GetLocalActorUserData.Invoke().userID);
            displayName = ValueOutput<string>(nameof(displayName), (f) => ClientBridge.GetLocalActorUserData.Invoke().displayName);
            username = ValueOutput<string>(nameof(username), (f) => ClientBridge.GetLocalActorUserData.Invoke().username);
            isSignedIn = ValueOutput<bool>(nameof(isSignedIn), (f) => ClientBridge.GetLocalActorUserData.Invoke().isSignedIn);
            isSpaceAdmin = ValueOutput<bool>(nameof(isSpaceAdmin), (f) => ClientBridge.GetLocalActorUserData.Invoke().isSpaceAdmin);
            platform = ValueOutput<SpatialPlatform>(nameof(platform), (f) => {
                return VisualScriptingUtility.ConvertClientPlatformToScriptingPlatform(ClientBridge.GetLocalActorUserData.Invoke().platform);
            });
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
        public ValueOutput isSignedIn { get; private set; }
        [DoNotSerialize]
        public ValueOutput isSpaceAdmin { get; private set; }
        [DoNotSerialize]
        public ValueOutput platform { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);

            exists = ValueOutput<bool>(nameof(exists), (f) => ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor)).exists);
            userID = ValueOutput<string>(nameof(userID), (f) => ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor)).userID);
            displayName = ValueOutput<string>(nameof(displayName), (f) => ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor)).displayName);
            username = ValueOutput<string>(nameof(username), (f) => ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor)).username);
            isSignedIn = ValueOutput<bool>(nameof(isSignedIn), (f) => ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor)).isSignedIn);
            isSpaceAdmin = ValueOutput<bool>(nameof(isSpaceAdmin), (f) => ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor)).isSpaceAdmin);
            platform = ValueOutput<SpatialPlatform>(nameof(platform), (f) =>
                VisualScriptingUtility.ConvertClientPlatformToScriptingPlatform(ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor)).platform)
            );
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
            userID = ValueOutput<string>(nameof(userID), (f) => ClientBridge.GetLocalActorUserData.Invoke().userID);
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
                ClientBridge.ActorUserData userData = ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor));
                return userData.exists ? userData.userID : "";
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
            actorName = ValueOutput<string>(nameof(actorName), (f) => ClientBridge.GetLocalActorUserData.Invoke().displayName);
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
                ClientBridge.ActorUserData userData = ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor));
                return userData.exists ? userData.displayName : "";
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
            username = ValueOutput<string>(nameof(username), (f) => ClientBridge.GetLocalActorUserData.Invoke().username);
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
                ClientBridge.ActorUserData userData = ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor));
                return userData.exists ? userData.username : "";
            });
        }
    }

    [UnitTitle("Local Actor: Is Signed In")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Is Signed In")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorIsSignedInNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput isSignedIn { get; private set; }

        protected override void Definition()
        {
            isSignedIn = ValueOutput<bool>(nameof(isSignedIn), (f) => ClientBridge.GetLocalActorUserData.Invoke().isSignedIn);
        }
    }

    [UnitTitle("Actor: Is Signed In")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Is Signed In")]
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
                ClientBridge.ActorUserData userData = ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor));
                return userData.exists && userData.isSignedIn;
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
            isSpaceAdmin = ValueOutput<bool>(nameof(isSpaceAdmin), (f) => ClientBridge.GetLocalActorUserData.Invoke().isSpaceAdmin);
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
                ClientBridge.ActorUserData userData = ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor));
                return userData.exists && userData.isSpaceAdmin;
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
            actorPlatform = ValueOutput<SpatialPlatform>(nameof(actorPlatform),
                (f) => VisualScriptingUtility.ConvertClientPlatformToScriptingPlatform(ClientBridge.GetLocalActorUserData.Invoke().platform)
            );
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

            actorPlatform = ValueOutput<SpatialPlatform>(nameof(actorPlatform),
                (f) => VisualScriptingUtility.ConvertClientPlatformToScriptingPlatform(ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor)).platform)
            );
            actorExists = ValueOutput<bool>(nameof(actorExists), (f) => ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor)).exists);
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
            isTalking = ValueOutput<bool>(nameof(isTalking), (f) => ClientBridge.GetLocalActorUserData.Invoke().isTalking);
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
                ClientBridge.ActorUserData userData = ClientBridge.GetActorUserData.Invoke(f.GetValue<int>(actor));
                return userData.exists && userData.isTalking;
            });
        }
    }
}
