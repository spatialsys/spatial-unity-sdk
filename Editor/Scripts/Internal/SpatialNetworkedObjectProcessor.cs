using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// Processes all SpatialNetworkObject components in the scene and prefabs to ensure they are correctly set up.
    /// </summary>
    [InitializeOnLoad]
    public class SpatialNetworkedObjectProcessor : AssetPostprocessor
    {
        // Reuse lists to avoid allocations in Bake()
        private static List<SpatialNetworkObject> _allNetworkObjects = new();
        private static List<SpatialNetworkBehaviour> _allNetworkBehaviours = new();

        static SpatialNetworkedObjectProcessor()
        {
            EditorSceneManager.sceneSaving += OnSceneSaving;
            EditorApplication.playModeStateChanged += OnPlaymodeChange;
        }

        private static void OnSceneSaving(Scene scene, string path)
        {
            BakeScene(scene);
        }

        private static void OnPlaymodeChange(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingEditMode)
                return;

            for (int i = 0; i < EditorSceneManager.sceneCount; ++i)
                BakeScene(EditorSceneManager.GetSceneAt(i));
        }

        [PostProcessSceneAttribute(0)]
        public static void OnPostprocessScene()
        {
            SceneAsset activeSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
            if (Application.isPlaying || activeSceneAsset == null)
                return;

            BakeScene(SceneManager.GetActiveScene());
        }

        private static void BakeScene(Scene scene)
        {
            bool isDirty = false;

            // Bake all root network objects in the scene
            List<SpatialNetworkObject> allNetworkObjects = new();
            List<SpatialNetworkObject> networkObjects = new();
            foreach (var rootGO in scene.GetRootGameObjects())
            {
                rootGO.GetComponentsInChildren(includeInactive: true, networkObjects);
                allNetworkObjects.AddRange(networkObjects);

                IEnumerable<SpatialNetworkObject> rootNetworkObjects = networkObjects.Where(o => {
                    if (o.transform.parent == null)
                        return true;

                    return o.transform.parent.GetComponentInParent<SpatialNetworkObject>(includeInactive: true) == null;
                });

                foreach (var networkObject in rootNetworkObjects)
                {
                    isDirty |= Bake(networkObject);
                }
            }

            // All network object scene guids must be unique
            HashSet<int> usedGuids = new();
            foreach (SpatialNetworkObject obj in allNetworkObjects)
            {
                // "Missing Component" objects can be skipped
                if (obj == null)
                    continue;

                // Objects in scenes should not have a networkPrefabGuid
                obj.networkPrefabGuid = 0;

                // Generate new guid if it's missing
                if (obj.sceneObjectGuid == 0)
                {
                    obj.RefreshGuid();
                    isDirty = true;
                }

                // Keep refreshing the guid until it's unique
                while (usedGuids.Contains(obj.sceneObjectGuid))
                {
                    Debug.LogWarning($"Duplicate sceneObjectGuid found in scene {scene.name} for object {obj.name}; Generating a new one. " +
                        "This will result in older multiplayer clients being incompatible with newer ones.");
                    obj.RefreshGuid();
                    isDirty = true;
                }

                usedGuids.Add(obj.sceneObjectGuid);
            }

            if (isDirty)
                EditorSceneManager.MarkSceneDirty(scene);
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                if (assetPath.EndsWith(".prefab"))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    SpatialNetworkObject networkObject = prefab.GetComponent<SpatialNetworkObject>();
                    if (networkObject != null)
                        BakePrefab(networkObject);
                }
            }
        }

        private static void OnPostprocessPrefab(GameObject root)
        {
            SpatialNetworkObject networkObject = root.GetComponent<SpatialNetworkObject>();
            if (networkObject == null)
                return;

            BakePrefab(networkObject);
        }

        private static void BakePrefab(SpatialNetworkObject networkObject)
        {
            // "Missing Component" objects can be skipped
            if (networkObject == null)
                return;

            bool isDirty = Bake(networkObject);

            int networkPrefabID = NetworkPrefabTable.RegisterPrefab(networkObject);
            isDirty |= Set(networkObject, ref networkObject.networkPrefabGuid, networkPrefabID);

            if (isDirty)
                UnityEditor.EditorUtility.SetDirty(networkObject);
        }

        private static bool Bake(SpatialNetworkObject rootObject)
        {
            if (rootObject == null)
            {
                Debug.LogError($"{nameof(SpatialNetworkedObjectProcessor)}.{nameof(Bake)}: gameObject must have a {nameof(SpatialNetworkObject)} component");
                return false;
            }

            rootObject.gameObject.GetComponentsInChildren(includeInactive: true, _allNetworkObjects);
            rootObject.gameObject.GetComponentsInChildren(includeInactive: true, _allNetworkBehaviours);

            bool isDirty = false;
            List<SpatialNetworkBehaviour> behaviours = new();
            for (int i = 0; i < _allNetworkObjects.Count; ++i)
            {
                SpatialNetworkObject networkObject = _allNetworkObjects[i];
                _allNetworkObjects[i] = null; // optimization so we don't try to find it as a nested object again

                // Assign root
                if (networkObject == rootObject)
                {
                    isDirty |= Set(networkObject, ref networkObject.rootObject, null);
                }
                else
                {
                    isDirty |= Set(networkObject, ref networkObject.rootObject, rootObject);
                }

                // Assign parent
                SpatialNetworkObject parentObj = null;
                if (networkObject.transform.parent != null)
                    parentObj = networkObject.transform.parent.GetComponentInParent<SpatialNetworkObject>(includeInactive: true);
                isDirty |= Set(networkObject, ref networkObject.parentObject, parentObj);

                // Find nested objects
                SpatialNetworkObject[] nestedObjs = _allNetworkObjects.Where(x => x != null && x.transform.IsChildOf(networkObject.transform)).ToArray();
                isDirty |= Set(networkObject, ref networkObject.nestedObjects, nestedObjs);

                // Find associated behaviours
                behaviours.Clear();
                for (int j = 0; j < _allNetworkBehaviours.Count; ++j)
                {
                    SpatialNetworkBehaviour behaviour = _allNetworkBehaviours[j];
                    if (behaviour != null && behaviour.GetComponentInParent<SpatialNetworkObject>(includeInactive: true) == networkObject)
                    {
                        behaviours.Add(behaviour);
                        _allNetworkBehaviours[j] = null;
                    }
                }
                isDirty |= Set(networkObject, ref networkObject.behaviours, behaviours.ToArray());
            }

            return isDirty;
        }

        private static bool Set<T>(MonoBehaviour host, ref T field, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                return true;
            }

            return false;
        }

        private static bool Set<T>(MonoBehaviour host, ref T[] field, T[] value)
        {
            var comparer = EqualityComparer<T>.Default;
            if (field == null || field.Length != value.Length || !field.SequenceEqual(value, comparer))
            {
                field = value.ToArray();
                return true;
            }

            return false;
        }
    }
}
