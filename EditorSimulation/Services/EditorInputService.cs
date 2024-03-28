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

        public EditorInputService()
        {
            GameObject g = new GameObject();
            g.name = "[Spatial SDK] Input Service";
            var inputHelper = g.AddComponent<EditorInputServiceHelper>();
            inputHelper.service = this;
        }

        public void Update()
        {
            if (_currentInputCaptureListener == null)
                return;

            if (_currentInputCaptureListener is IAvatarInputActionsListener avatarListener)
            {
                UpdateAvatarInputCapture(avatarListener);
            }
            else if (_currentInputCaptureListener is IVehicleInputActionsListener vehicleListener)
            {
                UpdateVehicleInputCapture(vehicleListener);
            }

        }

        private void UpdateAvatarInputCapture(IAvatarInputActionsListener avatarListener)
        {
            // TOOD: Implement
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

        public void PlayVibration(float frequency, float amplitude, float duration)
        {
            // Can't play vibrations in editor
        }
    }
}