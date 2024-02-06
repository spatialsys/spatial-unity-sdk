using System;
using UnityEngine;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.Internal
{
    /// <summary>
    /// Component that listens to input capture listener events and broadcasts them as c# events.
    /// </summary>
    [InternalType]
    public class SpatialInputActionsListenerComponent : MonoBehaviour, IAvatarInputActionsListener, IVehicleInputActionsListener
    {
        public delegate void Vector2InputDelegate(InputPhase inputPhase, Vector2 inputMove);
        public delegate void FloatInputDelegate(InputPhase inputPhase, float input);
        public delegate void InputDelegate(InputPhase inputPhase);
        public delegate void BoolDelegate(bool boolean);
        public delegate void InputCaptureTypeDelegate(InputCaptureType inputCaptureType);

        // Input capture
        public event InputCaptureTypeDelegate onInputCaptureStartedEvent;
        public event InputCaptureTypeDelegate onInputCaptureStoppedEvent;

        // Avatar events
        public event Vector2InputDelegate onAvatarMoveInputEvent;
        public event InputDelegate onAvatarJumpInputEvent;
        public event InputDelegate onAvatarSprintInputEvent;
        public event InputDelegate onAvatarActionInputEvent;
        public event BoolDelegate onAvatarAutoSprintToggledEvent;

        // Vehicle events
        public event Vector2InputDelegate onVehicleSteerInputEvent;
        public event FloatInputDelegate onVehicleThrottleInputEvent;
        public event FloatInputDelegate onVehicleReverseInputEvent;
        public event InputDelegate onVehiclePrimaryActionInputEvent;
        public event InputDelegate onVehicleSecondaryActionInputEvent;
        public event Action onVehicleExitInputEvent;

        private void OnDisable()
        {
            SpatialBridge.inputService.ReleaseInputCapture(this);
        }

        public void ClearListeners()
        {
            onInputCaptureStartedEvent = null;
            onInputCaptureStoppedEvent = null;

            // Avatar events
            onAvatarMoveInputEvent = null;
            onAvatarJumpInputEvent = null;
            onAvatarSprintInputEvent = null;
            onAvatarActionInputEvent = null;
            onAvatarAutoSprintToggledEvent = null;

            // Vehicle events
            onVehicleSteerInputEvent = null;
            onVehicleThrottleInputEvent = null;
            onVehicleReverseInputEvent = null;
            onVehiclePrimaryActionInputEvent = null;
            onVehicleSecondaryActionInputEvent = null;
            onVehicleExitInputEvent = null;
        }

        public void OnInputCaptureStarted(InputCaptureType type)
        {
            onInputCaptureStartedEvent?.Invoke(type);
        }

        public void OnInputCaptureStopped(InputCaptureType type)
        {
            onInputCaptureStoppedEvent?.Invoke(type);
        }

        public void OnAvatarMoveInput(InputPhase inputPhase, Vector2 inputMove)
        {
            onAvatarMoveInputEvent?.Invoke(inputPhase, inputMove);
        }

        public void OnAvatarJumpInput(InputPhase inputPhase)
        {
            onAvatarJumpInputEvent?.Invoke(inputPhase);
        }

        public void OnAvatarSprintInput(InputPhase inputPhase)
        {
            onAvatarSprintInputEvent?.Invoke(inputPhase);
        }

        public void OnAvatarActionInput(InputPhase inputPhase)
        {
            onAvatarActionInputEvent?.Invoke(inputPhase);
        }

        public void OnAvatarAutoSprintToggled(bool on)
        {
            onAvatarAutoSprintToggledEvent?.Invoke(on);
        }

        public void OnVehicleSteerInput(InputPhase inputPhase, Vector2 inputSteer)
        {
            onVehicleSteerInputEvent?.Invoke(inputPhase, inputSteer);
        }

        public void OnVehicleThrottleInput(InputPhase inputPhase, float inputThrottle)
        {
            onVehicleThrottleInputEvent?.Invoke(inputPhase, inputThrottle);
        }

        public void OnVehicleReverseInput(InputPhase inputPhase, float inputReverse)
        {
            onVehicleReverseInputEvent?.Invoke(inputPhase, inputReverse);
        }

        public void OnVehiclePrimaryActionInput(InputPhase inputPhase)
        {
            onVehiclePrimaryActionInputEvent?.Invoke(inputPhase);
        }

        public void OnVehicleSecondaryActionInput(InputPhase inputPhase)
        {
            onVehicleSecondaryActionInputEvent?.Invoke(inputPhase);
        }

        public void OnVehicleExitInput()
        {
            onVehicleExitInputEvent?.Invoke();
        }
    }
}
