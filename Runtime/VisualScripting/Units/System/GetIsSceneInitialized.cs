using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial System: Is Scene Initialized")]
    [UnitSurtitle("Spatial System")]
    [UnitShortTitle("Is Scene Initialized")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetIsSceneInitialized : Unit
    {
        [DoNotSerialize]
        public ValueOutput isInitialized { get; private set; }
        protected override void Definition()
        {
            isInitialized = ValueOutput<bool>(nameof(isInitialized), (f) => ClientBridge.GetIsSceneInitialized?.Invoke() ?? true);
        }
    }
}
