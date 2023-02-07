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

        // Limit this component to one per scene.
        protected virtual bool _limitOnePerScene { get; }

#if UNITY_EDITOR
        private bool _isFirstComponent = false;
        private void OnValidate()
        {
            if(_limitOnePerScene)
            {
                // Destroy this component if it is not the first component in the scene.
                Object[] foundComponents = FindObjectsOfType(this.GetType());
                if (foundComponents.Length > 1)
                {
                    Debug.LogError($"There should only be one <{this.GetType().Name}> in the scene.");
                    if (!_isFirstComponent)
                    {
                        UnityEditor.EditorApplication.delayCall += () => {
                            DestroyImmediate(this);
                        };
                    }
                }
                else
                {
                    _isFirstComponent = true;
                }
            }
        }
#endif
    }
}
