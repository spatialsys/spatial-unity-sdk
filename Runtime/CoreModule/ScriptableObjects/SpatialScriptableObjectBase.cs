using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Scriptable Objects")]
    public abstract class SpatialScriptableObjectBase : ScriptableObject
    {
        // these values show up in the black inspector block
        public abstract string prettyName { get; }
        public abstract string tooltip { get; }
        public abstract string documentationURL { get; }

        // used to mark components that will likely have breaking changes in the future
        public virtual bool isExperimental => false;
    }
}
