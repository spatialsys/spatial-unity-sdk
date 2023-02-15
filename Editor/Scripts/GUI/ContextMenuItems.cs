using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public static class ContextMenuItems
    {
        static GameObject CreateGameObject(MenuCommand menuCommand, string name)
        {
            GameObject go = new GameObject(name);
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            return go;
        }

        static GameObject CreateGameObjectWithTrigger(MenuCommand menuCommand, string name)
        {
            var go = CreateGameObject(menuCommand, name);
            var collider = go.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            return go;
        }

        // Scene setup
        [MenuItem("GameObject/Spatial/Entrance Point", false, 0)]
        static void CreateEntrancePoint(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Entrance Point").AddComponent<SpatialEntrancePoint>();
        }

        [MenuItem("GameObject/Spatial/Thumbnail Camera", false, 1)]
        static void CreateThumbnailCamera(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Thumbnail Camera").AddComponent<SpatialThumbnailCamera>();
        }

        // Interactivity
        [MenuItem("GameObject/Spatial/Seat Hotspot", false, 20)]
        static void CreateSeatHotspot(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Seat Hotspot").AddComponent<SpatialSeatHotspot>();
        }

        [MenuItem("GameObject/Spatial/Trigger Event", false, 21)]
        static void CreateTriggerEvent(MenuCommand menuCommand)
        {
            CreateGameObjectWithTrigger(menuCommand, "Trigger Event").AddComponent<SpatialTriggerEvent>();
        }

        [MenuItem("GameObject/Spatial/Avatar Teleporter", false, 22)]
        static void CreateAvatarTeleporter(MenuCommand menuCommand)
        {
            // Create teleporter
            var teleporter = CreateGameObjectWithTrigger(menuCommand, "Avatar Teleporter").AddComponent<SpatialAvatarTeleporter>();

            // Create target location
            var targetLocation = new GameObject("Avatar Teleporter Target").transform;
            targetLocation.position = teleporter.transform.position + new Vector3(2, 0, 2);
            teleporter.targetLocation = targetLocation;
        }

        // Objects
        [MenuItem("GameObject/Spatial/Empty Frame", false, 40)]
        static void CreateEmptyFrame(MenuCommand menuCommand)
        {
            CreateGameObjectWithTrigger(menuCommand, "Empty Frame").AddComponent<SpatialEmptyFrame>();
        }

        [MenuItem("GameObject/Spatial/Projector Surface", false, 41)]
        static void CreateProjectorSurface(MenuCommand menuCommand)
        {
            CreateGameObjectWithTrigger(menuCommand, "Projector Surface").AddComponent<SpatialProjectorSurface>();
        }

        [MenuItem("GameObject/Spatial/Interactable", false, 43)]
        static void CreateInteractable(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Interactable").AddComponent<SpatialInteractable>();
        }

        [MenuItem("GameObject/Spatial/Point of Interest", false, 44)]
        static void CreatePointOfInterest(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Point of Interest").AddComponent<SpatialPointOfInterest>();
        }

        [MenuItem("GameObject/Spatial/Quest", false, 44)]
        static void CreateQuest(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Quest").AddComponent<SpatialQuest>();
        }
    }
}