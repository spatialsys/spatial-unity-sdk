using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    // Helper class used to run coroutines in the editor. By placing this class on a shared file, Unity
    // doesn't detect this file as an 'Editor' file and lets us attach it to a GameObject.
    public class EditorInputServiceHelper : MonoBehaviour
    {
        public EditorInputService service;

        private void Update()
        {
            service.Update();
        }
    }

    public class EditorInputService : IInputService
    {
        private enum AvatarInputOverrideFlags
        {
            None = 0,
            Movement = 1 << 0,
            Jump = 1 << 1,
            Sprint = 1 << 2,
            ActionButton = 1 << 3,
        }

        private IInputActionsListener _currentInputCaptureListener = null;
        private AvatarInputOverrideFlags _avatarFlags = AvatarInputOverrideFlags.None;
        private VehicleInputFlags _vehicleFlags = VehicleInputFlags.None;
        private InputCaptureType _inputCaptureType = InputCaptureType.None;

        private IAvatar avatar => SpatialBridge.actorService.localActor.avatar;

        public EditorInputService()
        {
            GameObject g = new GameObject();
            g.name = "[Spatial SDK] Input Service";
            var inputHelper = g.AddComponent<EditorInputServiceHelper>();
            inputHelper.service = this;

            var avatar = SpatialBridge.actorService.localActor.avatar;
        }

        public void Update()
        {
            if (_currentInputCaptureListener == null)
            {
                UpdateDefaultInputCapture();
            }
            else if (_currentInputCaptureListener is IAvatarInputActionsListener avatarListener)
            {
                UpdateAvatarInputCapture(avatarListener);
            }
            else if (_currentInputCaptureListener is IVehicleInputActionsListener vehicleListener)
            {
                UpdateVehicleInputCapture(vehicleListener);
            }
        }

        private void UpdateDefaultInputCapture()
        {
            UpdateDefaultMovement();
            UpdateDefaultJump();
        }

        private void UpdateDefaultMovement()
        {
            avatar.Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), Input.GetKey(KeyCode.LeftShift));
        }

        private void UpdateDefaultJump()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                avatar.Jump();
            }
        }

        private void UpdateAvatarInputCapture(IAvatarInputActionsListener avatarListener)
        {
            // Movement
            if ((_avatarFlags & AvatarInputOverrideFlags.Movement) != 0)
            {
                Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                if (moveInput != Vector2.zero)
                {
                    avatarListener.OnAvatarMoveInput(InputPhase.OnHold, moveInput);
                }
            }
            else
            {
                UpdateDefaultMovement();
            }

            // Jumping
            if (CheckInputKey(KeyCode.Space, out InputPhase phase))
            {
                avatarListener.OnAvatarJumpInput(phase);
            }
            else
            {
                UpdateDefaultJump();
            }

            // Sprinting
            if (CheckInputKey(KeyCode.LeftShift, out phase))
            {
                avatarListener.OnAvatarSprintInput(phase);
            }

            // Action button
            if (CheckInputKey(KeyCode.F, out phase))
            {
                avatarListener.OnAvatarActionInput(phase);
            }
        }

        private bool CheckInputKey(KeyCode key, out InputPhase phase)
        {
            if (Input.GetKeyDown(key))
            {
                phase = InputPhase.OnPressed;
                return true;
            }
            else if (Input.GetKey(key))
            {
                phase = InputPhase.OnHold;
                return true;
            }
            else if (Input.GetKeyUp(key))
            {
                phase = InputPhase.OnReleased;
                return true;
            }
            phase = InputPhase.OnReleased;

            return false;
        }

        private void UpdateVehicleInputCapture(IVehicleInputActionsListener vehicleListener)
        {
            // TODO: Implement
        }

        //--------------------------------------------------------------------------------------------------------------
        // IInputService implementation
        //--------------------------------------------------------------------------------------------------------------

        public event IInputService.InputCaptureStartedDelegate onInputCaptureStarted;
        public event IInputService.InputCaptureStoppedDelegate onInputCaptureStopped;

        public void StartAvatarInputCapture(bool movement, bool jump, bool sprint, bool actionButton, IAvatarInputActionsListener listener)
        {
            if (_currentInputCaptureListener != null)
                ReleaseInputCapture(_currentInputCaptureListener);

            if (listener == null)
                return;

            AvatarInputOverrideFlags flags = AvatarInputOverrideFlags.None;

            if (movement)
                flags |= AvatarInputOverrideFlags.Movement;
            if (jump)
                flags |= AvatarInputOverrideFlags.Jump;
            if (sprint)
                flags |= AvatarInputOverrideFlags.Sprint;
            if (actionButton)
                flags |= AvatarInputOverrideFlags.ActionButton;

            if (flags == AvatarInputOverrideFlags.None)
                return;
            _avatarFlags = flags;
            _currentInputCaptureListener = listener;
            _inputCaptureType = InputCaptureType.Avatar;

            _currentInputCaptureListener.OnInputCaptureStarted(InputCaptureType.Avatar);
            onInputCaptureStarted?.Invoke(listener, InputCaptureType.Avatar);
        }

        public void StartVehicleInputCapture(VehicleInputFlags flags, Sprite primaryButtonSprite, Sprite secondaryButtonSprite, IVehicleInputActionsListener listener)
        {
            if (_currentInputCaptureListener != null)
                ReleaseInputCapture(_currentInputCaptureListener);

            if (listener == null)
                return;

            _vehicleFlags = flags;
            _currentInputCaptureListener = listener;
            _inputCaptureType = InputCaptureType.Vehicle;

            _currentInputCaptureListener.OnInputCaptureStarted(InputCaptureType.Vehicle);
            onInputCaptureStarted?.Invoke(listener, InputCaptureType.Vehicle);
        }

        public void StartCompleteCustomInputCapture(IInputActionsListener listener)
        {
            if (_currentInputCaptureListener != null)
                ReleaseInputCapture(_currentInputCaptureListener);

            if (listener == null)
                return;

            _currentInputCaptureListener = listener;
            _inputCaptureType = InputCaptureType.Custom;

            _currentInputCaptureListener.OnInputCaptureStarted(_inputCaptureType);
            onInputCaptureStarted?.Invoke(listener, _inputCaptureType);
        }

        public void ReleaseInputCapture(IInputActionsListener listener)
        {
            if (_currentInputCaptureListener != listener || listener == null)
                return;

            _currentInputCaptureListener = null;
            _inputCaptureType = InputCaptureType.None;
            listener.OnInputCaptureStopped(_inputCaptureType);
            onInputCaptureStopped?.Invoke(listener, _inputCaptureType);
        }

        public void SetEmoteBindingsEnabled(bool enabled) { }

        public void PlayVibration(float frequency, float amplitude, float duration)
        {
            // Can't play vibrations in editor
        }
    }
}