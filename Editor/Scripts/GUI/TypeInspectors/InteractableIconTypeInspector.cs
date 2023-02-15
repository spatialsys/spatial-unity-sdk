using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using IconType = SpatialSys.UnitySDK.SpatialInteractable.IconType;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomPropertyDrawer(typeof(IconType))]
    public class InteractableIconTypeInspector : UnityEditor.PropertyDrawer
    {
        private const int ICON_WIDTH = 16;
        private const int ICON_PADDING = 8;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            Sprite sprite;
            // Draw label / dropdown
            rect.width -= ICON_WIDTH + ICON_PADDING;
            label = EditorGUI.BeginProperty(rect, label, property);
            {
                property.intValue = (int)(IconType)EditorGUI.EnumPopup(rect, label, (IconType)property.intValue);

                sprite = LoadSprite((IconType)property.intValue);

                property.serializedObject.FindProperty(nameof(SpatialInteractable.icon)).objectReferenceValue = sprite;
            }
            EditorGUI.EndProperty();

            if (!sprite)
                sprite = LoadSprite((IconType)property.intValue);
            
            // Draw icon at right side (if we found a valid texture)
            if (sprite != null)
            {
                rect.x += rect.width + ICON_PADDING;
                rect.width = ICON_WIDTH;
                GUI.DrawTexture(rect, sprite.texture, ScaleMode.ScaleToFit);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        public Sprite LoadSprite(IconType iconType)
        {
            return SpatialGUIUtility.LoadSprite($"InteractableIcons/{iconType.ToString().ToLower()}.png");
        }
    }
}