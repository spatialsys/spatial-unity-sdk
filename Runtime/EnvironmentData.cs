using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class EnvironmentData : MonoBehaviour
    {
        public const int VERSION = 1;

        public SpatialSeatHotspot[] seats;
        public SpatialEntrancePoint[] entrancePoints;
        public SpatialEmptyFrame[] emptyFrames;
    }
}
