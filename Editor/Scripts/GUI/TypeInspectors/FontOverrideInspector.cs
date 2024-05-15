using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using SpatialSys.UnitySDK.Internal;
using TMPro;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomPropertyDrawer(typeof(FontOverride))]
    public class FontOverrideInspector : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            SerializedProperty overrideFontBool = property.FindPropertyRelative(nameof(FontOverride.overrideFont));

            rect.height = EditorGUIUtility.singleLineHeight;

            overrideFontBool.boolValue = EditorGUI.ToggleLeft(rect, label, overrideFontBool.boolValue);
            if (overrideFontBool.boolValue)
            {
                EditorGUI.indentLevel++;

                // Override properties
                SerializedProperty overrideFont = property.FindPropertyRelative(nameof(FontOverride.font));
                SerializedProperty overrideMaterial = property.FindPropertyRelative(nameof(FontOverride.material));

                // Font asset selector
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, overrideFont, true);

                // Material dropdown
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                DrawMaterialPopup(rect, overrideFont, overrideMaterial);

                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty overrideFont = property.FindPropertyRelative(nameof(FontOverride.overrideFont));
            if (overrideFont.boolValue)
            {
                return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 4;
            }
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private void DrawMaterialPopup(Rect rect, SerializedProperty overrideFont, SerializedProperty overrideMaterial)
        {
            List<Material> materials = new List<Material>();
            materials.Add(null);

            if (overrideFont.objectReferenceValue != null)
            {
                Texture atlas = ((TMP_FontAsset)overrideFont.objectReferenceValue).atlasTexture;
                materials.AddRange(AssetDatabase.FindAssets("t:Material")
                    .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                    .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(Material)))
                    .Select(obj => (Material)obj)
                    .Where(mat => mat.shader.name == "TextMeshPro/Distance Field" && mat.mainTexture == atlas)
                    .OrderBy(mat => mat.name)
                );
            }

            int index = materials.IndexOf(overrideMaterial.objectReferenceValue as Material);

            index = EditorGUI.Popup(rect, overrideMaterial.displayName, index, materials.Select(m => m?.name ?? "<None>").ToArray());
            overrideMaterial.objectReferenceValue = index == -1 ? materials[0] : materials[index];
        }
    }
}