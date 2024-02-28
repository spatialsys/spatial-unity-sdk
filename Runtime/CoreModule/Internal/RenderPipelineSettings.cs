using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

namespace SpatialSys.UnitySDK.Internal
{
    /// <summary>
    /// Render pipeline settings based on UniversalRenderPipelineAsset.
    /// https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@12.1/manual/universalrp-asset.html
    /// We expose only the settings that don't require shader variant.
    /// </summary>
    /// TODO: Support shader variant compilation from build machine and expose more settings.
    [System.Serializable]
    public class RenderPipelineSettings
    {
        // Rendering
        [HideInInspector] public bool supportsCameraDepthTexture = true;
        public bool supportsCameraOpaqueTexture = true;
        public Downsampling opaqueDownsampling = Downsampling._2xBilinear;
        // public bool supportsTerrainHoles = false; // Not recommend to use Terrain
        // public bool useSRPBatcher = true;
        // public bool supportsDynamicBatching = false;
        // public PipelineDebugLevel debugLevel = PipelineDebugLevel.Disabled;
        // public ShaderVariantLogLevel shaderVariantLogLevel = ShaderVariantLogLevel.Disabled;


        // Quality
        public bool supportsHDR = true;
        public MsaaQuality msaaSampleCount = MsaaQuality._2x;
        // [Range(0f, 2f)] public float renderScale = 1.0f; // Spatial controls this in settings
        // public UpscalingFilterSelection upscalingFilter = UpscalingFilterSelection.Auto;
        // public bool fsrOverrideSharpness;
        // public float fsrSharpness;

        // Lighting
        public LightRenderingMode mainLightRenderingMode = LightRenderingMode.PerPixel;
        // public bool supportsMainLightShadows = true;
        public ShadowResolution mainLightShadowmapResolution = ShadowResolution._2048;

        // This requires shader variant compilation from build machine. (_ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS)
        // We just use Per-Pixel from SDK version 1.18.0 (Per-Vertex before), and don't expose this setting to the user.
        [HideInInspector] public LightRenderingMode additionalLightsRenderingMode = LightRenderingMode.PerPixel;

        // Max count is defined in URP: UniversalRenderPipeline.maxPerObjectLights but sealed.
        // Set min not 0 because non-additional lights variants will be stripped, and make sure Unity won't use the variant.
        [Range(1, 8)] public int maxAdditionalLightsCount = 4;
        // public bool supportsAdditionalLightShadows = false; // Requires shader variant (_ADDITIONAL_LIGHT_SHADOWS)
        // public ShadowResolution additionalLightsShadowmapResolution = ShadowResolution._512;

        // public int additionalLightsShadowResolutionTierLow = 256;
        // public int additionalLightsShadowResolutionTierMedium = 512;
        // public int additionalLightsShadowResolutionTierHigh = 1024;

        // public LightCookieResolution additionalLightsCookieResolution = LightCookieResolution._2048;
        // public LightCookieFormat additionalLightsCookieFormat = LightCookieFormat.ColorHigh;

        // public bool reflectionProbeBlending = true; // Requires shader variant (_REFLECTION_PROBE_BLENDING)
        // public bool reflectionProbeBoxProjection = true; // Requires shader variant (_REFLECTION_PROBE_BOX_PROJECTION)

        // public bool supportsMixedLighting = true;
        // Requires variant (_LIGHT_LAYERS)
        // We just set true from SDK version 1.18.0 (false before), and don't expose this setting to the user.
        [HideInInspector] public bool supportsLightLayers => true;


        // Shadows
        [MinAttribute(0f)] public float shadowDistance = 30.0f; // Spatial default is 30m
        [Range(1, 4)] public int shadowCascadeCount = 1;
        public float cascade2Split = 0.25f;
        public Vector2 cascade3Split = new Vector2(0.1f, 0.3f);
        public Vector3 cascade4Split = new Vector3(0.067f, 0.2f, 0.467f);
        public float cascadeBorder = 0.2f;
        public float shadowDepthBias = 1.0f;
        public float shadowNormalBias = 1.0f;
        // public bool supportsSoftShadows = true;
        // public bool conservativeEnclosingSphere = true;


        // Post-processing
        public ColorGradingMode colorGradingMode = ColorGradingMode.LowDynamicRange;
        [Range(16, 64)] public int colorGradingLutSize = 32;
        // public bool useFastSRGBLinearConversion = false;
        // public VolumeFrameworkUpdateMode volumeFrameworkUpdateMode = VolumeFrameworkUpdateMode.EveryFrame;
    }
}
