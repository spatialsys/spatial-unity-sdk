using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public static readonly HashSet<string> namespaceAllowList = new HashSet<string>() {
            "UnityEngine",
            "UnityEngine.UI",
            "Unity.VisualScripting",
            "System",
            "System.Collections",
            "System.Collections.Specialized",
            "TMPro",
            "SpatialSys.UnitySDK.VisualScripting",
        };

        public static readonly HashSet<string> assemblyAllowList = new HashSet<string>() {
            "SpatialSys.UnitySDK.VisualScripting",
            "SpatialSys.UnitySDK",

            "UnityEngine.CoreModule",
            "UnityEngine.AudioModule",
            "UnityEngine.PhysicsModule",
            "UnityEngine.AnimationModule",
            "UnityEngine.UIModule",
            "UnityEngine.UI",
            "UnityEngine.ParticleSystemModule",
            "UnityEngine.VehiclesModule",

            "Unity.TextMeshPro",

            "Unity.VisualScripting.Flow",//contains all the if, for, while, etc nodes
            "Unity.VisualScripting.State",//state graph nodes (enter, exit)

            //Note! This assembly is actually forcebly included in the VS assembly list.
            "Unity.VisualScripting.Core",//AotList & AotDictionary

            "mscorlib",
            "netstandard",//collections
            "System",//
        };

        //Custom types from non unity assemblies need to be added here to have their nodes generated
        //EX: Even if we added SpatialSys.UnitySDK above, we would still need to add SpatialTriggerEvent here for its nodes to generate.
        //used in NodeGeneration.cs
        public static readonly List<Type> typeGeneration = new List<Type>() {
            //Default VS types:
            typeof(object),
            typeof(bool),
            typeof(int),
            typeof(uint),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(byte),
            typeof(string),
            typeof(DateTime),
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
            typeof(WheelCollider),
            typeof(WheelFrictionCurve),
            typeof(WheelHit),
            typeof(ICollection),
            typeof(IList),
            typeof(ArrayList),
            typeof(IDictionary),
            typeof(IOrderedDictionary),
            typeof(OrderedDictionary),
            typeof(CombineInstance),

            //Spatial Types
            typeof(SpatialPlatform),
            typeof(SpatialQuestTaskType),
            typeof(SpatialQuestStatus),
            typeof(SpatialCameraMode),
            typeof(SpatialCameraRotationMode),
            typeof(NetworkEventTargets),

            //Additional types
            typeof(ParticleSystem),
        };

        //Block types from instantiating in the AOT graph, but also stops them from generating in the editor. So you won't see them in the fuzzy finder.
        public static readonly HashSet<Type> typeBlockList = new HashSet<Type>() {
            typeof(Application),//contains plenty of dangerous and breaking methods
            typeof(Resources),//lets users load any resource by string
            typeof(UnityEngine.SceneManagement.Scene),//Scene.GetRootGameobjets is dangerous
            typeof(UnityEngine.SceneManagement.SceneManager),//Load/Unload scene is dangerous
            typeof(TMPro.TMP_PackageResourceImporterWindow),//compile errors. (editor only)
        };

        // Allow these specific members (nodes)
        // members contained in this list may normally be filtered out by other rules
        public static readonly MemberInfo[][] memberAllowList = {
            typeof(WheelCollider).GetMember(nameof(WheelCollider.GetGroundHit)),
            typeof(WheelCollider).GetMember(nameof(WheelCollider.GetWorldPose)),
            typeof(Vector3).GetMember(nameof(Vector3.OrthoNormalize)),
            typeof(UnityEngine.Mathf).GetMember(nameof(UnityEngine.Mathf.SmoothDamp)),
            typeof(UnityEngine.Physics).GetMember(nameof(UnityEngine.Physics.Raycast)),

            // Allow only basic overload of Parse/TryParse methods.
            new MemberInfo[] {
                typeof(float).GetMethod(nameof(float.Parse), new Type[] { typeof(string) }),
                typeof(int).GetMethod(nameof(int.Parse), new Type[] { typeof(string) }),
                typeof(bool).GetMethod(nameof(bool.Parse), new Type[] { typeof(string) }),
                typeof(byte).GetMethod(nameof(byte.Parse), new Type[] { typeof(string) }),
                typeof(double).GetMethod(nameof(double.Parse), new Type[] { typeof(string) }),
                typeof(uint).GetMethod(nameof(uint.Parse), new Type[] { typeof(string) }),
                typeof(long).GetMethod(nameof(long.Parse), new Type[] { typeof(string) }),
                typeof(ulong).GetMethod(nameof(ulong.Parse), new Type[] { typeof(string) }),
                typeof(short).GetMethod(nameof(short.Parse), new Type[] { typeof(string) }),
                typeof(ushort).GetMethod(nameof(ushort.Parse), new Type[] { typeof(string) }),
                typeof(DateTime).GetMethod(nameof(DateTime.Parse), new Type[] { typeof(string) }),

                typeof(float).GetMethod(nameof(float.TryParse), new Type[] { typeof(string), typeof(float).MakeByRefType() }),
                typeof(int).GetMethod(nameof(int.TryParse), new Type[] { typeof(string), typeof(int).MakeByRefType() }),
                typeof(bool).GetMethod(nameof(bool.TryParse), new Type[] { typeof(string), typeof(bool).MakeByRefType() }),
                typeof(byte).GetMethod(nameof(byte.TryParse), new Type[] { typeof(string), typeof(byte).MakeByRefType() }),
                typeof(double).GetMethod(nameof(double.TryParse), new Type[] { typeof(string), typeof(double).MakeByRefType() }),
                typeof(uint).GetMethod(nameof(uint.TryParse), new Type[] { typeof(string), typeof(uint).MakeByRefType() }),
                typeof(long).GetMethod(nameof(long.TryParse), new Type[] { typeof(string), typeof(long).MakeByRefType() }),
                typeof(ulong).GetMethod(nameof(ulong.TryParse), new Type[] { typeof(string), typeof(ulong).MakeByRefType() }),
                typeof(short).GetMethod(nameof(short.TryParse), new Type[] { typeof(string), typeof(short).MakeByRefType() }),
                typeof(ushort).GetMethod(nameof(ushort.TryParse), new Type[] { typeof(string), typeof(ushort).MakeByRefType() }),
                typeof(DateTime).GetMethod(nameof(DateTime.TryParse), new Type[] { typeof(string), typeof(DateTime).MakeByRefType() }),
            }
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
            typeof(float).GetMember(nameof(float.Parse)),
            typeof(float).GetMember(nameof(float.TryParse)),
            typeof(float).GetMember(nameof(float.TryFormat)),
            typeof(int).GetMember(nameof(int.Parse)),
            typeof(int).GetMember(nameof(int.TryParse)),
            typeof(int).GetMember(nameof(int.TryFormat)),
            typeof(bool).GetMember(nameof(bool.Parse)),
            typeof(bool).GetMember(nameof(bool.TryParse)),
            typeof(bool).GetMember(nameof(bool.TryFormat)),
            typeof(byte).GetMember(nameof(byte.Parse)),
            typeof(byte).GetMember(nameof(byte.TryParse)),
            typeof(byte).GetMember(nameof(byte.TryFormat)),
            typeof(double).GetMember(nameof(double.Parse)),
            typeof(double).GetMember(nameof(double.TryFormat)),
            typeof(double).GetMember(nameof(double.TryParse)),
            typeof(uint).GetMember(nameof(uint.Parse)),
            typeof(uint).GetMember(nameof(uint.TryParse)),
            typeof(uint).GetMember(nameof(uint.TryFormat)),
            typeof(long).GetMember(nameof(long.Parse)),
            typeof(long).GetMember(nameof(long.TryParse)),
            typeof(long).GetMember(nameof(long.TryFormat)),
            typeof(ulong).GetMember(nameof(ulong.Parse)),
            typeof(ulong).GetMember(nameof(ulong.TryParse)),
            typeof(ulong).GetMember(nameof(ulong.TryFormat)),
            typeof(short).GetMember(nameof(short.Parse)),
            typeof(short).GetMember(nameof(short.TryParse)),
            typeof(short).GetMember(nameof(short.TryFormat)),
            typeof(ushort).GetMember(nameof(ushort.Parse)),
            typeof(ushort).GetMember(nameof(ushort.TryParse)),
            typeof(ushort).GetMember(nameof(ushort.TryFormat)),
            typeof(DateTime).GetMember(nameof(DateTime.Parse)),
            typeof(DateTime).GetMember(nameof(DateTime.ParseExact)),
            typeof(DateTime).GetMember(nameof(DateTime.TryParse)),
            typeof(DateTime).GetMember(nameof(DateTime.TryParseExact)),
            typeof(DateTime).GetMember(nameof(DateTime.TryFormat)),

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

            //Camera
            //Don't want creators messing with the main camera. We will let them customize camera pos etc through cinemachine
            typeof(Camera).GetMember(nameof(Camera.main)),
            typeof(Camera).GetMember(nameof(Camera.allCameras)),
            typeof(Camera).GetMember(nameof(Camera.current)),
            typeof(Camera).GetMember(nameof(Camera.scene)),//no scene access allowed
        };

        private static List<Type> supportedTypes = new List<Type>();
        private static List<MemberInfo> blockedMembers = new List<MemberInfo>();
        private static List<MemberInfo> allowedMembers = new List<MemberInfo>();

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
            allowedMembers = memberAllowList.SelectMany(x => x).ToList();

            IEnumerable<Assembly> filteredAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => assemblyAllowList.Contains(a.GetName().Name));

            foreach (Assembly assembly in filteredAssemblies)
            {
                foreach (Type type in assembly.GetExportedTypesCached())
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

            if (unit is MemberUnit allowedMemberUnit)
            {
                Unity.VisualScripting.Member info = allowedMemberUnit.member;
                if (allowedMembers.Contains(info.info))
                {
                    return new NodeFilterResponse(true, analytics.Identifier, "MemberAllowList");
                }
            }

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
                //Type check
                if (!supportedTypes.Contains(info.declaringType))
                {
                    return new NodeFilterResponse(false, analytics.Identifier, "MemberTypeBlocked");
                }
                //Namespace check
                if (!namespaceAllowList.Contains(info.declaringType.Namespace))
                {
                    return new NodeFilterResponse(false, analytics.Identifier, "MemberNamespaceBlocked");
                }
                //check for extra nodes generated by users
                if (!TypeIsExplicitlySupported(info.declaringType))
                {
                    return new NodeFilterResponse(false, analytics.Identifier, "MemberTypeNotExplicitlySupported");
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

        //Is this type in the TypeGeneration list or is it a type that VS inherently supports?
        //This filter is based on IncludeInSetting() inside visualscripting@1.7.8/Editor/VisualScripting.Core/Reflection/Codebase.cs
        private static bool TypeIsExplicitlySupported(Type t)
        {
            if (typeGeneration.Contains(t))
            {
                return true;
            }

            if ((t.IsEnum || typeof(UnityEngine.Object).IsAssignableFrom(t)))
            {
                return true;
            }

            //don't check for [IncludeInSettings] because we don't use it, and the default VS nodes that use it are not members anyway

            return false;
        }
    }
}
