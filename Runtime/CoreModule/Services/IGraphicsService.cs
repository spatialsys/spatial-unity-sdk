using UnityEngine;
using UnityEngine.Rendering.Universal;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// This service provides access to all render pipeline asset related functionality: Lighting, shadows, quality, etc.
    /// </summary>
    [DocumentationCategory("Services/Graphics Service")]
    public interface IGraphicsService
    {
        // Rendering
        /// <summary>
        /// If enabled the pipeline will generate camera's depth that can be bound in shaders as <c>_CameraDepthTexture</c>.
        /// </summary>
        bool supportsCameraDepthTexture { get; set; }

        /// <summary>
        /// If enabled the pipeline will copy the screen to texture after opaque objects are drawn. For transparent objects this can be bound in shaders as <c>_CameraOpaqueTexture</c>.
        /// </summary>
        bool supportsCameraOpaqueTexture { get; set; }

        /// <summary>
        /// The downsampling method that is used for the opaque texture.
        /// </summary>
        Downsampling opaqueDownsampling { get; set; }


        // Quality
        /// <summary>
        /// Controls the global HDR settings.
        /// </summary>
        bool supportsHDR { get; set; }

        /// <summary>
        /// Controls the global anti aliasing settings.
        /// We recommend not to use more than 4x MSAA for performance reasons.
        /// </summary>
        MsaaQuality msaaSampleCount { get; set; }


        // Lighting
        /// <summary>
        /// Resolution of the main light shadowmap texture. If cascades are enabled, cascades will be packed into an atlas and this setting controls the maximum shadows atlas resolution.
        /// </summary>
        ShadowResolution mainLightShadowmapResolution { get; set; }

        /// <summary>
        /// Additional lights support.
        ///
        /// Range: <c>1-8</c>
        ///
        /// Spatial default: <c>4</c>
        /// </summary>
        int maxAdditionalLightsCount { get; set; }


        // Shadows
        /// <summary>
        /// Maximum shadow rendering distance in meters. Longer distance will cause lower shadow quality.
        ///
        /// Spatial default: <c>30m</c>
        /// </summary>
        float shadowDistance { get; set; }

        /// <summary>
        /// The distance of the last cascade in.
        ///
        /// Range: <c>0-1</c>.
        ///
        /// Spatial default: <c>0.2</c>
        /// </summary>
        float cascadeBorder { get; set; }

        /// <summary>
        /// Controls the distance at which the shadows will be pushed away from the light. Useful for avoiding false self-shadowing artifacts.
        ///
        /// Default: <c>1.0</c>
        /// </summary>
        float shadowDepthBias { get; set; }

        /// <summary>
        /// Controls distance at which the shadow casting surfaces will be shrunk along the surface normal. Useful for avoiding false self-shadowing artifacts.
        ///
        /// Default: <c>1.0</c>
        /// </summary>
        float shadowNormalBias { get; set; }


        // Post-processing
        /// <summary>
        /// Defines how color grading will be applied. Operators will react differently depending on the mode.
        /// </summary>
        ColorGradingMode colorGradingMode { get; set; }

        /// <summary>
        /// Sets the size of the internal and external color grading lookup textures (LUTs).
        /// Do not exceed 64 for performance reasons.
        ///
        /// Default: <c>32</c>
        /// </summary>
        int colorGradingLutSize { get; set; }
    }
}
