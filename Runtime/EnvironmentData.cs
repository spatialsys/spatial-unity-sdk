using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SpatialSys.UnitySDK
{
    public class EnvironmentData : MonoBehaviour
    {
        public const int VERSION = 1;

        public SpatialSeatHotspot[] seats;
        public SpatialEntrancePoint[] entrancePoints;
        public SpatialTriggerEvent[] triggerEvents;
        public SpatialEmptyFrame[] emptyFrames;
        public SpatialAvatarTeleporter[] avatarTeleporters;

        public Volume[] renderingVolumes;
    }
}
