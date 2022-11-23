using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.Callbacks;

namespace SpatialSys.UnitySDK.Editor
{
    public static class SceneProcessor
    {
        [PostProcessSceneAttribute(0)]
        public static void OnPostprocessScene()
        {
            PackageConfig config = PackageConfig.instance;
            if (Application.isPlaying ||
                config == null ||
                config.environment.variants == null ||
                !Array.Exists(config.environment.variants, (variant) => {
                    SceneAsset asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
                    return asset != null && asset == variant.scene;
                }))
            {
                return;
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
        }
    }
}
