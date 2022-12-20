using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;

namespace SpatialSys.UnitySDK.Editor
{
    public class ValidComponents
    {
        private static readonly HashSet<Type> _allowedComponentTypes = new HashSet<Type>() {
            // Unity
            typeof(Transform),

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
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasGroup),
            typeof(CanvasRenderer),
            typeof(Button),
            typeof(Dropdown),
            typeof(Graphic),
            typeof(Image),
            typeof(InputField),
            typeof(Mask),
            typeof(MaskableGraphic),
            typeof(RawImage),
            typeof(RectMask2D),
            typeof(Scrollbar),
            typeof(ScrollRect),
            typeof(Selectable),
            typeof(Slider),
            typeof(Text),
            typeof(Toggle),
            typeof(ToggleGroup),
            typeof(AspectRatioFitter),
            typeof(CanvasScaler),
            typeof(ContentSizeFitter),
            typeof(GridLayoutGroup),
            typeof(HorizontalLayoutGroup),
            typeof(HorizontalOrVerticalLayoutGroup),
            typeof(LayoutElement),
            typeof(LayoutGroup),
            typeof(VerticalLayoutGroup),
            typeof(BaseMeshEffect),
            typeof(Outline),
            typeof(PositionAsUV1),
            typeof(Shadow),

            // Physics
            typeof(Collider),
            typeof(Rigidbody),
            typeof(Cloth),
            typeof(CharacterJoint),
            typeof(ConfigurableJoint),
            typeof(FixedJoint),
            typeof(HingeJoint),
            typeof(SpringJoint),
            typeof(ConstantForce),

            // Rendering
            typeof(Camera),
            typeof(Light),
            typeof(LightProbeGroup),
            typeof(LightProbeProxyVolume),
            typeof(Projector),
            typeof(ReflectionProbe),
            typeof(UniversalAdditionalCameraData),
            typeof(UniversalAdditionalLightData),
            typeof(Volume),
            // Rendering.Mesh
            typeof(LODGroup),
            typeof(MeshFilter),
            typeof(Terrain),
            typeof(Tree),
            // Rendering.Renderers
            typeof(BillboardRenderer),
            typeof(LineRenderer),
            typeof(MeshRenderer),
            typeof(ParticleSystem),
            typeof(ParticleSystemRenderer),
            typeof(SkinnedMeshRenderer),
            typeof(SpriteRenderer),
            typeof(TrailRenderer),

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

        [ComponentTest(typeof(UnityEngine.Object))]
        public static void IsValidComponent(UnityEngine.Object target)
        {
            // Components can be "null" if the script is missing; These are automatically removed during scene build
            if (target == null)
                return;

            // Ignore this component if it has a [EditorOnly] attribute. These are automatically removed during scene build
            if (target.GetType().GetCustomAttributes(typeof(EditorOnlyAttribute), true).Length > 0)
                return;

            Type targetType = target.GetType();
            if (!_allowedComponentTypes.Any(t => t == targetType || t.IsAssignableFrom(targetType)))
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
