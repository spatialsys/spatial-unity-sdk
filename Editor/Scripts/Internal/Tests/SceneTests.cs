using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
                    null,
                    TestResponseType.Fail,
                    "No thumbnail camera found in scene: " + scene.name,
                    "Every scene needs a thumbnail camera."
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

            if (foundCameras.Length > 1)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Fail,
                    "Multiple thumbnail cameras found in scene: " + scene.name,
                    "There should only be one thumbnail camera per scene."
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
                    null,
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

        [SceneTest]
        public static void UnityEventSecurityTest(Scene scene)
        {
            if (SpatialValidator.validationContext != ValidationContext.Publishing)
                return;

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
                        null,
                        TestResponseType.Fail,
                        "Unsupported UnityEvent Type",
                        $"UnityEvent of type \"{eventTypeStr}\" was found in {scene.name}. Only Unity Events targeting a UnityEngine.Object are supported."
                    ));
                }
            }
        }
    }
}
