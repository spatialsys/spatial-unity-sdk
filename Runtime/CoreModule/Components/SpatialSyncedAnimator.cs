using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Core/Components")]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class SpatialSyncedAnimator : SpatialComponentBase, ISpatialComponentWithOwner
    {
        public override string prettyName => "Synced Animator";
        public override string tooltip => "The animator on this gameobject will have its parameters and triggers synced with all connected users";
        public override string documentationURL => "https://toolkit.spatial.io/docs/multiplayer/synced-animator";

        public override bool isExperimental => true;

        [HideInInspector]
        public Animator animator;
        [HideInInspector]
        public int id;

        /// <summary>
        /// Sets a parameter on the animator. This will be synced with all connected users.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">Value to set.</param>
        public void SetParameter(string parameterName, object value)
        {
            SpatialBridge.spatialComponentService.SetSyncedAnimatorParameter(this, parameterName, value);
        }

        /// <summary>
        /// Sets a trigger on the animator. This will be synced with all connected users.
        /// </summary>
        /// <param name="triggerName">Name of the trigger</param>
        public void SetTrigger(string triggerName)
        {
            SpatialBridge.spatialComponentService.SetSyncedAnimatorTrigger(this, triggerName);
        }
    }
}
