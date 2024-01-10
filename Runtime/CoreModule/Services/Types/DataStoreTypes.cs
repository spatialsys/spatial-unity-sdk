
namespace SpatialSys.UnitySDK
{
    public enum DataStoreResponseCode
    {
        Ok = 0,

        // Unexpected error; Typically when it is not one of the other errors
        UnknownError = 1,

        // 100-199; Errors that are the fault of the developer (end users)
        // Key errors (100-104)
        VariableKeyInvalid = 100,
        VariableKeyTooLong = 101,
        VariableKeyInvalidCharacters = 102,
        // Name errors (105-109)
        VariableNameTooLong = 105,
        VariableNameInvalidCharacters = 106,
        // Other variable errors (110-119)
        VariableDoesNotExist = 110,
        VariableDepthTooDeep = 111,
        // Value errors (120+)
        UnsupportedValueType = 120,
        UnsupportedDictionaryKeyType = 121,
        ValueTypeUnkown = 122,

        // 200-299; Errors that are a fault in the client-side code (not user developer error)
        InternalError = 200, // Fault in spatial code
        OperationCancelled = 201,

        // 300-399; Server Errors
        InternalServerError = 300,
    }
}
