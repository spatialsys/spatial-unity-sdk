using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public abstract class SpatialComponentBase : MonoBehaviour
    {
        //these values show up in the black inspector block
        public virtual string prettyName { get; }
        public virtual string tooltip { get; }
        public virtual string documentationURL { get; }

        //used to mark components that will likely have breaking changes in the future
        public virtual bool isExperimental { get; }
    }
}
