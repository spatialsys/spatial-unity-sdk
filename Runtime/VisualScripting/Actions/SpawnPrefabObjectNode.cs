using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Spawn Prefab Object")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Spawn Prefab Object")]
    [UnitCategory("Spatial\\Actions")]
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
            // TODO: scale support? might get abused.

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SpawnPrefabObject?.Invoke(f.GetValue<string>(sku), f.GetValue<Vector3>(spawnPosition), f.GetValue<Quaternion>(spawnRotation));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
