using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialSyncedVariables))]
    public class SpatialSyncedVariablesEditor : UnityEditor.Editor
    {
        private bool _initialized;
        private Texture2D _backgroundTexture;
        private Texture2D _buttonTexture;
        private Texture2D _docsLinkTexture;
        private GUIStyle _areaStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _subTitleStyle;
        private GUIStyle _helpButtonStyle;
        private GUIStyle _syncedStyle;
        private GUIStyle _unsyncedStyle;

        private static readonly string[] _excludedProperties = new string[] { "m_Script" };

        void OnEnable()
        {
            var variables = target as SpatialSyncedVariables;
            if (variables != null)
            {
                variables.hideFlags = HideFlags.HideInInspector;
            }
        }

        private void InitializeIfNecessary()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            _backgroundTexture = SpatialGUIUtility.LoadGUITexture("GUI/TooltipBackground.png");
            _buttonTexture = SpatialGUIUtility.LoadGUITexture("GUI/ButtonBackground.png");
            _docsLinkTexture = SpatialGUIUtility.LoadGUITexture("Icons/icon_docsLink.png");

            _areaStyle = new GUIStyle() {
                border = new RectOffset(8, 8, 8, 8),
                padding = new RectOffset(8, 8, 8, 8),
            };
            _areaStyle.normal.background = _backgroundTexture;
            _areaStyle.normal.textColor = Color.white;

            _titleStyle = new GUIStyle() {
                fontStyle = FontStyle.Bold,
                fontSize = 16,
                wordWrap = true,
            };
            _titleStyle.normal.textColor = Color.white;

            _subTitleStyle = new GUIStyle() {
                fontSize = 11,
                wordWrap = true,
                richText = true,
            };
            _subTitleStyle.normal.textColor = new Color(1, 1, 1, .75f);

            _helpButtonStyle = new GUIStyle() {
                border = new RectOffset(8, 8, 8, 8),
                padding = new RectOffset(4, 4, 4, 4),
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                fixedWidth = 48,
            };
            _helpButtonStyle.active.background = _buttonTexture;
            _helpButtonStyle.normal.background = _buttonTexture;

            _syncedStyle = new GUIStyle(GUI.skin.button) {
                fontStyle = FontStyle.Bold,
            };
            _syncedStyle.normal.textColor = new Color32(157, 204, 57, 255);

            _unsyncedStyle = new GUIStyle(GUI.skin.button) {
                fontStyle = FontStyle.Bold,
            };
            _unsyncedStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color32(200, 200, 200, 255) : new Color32(0, 0, 0, 255);
        }

        public override void OnInspectorGUI()
        {
            InitializeIfNecessary();
            var syncedVariables = target as SpatialSyncedVariables;
            serializedObject.Update();


            GUILayout.BeginVertical(_areaStyle);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(syncedVariables.prettyName, _titleStyle);
                    if (!string.IsNullOrEmpty(syncedVariables.documentationURL))
                    {
                        GUILayout.Space(4);
                        if (GUILayout.Button(_docsLinkTexture, _helpButtonStyle))
                        {
                            Application.OpenURL(syncedVariables.documentationURL);
                        }
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(6);
                GUILayout.Label(syncedVariables.tooltip, _subTitleStyle);
            }
            GUILayout.EndVertical();

            GUILayout.Space(8);

            serializedObject.ApplyModifiedProperties();

            if (syncedVariables.TryGetComponent(out Variables variables))
            {
                bool hasSomeVariables = false;
                foreach (VariableDeclaration variable in variables.declarations)
                {
                    if (variable.typeHandle.Identification == null)
                    {
                        continue;
                    }
                    Type variableType = Type.GetType(variable.typeHandle.Identification);
                    if (TypeIsSyncable(variableType))
                    {
                        hasSomeVariables = true;
                        var syncedVariableDate = syncedVariables.variableSettings.Find(x => x.name == variable.name);
                        var isSynced = syncedVariableDate != null;

                        EditorGUILayout.BeginHorizontal();

                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.Button(variableType.HumanName(), new GUILayoutOption[] { GUILayout.Width(100) });
                        EditorGUILayout.TextField(variable.name, new GUILayoutOption[] { GUILayout.MinWidth(60) });
                        EditorGUI.EndDisabledGroup();
                        bool newIsSynced = isSynced;

                        if (GUILayout.Button(isSynced ? "Synced" : "Not Synced", isSynced ? _syncedStyle : _unsyncedStyle, new GUILayoutOption[] { GUILayout.Width(100) }))
                        {
                            newIsSynced = !isSynced;
                        }

                        if (!isSynced)
                        {
                            EditorGUI.BeginDisabledGroup(true);
                        }
                        bool newSaveWithScene = EditorGUILayout.ToggleLeft(
                            new GUIContent("Save with Space", "When checked the value will remain consistant across sessions even when nobody is present in a space. If unchecked the value will reset once the space is empty."),
                            isSynced ? syncedVariableDate.saveWithSpace : false,
                            new GUILayoutOption[] { GUILayout.Width(115) }
                            );
                        if (isSynced)
                        {
                            syncedVariableDate.saveWithSpace = newSaveWithScene;
                        }
                        if (!isSynced)
                        {
                            EditorGUI.EndDisabledGroup();
                        }
                        EditorGUILayout.EndHorizontal();

                        if (newIsSynced && !isSynced)
                        {
                            syncedVariables.variableSettings.Add(new SpatialSyncedVariables.Data() {
                                name = variable.name,
                                declaration = variable,
                            });
                        }

                        if (!newIsSynced && isSynced)
                        {
                            syncedVariables.variableSettings.Remove(syncedVariableDate);
                        }

                    }
                }

                if (!hasSomeVariables)
                {
                    SpatialGUIUtility.HelpBox(
                        "No sync-able variables found.",
                        "Define a new variable in the Visual Scripting Variables component with a compatible type.",
                        SpatialGUIUtility.HelpSectionType.Info);
                }

                // search through VariableSettings for any that are no longer valid
                List<SpatialSyncedVariables.Data> toRemove = new List<SpatialSyncedVariables.Data>();
                foreach (SpatialSyncedVariables.Data variableSetting in syncedVariables.variableSettings)
                {
                    if (!variables.declarations.IsDefined(variableSetting.name))
                    {
                        toRemove.Add(variableSetting);
                        continue;
                    }
                    if (!TypeIsSyncable(Type.GetType(variables.declarations.GetDeclaration(variableSetting.name).typeHandle.Identification)))
                    {
                        toRemove.Add(variableSetting);
                    }
                }

                if (toRemove.Count > 0)
                {
                    GUILayout.Space(8);
                    SpatialGUIUtility.HelpBox(
                        "Previously declared synced variables are no longer valid.",
                        "You probably renamed or changed the type of a variable. No worries! Feel free to clear these if this was not a mistake.",
                        SpatialGUIUtility.HelpSectionType.Warning);
                    GUILayout.Space(4);
                    foreach (SpatialSyncedVariables.Data variableSetting in toRemove)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.Button("INVALID", new GUILayoutOption[] { GUILayout.Width(100) });
                        EditorGUILayout.TextField(variableSetting.name);
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.Space(8);
                    if (GUILayout.Button("Clear Invalid Variables"))
                    {
                        foreach (SpatialSyncedVariables.Data remove in toRemove)
                        {
                            syncedVariables.variableSettings.Remove(remove);
                        }
                    }
                }
            }
            else
            {
                SpatialGUIUtility.HelpBox("Visual Scripting Variables component is missing", SpatialGUIUtility.HelpSectionType.Error);

            }

        }

        private bool TypeIsSyncable(Type type)
        {
            if (
                type == typeof(bool) ||
                type == typeof(int) ||
                type == typeof(float) ||
                type == typeof(string) ||
                type == typeof(Vector2) ||
                type == typeof(Vector3))
            {
                return true;
            }
            return false;
        }
    }
}
