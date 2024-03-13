using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UniversalRenderPipelineAsset = UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorGraphicsService : IGraphicsService
    {
        UniversalRenderPipelineAsset pipelineAsset => GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        FieldInfo opaqueDownsamplingFieldInfo = typeof(UniversalRenderPipelineAsset).GetField("m_OpaqueDownsampling", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo mainLightShadowmapResolutionFieldInfo = typeof(UniversalRenderPipelineAsset).GetField("m_MainLightShadowmapResolution", BindingFlags.Instance | BindingFlags.NonPublic);

        // Rendering
        public bool supportsCameraDepthTexture
        {
            get => pipelineAsset.supportsCameraDepthTexture;
            set => pipelineAsset.supportsCameraDepthTexture = value;
        }
        public bool supportsCameraOpaqueTexture
        {
            get => pipelineAsset.supportsCameraOpaqueTexture;
            set => pipelineAsset.supportsCameraOpaqueTexture = value;
        }
        public Downsampling opaqueDownsampling
        {
            get => pipelineAsset.opaqueDownsampling;
            set => opaqueDownsamplingFieldInfo.SetValue(pipelineAsset, value);
        }

        // Quality
        public bool supportsHDR
        {
            get => pipelineAsset.supportsHDR;
            set => pipelineAsset.supportsHDR = value;
        }
        public MsaaQuality msaaSampleCount
        {
            get => (MsaaQuality)pipelineAsset.msaaSampleCount;
            set => pipelineAsset.msaaSampleCount = (int)value;
        }
        public ShadowResolution mainLightShadowmapResolution
        {
            get => (ShadowResolution)pipelineAsset.mainLightShadowmapResolution;
            set => mainLightShadowmapResolutionFieldInfo.SetValue(pipelineAsset, value);
        }
        public int maxAdditionalLightsCount
        {
            get => pipelineAsset.maxAdditionalLightsCount;
            set => pipelineAsset.maxAdditionalLightsCount = value;
        }

        // Shadows
        public float shadowDistance
        {
            get => pipelineAsset.shadowDistance;
            set => pipelineAsset.shadowDistance = value;
        }
        public float cascadeBorder
        {
            get => pipelineAsset.cascadeBorder;
            set => pipelineAsset.cascadeBorder = value;
        }
        public float shadowDepthBias
        {
            get => pipelineAsset.shadowDepthBias;
            set => pipelineAsset.shadowDepthBias = value;
        }
        public float shadowNormalBias
        {
            get => pipelineAsset.shadowNormalBias;
            set => pipelineAsset.shadowNormalBias = value;
        }

        // Post-processing
        public ColorGradingMode colorGradingMode
        {
            get => pipelineAsset.colorGradingMode;
            set => pipelineAsset.colorGradingMode = value;
        }
        public int colorGradingLutSize
        {
            get => pipelineAsset.colorGradingLutSize;
            set => pipelineAsset.colorGradingLutSize = value;
        }
    }
}