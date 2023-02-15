using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialPointOfInterest))]
    public class SpatialPointOfInterestEditor : SpatialComponentEditorBase
    {
        public void OnSceneGUI()
        {
            var t = target as SpatialPointOfInterest;

            SpatialHandles.RadiusHandle(t.transform.position, ref t.textDisplayRadius);
            SpatialHandles.RadiusHandle(t.transform.position, ref t.markerDisplayRadius);
        }
    }
}