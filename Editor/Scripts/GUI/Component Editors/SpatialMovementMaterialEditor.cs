using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialMovementMaterial)), CanEditMultipleObjects]
    public class SpatialMovementMaterialEditor : SpatialComponentEditorBase
    {
        private SerializedProperty _splitWalkAndRun;
        private SerializedProperty _stepSound;
        private SerializedProperty _stepVFX;
        private SerializedProperty _walkStepSound;
        private SerializedProperty _walkStepVFX;
        private SerializedProperty _runStepSound;
        private SerializedProperty _runStepVFX;
        private SerializedProperty _jumpSound;
        private SerializedProperty _jumpVFX;
        private SerializedProperty _landSound;
        private SerializedProperty _landVFX;
        private SerializedProperty _stopSound;
        private SerializedProperty _stopVFX;
        private SerializedProperty _takeoffSound;
        private SerializedProperty _takeoffVFX;
        private SerializedProperty _usePhysicsMaterialIfAvailable;
        private SerializedProperty _dynamicFriction;
        private SerializedProperty _staticFriction;
        private SerializedProperty _frictionCombine;
        private SerializedProperty _syncSFX;
        private SerializedProperty _syncVolume;
        private SerializedProperty _syncVFX;
        private SerializedProperty _limitSyncDistance;
        private SerializedProperty _maxSyncDistance;

        private bool _acceptedWarning;

        private bool _audioVisualFoldout = true;
        private bool _movementPhysicsFoldout = true;
        private bool _additionalSettingsFoldout = false;



        void OnEnable()
        {
            _splitWalkAndRun = serializedObject.FindProperty(nameof(SpatialMovementMaterial.splitRunAndWalk));
            _stepSound = serializedObject.FindProperty(nameof(SpatialMovementMaterial.stepSound));
            _stepVFX = serializedObject.FindProperty(nameof(SpatialMovementMaterial.stepVFX));
            _walkStepSound = serializedObject.FindProperty(nameof(SpatialMovementMaterial.walkStepSound));
            _walkStepVFX = serializedObject.FindProperty(nameof(SpatialMovementMaterial.walkStepVFX));
            _runStepSound = serializedObject.FindProperty(nameof(SpatialMovementMaterial.runStepSound));
            _runStepVFX = serializedObject.FindProperty(nameof(SpatialMovementMaterial.runStepVFX));
            _jumpSound = serializedObject.FindProperty(nameof(SpatialMovementMaterial.jumpSound));
            _jumpVFX = serializedObject.FindProperty(nameof(SpatialMovementMaterial.jumpVFX));
            _landSound = serializedObject.FindProperty(nameof(SpatialMovementMaterial.landSound));
            _landVFX = serializedObject.FindProperty(nameof(SpatialMovementMaterial.landVFX));
            _stopSound = serializedObject.FindProperty(nameof(SpatialMovementMaterial.stopSound));
            _stopVFX = serializedObject.FindProperty(nameof(SpatialMovementMaterial.stopVFX));
            _takeoffSound = serializedObject.FindProperty(nameof(SpatialMovementMaterial.takeoffSound));
            _takeoffVFX = serializedObject.FindProperty(nameof(SpatialMovementMaterial.takeoffVFX));
            _usePhysicsMaterialIfAvailable = serializedObject.FindProperty(nameof(SpatialMovementMaterial.usePhysicsMaterial));
            _dynamicFriction = serializedObject.FindProperty(nameof(SpatialMovementMaterial.dynamicFriction));
            _staticFriction = serializedObject.FindProperty(nameof(SpatialMovementMaterial.staticFriction));
            _frictionCombine = serializedObject.FindProperty(nameof(SpatialMovementMaterial.frictionCombine));
            _syncSFX = serializedObject.FindProperty(nameof(SpatialMovementMaterial.syncSFX));
            _syncVolume = serializedObject.FindProperty(nameof(SpatialMovementMaterial.syncVolume));
            _syncVFX = serializedObject.FindProperty(nameof(SpatialMovementMaterial.syncVFX));
            _limitSyncDistance = serializedObject.FindProperty(nameof(SpatialMovementMaterial.limitSyncDistance));
            _maxSyncDistance = serializedObject.FindProperty(nameof(SpatialMovementMaterial.maxSyncDistance));

            _acceptedWarning = EditorPrefs.GetBool("MovementMaterialVFXEmitWarning", false);
        }

        public override void DrawFields()
        {
            // VFX need a very particular implementation so we give a warning for first time users.
            if (!_acceptedWarning)
            {
                SpatialGUIUtility.HelpBox("Important VFX Information!", "Particle systems used with a movement material are triggered with an Emit( ) call. The emmission components will be ingored. For more detail on what this means and how to work with it check out the documentation linked above.", SpatialGUIUtility.HelpSectionType.Warning);
                if (GUILayout.Button("I Understand"))
                {
                    EditorPrefs.SetBool("MovementMaterialVFXEmitWarning", true);
                    _acceptedWarning = true;
                }
            }

            SpatialMovementMaterial targetComponent = target as SpatialMovementMaterial;
            if (_audioVisualFoldout = EditorGUILayout.Foldout(_audioVisualFoldout, "Audio & Visuals", true, EditorStyles.foldoutHeader))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(new GUIContent("Footsteps", "Triggered everytime the player takes a step while grounded."), EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_splitWalkAndRun);
                if (targetComponent.splitRunAndWalk)
                {
                    EditorGUILayout.LabelField("Walk", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_walkStepSound);
                    CheckForParticleSystem(targetComponent.walkStepVFX);
                    EditorGUILayout.PropertyField(_walkStepVFX);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField("Run", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_runStepSound);
                    CheckForParticleSystem(targetComponent.runStepVFX);
                    EditorGUILayout.PropertyField(_runStepVFX);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.PropertyField(_stepSound);
                    CheckForParticleSystem(targetComponent.stepVFX);
                    EditorGUILayout.PropertyField(_stepVFX);
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField(new GUIContent("Jump", "Triggered every time the player jumps while grounded."), EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_jumpSound);
                CheckForParticleSystem(targetComponent.jumpVFX);
                EditorGUILayout.PropertyField(_jumpVFX);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField(new GUIContent("Land", "Triggered when the player lands if they are moving fast enough."), EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_landSound);
                CheckForParticleSystem(targetComponent.landVFX);
                EditorGUILayout.PropertyField(_landVFX);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField(new GUIContent("Stop", "Triggered when the player decelerates quickly while grounded."), EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_stopSound);
                CheckForParticleSystem(targetComponent.stopVFX);
                EditorGUILayout.PropertyField(_stopVFX);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField(new GUIContent("Takeoff", "Triggered when the player accelerates quickly while grounded."), EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_takeoffSound);
                CheckForParticleSystem(targetComponent.takeoffVFX);
                EditorGUILayout.PropertyField(_takeoffVFX);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            if (_movementPhysicsFoldout = EditorGUILayout.Foldout(_movementPhysicsFoldout, new GUIContent("Movement Physics", "Control how grippy or slippery the player controls on this material."), true, EditorStyles.foldoutHeader))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_usePhysicsMaterialIfAvailable);
                EditorGUILayout.PropertyField(_dynamicFriction);
                EditorGUILayout.PropertyField(_staticFriction);
                EditorGUILayout.PropertyField(_frictionCombine);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
            if (_additionalSettingsFoldout = EditorGUILayout.Foldout(_additionalSettingsFoldout, new GUIContent("Additional Settings"), true, EditorStyles.foldoutHeader))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_syncSFX);
                EditorGUILayout.PropertyField(_syncVolume);
                EditorGUILayout.PropertyField(_syncVFX);
                EditorGUILayout.PropertyField(_limitSyncDistance);
                if (targetComponent.limitSyncDistance)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_maxSyncDistance);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
        }

        private void CheckForParticleSystem(GameObject vfx)
        {
            if (vfx != null && !vfx.GetComponent<ParticleSystem>())
            {
                SpatialGUIUtility.HelpBox("Invalid VFX Prefab", "Make sure the VFX is a Particle System.", SpatialGUIUtility.HelpSectionType.Error);
            }
        }
    }
}
