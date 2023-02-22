using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Camera State")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Camera State")]
    [UnitCategory("Spatial\\Get Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetCameraStateNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Position")]
        public ValueOutput cameraPosition { get; private set; }
        [DoNotSerialize]
        [PortLabel("Rotation")]
        public ValueOutput cameraRotation { get; private set; }

        protected override void Definition()
        {
            cameraPosition = ValueOutput<Vector3>(nameof(cameraPosition), (f) => ClientBridge.GetCameraPosition.Invoke());
            cameraRotation = ValueOutput<Quaternion>(nameof(cameraRotation), (f) => ClientBridge.GetCameraRotation.Invoke());
        }
    }
}
