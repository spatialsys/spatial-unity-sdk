
namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Response code for data store operations.
    /// </summary>
    [DocumentationCategory("User World Data Store Service")]
    public enum DataStoreResponseCode
    {
        /// <summary>
        /// Operation was successful.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Unexpected error; Typically when it is not one of the other errors.
        /// </summary>
        UnknownError = 1,

        #region 100-199; Errors that are the fault of the developer (end users)

        #region Key errors (100-104)
        /// <summary>
        /// Variable key is invalid.
        /// </summary>
        VariableKeyInvalid = 100,
        /// <summary>
        /// Variable key is too long.
        /// </summary>
        VariableKeyTooLong = 101,
        /// <summary>
        /// Variable key contains invalid characters.
        /// </summary>
        VariableKeyInvalidCharacters = 102,
        #endregion

        #region Name errors (105-109)
        /// <summary>
        /// Variable name is too long.
        /// </summary>
        VariableNameTooLong = 105,
        /// <summary>
        /// Variable name contains invalid characters.
        /// </summary>
        VariableNameInvalidCharacters = 106,
        #endregion

        #region Other variable errors (110-119)
        /// <summary>
        /// Variable does not exist.
        /// </summary>
        VariableDoesNotExist = 110,
        /// <summary>
        /// Variable depth is too deep.
        /// </summary>
        VariableDepthTooDeep = 111,
        #endregion

        #region Value errors (120+)
        /// <summary>
        /// Value type is unsupported.
        /// </summary>
        UnsupportedValueType = 120,
        /// <summary>
        /// Dictionary key type is unsupported.
        /// </summary>
        UnsupportedDictionaryKeyType = 121,
        /// <summary>
        /// Value type is unknown.
        /// </summary>
        ValueTypeUnkown = 122,
        #endregion

        #endregion

        #region 200-299; Errors that are a fault in the client-side code (not user developer error)
        /// <summary>
        /// Fault in spatial code
        /// </summary>
        InternalError = 200, // Fault in spatial code
        /// <summary>
        /// The operation was cancelled.
        /// </summary>
        OperationCancelled = 201,
        #endregion

        #region 300-399; Server Errors
        /// <summary>
        /// The server encountered an error.
        /// </summary>
        InternalServerError = 300,
        #endregion
    }
}
