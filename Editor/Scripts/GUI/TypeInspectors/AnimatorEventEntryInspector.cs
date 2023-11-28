using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorEvent.AnimatorEventEntry))]
    public class AnimatorEventInspector : UnityEditor.PropertyDrawer
    {
        private bool _initialized = false;

        private Rect _topLeftRect;
        private Rect _topRightRect;
        private Rect _leftRect;
        private Rect _middleRect;
        private Rect _rightRect;

        private GUIStyle _syncedStyle;
        private GUIStyle _unsyncedStyle;

        private void InitializeIfNecessary(Rect position)
        {
            if (_initialized)
            {
                return;
            }

            _topLeftRect = new Rect(position.x, position.y, position.width * .65f, EditorGUIUtility.singleLineHeight);
            _topRightRect = new Rect(position.x + position.width * .66f, position.y, position.width * .32f, EditorGUIUtility.singleLineHeight);

            _leftRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 3, position.width * .32f, EditorGUIUtility.singleLineHeight);
            _middleRect = new Rect(position.x + position.width * .33f, position.y + EditorGUIUtility.singleLineHeight + 3, position.width * .32f, EditorGUIUtility.singleLineHeight);
            _rightRect = new Rect(position.x + position.width * .66f, position.y + EditorGUIUtility.singleLineHeight + 3, position.width * .32f, EditorGUIUtility.singleLineHeight);

            _syncedStyle = new GUIStyle() {
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(5, 0, 2, 0),
            };
            _syncedStyle.normal.textColor = new Color32(157, 204, 57, 255);

            _unsyncedStyle = new GUIStyle() {
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(5, 0, 2, 0),
            };
            _unsyncedStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color32(200, 200, 200, 255) : new Color32(0, 0, 0, 255);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializeIfNecessary(position);

            EditorGUI.BeginProperty(position, label, property);
            Animator animator = (Animator)property.FindPropertyRelative("animator").objectReferenceValue;

            EditorGUI.PropertyField(_topLeftRect, property.FindPropertyRelative("animator"), GUIContent.none);

            if (animator != null && animator.TryGetComponent(out SpatialSyncedAnimator syncedAniamtor))
            {
                EditorGUI.LabelField(_topRightRect, "Synchronized", _syncedStyle);
            }
            else
            {
                EditorGUI.LabelField(_topRightRect, "Not Synchronized", _unsyncedStyle);
            }

            //There is a bug in the unity animator that causes it to loose all parameters when you save the scene :facepalm:
            if (animator != null && animator.parameterCount <= 0)
            {
                animator.enabled = false;
                animator.enabled = true;
            }

            if (animator != null && animator.parameterCount > 0)
            {
                string[] paramNames = animator.parameters.Select((parm) => parm.name).ToArray();
                int index = Mathf.Max(0, Array.IndexOf(paramNames, property.FindPropertyRelative("parameter").stringValue));
                index = EditorGUI.Popup(_leftRect, index, paramNames);
                property.FindPropertyRelative("parameter").stringValue = paramNames[index];

                string propertyName = property.FindPropertyRelative("parameter").stringValue;
                int propertyIndex = -1;

                for (int i = 0; i < animator.parameterCount; i++)
                {
                    if (propertyName == animator.parameters[i].name)
                    {
                        propertyIndex = i;
                        break;
                    }
                }
                if (propertyIndex == -1)
                {
                    EditorGUI.LabelField(_middleRect, "Invalid Parameter");
                }
                else if (animator.parameters[propertyIndex].type == AnimatorControllerParameterType.Trigger)
                {
                    EditorGUI.LabelField(_middleRect, "Trigger");
                }
                else if (animator.parameters[propertyIndex].type == AnimatorControllerParameterType.Bool)
                {
                    EditorGUI.LabelField(_middleRect, "Bool Set");

                    bool boolValue = property.FindPropertyRelative("boolValue").boolValue;
                    boolValue = EditorGUI.ToggleLeft(_rightRect, "", boolValue);
                    property.FindPropertyRelative("boolValue").boolValue = boolValue;
                }
                else
                {
                    AnimatorEvent.OperationType opType = (AnimatorEvent.OperationType)property.FindPropertyRelative("operation").enumValueIndex;
                    opType = (AnimatorEvent.OperationType)EditorGUI.EnumPopup(_middleRect, opType);
                    property.FindPropertyRelative("operation").enumValueIndex = (int)opType;

                    if (animator.parameters[propertyIndex].type == AnimatorControllerParameterType.Float)
                    {
                        float floatValue = property.FindPropertyRelative("floatValue").floatValue;
                        floatValue = EditorGUI.FloatField(_rightRect, floatValue);
                        property.FindPropertyRelative("floatValue").floatValue = floatValue;
                    }
                    else if (animator.parameters[propertyIndex].type == AnimatorControllerParameterType.Int)
                    {
                        int intValue = property.FindPropertyRelative("intValue").intValue;
                        intValue = EditorGUI.IntField(_rightRect, intValue);
                        property.FindPropertyRelative("intValue").intValue = intValue;
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(_leftRect, "There are no parameters defined in this animator");
            }

            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2f + 10f;
        }
    }
}