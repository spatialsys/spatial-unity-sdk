using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Avatar Materials")]

    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Get Avatar Materials")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarMaterialNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Avatar Exists")]
        public ValueOutput avatarExists { get; private set; }

        [DoNotSerialize]
        [PortLabel("Material")]
        public ValueOutput avatarMaterials { get; private set; }

        protected override void Definition()
        {
            avatarExists = ValueOutput<bool>(nameof(avatarExists), (f) => ClientBridge.GetLocalAvatarBodyExist?.Invoke() ?? false);
            avatarMaterials = ValueOutput<Material[]>(nameof(avatarMaterials), (f) => ClientBridge.GetLocalAvatarMaterials?.Invoke() ?? null);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Actor: Get Avatar Materials")]

    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get Avatar Materials")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetAvatarMaterialNode : Unit
    {
        [DoNotSerialize]
        [NullMeansSelf]
        public ValueInput actor { get; private set; }

        [DoNotSerialize]
        [PortLabel("Avatar Exists")]
        public ValueOutput avatarExists { get; private set; }

        [DoNotSerialize]
        [PortLabel("Materials")]
        public ValueOutput avatarMaterials { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);
            avatarExists = ValueOutput<bool>(nameof(avatarExists), (f) => ClientBridge.GetAvatarExists?.Invoke(f.GetValue<int>(actor)) ?? false);
            avatarMaterials = ValueOutput<Material[]>(nameof(avatarMaterials), (f) => ClientBridge.GetAvatarMaterials?.Invoke(f.GetValue<int>(actor)) ?? null);
        }
    }
}
