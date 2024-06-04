using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.Callbacks;
using UnityEngine.Rendering.Universal;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.Editor
{
    public static class SpatialPackageProcessor
    {
        [PostProcessSceneAttribute(1)]
        public static void OnPostprocessScene()
        {
            SceneAsset activeSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
            if (Application.isPlaying || activeSceneAsset == null)
                return;

            // Validate that the active scene is part of the active package
            if (ProjectConfig.activePackageConfig is SpaceConfig spaceConfig)
            {
                if (spaceConfig.scene != activeSceneAsset)
                    return;
            }
            else if (ProjectConfig.activePackageConfig is SpaceTemplateConfig spaceTemplateConfig)
            {
                if (spaceTemplateConfig.variants == null ||
                    !Array.Exists(spaceTemplateConfig.variants, (variant) => variant.scene == activeSceneAsset))
                {
                    return;
                }
            }
            else
            {
                return; // No processing for non-scene based packages
            }

            Scene activeScene = SceneManager.GetActiveScene();
            foreach (var rootGO in activeScene.GetRootGameObjects())
            {
                ProcessRootGameObject(rootGO);
            }

            ProcessCameras(GameObject.FindObjectsOfType<Camera>(true));

            // users should not be adding EnvironmentData to their scene. remove any and recreate
            EnvironmentData[] dataInScene = GameObject.FindObjectsOfType<EnvironmentData>(true);
            for (int i = 0; i < dataInScene.Length; i++)
            {
                UnityEngine.Object.DestroyImmediate(dataInScene[i]);
            }

            EnvironmentData data = new GameObject("EnvironmentData").AddComponent<EnvironmentData>();
            data.savedProjectSettings = ProjectConfig.activePackageConfig.savedProjectSettings;

            // Spatial components
            data.seats = GameObject.FindObjectsOfType<SpatialSeatHotspot>(true);
            data.entrancePoints = GameObject.FindObjectsOfType<SpatialEntrancePoint>(true);
            data.emptyFrames = GameObject.FindObjectsOfType<SpatialEmptyFrame>(true);
            data.thumbnailCamera = GameObject.FindObjectOfType<SpatialThumbnailCamera>(true);
            if (data.thumbnailCamera != null)
            {
                data.thumbnailCamera.fieldOfView = data.thumbnailCamera.TryGetComponent(out Camera camera) ? camera.fieldOfView : 85f;
            }
            data.projectorSurfaces = GameObject.FindObjectsOfType<SpatialProjectorSurface>(true);
            data.quests = GameObject.FindObjectsOfType<SpatialQuest>(true);

            // Unity components
            data.renderingVolumes = GameObject.FindObjectsOfType<UnityEngine.Rendering.Volume>(true);
            data.enableFog = RenderSettings.fog;

            // Environment Setting
            SpatialEnvironmentSettingsOverrides environmentSettingsOverrides = GameObject.FindObjectOfType<SpatialEnvironmentSettingsOverrides>(true);
            data.environmentSettings = environmentSettingsOverrides != null ? environmentSettingsOverrides.environmentSettings : new EnvironmentSettings();

            // RenderPipeline Setting
            SpatialRenderPipelineSettingsOverrides renderPipelineSettingsOverrides = GameObject.FindObjectOfType<SpatialRenderPipelineSettingsOverrides>(true);
            data.renderPipelineSettings = (renderPipelineSettingsOverrides != null && renderPipelineSettingsOverrides.overrideSettings) ? renderPipelineSettingsOverrides.renderPipelineSettings : new RenderPipelineSettings();

            // Animators
            (data.syncedAnimators, data.unsyncedAnimators) = ProcessAnimators(GameObject.FindObjectsOfType<Animator>(true));

            // Spatial events
            SpatialTriggerEvent[] triggerEvents = GameObject.FindObjectsOfType<SpatialTriggerEvent>(true);
            SpatialInteractable[] interactables = GameObject.FindObjectsOfType<SpatialInteractable>(true);
            SpatialPointOfInterest[] pointsOfInterest = GameObject.FindObjectsOfType<SpatialPointOfInterest>(true);
            data.spatialEvents = ProcessSpatialEvents(triggerEvents, interactables, pointsOfInterest, data.quests);

            data.syncedObjects = ProcessSyncedObjects(AssetDatabase.GetAssetPath(activeSceneAsset), GameObject.FindObjectsOfType<SpatialSyncedObject>(true));

            foreach (GameObject g in GameObject.FindObjectsOfType<GameObject>())
            {
                ProcessGameObject(g);
            }

            // Embedded package assets
            if (ProjectConfig.activePackageConfig is SpaceConfig spaceConfig2 && spaceConfig2.embeddedPackageAssets != null && spaceConfig2.embeddedPackageAssets.Length > 0)
            {
                data.embeddedPackageAssets = new EmbeddedPackageAsset[spaceConfig2.embeddedPackageAssets.Length];
                Array.Copy(spaceConfig2.embeddedPackageAssets, data.embeddedPackageAssets, spaceConfig2.embeddedPackageAssets.Length);
            }

            // Network object references
            if (ProjectConfig.activePackageConfig is SpaceConfig spaceConfig3)
            {
                List<SpatialNetworkObjectReferenceData> nwObjectRefs = new List<SpatialNetworkObjectReferenceData>();
                if (spaceConfig3.networkPrefabs != null && spaceConfig3.networkPrefabs.Length > 0)
                    nwObjectRefs.AddRange(spaceConfig3.networkPrefabs);

                // Include all root network objects in the scene
                SpatialNetworkObject[] sceneNetworkObjects = GameObject.FindObjectsOfType<SpatialNetworkObject>(includeInactive: true);
                foreach (SpatialNetworkObject networkObject in sceneNetworkObjects)
                {
                    if (networkObject.rootObject == null)
                    {
                        SpatialNetworkObjectReferenceData refData = new SpatialNetworkObjectReferenceData();
                        refData.referenceType = NetworkPrefabReferenceType.SceneEmbedded;
                        refData.networkObject = networkObject;
                        nwObjectRefs.Add(refData);
                    }
                }

                data.networkObjectReferences = nwObjectRefs.ToArray();
            }
        }

        private static void AddSpatialEvent(List<SpatialEvent> list, SpatialEvent ev)
        {
            list.Add(ev);
            ev.id = list.Count - 1;
        }

        private static void ProcessRootGameObject(GameObject rootGO)
        {
            // Delete all components that are tagged with the EditorOnly attribute or the "EditorOnly" tag
            foreach (var component in rootGO.GetComponentsInChildren<Component>(true))
            {
                // component can be null if script is missing
                if (component != null)
                {
                    Type type = component.GetType();
                    if (type.IsEditorOnlyType())
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                    }
                    else if (type.IsProBuilderMesh())
                    {
                        ProBuilderStripUtility.Strip(component);
                    }
                }
            }

            // Delete all game objects that have the "EditorOnly" tag
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
                        transform.gameObject.layer = SpatialSDKPhysicsSettings.GetEffectiveLayer(transform.gameObject.layer);
                        // Remove "missing script components" from game object
                        // !! NOTE: We always want to remove "missing script" components, removing this logic may cause
                        //  assets to behave differently as the Spatial runtime available components change over time
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(transform.gameObject);
                    }
                }
            }
        }

        private static void ProcessCameras(Camera[] cameras)
        {
            // Delete non render texture cameras
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

        private static (SpatialSyncedAnimator[], Animator[]) ProcessAnimators(Animator[] allAnimators)
        {
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
            return (foundSyncedAnimators.ToArray(), foundUnsyncedAnimators.ToArray());
        }

        private static SpatialEvent[] ProcessSpatialEvents(SpatialTriggerEvent[] triggerEvents, SpatialInteractable[] interactables, SpatialPointOfInterest[] pointsOfInterest, SpatialQuest[] quests)
        {
            List<SpatialEvent> spatialEventsList = new List<SpatialEvent>();
            if (triggerEvents != null)
            {
                foreach (SpatialTriggerEvent triggerEvent in triggerEvents)
                {
                    AddSpatialEvent(spatialEventsList, triggerEvent.onEnterEvent);
                    AddSpatialEvent(spatialEventsList, triggerEvent.onExitEvent);
                }
            }
            if (interactables != null)
            {
                foreach (SpatialInteractable interactable in interactables)
                {
                    AddSpatialEvent(spatialEventsList, interactable.onInteractEvent);
                    AddSpatialEvent(spatialEventsList, interactable.onEnterEvent);
                    AddSpatialEvent(spatialEventsList, interactable.onExitEvent);
                }
            }
            if (pointsOfInterest != null)
            {
                foreach (SpatialPointOfInterest poi in pointsOfInterest)
                {
                    AddSpatialEvent(spatialEventsList, poi.onTextDisplayedEvent);
                }
            }
            if (quests != null)
            {
                foreach (SpatialQuest quest in quests)
                {
                    AddSpatialEvent(spatialEventsList, quest.onStartedEvent);
                    AddSpatialEvent(spatialEventsList, quest.onCompletedEvent);
                    AddSpatialEvent(spatialEventsList, quest.onResetEvent);
                    AddSpatialEvent(spatialEventsList, quest.onPreviouslyCompleted);
                    foreach (SpatialQuest.Task task in quest.tasks)
                    {
                        AddSpatialEvent(spatialEventsList, task.onStartedEvent);
                        AddSpatialEvent(spatialEventsList, task.onCompletedEvent);
                        AddSpatialEvent(spatialEventsList, task.onPreviouslyCompleted);
                    }
                }
            }

            // Give animation events an animator ID or remove them if null
            foreach (SpatialEvent spatialEvent in spatialEventsList)
            {
                if (!spatialEvent.hasAnimatorEvent)
                    continue;

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

            return spatialEventsList.ToArray();
        }

        private static SpatialSyncedObject[] ProcessSyncedObjects(string assetPath, SpatialSyncedObject[] embeddedSyncedObjects)
        {
            // setup with all synced object prefabs used by asset
            List<SpatialSyncedObject> prefabSyncedObjects = AssetDatabase.GetDependencies(assetPath)
                .Select(AssetDatabase.LoadAssetAtPath<SpatialSyncedObject>)
                .Where(obj => obj != null)
                .ToList();

            foreach (SpatialSyncedObject sceneSyncedObject in embeddedSyncedObjects)
            {
                sceneSyncedObject.destroyOnCreatorDisconnect = false; // does not make sense so force it to false.
            }

            HashSet<SpatialSyncedObject> syncedObjects = new HashSet<SpatialSyncedObject>(prefabSyncedObjects);
            syncedObjects.UnionWith(embeddedSyncedObjects);

            return syncedObjects.ToArray();
        }

        public static void ProcessPackageAsset(SpatialPackageAsset asset)
        {
            asset.savedProjectSettings = ProjectConfig.activePackageConfig.savedProjectSettings;

            if (asset is SpatialPrefabObject prefabObject)
                ProcessPrefabObject(prefabObject);

            UnityEditor.EditorUtility.SetDirty(asset);
        }

        private static void ProcessPrefabObject(SpatialPrefabObject prefabObject)
        {
            GameObject gameObject = prefabObject.gameObject;
            Transform[] transform = gameObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in transform)
            {
                ProcessGameObject(t.gameObject);
            }

            ProcessRootGameObject(gameObject);
            ProcessCameras(gameObject.GetComponentsInChildren<Camera>(true));

            prefabObject.seats = gameObject.GetComponentsInChildren<SpatialSeatHotspot>(true);
            prefabObject.triggerEvents = gameObject.GetComponentsInChildren<SpatialTriggerEvent>(true);
            prefabObject.interactables = gameObject.GetComponentsInChildren<SpatialInteractable>(true);
            prefabObject.pointsOfInterest = gameObject.GetComponentsInChildren<SpatialPointOfInterest>(true);

            (prefabObject.syncedAnimators, prefabObject.unsyncedAnimators) = ProcessAnimators(gameObject.GetComponentsInChildren<Animator>(true));

            prefabObject.spatialEvents = ProcessSpatialEvents(prefabObject.triggerEvents, prefabObject.interactables, prefabObject.pointsOfInterest, null);

            prefabObject.syncedObjects = ProcessSyncedObjects(AssetDatabase.GetAssetPath(gameObject), gameObject.GetComponentsInChildren<SpatialSyncedObject>(true));
            UnityEditor.EditorUtility.SetDirty(prefabObject);
        }

        private static void ProcessGameObject(GameObject gameObject)
        {
            gameObject.layer = SpatialSDKPhysicsSettings.GetEffectiveLayer(gameObject.layer);
        }
    }
}
