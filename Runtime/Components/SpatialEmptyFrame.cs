using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialEmptyFrame : SpatialComponentBase
    {
        public override string prettyName => "Empty Frame";
        public override string tooltip => "Use to specify a location where an empty frame will be created when the space is opened in spatial.";

        public float size = 1f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(size, size, 0f));
            Gizmos.DrawLine(Vector3.zero, new Vector3(0f, 0f, .4f));
        }
    }
}
