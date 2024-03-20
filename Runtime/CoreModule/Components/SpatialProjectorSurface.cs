using System;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Components")]
    public class SpatialProjectorSurface : SpatialComponentBase
    {
        public const int LATEST_VERSION = 1;

        [HideInInspector]
        public int version;

        public override string prettyName => "Projector Surface";
        public override string tooltip => "Define a surface that can be used to project screen-shares and video streams on to.";
        public override string documentationURL => null;

        [HideInInspector]
        [Obsolete("Use size2D instead")]
        public float size = 1f;

        public Vector2 size2D = new Vector2(2f, 1f);
        public bool dotsVisible = false;

        private void Awake()
        {
            UpgradeDataIfNecessary();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Vector2 rect = size2D;
            Gizmos.DrawWireCube(Vector3.zero, rect);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            UpgradeDataIfNecessary();
        }
#endif

        public void UpgradeDataIfNecessary()
        {
            if (version == LATEST_VERSION)
                return;

            if (version == 0)
            {
#pragma warning disable 0618
                size2D = new Vector2(2f, 1f) * size;
#pragma warning restore 0618
                version = 1;
            }
        }
    }
}
