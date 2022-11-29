using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [RequireComponent(typeof(Animator))]
    public class SpatialSyncedAnimator : SpatialComponentBase
    {
        public override string prettyName => "Synced Animator";
        public override string tooltip => "The animator on this gameobject will have its parameters and triggers synced with all connected users";
        public override string documentationURL => "https://spatialxr.notion.site/Synced-Animator-43ee9846252541a3a62b05e46433dc25";

        public override bool isExperimental => true;

        [HideInInspector]
        public Animator animator;
        [HideInInspector]
        public int id;
    }
}
