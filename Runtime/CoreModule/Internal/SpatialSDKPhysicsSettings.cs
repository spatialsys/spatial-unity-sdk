using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpatialSys.UnitySDK.Internal
{
#if !SPATIAL_UNITYSDK_INTERNAL && UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class SpatialSDKPhysicsSettings
    {
        //default names for the layers creators can customize
        public static readonly Dictionary<int, string> userLayers = new Dictionary<int, string>(){
            { 6, "CustomLayer1"},
            { 7, "CustomLayer2"},
            { 8, "CustomLayer3"},
            { 9, "CustomLayer4"},
            { 10, "CustomLayer5"},
            { 11, "CustomLayer6"},
            { 12, "CustomLayer7"},
            { 13, "CustomLayer8"}
        };

        //range of layers that are customizable, used in various places to loop through layers.
        //make sure this stays in sync with the userLayers dictionary
        public static readonly Vector2Int userLayersRange = new Vector2Int(6, 13);

        // spatial layers that users can set gameobjects to use, which won't be overriden when loaded
        public static readonly List<int> settableSpatialLayers = new List<int> { 14, 31 };

        //names for the layers spatial controls. These are forced.
        public static readonly Dictionary<int, string> spatialLayers = new Dictionary<int, string>(){
            { 3, "" },
            { 14, "Vehicles" },
            { 15, "" },
            { 16, "" },
            { 17, "" },
            { 18, "" },
            { 19, "" },
            { 20, "" },
            { 21, "" },
            { 22, "" },
            { 23, "" },
            { 24, "" },
            { 25, "" },
            { 26, "" },
            { 27, "" },
            { 28, "" },
            { 29, "AvatarRemote" },
            { 30, "AvatarLocal" },
            { 31, "Environment" }
        };
        //what layers do we save physics settings for?
        public static readonly HashSet<int> customizableCollisionLayers = new HashSet<int> { 6, 7, 8, 9, 10, 11, 12, 13, 14, 29, 30, 31 };
        //layer settings that spatial controls. We make sure these match up in-editor
        public static readonly CollisionPair[] forcedCollisionSettings = new CollisionPair[] {
            new CollisionPair(14, 29, true),
            new CollisionPair(14, 30, true),
            new CollisionPair(29, 30, true),
            new CollisionPair(29, 29, true),
            new CollisionPair(30, 30, true),
            new CollisionPair(31, 31, false),
            new CollisionPair(31, 30, false),
            new CollisionPair(31, 29, false),
            new CollisionPair(31, 14, false),
        };

        static SpatialSDKPhysicsSettings()
        {
#if !SPATIAL_UNITYSDK_INTERNAL && UNITY_EDITOR
            EditorApplication.update += VerifyLayers;
#endif
        }

        public static int GetEffectiveLayer(int originalLayer)
        {
            if (userLayers.ContainsKey(originalLayer))
            {
                return originalLayer;
            }
            else if (settableSpatialLayers.Contains(originalLayer))
            {
                return originalLayer;
            }
            else
            {
                return 31;
            }
        }

#if UNITY_EDITOR
        private static void VerifyLayers()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            if (layers == null || !layers.isArray)
            {
                Debug.LogWarning("Can't set up the layers. It's possible the format of the layers and tags data has changed in this version of Unity.");
                return;
            }

            //reset all empty user layers to default
            foreach (KeyValuePair<int, string> layer in userLayers)
            {
                if (string.IsNullOrEmpty(LayerMask.LayerToName(layer.Key)))
                {
                    SerializedProperty layerSP = layers.GetArrayElementAtIndex(layer.Key);
                    layerSP.stringValue = layer.Value;
                }
            }

            //force set all spatial layers to default
            bool setLayerName = false;
            foreach (KeyValuePair<int, string> layer in spatialLayers)
            {
                if (LayerMask.LayerToName(layer.Key) != layer.Value)
                {
                    setLayerName = true;
                    SerializedProperty layerSP = layers.GetArrayElementAtIndex(layer.Key);
                    layerSP.stringValue = layer.Value;
                }
            }
            if (setLayerName)
            {
                Debug.LogWarning($"The Creator Toolkit reserves layers 14-31. Your project settings were just changed to enforce this. You can customize layers {userLayersRange.x}-{userLayersRange.y} freely.");
            }

            //force set all spatial layer collision settings
            bool setLayerDefaultValue = false;
            foreach (CollisionPair setting in forcedCollisionSettings)
            {
                if (Physics.GetIgnoreLayerCollision(setting.layer1, setting.layer2) != setting.ignore)
                {
                    setLayerDefaultValue = true;
                    Physics.IgnoreLayerCollision(setting.layer1, setting.layer2, setting.ignore);
                }
            }
            if (setLayerDefaultValue)
            {
                Debug.LogWarning($"The Creator Toolkit forces some collision settings for the managed layers (14 & 29-31). Your project settings were just changed to enforce this. You can customize layers {userLayersRange.x}-{userLayersRange.y} freely.");
            }

            tagManager.ApplyModifiedProperties();
        }
#endif

        public static List<CollisionPair> SavePhysicsSettings(bool get2D = false)
        {
            List<CollisionPair> settings = new List<CollisionPair>();
            var processedLayers = new HashSet<int>();
            foreach (int customLayer in userLayers.Keys)
            {
                foreach (int otherLayer in customizableCollisionLayers)
                {
                    // Checks if we have already processed the "inverse pair" of this (e.g. (6, 7) and (7, 6) are equivalent).
                    if (processedLayers.Contains(otherLayer))
                        continue;

                    bool ignore = false;
                    if (get2D)
                    {
                        ignore = Physics2D.GetIgnoreLayerCollision(customLayer, otherLayer);
                    }
                    else
                    {
                        ignore = Physics.GetIgnoreLayerCollision(customLayer, otherLayer);
                    }

                    if (CollisionSettingIsCustomizable(customLayer, otherLayer))
                    {
                        settings.Add(new CollisionPair(customLayer, otherLayer, ignore));
                    }
                }

                processedLayers.Add(customLayer);
            }
            return settings;
        }

        //gets the default physics settings (ignore = false) for all the customizable layer pairs
        public static List<CollisionPair> GetDefaultPhysicsSettings()
        {
            List<CollisionPair> settings = new List<CollisionPair>();
            var processedLayers = new HashSet<int>();
            foreach (int layer in customizableCollisionLayers)
            {
                foreach (int otherLayer in customizableCollisionLayers)
                {
                    if (processedLayers.Contains(otherLayer))
                        continue;

                    if (CollisionSettingIsCustomizable(layer, otherLayer))
                    {
                        settings.Add(new CollisionPair(layer, otherLayer, false));
                    }
                }
                processedLayers.Add(layer);
            }
            return settings;
        }

        public static bool CollisionSettingIsCustomizable(int layer1, int layer2)
        {
            if (!customizableCollisionLayers.Contains(layer1) || !customizableCollisionLayers.Contains(layer2))
            {
                return false;
            }

            // cannot customize if both layers are from Spatial
            if (spatialLayers.ContainsKey(layer1) && spatialLayers.ContainsKey(layer2))
            {
                return false;
            }
            return true;
        }
    }
}
