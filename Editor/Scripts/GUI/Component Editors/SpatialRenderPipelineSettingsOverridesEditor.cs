using System.Reflection;
using UnityEngine;
using UnityEditor;
using SpatialSys.UnitySDK.Internal;
using UnityEngine.Rendering.Universal;
using ShadowCascadeGUI = UnityEditor.Rendering.ShadowCascadeGUI;
using UnityEngine.Rendering;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialRenderPipelineSettingsOverrides))]
    public class SpatialRenderPipelineSettingsOverridesEditor : SpatialComponentEditorBase
    {
        private SerializedProperty _renderPipelineSettings;
        private SpatialRenderPipelineSettingsEditor _renderPipelineSettingsEditor;

        private SerializedProperty _overrideRenderPipelineSettings;

        void OnEnable()
        {
            _overrideRenderPipelineSettings = serializedObject.FindProperty("overrideSettings");

            _renderPipelineSettings = serializedObject.FindProperty("renderPipelineSettings");
            _renderPipelineSettingsEditor = new SpatialRenderPipelineSettingsEditor();
            _renderPipelineSettingsEditor.FindProperties(_renderPipelineSettings);
        }

        public override void DrawFields()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_overrideRenderPipelineSettings, new GUIContent("Override Render Pipeline Settings"));
            if (!_overrideRenderPipelineSettings.boolValue)
            {
                ApplyChangesCurrentRenderPipelineAsset(new RenderPipelineSettings());
                EditorGUILayout.HelpBox("Default settings will be used.", MessageType.Info);
                return;
            }

            _renderPipelineSettingsEditor.DrawFields();

            SpatialRenderPipelineSettingsOverrides targetComponent = target as SpatialRenderPipelineSettingsOverrides;

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                ApplyChangesCurrentRenderPipelineAsset(targetComponent.renderPipelineSettings);
            }

            EditorGUILayout.Space(20);
            string changedProperties = GetChangedProperties(targetComponent.renderPipelineSettings, new RenderPipelineSettings());
            EditorGUILayout.HelpBox(changedProperties, MessageType.Info);
        }

        private void ApplyChangesCurrentRenderPipelineAsset(RenderPipelineSettings renderPipelineSettings)
        {
            UniversalRenderPipelineAsset pipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (pipelineAsset != null)
            {
                // Unity doesn't expose some settings in the UniversalRenderPipelineAsset, so we need to use reflection to access them.
                // https://gist.github.com/JimmyCushnie/e998cdec15394d6b68a4dbbf700f66ce
                var pipelineAssetType = typeof(UniversalRenderPipelineAsset);
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                FieldInfo opaqueDownsamplingFieldInfo = pipelineAssetType.GetField("m_OpaqueDownsampling", flags);
                FieldInfo mainLightShadowmapResolutionFieldInfo = pipelineAssetType.GetField("m_MainLightShadowmapResolution", flags);

                pipelineAsset.supportsCameraDepthTexture = renderPipelineSettings.supportsCameraDepthTexture;
                pipelineAsset.supportsCameraOpaqueTexture = renderPipelineSettings.supportsCameraOpaqueTexture;
                // pipelineAsset.opaqueDownsampling = (Downsampling)renderPipelineSettings.opaqueDownsampling;
                opaqueDownsamplingFieldInfo.SetValue(pipelineAsset, (Downsampling)renderPipelineSettings.opaqueDownsampling);

                pipelineAsset.supportsHDR = renderPipelineSettings.supportsHDR;
                pipelineAsset.msaaSampleCount = (int)renderPipelineSettings.msaaSampleCount;

                // pipelineAsset.mainLightShadowmapResolution = renderPipelineSettings.mainLightShadowmapResolution;
                mainLightShadowmapResolutionFieldInfo.SetValue(pipelineAsset, renderPipelineSettings.mainLightShadowmapResolution);
                pipelineAsset.maxAdditionalLightsCount = renderPipelineSettings.maxAdditionalLightsCount;

                pipelineAsset.shadowDistance = renderPipelineSettings.shadowDistance;
                pipelineAsset.cascadeBorder = renderPipelineSettings.cascadeBorder;
                pipelineAsset.shadowDepthBias = renderPipelineSettings.shadowDepthBias;
                pipelineAsset.shadowNormalBias = renderPipelineSettings.shadowNormalBias;

                pipelineAsset.colorGradingMode = (ColorGradingMode)renderPipelineSettings.colorGradingMode;
                pipelineAsset.colorGradingLutSize = renderPipelineSettings.colorGradingLutSize;
            }
        }

        private string GetChangedProperties(RenderPipelineSettings currentSettings, RenderPipelineSettings defaultSettings)
        {
            string changedList = "(Changed Properties)";

            if (currentSettings.supportsCameraDepthTexture != defaultSettings.supportsCameraDepthTexture)
                changedList += $"\nSupports Camera Depth Texture: {currentSettings.supportsCameraDepthTexture}";
            if (currentSettings.supportsCameraOpaqueTexture != defaultSettings.supportsCameraOpaqueTexture)
                changedList += $"\nSupports Camera Opaque Texture: {currentSettings.supportsCameraOpaqueTexture}";
            if (currentSettings.opaqueDownsampling != defaultSettings.opaqueDownsampling)
                changedList += $"\nOpaque Downsampling: {currentSettings.opaqueDownsampling}";

            if (currentSettings.supportsHDR != defaultSettings.supportsHDR)
                changedList += $"\nSupports HDR: {currentSettings.supportsHDR}";
            if (currentSettings.msaaSampleCount != defaultSettings.msaaSampleCount)
                changedList += $"\nMSAA Sample Count: {currentSettings.msaaSampleCount}";

            if (currentSettings.mainLightShadowmapResolution != defaultSettings.mainLightShadowmapResolution)
                changedList += $"\nMain Light Shadowmap Resolution: {currentSettings.mainLightShadowmapResolution}";
            if (currentSettings.maxAdditionalLightsCount != defaultSettings.maxAdditionalLightsCount)
                changedList += $"\nMax Additional Lights Count: {currentSettings.maxAdditionalLightsCount}";

            if (currentSettings.shadowDistance != defaultSettings.shadowDistance)
                changedList += $"\nShadow Distance: {currentSettings.shadowDistance}";
            if (currentSettings.cascadeBorder != defaultSettings.cascadeBorder)
                changedList += $"\nCascade Border: {currentSettings.cascadeBorder}";
            if (currentSettings.shadowDepthBias != defaultSettings.shadowDepthBias)
                changedList += $"\nShadow Depth Bias: {currentSettings.shadowDepthBias}";
            if (currentSettings.shadowNormalBias != defaultSettings.shadowNormalBias)
                changedList += $"\nShadow Normal Bias: {currentSettings.shadowNormalBias}";

            if (currentSettings.colorGradingMode != defaultSettings.colorGradingMode)
                changedList += $"\nColor Grading Mode: {currentSettings.colorGradingMode}";
            if (currentSettings.colorGradingLutSize != defaultSettings.colorGradingLutSize)
                changedList += $"\nColor Grading LUT Size: {currentSettings.colorGradingLutSize}";

            return changedList;
        }
    }
}