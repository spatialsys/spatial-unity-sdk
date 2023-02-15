using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Linq;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.Editor
{
    public class ValidComponents
    {
        public static HashSet<Type> allowedComponentTypes = new HashSet<Type>() {
            // Unity
            typeof(Transform),

            // Visual Scripting
            typeof(Variables),
            typeof(ScriptMachine),
            typeof(SceneVariables),

            // Audio
            typeof(AudioChorusFilter),
            typeof(AudioDistortionFilter),
            typeof(AudioEchoFilter),
            typeof(AudioHighPassFilter),
            typeof(AudioLowPassFilter),
            typeof(AudioReverbFilter),
            typeof(AudioReverbZone),
            typeof(AudioSource),

            // Video
            typeof(VideoPlayer),

            // UI
            typeof(AspectRatioFitter),
            typeof(BaseMeshEffect), // Includes effects like shadow, outline, etc.
            typeof(Canvas),
            typeof(CanvasGroup),
            typeof(CanvasRenderer),
            typeof(CanvasScaler),
            typeof(ContentSizeFitter),
            typeof(Graphic), // Mostly visual UI components are derived from this (image, text, etc)
            typeof(GraphicRaycaster),
            typeof(LayoutElement),
            typeof(LayoutGroup),
            typeof(Mask),
            typeof(RectMask2D),
            typeof(ScrollRect),
            typeof(Selectable), // Most interactive UI components are derived from this (button, slider, dropdown, etc)
            typeof(ToggleGroup),

            // Physics
            typeof(Collider),
            typeof(Rigidbody),
            typeof(Cloth),
            typeof(Joint),
            typeof(ConstantForce),

            // Rendering
            typeof(Camera),
            typeof(Light),
            typeof(LightProbeGroup),
            typeof(LightProbeProxyVolume),
            typeof(ParticleSystem),
            typeof(Projector),
            typeof(ReflectionProbe),
            typeof(Renderer),
            typeof(UniversalAdditionalCameraData),
            typeof(UniversalAdditionalLightData),
            typeof(Volume),
            // Rendering.Mesh
            typeof(LODGroup),
            typeof(MeshFilter),
            typeof(Terrain),
            typeof(Tree),

            // Particles
            typeof(ParticleSystemForceField),

            // Animation
            typeof(Animator),

            // Occulusion
            typeof(OcclusionArea),
            typeof(OcclusionPortal),

            // AI
            typeof(NavMeshAgent),
            typeof(NavMeshObstacle),
            typeof(OffMeshLink),

            // Spatial
            typeof(SpatialComponentBase),
        };

        [ComponentTest(typeof(Component))]
        public static void TestValidComponent(Component target)
        {
            // Components can be "null" if the script is missing; These are automatically removed during scene build
            if (target == null)
                return;

            Type targetType = target.GetType();

            // Ignore this component if it has a [EditorOnly] attribute. These are automatically removed during scene build
            if (targetType.GetCustomAttributes(typeof(EditorOnlyAttribute), true).Length > 0)
                return;

            if (!allowedComponentTypes.Any(t => t.IsAssignableFrom(targetType)))
            {
                // Maybe do some type specific messages. For example reasure people that we have an event system active etc.
                SpatialTestResponse resp = new SpatialTestResponse(
                    target, TestResponseType.Fail, "Object has unsupported component type: " + targetType.ToString(),
                    "Certain components are not allowed in scenes. To fix this error remove the offending component from the object."
                );

                // TODO: not including this fix right now because it can fail if a component has a requirement (inputModule requires EventSystem etc.)
                /*
                resp.SetAutoFix(isSafe: false, "Deletes the component from the offending gameObject.",
                    (target) => {
                        GameObject g = ((Component)target).gameObject;
                        GameObject.DestroyImmediate(target);
                        //target will be null so we need to select an mark dirty here.
                        UnityEditor.Selection.activeObject = g;
                        UnityEditor.EditorUtility.SetDirty(g);
                        EditorSceneManager.SaveOpenScenes();
                    }
                );
                */

                SpatialValidator.AddResponse(resp);
            }
        }
    }
}
