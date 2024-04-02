using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [RequireComponent(typeof(Collider))]
    [DocumentationCategory("Core/Components")]
    public class SpatialMovementMaterialSurface : SpatialComponentBase
    {
        public override string prettyName => "Movement Material Surface";
        public override string tooltip => $"Apply a movement material to this collider to define how avatars interact with this surface.";
        public override string documentationURL => "https://docs.spatial.io/movement-materials";
        public override bool isExperimental => false;

        public SpatialMovementMaterial movementMaterial;
    }
}
