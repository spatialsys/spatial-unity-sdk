using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Spawn Prefab Object from Package")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Spawn Prefab Object from Package")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(SpatialPrefabObject))]
    public class SpawnPrefabObjectNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput sku { get; private set; }
        [DoNotSerialize]
        public ValueInput spawnPosition { get; private set; }
        [DoNotSerialize]
        public ValueInput spawnRotation { get; private set; }

        protected override void Definition()
        {
            sku = ValueInput<string>(nameof(sku));
            spawnPosition = ValueInput<Vector3>(nameof(spawnPosition));
            spawnRotation = ValueInput<Quaternion>(nameof(spawnRotation), Quaternion.identity);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SpawnPrefabObjectFromPackage?.Invoke(f.GetValue<string>(sku), f.GetValue<Vector3>(spawnPosition), f.GetValue<Quaternion>(spawnRotation));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Spawn Prefab Object from Embedded Package Asset")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Spawn Prefab Object from Embedded Package Asset")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(SpatialPrefabObject))]
    public class SpawnPrefabObjectFromEmbeddedNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput assetID { get; private set; }
        [DoNotSerialize]
        public ValueInput spawnPosition { get; private set; }
        [DoNotSerialize]
        public ValueInput spawnRotation { get; private set; }

        protected override void Definition()
        {
            assetID = ValueInput<string>(nameof(assetID));
            spawnPosition = ValueInput<Vector3>(nameof(spawnPosition));
            spawnRotation = ValueInput<Quaternion>(nameof(spawnRotation), Quaternion.identity);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SpawnPrefabObjectFromEmbedded?.Invoke(f.GetValue<string>(assetID), f.GetValue<Vector3>(spawnPosition), f.GetValue<Quaternion>(spawnRotation));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
