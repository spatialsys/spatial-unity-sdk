using System;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK
{
    [InternalType]
    [System.Serializable]
    public struct CollisionPair
    {
        public int layer1;
        public int layer2;
        public bool ignore;

        public CollisionPair(int layer1, int layer2, bool ignore)
        {
            this.layer1 = layer1;
            this.layer2 = layer2;
            this.ignore = ignore;
        }
    }

    [InternalType]
    public class SavedProjectSettings : ScriptableObject
    {
        [HideInInspector] public string publishedSDKVersion;
        [HideInInspector] public List<CollisionPair> customCollisionSettings;
        [HideInInspector] public List<CollisionPair> customCollision2DSettings;
    }
}
