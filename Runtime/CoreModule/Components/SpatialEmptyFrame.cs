using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Spatial Components")]
    public class SpatialEmptyFrame : SpatialComponentBase
    {
        public override string prettyName => "Empty Frame";
        public override string tooltip => "Use to specify a location where an empty frame will be created when the space is opened in spatial.";
        public override string documentationURL => "https://docs.spatial.io/components/empty-frame";

        public float size = 1f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(size, size, 0f));
        }
    }
}
