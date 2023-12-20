using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Avatar Body Materials")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Get Avatar Materials")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarMaterialNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Avatar Is Loaded")]
        public ValueOutput avatarExists { get; private set; }

        [DoNotSerialize]
        [PortLabel("Material")]
        public ValueOutput avatarMaterials { get; private set; }

        protected override void Definition()
        {
            avatarExists = ValueOutput<bool>(nameof(avatarExists), (f) => SpatialBridge.actorService.localActor.avatar.isBodyLoaded);
            avatarMaterials = ValueOutput<Material[]>(nameof(avatarMaterials), (f) => SpatialBridge.actorService.localActor.avatar.bodyMaterials);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Actor: Get Avatar Body Materials")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get Avatar Materials")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetAvatarMaterialNode : Unit
    {
        [DoNotSerialize]
        [NullMeansSelf]
        public ValueInput actor { get; private set; }

        [DoNotSerialize]
        [PortLabel("Avatar Is Loaded")]
        public ValueOutput avatarExists { get; private set; }

        [DoNotSerialize]
        [PortLabel("Materials")]
        public ValueOutput avatarMaterials { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);
            avatarExists = ValueOutput<bool>(nameof(avatarExists), (f) => GetActor(f)?.avatar?.isBodyLoaded ?? false);
            avatarMaterials = ValueOutput<Material[]>(nameof(avatarMaterials), (f) => GetActor(f)?.avatar?.bodyMaterials);
        }

        private IActor GetActor(Flow f)
        {
            if (SpatialBridge.actorService.actors.TryGetValue(f.GetValue<int>(actor), out IActor a))
                return a;
            return null;
        }
    }
}
