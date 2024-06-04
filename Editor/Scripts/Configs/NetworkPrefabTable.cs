using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// Basic table for all prefabs in the project that have a SpatialNetworkObject at the root.
    /// </summary>
    public class NetworkPrefabTable : ScriptableObject
    {
        public const string ASSET_PATH = "Assets/Spatial/NetworkPrefabTable.asset";

        private static NetworkPrefabTable _cachedInstance;
        public static NetworkPrefabTable instance
        {
            get
            {
                if (_cachedInstance == null)
                {
                    _cachedInstance = AssetDatabase.LoadAssetAtPath<NetworkPrefabTable>(ASSET_PATH);
                    if (_cachedInstance == null)
                    {
                        _cachedInstance = CreateInstance<NetworkPrefabTable>();
                        Directory.CreateDirectory(Path.GetDirectoryName(ASSET_PATH));
                        AssetDatabase.CreateAsset(_cachedInstance, ASSET_PATH);
                        AssetDatabase.SaveAssets();
                    }
                }
                return _cachedInstance;
            }
        }

        /// <summary>
        /// List of all prefabs in the project; the index of the prefab in the list is the prefab's network ID
        /// As the project grows, some prefabs will be deleted, so the list will have gaps. It's important to keep the
        /// prefab IDs consistent, so we never remove an entry from the list, only mark it as deleted (null).
        /// </summary>
        [SerializeField]
        private List<string> _prefabGuids = new List<string>();

        private Dictionary<string, int> _guidToPrefabID = new Dictionary<string, int>();

        private void OnValidate()
        {
            _guidToPrefabID.Clear();
            for (int i = 0; i < _prefabGuids.Count; i++)
            {
                if (_prefabGuids[i] != null)
                    _guidToPrefabID[_prefabGuids[i]] = i + 1; // PrefabID 0 is considered invalid
            }
        }

        public static int RegisterPrefab(SpatialNetworkObject networkObject)
        {
            string assetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(networkObject));

            // Already registered
            if (instance._guidToPrefabID.TryGetValue(assetGuid, out int prefabID))
                return prefabID;

            instance._prefabGuids.Add(assetGuid);

            // PrefabID 0 is considered invalid, so we start at 1
            prefabID = instance._prefabGuids.Count;
            instance._guidToPrefabID[assetGuid] = prefabID;

            UnityEditor.EditorUtility.SetDirty(instance);
            return prefabID;
        }
    }
}