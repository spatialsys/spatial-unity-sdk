using SpatialSys.UnitySDK.Internal;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [InternalType]
    public abstract class SpatialComponentBase : MonoBehaviour
    {
        // these values show up in the black inspector block
        public abstract string prettyName { get; }
        public abstract string tooltip { get; }
        public abstract string documentationURL { get; }

        // used to mark components that will likely have breaking changes in the future
        public virtual bool isExperimental => false;

        protected virtual bool _limitOnePerScene => false;

#if UNITY_EDITOR
        private bool _isFirstComponent = false;
        protected virtual void OnValidate()
        {
            if (_limitOnePerScene)
            {
                // Destroy this component if it is not the first component in the scene.
                System.Type type = GetType();
                Object[] foundComponents = FindObjectsOfType(type);
                if (foundComponents.Length > 1)
                {
                    if (!_isFirstComponent)
                    {
                        UnityEditor.EditorApplication.delayCall += () => {
                            DestroyImmediate(this);
                        };
                    }
                    UnityEditor.EditorUtility.DisplayDialog("Multiple instances not allowed", $"There should only be one <{type.Name}> in the scene.", "OK");
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
