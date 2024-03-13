using UnityEngine;
using UnityEngine.Rendering.Universal;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// This service provides access to all render pipeline asset related functionality: Lighting, shadows, quality, etc.
    /// </summary>
    [DocumentationCategory("Graphics Service")]
    public interface IGraphicsService
    {
        // Rendering
        /// <summary>
        /// If enabled the pipeline will generate camera's depth that can be bound in shaders as _CameraDepthTexture.
        /// </summary>
        bool supportsCameraDepthTexture { get; set; }

        /// <summary>
        /// If enabled the pipeline will copy the screen to texture after opaque objects are drawn. For transparent objects this can be bound in shaders as _CameraOpaqueTexture.
        /// </summary>
        bool supportsCameraOpaqueTexture { get; set; }

        /// <summary>
        /// The downsampling method that is used for the opaque texture.
        /// Downsampling { None, _2xBilinear, _4xBox, _4xBilinear }
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
        /// MsaaQuality { _1x, _2x, _4x, _8x }
        /// </summary>
        MsaaQuality msaaSampleCount { get; set; }


        // Lighting
        /// <summary>
        /// Resolution of the main light shadowmap texture. If cascades are enabled, cascades will be packed into an atlas and this setting controls the maximum shadows atlas resolution.
        /// ShadowResolution { _256, _512, _1024, _2048, _4096 }
        /// </summary>
        ShadowResolution mainLightShadowmapResolution { get; set; }

        /// <summary>
        /// Additional lights support.(Range 1~8)
        /// Spatial default: 4
        /// </summary>
        int maxAdditionalLightsCount { get; set; }


        // Shadows
        /// <summary>
        /// Maximum shadow rendering distance in meters. Longer distance will cause lower shadow quality.
        /// Spatial default: 30m
        /// </summary>
        float shadowDistance { get; set; }

        /// <summary>
        /// The distance of the last cascade in 0 to 1 range.
        /// Spatial default: 0.2
        /// </summary>
        float cascadeBorder { get; set; }

        /// <summary>
        /// Controls the distance at which the shadows will be pushed away from the light. Useful for avoiding false self-shadowing artifacts.
        /// Default: 1.0
        /// </summary>
        float shadowDepthBias { get; set; }

        /// <summary>
        /// Controls distance at which the shadow casting surfaces will be shrunk along the surface normal. Useful for avoiding false self-shadowing artifacts.
        /// Default: 1.0
        /// </summary>
        float shadowNormalBias { get; set; }


        // Post-processing
        /// <summary>
        /// Defines how color grading will be applied. Operators will react differently depending on the mode.
        /// ColorGradingMode { LowDynamicRange, HighDynamicRange }
        /// </summary>
        ColorGradingMode colorGradingMode { get; set; }

        /// <summary>
        /// Sets the size of the internal and external color grading lookup textures (LUTs).
        /// Do not exceed 64 for performance reasons.
        /// Default: 32
        /// </summary>
        int colorGradingLutSize { get; set; }
    }
}
