using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialNetworkVariables))]
    public class SpatialNetworkVariablesEditor : UnityEditor.Editor
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
        private bool _saveWithSceneSupported;

        private static readonly string[] _excludedProperties = new string[] { "m_Script" };

        void OnEnable()
        {
            var variables = target as SpatialNetworkVariables;
            if (variables != null)
            {
                variables.hideFlags = HideFlags.HideInInspector;
                _saveWithSceneSupported = variables.GetComponent<SpatialSyncedObject>() != null;
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
            var networkVariables = target as SpatialNetworkVariables;
            serializedObject.Update();


            GUILayout.BeginVertical(_areaStyle);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(networkVariables.prettyName, _titleStyle);
                    if (!string.IsNullOrEmpty(networkVariables.documentationURL))
                    {
                        GUILayout.Space(4);
                        if (GUILayout.Button(_docsLinkTexture, _helpButtonStyle))
                        {
                            Application.OpenURL(networkVariables.documentationURL);
                        }
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(6);
                GUILayout.Label(networkVariables.tooltip, _subTitleStyle);
            }
            GUILayout.EndVertical();

            GUILayout.Space(8);

            serializedObject.ApplyModifiedProperties();

            if (networkVariables.TryGetComponent(out Variables variables))
            {
                bool hasSomeVariables = false;
                foreach (VariableDeclaration variable in variables.declarations)
                {
                    if (variable.typeHandle.Identification == null)
                    {
                        continue;
                    }
                    Type variableType = Type.GetType(variable.typeHandle.Identification);
                    if (IsSyncable(variable))
                    {
                        hasSomeVariables = true;
                        SpatialNetworkVariables.Data networkVariableData = networkVariables.variableSettings.Find(x => x.name == variable.name);
                        bool isSynced = networkVariableData != null;

                        EditorGUILayout.BeginHorizontal();

                        EditorGUI.BeginDisabledGroup(true);
                        string id = $"ID: {(networkVariableData == null ? "-" : networkVariableData.id)}";
                        GUILayout.Button(id, new GUILayoutOption[] { GUILayout.Width(40) });
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

                        bool newSaveWithScene = false;
                        if (_saveWithSceneSupported)
                        {
                            EditorGUILayout.ToggleLeft(
                                new GUIContent("Save with Space", "When checked the value will remain consistant across sessions even when nobody is present in a space. If unchecked the value will reset once the space is empty."),
                                isSynced ? networkVariableData.saveWithSpace : false,
                                new GUILayoutOption[] { GUILayout.Width(115) }
                            );
                        }

                        if (isSynced)
                        {
                            if (_saveWithSceneSupported && newSaveWithScene != networkVariableData.saveWithSpace)
                            {
                                Undo.RecordObject(networkVariables, $"Save {variable.name} With Space");
                                networkVariableData.saveWithSpace = newSaveWithScene;
                                UnityEditor.EditorUtility.SetDirty(networkVariables);
                            }
                        }
                        else
                        {
                            EditorGUI.EndDisabledGroup();
                        }

                        EditorGUILayout.EndHorizontal();

                        if (newIsSynced && !isSynced)
                        {
                            Undo.RecordObject(networkVariables, $"Sync {variable.name}");
                            networkVariables.variableSettings.Add(new SpatialNetworkVariables.Data() {
                                id = networkVariables.GenerateUniqueVariableID(),
                                name = variable.name,
                                declaration = variable,
                            });
                            UnityEditor.EditorUtility.SetDirty(networkVariables);
                        }

                        if (!newIsSynced && isSynced)
                        {
                            Undo.RecordObject(networkVariables, $"Don't Sync {variable.name}");
                            networkVariables.variableSettings.Remove(networkVariableData);
                            UnityEditor.EditorUtility.SetDirty(networkVariables);
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
                List<SpatialNetworkVariables.Data> toRemove = new List<SpatialNetworkVariables.Data>();
                foreach (SpatialNetworkVariables.Data variableSetting in networkVariables.variableSettings)
                {
                    if (string.IsNullOrEmpty(variableSetting.name) || !variables.declarations.IsDefined(variableSetting.name))
                    {
                        toRemove.Add(variableSetting);
                        continue;
                    }
                    if (!IsSyncable(variables.declarations.GetDeclaration(variableSetting.name)))
                    {
                        toRemove.Add(variableSetting);
                    }
                }

                if (toRemove.Count > 0)
                {
                    GUILayout.Space(8);
                    SpatialGUIUtility.HelpBox(
                        "Previously declared network variables are no longer valid.",
                        "You probably renamed or changed the type of a variable. No worries! Feel free to clear these if this was not a mistake.",
                        SpatialGUIUtility.HelpSectionType.Warning);
                    GUILayout.Space(4);
                    foreach (SpatialNetworkVariables.Data variableSetting in toRemove)
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
                        foreach (SpatialNetworkVariables.Data remove in toRemove)
                        {
                            networkVariables.variableSettings.Remove(remove);
                        }
                    }
                }
            }
            else
            {
                SpatialGUIUtility.HelpBox("Visual Scripting Variables component is missing", SpatialGUIUtility.HelpSectionType.Error);
            }
        }

        public static bool IsSyncable(VariableDeclaration variable)
        {
            Type variableType = Type.GetType(variable.typeHandle.Identification);

            // For visual scripting, there is no Color32 type, so we need to check for Color
            if (variableType == typeof(Color))
                return true;

            return INetworkVariable.CURRENTLY_SUPPORTED_TYPES.Contains(variableType);
        }
    }
}
