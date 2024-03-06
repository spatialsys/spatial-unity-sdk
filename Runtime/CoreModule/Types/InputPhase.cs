namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Phase of the input event.
    /// </summary>
    [DocumentationCategory("Input Service")]
    public enum InputPhase
    {
        /// <summary>
        /// Input was pressed.
        /// </summary>
        OnPressed = 0,

        /// <summary>
        /// Input is being held.
        /// </summary>
        OnHold = 1,

        /// <summary>
        /// Input was released.
        /// </summary>
        OnReleased = 2
    }
}
