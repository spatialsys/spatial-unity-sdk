using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialAvatarAttachment))]
    public class SpatialAvatarAttachmentEditor : SpatialComponentEditorBase
    {
        private const int SECTION_SPACING = 10;
        private readonly Color PROPERTY_ERROR_COLOR = Color.yellow;

        private SerializedProperty _primarySlotProp;
        private SerializedProperty _additionalSlotsProp;
        private SerializedProperty _categoryProp;

        private SerializedProperty _isSkinnedToHumanoidSkeletonProp;

        private SerializedProperty _attachToBoneProp;
        private SerializedProperty _attachBoneTargetProp;
        private SerializedProperty _attachBoneOffsetProp;
        private SerializedProperty _attachBoneRotationOffsetProp;

        private SerializedProperty _ikTargetsEnabledProp;
        private SerializedProperty _ikLeftHandTargetProp;
        private SerializedProperty _ikRightHandTargetProp;
        private SerializedProperty _ikLeftFootTargetProp;
        private SerializedProperty _ikRightFootTargetProp;

        private SerializedProperty _customActionsEnabledProp;
        private SerializedProperty _customActionsCountProp;

        private SerializedProperty _overrideAvatarAnimationsProp;
        private SerializedProperty _avatarAnimSettingsProp;
        private string _currentAnimConfigFoldoutName = null;

        private SerializedProperty _attachmentAnimatorTypeProp;
        private SerializedProperty _attachmentAnimClipsProp;

        private void InitializePropertiesIfNecessary()
        {
            if (_primarySlotProp != null)
                return;

            _primarySlotProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.primarySlot));
            _additionalSlotsProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.additionalSlots));
            _categoryProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.category));

            _isSkinnedToHumanoidSkeletonProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.isSkinnedToHumanoidSkeleton));

            _attachToBoneProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.attachToBone));
            _attachBoneTargetProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.attachBoneTarget));
            _attachBoneOffsetProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.attachBoneOffset));
            _attachBoneRotationOffsetProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.attachBoneRotationOffset));

            _ikTargetsEnabledProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.ikTargetsEnabled));
            _ikLeftHandTargetProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.ikLeftHandTarget));
            _ikRightHandTargetProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.ikRightHandTarget));
            _ikLeftFootTargetProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.ikLeftFootTarget));
            _ikRightFootTargetProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.ikRightFootTarget));

            _customActionsEnabledProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.customActionsEnabled));
            _customActionsCountProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.customActionsCount));

            _overrideAvatarAnimationsProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.overrideAvatarAnimations));
            _avatarAnimSettingsProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.avatarAnimSettings));

            _attachmentAnimatorTypeProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.attachmentAnimatorType));
            _attachmentAnimClipsProp = serializedObject.FindProperty(nameof(SpatialAvatarAttachment.attachmentAnimClips));
        }

        public override void DrawFields()
        {
            var attachment = target as SpatialAvatarAttachment;
            InitializePropertiesIfNecessary();

            // Basic settings
            EditorGUILayout.LabelField("Basic Settings", EditorStyles.boldLabel);
            {
                ValidatedProp(AvatarAttachmentComponentTests.ValidatePrimarySlotField, () => EditorGUILayout.PropertyField(_primarySlotProp));
                ValidatedProp(AvatarAttachmentComponentTests.ValidateAdditionalSlotsField, () => {
                    EditorGUI.BeginChangeCheck();
                    _additionalSlotsProp.enumValueFlag = (int)(object)EditorGUILayout.EnumFlagsField(new GUIContent(_additionalSlotsProp.displayName, _additionalSlotsProp.tooltip), (SpatialAvatarAttachment.SlotMask)_additionalSlotsProp.enumValueFlag);
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                });
                ValidatedProp(AvatarAttachmentComponentTests.ValidateCategoryField, () => EditorGUILayout.PropertyField(_categoryProp));
            }

            // Don't render other settings until this is set properly
            if (attachment.primarySlot == SpatialAvatarAttachment.Slot.None)
                return;

            // Skinning; This is one of the first things you select because it affects which features are available
            GUILayout.Space(SECTION_SPACING);
            EditorGUILayout.LabelField("Skinning (Coming Soon)", EditorStyles.boldLabel);
            GUI.enabled = attachment.skinningFeatureAvailable;
            EditorGUILayout.PropertyField(_isSkinnedToHumanoidSkeletonProp);
            GUI.enabled = true;

            // Bone attachment
            GUILayout.Space(SECTION_SPACING);
            EditorGUILayout.LabelField("Bone Attachment (Coming Soon)", EditorStyles.boldLabel);
            GUI.enabled = attachment.attachToBoneFeatureAvailable;
            if (!attachment.attachToBoneFeatureAvailable)
            {
                AvatarAttachmentComponentTests.ValidateAttachBoneFeatureAvailability(attachment, out string note);
                EditorGUILayout.HelpBox(note, MessageType.None);
            }
            else
            {
                EditorGUILayout.PropertyField(_attachToBoneProp);
                if (_attachToBoneProp.boolValue && attachment.attachToBoneFeatureAvailable)
                {
                    ValidatedProp(AvatarAttachmentComponentTests.ValidateAttachBoneTarget, () => {
                        HumanBodyBones[] validBoneOptions = AvatarAttachmentComponentTests.VALID_ATTACH_BONE_TARGETS_BY_SLOT[attachment.primarySlot].ToArray();
                        GUIContent[] validOptionNames = validBoneOptions.Select(bone => new GUIContent(bone.ToString())).ToArray();
                        int[] validOptionValues = validBoneOptions.Select(bone => (int)bone).ToArray();

                        EditorGUI.BeginChangeCheck();
                        _attachBoneTargetProp.enumValueIndex = EditorGUILayout.IntPopup(
                            new GUIContent(_attachBoneTargetProp.displayName, _attachBoneTargetProp.tooltip),
                            _attachBoneTargetProp.enumValueIndex, validOptionNames, validOptionValues
                        );
                        if (EditorGUI.EndChangeCheck())
                            serializedObject.ApplyModifiedProperties();
                    });

                    EditorGUILayout.PropertyField(_attachBoneOffsetProp);
                    EditorGUILayout.PropertyField(_attachBoneRotationOffsetProp);
                }
            }
            GUI.enabled = true;

            // IK Settings
            GUILayout.Space(SECTION_SPACING);
            EditorGUILayout.LabelField(new GUIContent("IK Settings (Coming Soon)", "Optionally set IK targets"), EditorStyles.boldLabel);
            GUI.enabled = attachment.ikFeatureAvailable;
            if (!attachment.ikFeatureAvailable)
            {
                AvatarAttachmentComponentTests.ValidateIKFeatureAvailability(attachment, out string note);
                EditorGUILayout.HelpBox(note, MessageType.None);
            }
            else
            {
                EditorGUILayout.PropertyField(_ikTargetsEnabledProp);
                if (attachment.ikTargetsEnabled)
                {
                    ValidatedProp(AvatarAttachmentComponentTests.ValidateIKTargetsSetting, () => {
                        SpatialAvatarAttachment.SlotMask occupiedSlots = attachment.occupiedSlots;
                        if (occupiedSlots.HasFlag(SpatialAvatarAttachment.SlotMask.LeftHand))
                            EditorGUILayout.PropertyField(_ikLeftHandTargetProp, new GUIContent("Left Hand Target"));
                        if (occupiedSlots.HasFlag(SpatialAvatarAttachment.SlotMask.RightHand))
                            EditorGUILayout.PropertyField(_ikRightHandTargetProp, new GUIContent("Right Hand Target"));
                        if (occupiedSlots.HasFlag(SpatialAvatarAttachment.SlotMask.Feet))
                        {
                            EditorGUILayout.PropertyField(_ikLeftFootTargetProp, new GUIContent("Left Foot Target"));
                            EditorGUILayout.PropertyField(_ikRightFootTargetProp, new GUIContent("Right Foot Target"));
                        }
                    });
                }
            }
            GUI.enabled = true;

            // Custom Actions
            GUILayout.Space(SECTION_SPACING);
            EditorGUILayout.LabelField(new GUIContent("Custom Actions (Coming Soon)", "Some attachments give the avatar new abilities, such as a Sword can have an 'Attack' animation"), EditorStyles.boldLabel);
            GUI.enabled = attachment.customActionsFeatureAvailable;
            if (!attachment.customActionsFeatureAvailable)
            {
                AvatarAttachmentComponentTests.ValidateCustomActionsFeatureAvailability(attachment, out string note);
                EditorGUILayout.HelpBox(note, MessageType.None);
            }
            else
            {
                EditorGUILayout.PropertyField(_customActionsEnabledProp);
                if (_customActionsEnabledProp.boolValue)
                    ValidatedProp(AvatarAttachmentComponentTests.ValidateCustomActionsCount, () => EditorGUILayout.PropertyField(_customActionsCountProp));
            }
            GUI.enabled = true;

            // Avatar Animation Settings
            GUILayout.Space(SECTION_SPACING);
            EditorGUILayout.LabelField("Avatar Animation Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_overrideAvatarAnimationsProp);
            if (attachment.overrideAvatarAnimations)
                EditorGUILayout.HelpBox("Attachments that override the avatar animations can be very powerful, but they have limitations where only one attachment with avatar anim overrides can be equipped at a time.", MessageType.None);

            ValidatedProp(AvatarAttachmentComponentTests.ValidateAvatarAnimSettings, () => {
                EditorGUILayout.LabelField("Avatar Animation Clip Configuration");
            });
            EditorGUI.indentLevel++;
            {
                foreach (var setting in attachment.avatarAnimSettings.AllSettings())
                    DrawAvatarAnimConfiguration(attachment, setting.Item2, _avatarAnimSettingsProp.FindPropertyRelative(setting.Item1));
            }
            EditorGUI.indentLevel--;
            if (attachment.customActionsEnabled && attachment.customActionsFeatureAvailable && attachment.customActionsCount > 0)
            {
                EditorGUILayout.LabelField("Custom Action Configuration");
                EditorGUI.indentLevel++;
                {
                    SerializedProperty customActionsProp = _avatarAnimSettingsProp.FindPropertyRelative(nameof(SpatialAttachmentAvatarAnimSettings.customActions));
                    for (int i = 0; i < attachment.customActionsCount; i++)
                    {
                        AttachmentAvatarAnimConfig config = attachment.avatarAnimSettings.customActions[i];
                        if (customActionsProp.arraySize > i)
                        {
                            SerializedProperty configProp = customActionsProp.GetArrayElementAtIndex(i);
                            DrawAvatarAnimConfiguration(attachment, config, configProp, i, $"Custom Action {i + 1}");
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }

            // Animator Settings
            GUILayout.Space(SECTION_SPACING);
            EditorGUILayout.LabelField("Attachment Animation Settings", EditorStyles.boldLabel);
            GUI.enabled = attachment.animatorFeatureAvailable;
            if (!attachment.animatorFeatureAvailable)
            {
                AvatarAttachmentComponentTests.ValidateAttachmentAnimatorFeatureAvailability(attachment, out string note);
                EditorGUILayout.HelpBox(note, MessageType.None);
            }
            else
            {
                ValidatedProp(AvatarAttachmentComponentTests.ValidateAttachmentAnimatorShouldExist, () => {
                    EditorGUILayout.PropertyField(_attachmentAnimatorTypeProp);
                });

                if (attachment.attachmentAnimatorType == SpatialAvatarAttachment.AttachmentAnimatorType.Standard)
                {
                    // TODO: custom render this property field so we can do better validation on it
                    // TODO: validate that at least one clip is set
                    // TODO: hide clips that are associated with hidden attachment setting on the avatar anim settings
                    EditorGUILayout.PropertyField(_attachmentAnimClipsProp);
                }
            }
            GUI.enabled = true;
        }

        private void ValidatedProp(AvatarAttachmentComponentTests.ComponentValidatationDelegate validatationDelegate, Action renderFieldAction)
        {
            var attachment = target as SpatialAvatarAttachment;
            bool isValid = validatationDelegate(attachment, out string error);
            GUI.color = isValid ? Color.white : PROPERTY_ERROR_COLOR;
            renderFieldAction();
            if (!isValid)
                EditorGUILayout.HelpBox(error, MessageType.None);
            GUI.color = Color.white;
        }

        private void DrawAvatarAnimConfiguration(SpatialAvatarAttachment attachment, AttachmentAvatarAnimConfig config, SerializedProperty configProp, int customActionIndex = 0, string customActionName = null)
        {
            // Can happen when modify customActionsCount
            if (configProp == null)
                return;

            bool isCustomAction = !string.IsNullOrEmpty(customActionName);
            bool canOverrideClip = attachment.overrideAvatarAnimations && configProp.name != nameof(SpatialAttachmentAvatarAnimSettings.sit) && configProp.name != nameof(SpatialAttachmentAvatarAnimSettings.emote);

            // For custom actions, we should always be able to set action animation clips
            if (isCustomAction && attachment.customActionsFeatureAvailable && attachment.customActionsEnabled)
                canOverrideClip = true;

            string displayName = isCustomAction ? customActionName : configProp.displayName;
            string displayNameFull = displayName;
            if (canOverrideClip & config.overrideClip != null)
                displayNameFull += $" (Clip: {config.overrideClip.name})";
            if (!config.attachmentVisible)
                displayNameFull += " (Attachment Hidden)";
            if (attachment.ikFeatureActive && config.disableIK)
                displayNameFull += " (IK Disabled)";

            bool isValid = AvatarAttachmentComponentTests.ValidateAttachmentAvatarAnimCustomActionConfig(attachment, config, customActionIndex, isCustomAction, out string error);
            GUI.color = isValid ? Color.white : PROPERTY_ERROR_COLOR;
            bool isFoldoutExpanded = displayName == _currentAnimConfigFoldoutName;
            bool foldout = EditorGUILayout.Foldout(isFoldoutExpanded, displayNameFull);
            if (foldout != isFoldoutExpanded)
                _currentAnimConfigFoldoutName = (foldout) ? displayName : null;

            if (foldout)
            {
                EditorGUI.indentLevel++;

                if (!isValid)
                    EditorGUILayout.HelpBox(error, MessageType.None);

                if (canOverrideClip)
                {
                    EditorGUILayout.PropertyField(configProp.FindPropertyRelative(nameof(AttachmentAvatarAnimConfig.overrideClip)));
                    if (config.overrideClip != null)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(configProp.FindPropertyRelative(nameof(AttachmentAvatarAnimConfig.overrideClipMale)));
                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUILayout.PropertyField(configProp.FindPropertyRelative(nameof(AttachmentAvatarAnimConfig.attachmentVisible)));

                if (attachment.ikFeatureAvailable)
                    EditorGUILayout.PropertyField(configProp.FindPropertyRelative(nameof(AttachmentAvatarAnimConfig.disableIK)));

                EditorGUI.indentLevel--;
            }

            GUI.color = Color.white;
        }
    }
}
