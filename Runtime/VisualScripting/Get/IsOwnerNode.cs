using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Is Space Owner")]
    public class IsSpaceOwner : Unit
    {
        [DoNotSerialize]
        public ValueOutput isOwner { get; private set; }

        protected override void Definition()
        {
            isOwner = ValueOutput<bool>(nameof(isOwner), (f) => ClientBridge.IsLocalOwner.Invoke());
        }
    }
}