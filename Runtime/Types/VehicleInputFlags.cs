using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [Flags]
    public enum VehicleInputFlags
    {
        None = 0,
        Steer1D = 1 << 0,
        Throttle = 1 << 1,
        Reverse = 1 << 2,
        PrimaryAction = 1 << 3,
        SecondaryAction = 1 << 4,
        Exit = 1 << 5,
    }
}