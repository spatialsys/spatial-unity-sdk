using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Service for handling all input related functionality.
    /// </summary>
    /// <remarks>To use this service, you must first call <see cref="StartAvatarInputCapture"/>,
    /// <see cref="StartVehicleInputCapture"/>, or <see cref="StartCompleteCustomInputCapture"/>.
    /// These methods will start capturing input and calling the listener methods when an overriden input event occurs.
    /// </remarks>
    [DocumentationCategory("Services/Input Service")]
    public interface IInputService
    {
        /// <summary>
        /// Event fired when input capture has started.
        /// </summary>
        event InputCaptureStartedDelegate onInputCaptureStarted;
        public delegate void InputCaptureStartedDelegate(IInputActionsListener listener, InputCaptureType type);

        /// <summary>
        /// Event fired when input capture has stopped.
        /// </summary>
        event InputCaptureStoppedDelegate onInputCaptureStopped;
        public delegate void InputCaptureStoppedDelegate(IInputActionsListener listener, InputCaptureType type);

        /// <summary>
        /// Starts overriding input for the avatar.
        /// </summary>
        /// <param name="movement">True if you want to override movement input (ignore Spatial movement)</param>
        /// <param name="jump">True if you want to override jump input (ignore Spatial jump)</param>
        /// <param name="sprint">True if you want to override spring input (ignore Spatial sprint)</param>
        /// <param name="actionButton">True if you want to override action input (ignore Spatial action)</param>
        /// <param name="listener">Listener object to capture all overriden avatar input events.</param>
        void StartAvatarInputCapture(bool movement, bool jump, bool sprint, bool actionButton, IAvatarInputActionsListener listener);

        /// <summary>
        /// Sets the input capture for the vehicle.
        /// </summary>
        /// <param name="flags">Flags to listen for</param>
        /// <param name="primaryButtonSprite">Button sprite for primary action</param>
        /// <param name="secondaryButtonSprite">Button sprite for secondary action</param>
        /// <param name="listener">Listener object to capture all vehicle capture events</param>
        void StartVehicleInputCapture(VehicleInputFlags flags, Sprite primaryButtonSprite, Sprite secondaryButtonSprite, IVehicleInputActionsListener listener);

        /// <summary>
        /// Disables default Spatial player input including camera control and mobile on-screen controls
        /// </summary>
        /// <param name="listener">Listener to capture when input capture has stopped</param>
        void StartCompleteCustomInputCapture(IInputActionsListener listener);

        /// <summary>
        /// Releases input capture for the given listener.
        /// </summary>
        /// <param name="listener">Listener to release</param>
        void ReleaseInputCapture(IInputActionsListener listener);
    }

    [DocumentationCategory("Services/Input Service")]
    public enum InputCaptureType
    {
        None,
        Avatar,
        Vehicle,
        Custom
    }

    /// <summary>
    /// Interface for listening to input capture events.
    /// </summary>
    [DocumentationCategory("Services/Input Service")]
    public interface IInputActionsListener
    {
        /// <summary>
        /// Called when input capture has stopped.
        /// </summary>
        void OnInputCaptureStarted(InputCaptureType type);

        /// <summary>
        /// Called when input capture has stopped.
        /// </summary>
        void OnInputCaptureStopped(InputCaptureType type);
    }

    /// <summary>
    /// Interface for listening to avatar input capture events. Only events overriden with <see cref="IInputService.StartAvatarInputCapture"/>
    /// will be triggered.
    /// </summary>
    [DocumentationCategory("Services/Input Service")]
    public interface IAvatarInputActionsListener : IInputActionsListener
    {
        /// <summary>
        /// Called when the user moves the avatar.
        /// </summary>
        /// <param name="inputPhase">Button input phase</param>
        /// <param name="inputMove">Directional input</param>
        void OnAvatarMoveInput(InputPhase inputPhase, Vector2 inputMove);

        /// <summary>
        /// Called when jump input has been captured.
        /// </summary>
        /// <param name="inputPhase">Button input phase</param>
        void OnAvatarJumpInput(InputPhase inputPhase);

        /// <summary>
        /// Called when sprint input has been captured.
        /// </summary>
        /// <param name="inputPhase">Button input phase</param>
        void OnAvatarSprintInput(InputPhase inputPhase);

        /// <summary>
        /// Called when action input has been captured.
        /// </summary>
        /// <param name="inputPhase">Button input phase</param>
        void OnAvatarActionInput(InputPhase inputPhase);

        /// <summary>
        /// Called when auto sprint has been toggled on.
        /// </summary>
        /// <param name="on">True if auto sprint has started, false if auto sprint has ended.</param>
        void OnAvatarAutoSprintToggled(bool on);
    }

    /// <summary>
    /// Interface for listening to vehicle input capture events. Only events overriden with <see cref="IInputService.StartVehicleInputCapture"/>
    /// will be triggered.
    /// </summary>
    [DocumentationCategory("Services/Input Service")]
    public interface IVehicleInputActionsListener : IInputActionsListener
    {
        /// <summary>
        /// Called when the user steers the vehicle.
        /// </summary>
        /// <param name="inputPhase">Button input phase</param>
        /// <param name="inputSteer">Steer movement</param>
        void OnVehicleSteerInput(InputPhase inputPhase, Vector2 inputSteer);

        /// <summary>
        /// Called when the user throttles the vehicle.
        /// </summary>
        /// <param name="inputPhase">Button input phase</param>
        /// <param name="inputThrottle">Throttle amount from 0 to 1</param>
        void OnVehicleThrottleInput(InputPhase inputPhase, float inputThrottle);

        /// <summary>
        /// Called when the user reverses the vehicle.
        /// </summary>
        /// <param name="inputPhase">Button input phase</param>
        /// <param name="inputReverse">Reverse amount from 0 to 1</param>
        void OnVehicleReverseInput(InputPhase inputPhase, float inputReverse);

        /// <summary>
        /// Called when the user presses the primary action button.
        /// </summary>
        /// <param name="inputPhase">Button input phase</param>
        void OnVehiclePrimaryActionInput(InputPhase inputPhase);

        /// <summary>
        /// Called when the user presses the secondary action button.
        /// </summary>
        /// <param name="inputPhase">Button input phase</param>
        void OnVehicleSecondaryActionInput(InputPhase inputPhase);

        /// <summary>
        /// Called when the user presses the exit vehicle button.
        /// </summary>
        void OnVehicleExitInput();
    }
}