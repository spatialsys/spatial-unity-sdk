using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

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
    }
}
