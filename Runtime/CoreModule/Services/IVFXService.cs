using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Handles Visual Effects on the spatial platform
    /// </summary>
    /// <example><code source="Services/VFXServiceExamples.cs" region="CreateFloatingText" lang="csharp"/></example>
    [DocumentationCategory("VFX Service")]
    public interface IVFXService
    {
        /// <summary>
        /// Creates a floating text object
        /// </summary>
        /// <param name="text">Text to render</param>
        /// <param name="style">Animation style</param>
        /// <param name="position">Position to place floating text</param>
        /// <param name="force">Impulse force of the object once it appears</param>
        /// <param name="color">Text color</param>
        /// <param name="gravity">Use gravity?</param>
        /// <param name="scaleCurve">Animation curve for scaling</param>
        /// <param name="alphaCurve">Animation curve for alpha</param>
        /// <param name="lifetime">Time to keep text visible</param>
        /// <example><code source="Services/VFXServiceExamples.cs" region="CreateFloatingText" lang="csharp"/></example>
        void CreateFloatingText(string text, FloatingTextAnimStyle style, Vector3 position, Vector3 force, Color color, bool gravity = false, AnimationCurve scaleCurve = null, AnimationCurve alphaCurve = null, float lifetime = 1);
    }
}