using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialPointOfInterest : SpatialComponentBase
    {
        public override string prettyName => "Point Of Interest";
        public override string tooltip => "A location marker that will display additional information when approaching it";
        public override string documentationURL => "https://spatialxr.notion.site/Point-Of-Interest-31f70fd2e1064e8d82b40a1834ed348c";
        public override bool isExperimental => true;
        public string title = "Point of interest";
        [Tooltip("Optional description of this point of interest.")]
        public string description;

        [Tooltip("Distance from the user where this point of interest will show the title and description")]
        public float textDisplayRadius = 3.0f;
        [Tooltip("Distance from the user where this point of interest will start showing an marker icon")]
        public float markerDisplayRadius = 6.0f;
        public SpatialEvent onTextDisplayedEvent;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (markerDisplayRadius < textDisplayRadius)
                markerDisplayRadius = textDisplayRadius;
        }
#endif
    }
}