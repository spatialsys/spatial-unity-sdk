using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.Editor
{
#if !SPATIAL_UNITYSDK_INTERNAL
    [InitializeOnLoad]
#endif
    public static class SpatialGameObjectCollisionHeader
    {
        private const string LAYER_WARNING_KEY = "SpatialSDK_LayerWarning";
        private const string COLLISION_HELP_DOCS = "https://toolkit.spatial.io/docs/managing-collisions";
        private const string TAGS_HELP_DOCS = "https://toolkit.spatial.io/docs/managing-collisions#tags-are-not-supported";

        private static bool _initialized;
        private static Texture2D _backgroundTexture;
        private static Texture2D _iconTexture;
        private static GUIStyle _areaStyle;
        private static GUIStyle _logoStyle;
        private static bool _showLayerWarning = false;

        //controls the layers and the order in which they are shown in the dropdown.
        private static readonly int[] layerValues = new int[]{
            31,
            14,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
        };

        static void InitializeIfNecessary()
        {
            if (_initialized)
            {
                return;
            }
            _showLayerWarning = !EditorPrefs.GetBool(LAYER_WARNING_KEY);
            _backgroundTexture = SpatialGUIUtility.LoadGUITexture("GUI/GameObjectHeaderBackground.png");
            _iconTexture = SpatialGUIUtility.LoadGUITexture("GUI/SpatialLogo.png");
            _areaStyle = new GUIStyle() {
                margin = new RectOffset(0, 4, 0, 0),
                border = new RectOffset(8, 8, 8, 8),
                padding = new RectOffset(6, 6, 6, 6),
            };
            _areaStyle.normal.background = _backgroundTexture;
            _areaStyle.normal.textColor = Color.white;

            _logoStyle = new GUIStyle() {
                fixedHeight = 20,
                fixedWidth = 20,
                padding = new RectOffset(0, 0, 0, 0),
                border = new RectOffset(0, 0, 0, 0),
            };
        }

        static SpatialGameObjectCollisionHeader()
        {
#if !SPATIAL_UNITYSDK_INTERNAL
            UnityEditor.Editor.finishedDefaultHeaderGUI += SpatialCollisionHeader;
#endif
        }

        private static void SpatialCollisionHeader(UnityEditor.Editor editor)
        {
            InitializeIfNecessary();

            if (editor.targets.Length > 1)
            {
                // Multi-object editing not supported
                return;
            }
            GameObject g = editor.target as GameObject;
            if (g == null)
            {
                return;
            }

            string[] layerOptions = new string[layerValues.Length];
            for (int i = 0; i < layerValues.Length; i++)
            {
                layerOptions[i] = LayerMask.LayerToName(layerValues[i]);
            }

            EditorGUILayout.BeginVertical(new GUIStyle() { margin = new RectOffset(8, 0, 0, 0) });

#if !SPATIAL_UNITYSDK_INTERNAL
            if (!EditorUtility.defaultTags.Contains(g.tag))
            {
                SpatialGUIUtility.HelpBox(
                    "Spatial does not support custom tags.",
                    "For more information on why this is the case, and how to work around it, click the button below.",
                    SpatialGUIUtility.HelpSectionType.Warning
                );
                GUILayout.Space(2);
                if (GUILayout.Button("Help"))
                {
                    Application.OpenURL(TAGS_HELP_DOCS);
                }
                GUILayout.Space(2);
            }
#endif

            EditorGUILayout.BeginHorizontal(_areaStyle);
            {
                GUILayout.Box(_iconTexture, _logoStyle);
                GUILayout.Label("Effective Layer", GUILayout.Width(90));
                int layer = g.layer;
                int effectiveLayer = SpatialSDKPhysicsSettings.GetEffectiveLayer(layer);
                int newLayer = EditorGUILayout.IntPopup(effectiveLayer, layerOptions, layerValues);
                if (newLayer != effectiveLayer)
                {
                    if (g.transform.childCount > 0)
                    {
                        int option = UnityEditor.EditorUtility.DisplayDialogComplex(
                            "Change Layer",
                            $"Do you want to set layer to {LayerMask.LayerToName(newLayer)} for all child objects as well?",
                            "Yes, change children",
                            "No, this object only",
                            "Cancel"
                        );

                        switch (option)
                        {
                            case 0:
                                List<UnityEngine.Object> objects = GetObjectsRecursively(g);
                                Undo.RecordObjects(objects.ToArray(), "Change Layer");
                                foreach (UnityEngine.Object o in objects)
                                {
                                    GameObject go = o as GameObject;
                                    go.layer = newLayer;
                                }
                                break;
                            case 1:
                                Undo.RecordObject(g, "Change Layer");
                                g.layer = newLayer;
                                break;
                            case 2:
                                break;
                        }
                        // Displaying the dialog nukes the gui state. Without this we throw errors for every GUILayout group we are still inside
                        EditorGUIUtility.ExitGUI();
                    }
                    else
                    {
                        Undo.RecordObject(g, "Change Layer");
                        g.layer = newLayer;
                    }
                }
                if (GUILayout.Button("Edit"))
                {
                    Selection.activeObject = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
                }
                if (GUILayout.Button("Help"))
                {
                    Application.OpenURL(COLLISION_HELP_DOCS);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (_showLayerWarning)
            {
                SpatialGUIUtility.HelpBox(
                    "Spatial converts certain layers when a space is loaded.",
                    "Use the effective layer section to keep track of what layer your gameobject is using." + Environment.NewLine + Environment.NewLine + "For more information click the help button in the section above.",
                    SpatialGUIUtility.HelpSectionType.Warning
                );
                GUILayout.Space(4);
                if (GUILayout.Button("Got it!"))
                {
                    EditorPrefs.SetBool(LAYER_WARNING_KEY, true);
                    _showLayerWarning = false;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private static List<UnityEngine.Object> GetObjectsRecursively(GameObject go)
        {
            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
            objects.Add(go);
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
            {
                objects.AddRange(GetObjectsRecursively(t.GetChild(i).gameObject));
            }
            return objects;
        }
    }
}
