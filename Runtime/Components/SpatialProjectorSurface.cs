using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialProjectorSurface : SpatialComponentBase
    {
        public override string prettyName => "Projector Surface";
        public override string tooltip => "Define a surface that can be used to project screen-shares and video streams on to.";

        public float size = 1f;
        public bool dotsVisible;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Vector2 rect = new Vector2(2f, 1f) * size;
            Gizmos.DrawWireCube(Vector3.zero, rect);
        }
    }
}
