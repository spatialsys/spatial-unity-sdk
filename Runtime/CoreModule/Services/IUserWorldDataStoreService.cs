
namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// This service provides access to the <c>Users</c> datastore for the current <c>world</c>. Spaces that belong to
    /// the same <c>world</c> share the same user world datastore.
    /// </summary>
    public interface IUserWorldDataStoreService
    {
        /// <summary>
        /// Retreives the value of a variable at the given key.
        /// </summary>
        /// <remarks>
        /// The key can be a <c>/</c> separated path to search through dictionaries. For example, the key <c>"player/inventory/loot"</c> will return the variable found at <c>dataStore["player"]["inventory"]["loot"]</c>.
        /// </remarks>
        /// <returns><c>Async</c> Returns the the current dataStore value if it exists, otherwise returns the default value provided.</returns>
        DataStoreGetVariableRequest GetVariable(string key, object defaultValue);

        /// <summary>
        /// Set the value of a variable at the given key.
        /// </summary>
        /// <remarks>
        /// The key can be a <c>/</c> separated path to set the value in a nested dictionary. For example, the key <c>"player/inventory/loot"</c> will set the variable found at <c>dataStore["player"]["inventory"]["loot"]</c>.
        /// </remarks>
        /// <returns><c>Async</c> Returns a boolean <c>succeeded</c> if the variable was succesfully set or not.</returns>
        DataStoreOperationRequest SetVariable(string key, object value);

        /// <summary>
        /// Delete the variable at the given key.
        /// </summary>
        /// <remarks>
        /// The key can be a <c>/</c> separated path to delete a variable in a nested dictionary. For example, the key <c>"player/inventory"</c> will delete the variable found at <c>dataStore["player"]["inventory"]</c> including any child variables.
        /// </remarks>
        /// <returns><c>Async</c> Returns a boolean <c>succeeded</c> if the variable was succesfully deleted or not.</returns>
        DataStoreOperationRequest DeleteVariable(string key);

        /// <summary>
        /// Checks if a variable exists at the given key.
        /// </summary>
        /// <remarks>
        /// The key can be a <c>/</c> separated path to find a variable in a nested dictionary. For example, the key <c>"player/inventory/loot"</c> will check if a variable exists at <c>dataStore["player"]["inventory"]</c>.
        /// </remarks>
        /// <returns><c>Async</c> Returns a boolean <c>hasVariable</c> if the variable exists or not.</returns>
        DataStoreHasVariableRequest HasVariable(string key);

        /// <summary>
        /// Checks if the dataStore has any variables.
        /// </summary>
        /// <returns><c>Async</c> Returns a boolean <c>hasAnyVariable</c> if any variable exists or not.</returns>
        DataStoreHasAnyVariableRequest HasAnyVariable();

        /// <summary>
        /// Delete all variables in the dataStore.
        /// </summary>
        /// <returns><c>Async</c> Returns a boolean <c>succeeded</c> if the operation was successful.</returns>
        DataStoreOperationRequest ClearAllVariables();

        /// <summary>
        /// Dump all variables as a json string. Useful for debugging.
        /// </summary>
        /// <returns><c>Async</c> Returns the current contents of the dataStore as a JSON string.</returns>
        DataStoreDumpVariablesRequest DumpVariablesAsJSON();
    }

    public class DataStoreOperationRequest : SpatialAsyncOperation
    {
        public bool succeeded;
        public DataStoreResponseCode responseCode;
    }

    public class DataStoreGetVariableRequest : DataStoreOperationRequest
    {
        public object value;
    }

    public class DataStoreHasVariableRequest : DataStoreOperationRequest
    {
        public bool hasVariable;
    }

    public class DataStoreHasAnyVariableRequest : DataStoreOperationRequest
    {
        public bool hasAnyVariable;
    }

    public class DataStoreDumpVariablesRequest : DataStoreOperationRequest
    {
        public string json;
    }
}