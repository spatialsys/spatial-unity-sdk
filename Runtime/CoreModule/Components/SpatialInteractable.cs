using SpatialSys.UnitySDK.Internal;
using UnityEngine;
using UnityEngine.Serialization;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Core/Components")]
    public class SpatialInteractable : SpatialComponentBase
    {
        public enum IconType
        {
            None,
            Charge,
            Couch,
            CrisisAlert,
            DoorFront,
            FitnessCenter,
            Flood,
            Hail,
            LunchDining,
            MusicNote,
            RamenDining,
            Soap,
            Timer,
            Weapon,
        }

        public override string prettyName => "Interactable";
        public override string tooltip => "An object that users can interact with to trigger an event";
        public override string documentationURL => "https://docs.spatial.io/interactable";

        public string interactText = "Interact";
        public IconType iconType;
        [HideInInspector]
        public Sprite icon;
        [FormerlySerializedAs("radius")]
        public float interactiveRadius = 5f;
        public float visibilityRadius = 10f;

        public SpatialEvent onInteractEvent;
        public SpatialEvent onEnterEvent;
        public SpatialEvent onExitEvent;

        private void Start()
        {
            SpatialBridge.spatialComponentService.InitializeInteractable(this);
        }

        public void UpdateRadius(float interactiveRadius, float visibilityRadius)
        {
            this.interactiveRadius = interactiveRadius;
            this.visibilityRadius = visibilityRadius;
            ValidateParameters();
        }

        private void ValidateParameters()
        {
            if (visibilityRadius < interactiveRadius)
            {
                visibilityRadius = interactiveRadius;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            ValidateParameters();
        }
#endif
    }
}
