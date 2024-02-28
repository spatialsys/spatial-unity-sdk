using UnityEngine;
using UnityEditor;
using SpatialSys.UnitySDK.Internal;
using UnityEngine.Rendering.Universal;
using ShadowCascadeGUI = UnityEditor.Rendering.ShadowCascadeGUI;
using UnityEngine.Rendering;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialRenderPipelineSettingsEditor
    {
        private bool _showRendering = true;
        private bool _showQuality = true;
        private bool _showLighting = true;
        private bool _showShadows = true;
        private bool _showPostProcessing = true;

        // Rendering
        private SerializedProperty _supportsCameraDepthTexture;
        private SerializedProperty _supportsCameraOpaqueTexture;
        private SerializedProperty _opaqueDownsampling;

        // Quality
        private SerializedProperty _supportsHDR;
        private SerializedProperty _msaaSampleCount;

        // Lighting
        private SerializedProperty _mainLightRenderingMode;
        private SerializedProperty _mainLightShadowmapResolution;
        private SerializedProperty _additionalLightsRenderingMode;
        private SerializedProperty _maxAdditionalLightsCount;

        // Shadows
        private SerializedProperty _shadowDistance;
        private SerializedProperty _shadowCascadeCount;
        private SerializedProperty _cascade2Split;
        private SerializedProperty _cascade3Split;
        private SerializedProperty _cascade4Split;
        private SerializedProperty _cascadeBorder;
        private SerializedProperty _shadowDepthBias;
        private SerializedProperty _shadowNormalBias;

        // Post-processing
        private SerializedProperty _colorGradingMode;
        private SerializedProperty _colorGradingLutSize;

        public void FindProperties(SerializedProperty serializedProperty)
        {
            _supportsCameraDepthTexture = serializedProperty.FindPropertyRelative("supportsCameraDepthTexture");
            _supportsCameraOpaqueTexture = serializedProperty.FindPropertyRelative("supportsCameraOpaqueTexture");
            _opaqueDownsampling = serializedProperty.FindPropertyRelative("opaqueDownsampling");

            _supportsHDR = serializedProperty.FindPropertyRelative("supportsHDR");
            _msaaSampleCount = serializedProperty.FindPropertyRelative("msaaSampleCount");

            _mainLightRenderingMode = serializedProperty.FindPropertyRelative("mainLightRenderingMode");
            _mainLightShadowmapResolution = serializedProperty.FindPropertyRelative("mainLightShadowmapResolution");
            _additionalLightsRenderingMode = serializedProperty.FindPropertyRelative("additionalLightsRenderingMode");
            _maxAdditionalLightsCount = serializedProperty.FindPropertyRelative("maxAdditionalLightsCount");

            _shadowDistance = serializedProperty.FindPropertyRelative("shadowDistance");
            _shadowCascadeCount = serializedProperty.FindPropertyRelative("shadowCascadeCount");
            _cascade2Split = serializedProperty.FindPropertyRelative("cascade2Split");
            _cascade3Split = serializedProperty.FindPropertyRelative("cascade3Split");
            _cascade4Split = serializedProperty.FindPropertyRelative("cascade4Split");
            _cascadeBorder = serializedProperty.FindPropertyRelative("cascadeBorder");
            _shadowDepthBias = serializedProperty.FindPropertyRelative("shadowDepthBias");
            _shadowNormalBias = serializedProperty.FindPropertyRelative("shadowNormalBias");

            _colorGradingMode = serializedProperty.FindPropertyRelative("colorGradingMode");
            _colorGradingLutSize = serializedProperty.FindPropertyRelative("colorGradingLutSize");
        }

        public void DrawFields()
        {
            EditorGUILayout.Space(5);
            _showRendering = EditorGUILayout.Foldout(_showRendering, "Rendering", EditorStyles.foldoutHeader);
            if (_showRendering)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_supportsCameraDepthTexture, RenderPipelineSettingsStyles.requireDepthTextureText);
                EditorGUILayout.PropertyField(_supportsCameraOpaqueTexture, RenderPipelineSettingsStyles.requireOpaqueTextureText);
                EditorGUI.BeginDisabledGroup(!_supportsCameraOpaqueTexture.boolValue);
                EditorGUILayout.PropertyField(_opaqueDownsampling, RenderPipelineSettingsStyles.opaqueDownsamplingText);
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);
            _showQuality = EditorGUILayout.Foldout(_showQuality, "Quality", EditorStyles.foldoutHeader);
            if (_showQuality)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_supportsHDR, RenderPipelineSettingsStyles.hdrText);
                EditorGUILayout.PropertyField(_msaaSampleCount, RenderPipelineSettingsStyles.msaaText);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);
            _showLighting = EditorGUILayout.Foldout(_showLighting, "Lighting", EditorStyles.foldoutHeader);
            if (_showLighting)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField($"We always use Per-Pixel lighting for main and additional lights from SDK version 1.18.0. (current version: {PackageManagerUtility.currentVersion})", EditorStyles.helpBox);

                EditorGUI.BeginDisabledGroup(true);
                _mainLightRenderingMode.intValue = (int)LightRenderingMode.PerPixel;
                EditorGUILayout.PropertyField(_mainLightRenderingMode, RenderPipelineSettingsStyles.mainLightRenderingModeText);
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_mainLightShadowmapResolution, RenderPipelineSettingsStyles.mainLightShadowmapResolutionText);
                EditorGUI.indentLevel--;

                EditorGUI.BeginDisabledGroup(true);
                _additionalLightsRenderingMode.intValue = (int)LightRenderingMode.PerPixel;
                EditorGUILayout.PropertyField(_additionalLightsRenderingMode, RenderPipelineSettingsStyles.addditionalLightsRenderingModeText);
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_maxAdditionalLightsCount, RenderPipelineSettingsStyles.perObjectLimit);
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);
            _showShadows = EditorGUILayout.Foldout(_showShadows, "Shadows", EditorStyles.foldoutHeader);
            if (_showShadows)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_shadowDistance, RenderPipelineSettingsStyles.shadowDistanceText);
                // Hide CascadeCount. Shadow cascades require a different shader variant.
                // EditorGUILayout.PropertyField(_shadowCascadeCount, RenderPipelineSettingsStyles.shadowCascadesText);

                EditorGUI.indentLevel++;
                int cascadeCount = _shadowCascadeCount.intValue;
                // bool useMetric = unit == EditorUtils.Unit.Metric;
                bool useMetric = true; // TODO: Editor state check. (What if Editor settings are not metric?)
                float baseMetric = _shadowDistance.floatValue;
                int cascadeSplitCount = cascadeCount - 1;
                DrawCascadeSliders(cascadeSplitCount, useMetric, baseMetric);
                DrawCascades(cascadeCount, useMetric, baseMetric);
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(_shadowDepthBias, RenderPipelineSettingsStyles.shadowDepthBias);
                EditorGUILayout.PropertyField(_shadowNormalBias, RenderPipelineSettingsStyles.shadowNormalBias);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);
            _showPostProcessing = EditorGUILayout.Foldout(_showPostProcessing, "Post-processing", EditorStyles.foldoutHeader);
            if (_showPostProcessing)
            {
                EditorGUI.indentLevel++;
                bool isHdrOn = _supportsHDR.boolValue;
                EditorGUILayout.PropertyField(_colorGradingMode, RenderPipelineSettingsStyles.colorGradingMode);
                if (!isHdrOn && _colorGradingMode.intValue == (int)ColorGradingMode.HighDynamicRange)
                    EditorGUILayout.HelpBox(RenderPipelineSettingsStyles.colorGradingModeWarning, MessageType.Warning);
                else if (isHdrOn && _colorGradingMode.intValue == (int)ColorGradingMode.HighDynamicRange)
                    EditorGUILayout.HelpBox(RenderPipelineSettingsStyles.colorGradingModeSpecInfo, MessageType.Info);

                // EditorGUILayout.DelayedIntField(_colorGradingLutSize, Styles.colorGradingLutSize);
                EditorGUILayout.PropertyField(_colorGradingLutSize, RenderPipelineSettingsStyles.colorGradingLutSize);
                _colorGradingLutSize.intValue = Mathf.Clamp(_colorGradingLutSize.intValue, UniversalRenderPipelineAsset.k_MinLutSize, UniversalRenderPipelineAsset.k_MaxLutSize);
                if (isHdrOn && _colorGradingMode.intValue == (int)ColorGradingMode.HighDynamicRange && _colorGradingLutSize.intValue < 32)
                    EditorGUILayout.HelpBox(RenderPipelineSettingsStyles.colorGradingLutSizeWarning, MessageType.Warning);
                EditorGUI.indentLevel--;
            }
        }

        // Copied from UniversalRenderPipelineAssetUI.Drawers.cs
        private void DrawCascadeSliders(int splitCount, bool useMetric, float baseMetric)
        {
            Vector4 shadowCascadeSplit = Vector4.one;
            if (splitCount == 3)
                shadowCascadeSplit = new Vector4(_cascade4Split.vector3Value.x, _cascade4Split.vector3Value.y, _cascade4Split.vector3Value.z, 1);
            else if (splitCount == 2)
                shadowCascadeSplit = new Vector4(_cascade3Split.vector2Value.x, _cascade3Split.vector2Value.y, 1, 0);
            else if (splitCount == 1)
                shadowCascadeSplit = new Vector4(_cascade2Split.floatValue, 1, 0, 0);

            float splitBias = 0.001f;
            float invBaseMetric = baseMetric == 0 ? 0 : 1f / baseMetric;

            // Ensure correct split order
            shadowCascadeSplit[0] = Mathf.Clamp(shadowCascadeSplit[0], 0f, shadowCascadeSplit[1] - splitBias);
            shadowCascadeSplit[1] = Mathf.Clamp(shadowCascadeSplit[1], shadowCascadeSplit[0] + splitBias, shadowCascadeSplit[2] - splitBias);
            shadowCascadeSplit[2] = Mathf.Clamp(shadowCascadeSplit[2], shadowCascadeSplit[1] + splitBias, shadowCascadeSplit[3] - splitBias);


            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < splitCount; ++i)
            {
                float value = shadowCascadeSplit[i];

                float minimum = i == 0 ? 0 : shadowCascadeSplit[i - 1] + splitBias;
                float maximum = i == splitCount - 1 ? 1 : shadowCascadeSplit[i + 1] - splitBias;

                if (useMetric)
                {
                    float valueMetric = value * baseMetric;
                    valueMetric = EditorGUILayout.Slider(EditorGUIUtility.TrTextContent($"Split {i + 1}", "The distance where this cascade ends and the next one starts."), valueMetric, 0f, baseMetric, null);

                    shadowCascadeSplit[i] = Mathf.Clamp(valueMetric * invBaseMetric, minimum, maximum);
                }
                else
                {
                    float valueProcentage = value * 100f;
                    valueProcentage = EditorGUILayout.Slider(EditorGUIUtility.TrTextContent($"Split {i + 1}", "The distance where this cascade ends and the next one starts."), valueProcentage, 0f, 100f, null);

                    shadowCascadeSplit[i] = Mathf.Clamp(valueProcentage * 0.01f, minimum, maximum);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                switch (splitCount)
                {
                    case 3:
                        _cascade4Split.vector3Value = shadowCascadeSplit;
                        break;
                    case 2:
                        _cascade3Split.vector2Value = shadowCascadeSplit;
                        break;
                    case 1:
                        _cascade2Split.floatValue = shadowCascadeSplit.x;
                        break;
                }
            }

            var borderValue = _cascadeBorder.floatValue;

            EditorGUI.BeginChangeCheck();
            if (useMetric)
            {
                var lastCascadeSplitSize = splitCount == 0 ? baseMetric : (1.0f - shadowCascadeSplit[splitCount - 1]) * baseMetric;
                var invLastCascadeSplitSize = lastCascadeSplitSize == 0 ? 0 : 1f / lastCascadeSplitSize;
                float valueMetric = borderValue * lastCascadeSplitSize;
                valueMetric = EditorGUILayout.Slider(EditorGUIUtility.TrTextContent("Last Border", "The distance of the last cascade."), valueMetric, 0f, lastCascadeSplitSize, null);

                borderValue = valueMetric * invLastCascadeSplitSize;
            }
            else
            {
                float valueProcentage = borderValue * 100f;
                valueProcentage = EditorGUILayout.Slider(EditorGUIUtility.TrTextContent("Last Border", "The distance of the last cascade."), valueProcentage, 0f, 100f, null);

                borderValue = valueProcentage * 0.01f;
            }

            if (EditorGUI.EndChangeCheck())
            {
                _cascadeBorder.floatValue = borderValue;
            }
        }

        // Copied from UniversalRenderPipelineAssetUI.Drawers.cs
        private void DrawCascades(int cascadeCount, bool useMetric, float baseMetric)
        {
            var cascades = new ShadowCascadeGUI.Cascade[cascadeCount];

            Vector3 shadowCascadeSplit = Vector3.zero;
            if (cascadeCount == 4)
                shadowCascadeSplit = _cascade4Split.vector3Value;
            else if (cascadeCount == 3)
                shadowCascadeSplit = _cascade3Split.vector2Value;
            else if (cascadeCount == 2)
                shadowCascadeSplit.x = _cascade2Split.floatValue;
            else
                shadowCascadeSplit.x = _cascade2Split.floatValue;

            float lastCascadePartitionSplit = 0;
            for (int i = 0; i < cascadeCount - 1; ++i)
            {
                cascades[i] = new ShadowCascadeGUI.Cascade() {
                    size = i == 0 ? shadowCascadeSplit[i] : shadowCascadeSplit[i] - lastCascadePartitionSplit, // Calculate the size of cascade
                    borderSize = 0,
                    cascadeHandleState = ShadowCascadeGUI.HandleState.Enabled,
                    borderHandleState = ShadowCascadeGUI.HandleState.Hidden,
                };
                lastCascadePartitionSplit = shadowCascadeSplit[i];
            }

            // Last cascade is special
            var lastCascade = cascadeCount - 1;
            cascades[lastCascade] = new ShadowCascadeGUI.Cascade() {
                size = lastCascade == 0 ? 1.0f : 1 - shadowCascadeSplit[lastCascade - 1], // Calculate the size of cascade
                borderSize = _cascadeBorder.floatValue,
                cascadeHandleState = ShadowCascadeGUI.HandleState.Hidden,
                borderHandleState = ShadowCascadeGUI.HandleState.Enabled,
            };

            EditorGUI.BeginChangeCheck();
            ShadowCascadeGUI.DrawCascades(ref cascades, useMetric, baseMetric);
            if (EditorGUI.EndChangeCheck())
            {
                if (cascadeCount == 4)
                    _cascade4Split.vector3Value = new Vector3(
                        cascades[0].size,
                        cascades[0].size + cascades[1].size,
                        cascades[0].size + cascades[1].size + cascades[2].size
                    );
                else if (cascadeCount == 3)
                    _cascade3Split.vector2Value = new Vector2(
                        cascades[0].size,
                        cascades[0].size + cascades[1].size
                    );
                else if (cascadeCount == 2)
                    _cascade2Split.floatValue = cascades[0].size;

                _cascadeBorder.floatValue = cascades[lastCascade].borderSize;
            }
        }
    }
}