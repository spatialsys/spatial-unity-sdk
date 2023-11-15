using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [CreateAssetMenu(fileName = "NewMovementMaterial", menuName = "Spatial/MovementMaterial", order = 2)]
    public class SpatialMovementMaterial : SpatialScriptableObjectBase
    {
        public override string prettyName => "Movement Material";
        public override string tooltip =>
@"Define the audio, visual, and physics properties of how avatars interact with a surface. 

Can be applied to a collider using the <b>Movement Material Surface</b> component.";
        public override string documentationURL => "https://docs.spatial.io/movement-materials";
        public override bool isExperimental => false;

        public bool splitRunAndWalk = false;
        public SpatialSFX stepSound;
        public GameObject stepVFX;
        public SpatialSFX walkStepSound;
        public GameObject walkStepVFX;
        public SpatialSFX runStepSound;
        public GameObject runStepVFX;
        public SpatialSFX jumpSound;
        public GameObject jumpVFX;
        public SpatialSFX landSound;
        public GameObject landVFX;
        public SpatialSFX stopSound;
        public GameObject stopVFX;
        public SpatialSFX takeoffSound;
        public GameObject takeoffVFX;

        [Tooltip("In the case that an object has a physics material in addition to a movement material should the physics material properties be used instead?")]
        public bool usePhysicsMaterial = true;
        [Tooltip("Friction applied while the avatar is trying to move.")]
        public float dynamicFriction = 1f;
        [Tooltip("Friction applied while the avatar is trying to stop.")]
        public float staticFriction = 1f;
        public PhysicMaterialCombine frictionCombine = PhysicMaterialCombine.Average;

        [Tooltip("Should the SFX be played for remote avatars?")]
        public bool syncSFX = true;
        [Tooltip("Synced SFX volume will be multiplied by this value. .5 means synced sounds will be half as loud as local sounds.")]
        public float syncVolume = .5f;
        [Tooltip("Should the VFX be played for remote avatars?")]
        public bool syncVFX = true;
        [Tooltip("Should the synced events only be played for avatars within a certain distance from the camera?")]
        public bool limitSyncDistance = true;
        public float maxSyncDistance = 50f;
    }
}
