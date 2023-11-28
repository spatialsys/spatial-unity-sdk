using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class ClampAttribute : PropertyAttribute
    {
        public float min;
        public float max;

        public ClampAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
