using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.Callbacks;
using UnityEngine.Rendering.Universal;

namespace SpatialSys.UnitySDK.Editor
{
    public static class SceneProcessor
    {
        [PostProcessSceneAttribute(0)]
        public static void OnPostprocessScene()
        {
            EnvironmentConfig envConfig = ProjectConfig.activePackage as EnvironmentConfig;
            if (Application.isPlaying ||
                envConfig == null ||
                envConfig.variants == null ||
                !Array.Exists(envConfig.variants, (variant) => {
                    SceneAsset asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
                    return asset != null && asset == variant.scene;
                }))
            {
                return;
            }

            // Delete all components that are tagged with the EditorOnly attribute
            Scene activeScene = SceneManager.GetActiveScene();
            foreach (var rootGO in activeScene.GetRootGameObjects())
            {
                foreach (var component in rootGO.GetComponentsInChildren<Component>(true))
                {
                    // component == null can be true if a component script is missing
                    if (component == null || component.GetType().GetCustomAttributes(typeof(EditorOnlyAttribute), true).Length > 0)
                        UnityEngine.Object.DestroyImmediate(component);
                }
            }

            // Delete all game objects that have the "EditorOnly" tag
            foreach (var rootGO in activeScene.GetRootGameObjects())
            {
                foreach (var transform in rootGO.GetComponentsInChildren<Transform>(true))
                {
                    // nullcheck necessary in case game object was already destroyed
                    if (transform != null && transform.gameObject.tag == "EditorOnly")
                        UnityEngine.Object.DestroyImmediate(transform.gameObject);
                }
            }

            //users should not be adding environmentData to their scene. remove any
            EnvironmentData[] dataInScene = GameObject.FindObjectsOfType<EnvironmentData>();
            for (int i = 0; i < dataInScene.Length; i++)
            {
                UnityEngine.Object.DestroyImmediate(dataInScene[i]);
            }

            //add fresh environment data
            EnvironmentData data;
            GameObject g = new GameObject();
            g.name = "EnvironmentData";
            data = g.AddComponent<EnvironmentData>();

            //spatial components
            data.seats = GameObject.FindObjectsOfType<SpatialSeatHotspot>();
            data.entrancePoints = GameObject.FindObjectsOfType<SpatialEntrancePoint>();
            data.triggerEvents = GameObject.FindObjectsOfType<SpatialTriggerEvent>();
            data.emptyFrames = GameObject.FindObjectsOfType<SpatialEmptyFrame>();
            data.avatarTeleporters = GameObject.FindObjectsOfType<SpatialAvatarTeleporter>();
            data.cameraPassthroughs = GameObject.FindObjectsOfType<SpatialCameraPassthrough>();
            data.thumbnailCamera = GameObject.FindObjectOfType<SpatialThumbnailCamera>();
            if (data.thumbnailCamera != null)
            {
                data.thumbnailCamera.fieldOfView = data.thumbnailCamera.TryGetComponent(out Camera camera) ? camera.fieldOfView : 85f;
            }
            data.projectorSurfaces = GameObject.FindObjectsOfType<SpatialProjectorSurface>();

            //unity components
            data.renderingVolumes = GameObject.FindObjectsOfType<UnityEngine.Rendering.Volume>();
            data.enableFog = RenderSettings.fog;

            //animators
            Animator[] allAnimators = GameObject.FindObjectsOfType<Animator>();
            List<SpatialSyncedAnimator> foundSyncedAnimators = new List<SpatialSyncedAnimator>();
            List<Animator> foundUnsyncedAnimators = new List<Animator>();
            foreach (Animator foundAnimator in allAnimators)
            {
                if (foundAnimator.TryGetComponent(out SpatialSyncedAnimator syncedAnimator))
                {
                    syncedAnimator.animator = foundAnimator;
                    foundSyncedAnimators.Add(syncedAnimator);
                    syncedAnimator.id = foundSyncedAnimators.Count - 1;
                }
                else
                {
                    foundUnsyncedAnimators.Add(foundAnimator);
                }
            }
            data.syncedAnimators = foundSyncedAnimators.ToArray();
            data.unsyncedAnimators = foundUnsyncedAnimators.ToArray();

            //spatial events
            List<SpatialEvent> spatialEventsList = new List<SpatialEvent>();
            foreach (SpatialTriggerEvent triggerEvent in data.triggerEvents)
            {
                spatialEventsList.Add(triggerEvent.onEnterEvent);
                triggerEvent.onEnterEvent.id = spatialEventsList.Count - 1;
                spatialEventsList.Add(triggerEvent.onExitEvent);
                triggerEvent.onExitEvent.id = spatialEventsList.Count - 1;
            }
            data.spatialEvents = spatialEventsList.ToArray();

            //Give animation events an animator ID or remove them if null
            foreach (SpatialEvent spatialEvent in data.spatialEvents)
            {
                for (int i = spatialEvent.animatorEvent.events.Count - 1; i >= 0; i--)
                {
                    AnimatorEvent.AnimatorEventEntry animatorEvent = spatialEvent.animatorEvent.events[i];
                    if (animatorEvent.animator == null)
                    {
                        spatialEvent.animatorEvent.events.RemoveAt(i);
                        continue;
                    }

                    //make sure this parameter is still valid
                    int parameterIndex = -1;
                    for (int j = 0; j < animatorEvent.animator.parameterCount; j++)
                    {
                        if (animatorEvent.parameter == animatorEvent.animator.parameters[j].name)
                        {
                            parameterIndex = j;
                            break;
                        }
                    }
                    if (parameterIndex == -1)
                    {
                        spatialEvent.animatorEvent.events.RemoveAt(i);
                        continue;
                    }
                    animatorEvent.parameterType = animatorEvent.animator.GetParameter(parameterIndex).type;
                    animatorEvent.syncedAnimator = animatorEvent.animator.GetComponent<SpatialSyncedAnimator>();
                }
            }

            // Delete non render texture cameras
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (Camera camera in cameras)
            {
                if (camera.targetTexture == null)
                {
                    if (camera.TryGetComponent(out UniversalAdditionalCameraData extraData))
                    {
                        GameObject.DestroyImmediate(extraData);
                    }
                    GameObject.DestroyImmediate(camera);
                }
            }
        }
    }
}
