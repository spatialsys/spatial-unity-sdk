using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialPointOfInterest : SpatialComponentBase
    {
        private const float SPRITE_WIDTH = 200; // Based on PointOfView setting in Spatial

        public override string prettyName => "Point Of Interest";
        public override string tooltip => "A location marker that will display additional information when approaching it";
        public override string documentationURL => "https://docs.spatial.io/components/point-of-interest";
        public override bool isExperimental => true;
        public string title = "Point of interest";
        [Tooltip("Optional description of this point of interest.")]
        public string description;

        public Sprite sprite = null;
        [Range(0, 1), Tooltip("Sprite width size in ratio (1: full, 0.5: half)")]
        public float spriteSize = 0.75f;

        [HideInInspector]
        public Vector2 spriteRectSize;

        [Tooltip("Distance from the user where this point of interest will show the title and description")]
        public float textDisplayRadius = 3.0f;
        [Tooltip("Distance from the user where this point of interest will start showing an marker icon")]
        public float markerDisplayRadius = 6.0f;
        [Tooltip("The marker will be visible on top of other objects")]
        public bool alwaysOnTop = false;
        public SpatialEvent onTextDisplayedEvent;

        private void Awake()
        {
            ClientBridge.InitializeSpatialPointOfInterest?.Invoke(this);
        }

        private void OnEnable()
        {
            ClientBridge.PointOfInterestEnabledChanged?.Invoke(this, true);
        }

        private void OnDisable()
        {
            ClientBridge.PointOfInterestEnabledChanged?.Invoke(this, false);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (markerDisplayRadius < textDisplayRadius)
                markerDisplayRadius = textDisplayRadius;

            if (sprite != null)
            {
                float width = SPRITE_WIDTH * spriteSize;
                float height = width * sprite.rect.height / sprite.rect.width;
                spriteRectSize = new Vector2(width, height);
            }
        }
#endif
    }
}