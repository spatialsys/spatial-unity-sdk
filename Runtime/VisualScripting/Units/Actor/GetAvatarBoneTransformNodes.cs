using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Bone Transform")]

    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Get Bone Transform")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarBoneTransformNode : Unit
    {
        [DoNotSerialize]
        [NullMeansSelf]
        public ValueInput humanBone { get; private set; }

        [DoNotSerialize]
        [PortLabel("Transform")]
        public ValueOutput avatarBoneTransform { get; private set; }

        [DoNotSerialize]
        [PortLabel("Avatar Exists")]
        public ValueOutput avatarExists { get; private set; }

        protected override void Definition()
        {
            humanBone = ValueInput<HumanBodyBones>(nameof(humanBone), HumanBodyBones.Hips);
            avatarBoneTransform = ValueOutput<Transform>(nameof(avatarBoneTransform), (f) => ClientBridge.GetLocalAvatarBoneTransform?.Invoke(f.GetValue<HumanBodyBones>(humanBone)) ?? null);
            avatarExists = ValueOutput<bool>(nameof(avatarExists), (f) => ClientBridge.GetLocalAvatarBodyExist?.Invoke() ?? false);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Actor: Get Bone Transform")]

    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get Bone Transform")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetAvatarBoneTransformNode : Unit
    {
        [DoNotSerialize]
        [NullMeansSelf]
        public ValueInput actor { get; private set; }
        [DoNotSerialize]
        [NullMeansSelf]
        public ValueInput humanBone { get; private set; }

        [DoNotSerialize]
        [PortLabel("Avatar Exists")]
        public ValueOutput avatarExists { get; private set; }
        [DoNotSerialize]
        [PortLabel("Transform")]
        public ValueOutput avatarBoneTransform { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);
            avatarExists = ValueOutput<bool>(nameof(avatarExists), (f) => ClientBridge.GetAvatarExists?.Invoke(f.GetValue<int>(actor)) ?? false);

            humanBone = ValueInput<HumanBodyBones>(nameof(humanBone), HumanBodyBones.Hips);
            avatarBoneTransform = ValueOutput<Transform>(nameof(avatarBoneTransform), (f) => ClientBridge.GetAvatarBoneTransform?.Invoke(f.GetValue<int>(actor), f.GetValue<HumanBodyBones>(humanBone)) ?? null);
        }
    }
}
