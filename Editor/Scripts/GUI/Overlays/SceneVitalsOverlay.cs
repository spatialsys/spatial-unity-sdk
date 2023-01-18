using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;

namespace SpatialSys.UnitySDK.Editor
{
    [Overlay(typeof(SceneView), "Scene Vitals", true)]
    [Icon("Packages/io.spatial.unitysdk/Editor/Textures/Icons/icon_sceneVitals.png")]
    public class SceneVitalsOverlay : Overlay, ITransientOverlay
    {
        //This is how you force an overlay to always be visible.
        //Users can still "hide" the overlay by docking it somewhere.
        //Without this the overlay would be disabled by default, and users would have
        //no way to know that it exists.
        bool ITransientOverlay.visible => true;

        private VisualElement _verticesBlock;
        private Label _verticesCount;
        private Label _verticesMax;

        //For some reason image tint does not get passed to child elements like text color.
        //So we need to assign the color class to each of these icons individually.
        private VisualElement _meshIcon;
        private VisualElement _textureIcon;
        private VisualElement _materialIcon;

        private VisualElement _sharedTexturesBlock;
        private VisualElement _sharedTexturesSubBlock;
        private Label _sharedTexturesCount;
        private Label _sharedTexturesMax;

        private Label _materialTexturesCount;
        private Label _lightmapTexturesCount;
        private VisualElement _reflectionProbeBlock;
        private Label _reflectionProbeCount;

        private VisualElement _materialsBlock;
        private Label _materialsCount;
        private Label _materialsMax;

        private VisualElement _noLightmapsWarning;
        private VisualElement _noLightprobesWarning;
        private VisualElement _highCollisionMeshWarning;

        private const string BASE_BLOCK_CLASS = "InfoBlock";
        private const string BASE_SUB_BLOCK_CLASS = "SubBlock";
        private const string GREEN_BLOCK_CLASS = "InfoBlock_green";
        private const string YELLOW_BLOCK_CLASS = "InfoBlock_yellow";
        private const string RED_BLOCK_CLASS = "InfoBlock_red";

        private double lastRefreshTime = -1f;
        private float autoRefreshEvery = 30f;

        public override VisualElement CreatePanelContent()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/io.spatial.unitysdk/Editor/Scripts/GUI/Overlays/SceneVitals/SceneVitals.uxml");
            VisualElement element = visualTree.Instantiate();
            var root = new VisualElement() { name = "My Toolbar Root" };
            root.Add(element);
            InitializeElements(root);
            UpdatePerformanceStats();

            // We refresh any time a scene is opened/saved, and periodically.
            // I didnt want to do onDirty because I thought it would be too spammy.
            EditorApplication.update += AutoRefreshTimer;
            EditorSceneManager.sceneOpened += (scene, mode) => UpdatePerformanceStats();
            EditorSceneManager.sceneSaved += (scene) => UpdatePerformanceStats();
            return root;
        }

        private void AutoRefreshTimer()
        {
            if (EditorApplication.timeSinceStartup - lastRefreshTime > autoRefreshEvery)
            {
                UpdatePerformanceStats();
            }
        }

        private void InitializeElements(VisualElement root)
        {
            _meshIcon = root.Q("MeshIcon");
            _textureIcon = root.Q("TextureIcon");
            _materialIcon = root.Q("MaterialIcon");

            _verticesBlock = root.Q("Vertices");
            _verticesCount = root.Query<Label>("VerticesCount").First();
            _verticesMax = root.Query<Label>("VerticesMax").First();

            _sharedTexturesBlock = root.Q("SharedTextures");
            _sharedTexturesSubBlock = root.Q("SharedTexturesSubBlock");
            _sharedTexturesCount = root.Query<Label>("SharedTexturesCount").First();
            _sharedTexturesMax = root.Query<Label>("SharedTexturesMax").First();
            _materialTexturesCount = root.Query<Label>("MaterialTexturesCount").First();
            _lightmapTexturesCount = root.Query<Label>("LightmapTexturesCount").First();
            _reflectionProbeBlock = root.Q("ReflectionProbes");
            _reflectionProbeCount = root.Query<Label>("ReflectionProbesCount").First();

            _materialsBlock = root.Q("Materials");
            _materialsCount = root.Query<Label>("MaterialsCount").First();
            _materialsMax = root.Query<Label>("MaterialsMax").First();

            _noLightmapsWarning = root.Q("NoLightmapsWarning");
            _noLightprobesWarning = root.Q("NoLightprobesWarning");
            _highCollisionMeshWarning = root.Q("HighCollisionMeshWarning");
        }

        private void UpdatePerformanceStats()
        {
            lastRefreshTime = EditorApplication.timeSinceStartup;
            PerformanceResponse resp = SpatialPerformance.GetActiveScenePerformanceResponse();

            // Change the refresh frequency based on how long the request takes.
            // For larger scenes or slow computers where it takes a while I don't want to introduce
            // micro stutters every 10s.
            autoRefreshEvery = Mathf.Clamp(resp.responseMiliseconds * 5f, 5f, 100f);

            SetBaseClass(_verticesBlock);
            SetBlockClassFromRatio(_verticesBlock, resp.vertPercent);
            _verticesCount.text = AbbreviateNumber(resp.verts);
            _verticesMax.text = "/ " + AbbreviateNumber(PerformanceResponse.MAX_SUGGESTED_VERTS);
            _meshIcon.ClearClassList();
            SetBlockClassFromRatio(_meshIcon, resp.vertPercent);

            SetBaseClass(_sharedTexturesBlock);
            SetBaseClass(_sharedTexturesSubBlock, true);
            SetBlockClassFromRatio(_sharedTexturesBlock, resp.sharedTexturePercent);
            SetBlockClassFromRatio(_sharedTexturesSubBlock, resp.sharedTexturePercent);
            _sharedTexturesCount.text = AbbreviateNumber(resp.sharedTextureMB) + "mb";
            _sharedTexturesMax.text = "/ " + AbbreviateNumber(PerformanceResponse.MAX_SUGGESTED_SHARED_TEXTURE_MB) + "mb";
            _materialTexturesCount.text = AbbreviateNumber(resp.materialTextureMB) + "mb";
            _lightmapTexturesCount.text = AbbreviateNumber(resp.lightmapTextureMB) + "mb";
            _reflectionProbeBlock.style.display = resp.reflectionProbeMB > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            _reflectionProbeCount.text = AbbreviateNumber(resp.reflectionProbeMB) + "mb";
            _textureIcon.ClearClassList();
            SetBlockClassFromRatio(_textureIcon, resp.sharedTexturePercent);

            SetBaseClass(_materialsBlock);
            SetBlockClassFromRatio(_materialsBlock, resp.uniqueMaterialsPercent);
            _materialsCount.text = AbbreviateNumber(resp.uniqueMaterials);
            _materialsMax.text = "/ " + AbbreviateNumber(PerformanceResponse.MAX_SUGGESTED_UNIQUE_MATERIALS);
            _materialIcon.ClearClassList();
            SetBlockClassFromRatio(_materialIcon, resp.uniqueMaterialsPercent);

            _noLightmapsWarning.style.display = resp.hasLightmaps ? DisplayStyle.None : DisplayStyle.Flex;
            //show if we have lightmaps but no light probes
            _noLightprobesWarning.style.display = resp.hasLightprobes || !resp.hasLightmaps ? DisplayStyle.None : DisplayStyle.Flex;
            _highCollisionMeshWarning.style.display = resp.meshColliderVertPercent < 1f ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void SetBaseClass(VisualElement element, bool isSubBlock = false)
        {
            element.ClearClassList();
            element.AddToClassList(isSubBlock ? BASE_SUB_BLOCK_CLASS : BASE_BLOCK_CLASS);
        }

        private void SetBlockClassFromRatio(VisualElement element, float ratio)
        {
            if (ratio > 1f)
            {
                element.AddToClassList(RED_BLOCK_CLASS);
            }
            else if (ratio > .6f)
            {
                element.AddToClassList(YELLOW_BLOCK_CLASS);
            }
            else
            {
                element.AddToClassList(GREEN_BLOCK_CLASS);
            }
        }

        private string AbbreviateNumber(int number)
        {
            if (number < 1000)
            {
                return number.ToString();
            }
            else if (number < 10000)
            {
                return (number / 1000f).ToString("0.#") + "K";
            }
            else if (number < 1000000)
            {
                return (number / 1000).ToString() + "K";
            }
            else if (number < 10000000)
            {
                return (number / 1000000f).ToString("0.#") + "M";
            }
            else
            {
                return (number / 1000000).ToString() + "M";
            }
        }
    }
}
