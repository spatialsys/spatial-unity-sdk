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

            // Delete all components that are tagged with the EditorOnly attribute or the "EditorOnly" tag
            Scene activeScene = SceneManager.GetActiveScene();
            foreach (var rootGO in activeScene.GetRootGameObjects())
            {
                foreach (var component in rootGO.GetComponentsInChildren<Component>(true))
                {
                    // component can be null if script is missing
                    if (component != null && component.GetType().GetCustomAttributes(typeof(EditorOnlyAttribute), true).Length > 0)
                        UnityEngine.Object.DestroyImmediate(component);
                }
            }

            // Delete all game objects that have the "EditorOnly" tag
            foreach (var rootGO in activeScene.GetRootGameObjects())
            {
                foreach (var transform in rootGO.GetComponentsInChildren<Transform>(true))
                {
                    // nullcheck necessary in case game object was already destroyed
                    if (transform != null)
                    {
                        // Remove "EditorOnly" tagged game objects
                        if (transform.gameObject.CompareTag("EditorOnly"))
                        {
                            UnityEngine.Object.DestroyImmediate(transform.gameObject);
                        }
                        else
                        {
                            // Remove "missing script components" from game object
                            // !! NOTE: We always want to remove "missing script" components, removing this logic may cause
                            //  assets to behave differently as the Spatial runtime available components change over time
                            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(transform.gameObject);
                        }
                    }
                }
            }

            //users should not be adding environmentData to their scene. remove any
            EnvironmentData[] dataInScene = GameObject.FindObjectsOfType<EnvironmentData>(true);
            for (int i = 0; i < dataInScene.Length; i++)
            {
                UnityEngine.Object.DestroyImmediate(dataInScene[i]);
            }

            // Add fresh environment data
            GameObject g = new GameObject("EnvironmentData");
            EnvironmentData data = g.AddComponent<EnvironmentData>();

            // Spatial components
            data.seats = GameObject.FindObjectsOfType<SpatialSeatHotspot>(true);
            data.entrancePoints = GameObject.FindObjectsOfType<SpatialEntrancePoint>(true);
            data.triggerEvents = GameObject.FindObjectsOfType<SpatialTriggerEvent>(true);
            data.emptyFrames = GameObject.FindObjectsOfType<SpatialEmptyFrame>(true);
            data.avatarTeleporters = GameObject.FindObjectsOfType<SpatialAvatarTeleporter>(true);
            data.cameraPassthroughs = GameObject.FindObjectsOfType<SpatialCameraPassthrough>(true);
            data.thumbnailCamera = GameObject.FindObjectOfType<SpatialThumbnailCamera>(true);
            if (data.thumbnailCamera != null)
            {
                data.thumbnailCamera.fieldOfView = data.thumbnailCamera.TryGetComponent(out Camera camera) ? camera.fieldOfView : 85f;
            }
            data.projectorSurfaces = GameObject.FindObjectsOfType<SpatialProjectorSurface>(true);
            data.interactables = GameObject.FindObjectsOfType<SpatialInteractable>(true);
            data.pointsOfInterest = GameObject.FindObjectsOfType<SpatialPointOfInterest>(true);
            data.quests = GameObject.FindObjectsOfType<SpatialQuest>(true);

            // Unity components
            data.renderingVolumes = GameObject.FindObjectsOfType<UnityEngine.Rendering.Volume>(true);
            data.enableFog = RenderSettings.fog;

            // Environment Setting
            SpatialEnvironmentSettingsOverrides environmentSettingsOverrides = GameObject.FindObjectOfType<SpatialEnvironmentSettingsOverrides>(true);
            data.environmentSettings = environmentSettingsOverrides != null ? environmentSettingsOverrides.environmentSettings : new EnvironmentSettings();

            // Animators
            Animator[] allAnimators = GameObject.FindObjectsOfType<Animator>(true);
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

            // Spatial events
            List<SpatialEvent> spatialEventsList = new List<SpatialEvent>();
            foreach (SpatialTriggerEvent triggerEvent in data.triggerEvents)
            {
                AddSpatialEvent(spatialEventsList, triggerEvent.onEnterEvent);
                AddSpatialEvent(spatialEventsList, triggerEvent.onExitEvent);
            }
            foreach (SpatialInteractable interactable in data.interactables)
            {
                AddSpatialEvent(spatialEventsList, interactable.onInteractEvent);
                AddSpatialEvent(spatialEventsList, interactable.onEnterEvent);
                AddSpatialEvent(spatialEventsList, interactable.onExitEvent);
            }
            foreach (SpatialPointOfInterest interactable in data.pointsOfInterest)
            {
                AddSpatialEvent(spatialEventsList, interactable.onTextDisplayedEvent);
            }
            foreach (SpatialQuest quest in data.quests)
            {
                AddSpatialEvent(spatialEventsList, quest.onStartedEvent);
                AddSpatialEvent(spatialEventsList, quest.onCompletedEvent);
                AddSpatialEvent(spatialEventsList, quest.onResetEvent);
                foreach (SpatialQuest.Task task in quest.tasks)
                {
                    AddSpatialEvent(spatialEventsList, task.onStartedEvent);
                    AddSpatialEvent(spatialEventsList, task.onCompletedEvent);
                }
            }
            data.spatialEvents = spatialEventsList.ToArray();

            // Give animation events an animator ID or remove them if null
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

                    // Make sure this parameter is still valid
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
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>(true);
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

        private static void AddSpatialEvent(List<SpatialEvent> list, SpatialEvent ev)
        {
            list.Add(ev);
            ev.id = list.Count - 1;
        }
    }
}
