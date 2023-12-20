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
        [PortLabel("Avatar Is Loaded")]
        public ValueOutput avatarExists { get; private set; }

        protected override void Definition()
        {
            humanBone = ValueInput<HumanBodyBones>(nameof(humanBone), HumanBodyBones.Hips);
            avatarBoneTransform = ValueOutput<Transform>(nameof(avatarBoneTransform), (f) => SpatialBridge.actorService.localActor.avatar.GetAvatarBoneTransform(f.GetValue<HumanBodyBones>(humanBone)));
            avatarExists = ValueOutput<bool>(nameof(avatarExists), (f) => SpatialBridge.actorService.localActor.avatar.isBodyLoaded);
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
        [PortLabel("Avatar Is Loaded")]
        public ValueOutput avatarExists { get; private set; }
        [DoNotSerialize]
        [PortLabel("Transform")]
        public ValueOutput avatarBoneTransform { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);
            avatarExists = ValueOutput<bool>(nameof(avatarExists), (f) => SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a) && (a.avatar?.isBodyLoaded ?? false));

            humanBone = ValueInput<HumanBodyBones>(nameof(humanBone), HumanBodyBones.Hips);
            avatarBoneTransform = ValueOutput<Transform>(nameof(avatarBoneTransform), (f) => {
                if (SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a))
                    return a.avatar?.GetAvatarBoneTransform(f.GetValue<HumanBodyBones>(humanBone));
                return null;
            });
        }
    }
}
