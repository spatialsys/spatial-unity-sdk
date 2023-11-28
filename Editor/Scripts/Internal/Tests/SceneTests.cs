using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    // Scene tests get run once on each scene inside the packageConfig.
    public class SceneTests
    {
        [SceneTest]
        public static void CheckForThumbnailCamera(Scene scene)
        {
            SpatialThumbnailCamera[] foundCameras = GameObject.FindObjectsOfType<SpatialThumbnailCamera>();

            if (foundCameras.Length == 0)
            {
                var resp = new SpatialTestResponse(
                    AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path),
                    TestResponseType.Fail,
                    $"No thumbnail camera found in scene: {scene.name}",
                    "Every scene needs a thumbnail camera to generate a thumbnail. Use a camera angle that best represents the space."
                );

                resp.SetAutoFix(false, "Creates a thumbnail camera in the scene.",
                    (target) => {
                        GameObject g = new GameObject();
                        g.name = "ThumbnailCamera";
                        g.transform.position = new Vector3(0f, 10f, -20f);
                        g.AddComponent<SpatialThumbnailCamera>();
                        UnityEditor.Selection.activeObject = g;
                        UnityEditor.EditorUtility.SetDirty(g);
                        EditorSceneManager.SaveOpenScenes();
                    }
                );
                SpatialValidator.AddResponse(resp);
            }
            else if (foundCameras.Length > 1)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path),
                    TestResponseType.Fail,
                    $"Multiple thumbnail cameras found in scene: {scene.name}",
                    $"There are {foundCameras.Length} thumbnail cameras in this scene: {scene.name}. Remove any extra thumbnail cameras to fix this issue."
                ));
            }
        }

        [SceneTest]
        public static void RequireEntrancePoint(Scene scene)
        {
            SpatialEntrancePoint[] foundEntrances = GameObject.FindObjectsOfType<SpatialEntrancePoint>();

            if (foundEntrances.Length == 0)
            {
                var resp = new SpatialTestResponse(
                    AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path),
                    TestResponseType.Fail,
                    "No entrance point found in scene: " + scene.name,
                    "Each scene requires at least one entrance point."
                );

                resp.SetAutoFix(
                    isSafe: false,
                    "Creates an entrance point in the scene",
                     (target) => {
                         GameObject g = new GameObject();
                         g.name = "EntrancePoint";
                         g.transform.position = new Vector3(0f, 5f, -10f);
                         g.AddComponent<SpatialEntrancePoint>();
                         UnityEditor.Selection.activeObject = g;
                         UnityEditor.EditorUtility.SetDirty(g);
                         EditorSceneManager.SaveOpenScenes();
                     }
                );

                SpatialValidator.AddResponse(resp);
            }
        }

        [SceneTest]
        public static void OnlyOneProjectorAllowed(Scene scene)
        {
            SpatialProjectorSurface[] foundProjectors = GameObject.FindObjectsOfType<SpatialProjectorSurface>();

            if (foundProjectors.Length > 1)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    foundProjectors[0],
                    TestResponseType.Fail,
                    "Multiple projector surfaces found in scene: " + scene.name,
                    "There should only be one projector surface per scene."
                ));
            }
        }

        [SceneTest]
        public static void WarnIfVideoPlayerIsUsed(Scene scene)
        {
            VideoPlayer[] videoPlayers = GameObject.FindObjectsOfType<VideoPlayer>();

            if (videoPlayers.Length > 0)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    videoPlayers[0],
                    TestResponseType.Warning,
                    "Video Players are unsupported on web",
                    "Currently, embedded video players are not supported on web, but they do work on other platforms. " +
                    "If this is intended, you can ignore this warning. We may add support for video players on web in the future."
                ));
            }
        }

        // This isn't a test, but just something we want to enforce
        // If auto-generation is on, building bundles will take a really long time if GI baking takes a long time
        [SceneTest]
        public static void DisableLightmapAutoGeneration(Scene scene)
        {
            // Disable auto generation of lightmaps to prevent really long bundle build times
            if (Lightmapping.giWorkflowMode != Lightmapping.GIWorkflowMode.OnDemand)
            {
                Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path),
                    TestResponseType.Warning,
                    $"Lightmapping auto generation was turned off for {Path.GetFileNameWithoutExtension(scene.path)}",
                    "This prevents issues with long build times if GI baking takes a long time."
                ));
            }
        }

        // [SceneTest] TEMPORARILY DISABLED because it is causing CI validation issues
        public static void UnityEventSecurityTest(Scene scene)
        {
            if (SpatialValidator.runContext != ValidationRunContext.PublishingPackage)
                return;

            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
            var assemblyList = new HashSet<string>(NodeFilter.assemblyAllowList);
            bool previousLineWasPropertyPath = false;

            foreach (string line in File.ReadLines(scene.path))
            {
                string eventTypeStr = null;
                if (previousLineWasPropertyPath)
                {
                    previousLineWasPropertyPath = false;
                    string[] splitString = line.Split("value:");
                    if (splitString.Length < 2)
                    {
                        continue;
                    }
                    eventTypeStr = splitString[1].Trim();
                }
                else if (!line.Contains("m_TargetAssemblyTypeName"))
                {
                    continue;
                }
                else if (line.Contains("propertyPath"))
                {
                    previousLineWasPropertyPath = true;
                    continue;
                }
                else
                {
                    string[] splitString = line.Split("m_TargetAssemblyTypeName:");
                    if (splitString.Length < 2)
                    {
                        continue;
                    }
                    eventTypeStr = splitString[1].Trim();
                }

                if (!string.IsNullOrEmpty(eventTypeStr))
                {
                    Type t = Type.GetType(eventTypeStr);
                    //null t could be targeting an internal type, so we still need to flag them.
                    if (t != null && typeof(UnityEngine.Object).IsAssignableFrom(t) && assemblyList.Contains(t.Assembly.GetName().Name))
                    {
                        continue;
                    }

                    SpatialValidator.AddResponse(new SpatialTestResponse(
                        sceneAsset,
                        TestResponseType.Fail,
                        "Unsupported UnityEvent Type",
                        $"UnityEvent of type \"{eventTypeStr}\" was found in {scene.name}. Only Unity Events targeting a UnityEngine.Object are supported."
                    ));
                }
            }
        }

        [SceneTest]
        public static void TestScenePerformance(Scene scene)
        {
            if (SpatialValidator.runContext == ValidationRunContext.UploadingToSandbox)
                return;

            PerformanceResponse response = SpatialPerformance.GetActiveScenePerformanceResponse();

            if (response.meshColliderVertPercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"Scene {scene.name} has a lot of high density mesh colliders ({response.meshColliderVerts}/{PerformanceResponse.MAX_SUGGESTED_COLLIDER_VERTS}).",
                    "You should try to use primitives or low density meshes for colliders where possible. "
                        + "High density collision geometry will impact the performance of your space.\n"
                        + "Here's a list of all objects with high density mesh colliders:\n - " + string.Join("\n - ", response.meshColliderVertCounts.Take(100).Select(m => $"{m.Item2} - {m.Item1}"))
                ));
            }

            if (response.vertPercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"Scene {scene.name} has too many vertices.",
                    "The scene has too many high detail models. It is recommended that you stay within the suggested limits or your asset may not perform well on all platforms.\n"
                        + "Here's a list of all objects with high vertex counts:\n - " + string.Join("\n - ", response.meshVertCounts.Take(100).Select(m => $"{m.Item2} - {m.Item1}"))
                ));
            }

            if (response.uniqueMaterialsPercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"Scene {scene.name} has many unique materials.",
                    $"It is encouraged for scenes to limit unique materials to around {PerformanceResponse.MAX_SUGGESTED_UNIQUE_MATERIALS}. "
                        + "The more unique materials you have, the less likely it is that your asset will perform well on all platforms. "
                        + "Look into texture atlasing techniques to share textures and materials across multiple separate objects."
                ));
            }

            if (response.sharedTexturePercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"Scene {scene.name} has too many shared textures.",
                    $"Space Templates are limited to {PerformanceResponse.MAX_SUGGESTED_SHARED_TEXTURE_MB} MB of shared textures. "
                        + "High memory usage can cause application crashes on lower end devices. It is highly recommended that you stay within the suggested limits. "
                        + "Compressing your textures will help reduce their size.\n"
                        + "Here's a list of all textures used by the scene:\n - " + string.Join("\n - ", response.textureMemorySizesMB.Take(100).Select(m => $"{m.Item2:0.00}MB - {m.Item1}"))
                ));
            }

            if (response.audioPercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"The audio clips in scene {scene.name} are using too much memory. ",
                    $"It is recommended that you limit your scene to {PerformanceResponse.MAX_SUGGESTED_AUDIO_MB} MB of audio memory. "
                        + "Converting audio files to .ogg and mono can help reduce its size."
                ));
            }
        }
    }
}
