using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor;
using System.Linq;
using System.Reflection;
using SpatialSys.UnitySDK.VisualScripting;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace SpatialSys.UnitySDK.Editor
{
    public struct NodeFilterResponse
    {
        public bool isAllowed;
        public string nodeIdentifier;
        public string responseMessage;//maybe make this an enum?

        public NodeFilterResponse(bool isAllowed, string nodeIdentifier, string responseMessage)
        {
            this.isAllowed = isAllowed;
            this.nodeIdentifier = nodeIdentifier;
            this.responseMessage = responseMessage;
        }
    }

    public class NodeFilter
    {
        public static int VS_FILTER_VERSION = 1;//increment when we want to force users to rebuild nodes on update

        public static readonly string[] namespaceAllowList = {
            "UnityEngine",
            "UnityEngine.UI",
            "Unity.VisualScripting",
            "System",
            "TMPro",
            "SpatialSys.UnitySDK.VisualScripting",
        };

        public static readonly string[] assemblyAllowList = {
            "SpatialSys.UnitySDK.VisualScripting",
            "SpatialSys.UnitySDK",

            "UnityEngine.CoreModule",
            "UnityEngine.AudioModule",
            "UnityEngine.PhysicsModule",
            "UnityEngine.AnimationModule",
            "UnityEngine.UIModule",
            "UnityEngine.UI",
            "UnityEngine.ParticleSystemModule",

            "Unity.TextMeshPro",

            "Unity.VisualScripting.Flow",//contains all the if, for, while, etc nodes
            "Unity.VisualScripting.State",//state graph nodes (enter, exit)

            "mscorlib",
        };

        //Custom types from non unity assemblies need to be added here to have their nodes generated
        //EX: Even if we added SpatialSys.UnitySDK above, we would still need to add SpatialTriggerEvent here for its nodes to generate.
        //used in NodeGeneration.cs
        public static readonly List<Type> typeGeneration = new List<Type>() {
            //Default VS types:
            typeof(object),
            typeof(bool),
            typeof(int),
            typeof(float),
            typeof(string),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(Matrix4x4),
            typeof(Rect),
            typeof(Bounds),
            typeof(Color),
            typeof(AnimationCurve),
            typeof(LayerMask),
            typeof(Ray),
            typeof(Ray2D),
            typeof(RaycastHit),
            typeof(RaycastHit2D),
            typeof(ContactPoint),
            typeof(ContactPoint2D),
            typeof(ParticleCollisionEvent),
            typeof(Mathf),
            typeof(Debug),
            typeof(Input),
            typeof(Touch),
            typeof(Screen),
            typeof(Cursor),
            typeof(Time),
            typeof(Random),
            typeof(Physics),
            typeof(Physics2D),
            typeof(GUI),
            typeof(GUILayout),
            typeof(GUIUtility),
            typeof(AudioMixerGroup),
            typeof(NavMesh),
            typeof(Gizmos),
            typeof(AnimatorStateInfo),
            typeof(BaseEventData),
            typeof(PointerEventData),
            typeof(AxisEventData),
            typeof(IList),
            typeof(IDictionary),
            typeof(Exception),
            typeof(AotList),
            typeof(AotDictionary),

            //Spatial Types
            typeof(SpatialPlatform),

            typeof(ParticleSystem),
        };

        //Block types from instantiating in the AOT graph, but also stops them from genrating in the editor. So you won't see them in the fuzzy finder.
        public static readonly Type[] typeBlockList = {
            typeof(Application),//contains plenty of dangerous and breaking methods
            typeof(Resources),//lets users load any resource by string
            typeof(UnityEngine.SceneManagement.Scene),//Scene.GetRootGameobjets is dangerous
            typeof(UnityEngine.SceneManagement.SceneManager),//Load/Unload scene is dangerous
            typeof(TMPro.TMP_PackageResourceImporterWindow),//compile errors. (editor only)
        };

        //Block specific members (nodes) from instantiating in the AOT graph
        //Most of these are fields that only exist in the editor
        public static readonly MemberInfo[][] memberBlockList = {
            //user could delete critical gameObjects with these
            typeof(GameObject).GetMember(nameof(GameObject.Find)),
            typeof(GameObject).GetMember(nameof(GameObject.FindWithTag)),
            typeof(GameObject).GetMember(nameof(GameObject.FindGameObjectWithTag)),
            typeof(GameObject).GetMember(nameof(GameObject.FindGameObjectsWithTag)),
            typeof(GameObject).GetMember(nameof(GameObject.SendMessage)),
            typeof(GameObject).GetMember(nameof(GameObject.SendMessageUpwards)),
            typeof(UnityEngine.Object).GetMember(nameof(UnityEngine.Object.FindObjectsOfTypeIncludingAssets)),
            typeof(UnityEngine.Object).GetMember(nameof(UnityEngine.Object.FindObjectOfType)),
            typeof(UnityEngine.Object).GetMember(nameof(UnityEngine.Object.FindObjectsOfType)),
            typeof(UnityEngine.Object).GetMember(nameof(UnityEngine.Object.FindObjectsOfTypeAll)),
            typeof(UnityEngine.Object).GetMember(nameof(UnityEngine.Object.FindSceneObjectsOfType)),
            typeof(UnityEngine.Object).GetMember(nameof(UnityEngine.Object.DontDestroyOnLoad)),
            typeof(UnityEngine.Object).GetMember(nameof(UnityEngine.Object.DestroyImmediate)),
            //users could call random methods
            typeof(Component).GetMember(nameof(Component.SendMessage)),
            typeof(Component).GetMember(nameof(Component.SendMessageUpwards)),

            //these are all #if UNITY_EDITOR members that cause compile errors
            typeof(MeshRenderer).GetMember(nameof(MeshRenderer.stitchLightmapSeams)),
            typeof(MeshRenderer).GetMember(nameof(MeshRenderer.receiveGI)),
            typeof(MeshRenderer).GetMember(nameof(MeshRenderer.scaleInLightmap)),
            typeof(LightProbeGroup).GetMember(nameof(LightProbeGroup.dering)),
            typeof(LightProbeGroup).GetMember(nameof(LightProbeGroup.probePositions)),// this is read only. We could allow this if we check for RO below
            typeof(Light).GetMember(nameof(Light.lightmapBakeType)),
            typeof(Light).GetMember(nameof(Light.SetLightDirty)),
            typeof(Light).GetMember(nameof(Light.shadowRadius)),
            typeof(Light).GetMember(nameof(Light.areaSize)),
            typeof(Light).GetMember(nameof(Light.shadowAngle)),
            typeof(Texture).GetMember(nameof(Texture.imageContentsHash)),
            typeof(Texture).GetMember(nameof(Texture.imageContentsHash)),
            typeof(Texture2D).GetMember(nameof(Texture2D.alphaIsTransparency)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.filteringAtrousPositionSigmaIndirect)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.filteringAtrousPositionSigmaDirect)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.filteringGaussRadiusAO)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.filteringGaussRadiusIndirect)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.filteringAtrousPositionSigmaAO)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.filteringGaussRadiusDirect)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.FilterType)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.filterTypeAO)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.filterTypeDirect)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.filterTypeIndirect)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.DenoiserType)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.denoiserTypeAO)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.denoiserTypeDirect)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.denoiserTypeIndirect)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.environmentSampleCount)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.lightProbeSampleCountMultiplier)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.trainingDataDestination)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.exportTrainingData)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.directionalityMode)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.extractAO)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.aoExponentDirect)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.aoExponentIndirect)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.aoMaxDistance)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.ao)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.lightmapCompression)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.lightmapPadding)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.indirectResolution)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.finalGather)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.filteringMode)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.prioritizeView)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.minBounces)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.maxBounces)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.indirectSampleCount)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.directSampleCount)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.Sampling)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.sampling)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.finalGatherFiltering)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.finalGatherRayCount)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.lightmapResolution)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.lightmapMaxSize)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.Lightmapper)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.lightmapper)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.indirectScale)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.albedoBoost)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.mixedBakeMode)),
            typeof(LightingSettings).GetMember(nameof(LightingSettings.autoGenerate)),
            typeof(MonoBehaviour).GetMember(nameof(MonoBehaviour.runInEditMode)),

            //mscorlib
            //these cause compile errors. "ReadOnlySpan<char>' may not be used as a type argument"
            typeof(Single).GetMember(nameof(Single.Parse)),
            typeof(Single).GetMember(nameof(Single.TryParse)),
            typeof(Single).GetMember(nameof(Single.TryFormat)),
            typeof(Int32).GetMember(nameof(Int32.Parse)),
            typeof(Int32).GetMember(nameof(Int32.TryParse)),
            typeof(Int32).GetMember(nameof(Int32.TryFormat)),
            typeof(Boolean).GetMember(nameof(Boolean.Parse)),
            typeof(Boolean).GetMember(nameof(Boolean.TryParse)),
            typeof(Boolean).GetMember(nameof(Boolean.TryFormat)),

            //UnityEngine.AudioModule (editor only / we don't have a gamepad module)
            typeof(AudioSource).GetMember(nameof(AudioSource.PlayOnGamepad)),
            typeof(AudioSource).GetMember(nameof(AudioSource.GamepadSpeakerSupportsOutputType)),
            typeof(AudioSource).GetMember(nameof(AudioSource.gamepadSpeakerOutputType)),
            typeof(AudioSource).GetMember(nameof(AudioSource.SetGamepadSpeakerRestrictedAudio)),
            typeof(AudioSource).GetMember(nameof(AudioSource.SetGamepadSpeakerMixLevelDefault)),
            typeof(AudioSource).GetMember(nameof(AudioSource.SetGamepadSpeakerMixLevel)),
            typeof(AudioSource).GetMember(nameof(AudioSource.DisableGamepadOutput)),

            //UI (editor only)
            typeof(UnityEngine.UI.Graphic).GetMember(nameof(UnityEngine.UI.Graphic.OnRebuildRequested)),
            typeof(UnityEngine.UI.Text).GetMember(nameof(UnityEngine.UI.Text.OnRebuildRequested)),

            //ParticleSystem (editor only)
            typeof(ParticleSystemRenderer).GetMember(nameof(ParticleSystemRenderer.supportsMeshInstancing)),
            typeof(ParticleSystemForceField).GetMember(nameof(ParticleSystemForceField.FindAll)),

            //Time
            typeof(UnityEngine.Time).GetMember(nameof(UnityEngine.Time.timeScale)),
            typeof(UnityEngine.Time).GetMember(nameof(UnityEngine.Time.fixedDeltaTime)),
            typeof(UnityEngine.Time).GetMember(nameof(UnityEngine.Time.maximumDeltaTime)),
            typeof(UnityEngine.Time).GetMember(nameof(UnityEngine.Time.maximumParticleDeltaTime)),
            typeof(UnityEngine.Time).GetMember(nameof(UnityEngine.Time.captureDeltaTime)),
            typeof(UnityEngine.Time).GetMember(nameof(UnityEngine.Time.captureFramerate)),
        };

        private static List<Type> supportedTypes = new List<Type>();
        private static List<MemberInfo> blockedMembers = new List<MemberInfo>();

        private static bool _initialized;

        private static void InitializeIfNecessary()
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;

            supportedTypes = new List<Type>();
            blockedMembers = memberBlockList.SelectMany(x => x).ToList();

            List<Assembly> filteredAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => assemblyAllowList.Contains(a.FullName.Split(',')[0])).ToList();

            //get all types in the filtered assemblies
            foreach (Assembly assembly in filteredAssemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!typeBlockList.Contains(type))
                    {
                        supportedTypes.Add(type);
                    }
                }
            }
        }

        public static NodeFilterResponse FilterNode(IUnit unit)
        {
            InitializeIfNecessary();

            AnalyticsIdentifier analytics = unit.GetAnalyticsIdentifier();

            //easiest way to remove pointer and ref nodes.
            if (analytics.Identifier.Contains("*") || analytics.Identifier.Contains("&"))
            {
                return new NodeFilterResponse(false, analytics.Identifier, "Ref/Pointer");
            }

            if (unit is Variables variableUnit)
            {
                return new NodeFilterResponse(true, analytics.Identifier, "VariableNode");
            }

            //Expose nodes expose editor only members sometimes.
            //Just blocking all of them for now.
            if (unit is Expose exposeUnit)
            {
                return new NodeFilterResponse(false, analytics.Identifier, "ExposeNode");
            }

            if (unit is Literal literalUnit)
            {
                if (supportedTypes.Contains(literalUnit.type) && namespaceAllowList.Contains(literalUnit.type.Namespace))
                {
                    return new NodeFilterResponse(true, analytics.Identifier, "LiteralAllowed");
                }
                else
                {
                    return new NodeFilterResponse(false, analytics.Identifier, "LiteralBlocked");
                }
            }

            if (unit is MemberUnit memberUnit)
            {
                //Unity VS has its own reflection data classes
                Unity.VisualScripting.Member info = memberUnit.member;

                if (info.declaringType == null)
                {
                    return new NodeFilterResponse(false, analytics.Identifier, "MemberUnitDeclaringTypeNull");
                }
                //assembly / namespace check
                if (!supportedTypes.Contains(info.declaringType) || !namespaceAllowList.Contains(info.declaringType.Namespace))
                {
                    return new NodeFilterResponse(false, analytics.Identifier, "MemberTypeBlocked");
                }
                //blocked member check for this type
                if (blockedMembers.Contains(info.info))
                {
                    return new NodeFilterResponse(false, analytics.Identifier, "MemberBlocked");
                }
                //check for inherited members
                //you get two different member Infos when calling getMember on the parent and child type.
                MemberInfo inheritedMember = info.declaringType.GetMember(info.info.Name)[0];
                if (info.isInherited && inheritedMember != null && blockedMembers.Contains(inheritedMember))
                {
                    return new NodeFilterResponse(false, analytics.Identifier, "InheritedMemberBlocked");
                }
                return new NodeFilterResponse(true, analytics.Identifier, "MemberAllowed");
            }

            //TODO: currently all event nodes are allowed. We will probably want to filter that in case users download plugins with event nodes.
            return new NodeFilterResponse(true, analytics.Identifier, "AllowedByDefault");
        }
    }
}
