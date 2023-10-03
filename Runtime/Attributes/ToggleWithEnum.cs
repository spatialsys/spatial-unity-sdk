using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class ToggleWithEnum : PropertyAttribute
    {
        public string targetPropertyName;
        public int[] validOptions;

        public ToggleWithEnum(string targetPropertyName, params int[] validOptions)
        {
            this.targetPropertyName = targetPropertyName;
            this.validOptions = validOptions;
        }
    }
}
