using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// This service provides access to the <c>Users</c> datastore for the current <c>world</c>. Spaces that belong to
    /// the same <c>world</c> share the same user world datastore.
    /// </summary>
    /// <remarks>
    /// <code source="Services/DataStoreExamples.cs" region="DataStoreMainExample" lang="csharp"></code>
    /// </remarks>
    [DocumentationCategory("Services/User World Data Store Service")]
    public interface IUserWorldDataStoreService
    {
        /// <summary>
        /// Retrieves the value of a variable at the given key.
        /// </summary>
        /// <remarks>
        /// The key can be a <c>/</c> separated path to search through dictionaries. For example, the key <c>"player/inventory/loot"</c> will return the variable found at <c>dataStore["player"]["inventory"]["loot"]</c>.
        /// </remarks>
        /// <returns><c>Async</c> Returns the the current dataStore value if it exists, otherwise returns the default value provided.</returns>
        /// <example><code source="Services/DataStoreExamples.cs" region="DataStoreMainExample" lang="csharp"/></example>
        DataStoreGetVariableRequest GetVariable(string key, object defaultValue);

        /// <summary>
        /// Set the value of a variable at the given key.
        /// </summary>
        /// <remarks>
        /// The key can be a <c>/</c> separated path to set the value in a nested dictionary. For example, the key <c>"player/inventory/loot"</c> will set the variable found at <c>dataStore["player"]["inventory"]["loot"]</c>.
        /// </remarks>
        /// <returns><c>Async</c> Returns a boolean <c>succeeded</c> if the variable was successfully set or not.</returns>
        /// <example><code source="Services/DataStoreExamples.cs" region="DataStoreMainExample" lang="csharp"/></example>
        DataStoreOperationRequest SetVariable(string key, object value);

        /// <summary>
        /// Delete the variable at the given key.
        /// </summary>
        /// <remarks>
        /// The key can be a <c>/</c> separated path to delete a variable in a nested dictionary. For example, the key <c>"player/inventory"</c> will delete the variable found at <c>dataStore["player"]["inventory"]</c> including any child variables.
        /// </remarks>
        /// <returns><c>Async</c> Returns a boolean <c>succeeded</c> if the variable was successfully deleted or not.</returns>
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

    /// <summary>
    /// Operation to be performed on the data store.
    /// </summary>
    [DocumentationCategory("Services/User World Data Store Service")]
    public class DataStoreOperationRequest : SpatialAsyncOperation
    {
        /// <summary>
        /// Whether the operation succeeded or not.
        /// </summary>
        public bool succeeded;
        /// <summary>
        /// The response code for the operation.
        /// </summary>
        public DataStoreResponseCode responseCode;
    }

    /// <summary>
    /// Request to get a variable from the data store.
    /// </summary>
    [DocumentationCategory("Services/User World Data Store Service")]
    public class DataStoreGetVariableRequest : DataStoreOperationRequest
    {
        /// <summary>
        /// The value of the variable.
        /// </summary>
        public object value;

        #region Primitives
        /// <summary>
        /// The value as a string.
        /// </summary>
        public string stringValue => value as string;
        /// <summary>
        /// The value as a boolean.
        /// </summary>
        public bool boolValue => value is bool parsedValue ? parsedValue : default;
        /// <summary>
        /// The value as an integer.
        /// </summary>
        public int intValue => value is int parsedValue ? parsedValue : default;
        /// <summary>
        /// The value as a float.
        /// </summary>
        public float floatValue => value is float parsedValue ? parsedValue : default;
        /// <summary>
        /// The value as a double.
        /// </summary>
        public double doubleValue => value is double parsedValue ? parsedValue : default;
        /// <summary>
        /// The value as a long.
        /// </summary>
        public long longValue => value is long parsedValue ? parsedValue : default;
        /// <summary>
        /// The value as a decimal.
        /// </summary>
        public decimal decimalValue => value is decimal parsedValue ? parsedValue : default;
        #endregion

        #region Unity types
        /// <summary>
        /// The value as a Vector2.
        /// </summary>
        public Vector2 vector2Value => value is Vector2 parsedValue ? parsedValue : default;
        /// <summary>
        /// The value as a Vector3.
        /// </summary>
        public Vector3 vector3Value => value is Vector3 parsedValue ? parsedValue : default;
        /// <summary>
        /// The value as a Vector4.
        /// </summary>
        public Vector4 vector4Value => value is Vector4 parsedValue ? parsedValue : default;
        /// <summary>
        /// The value as a Quaternion.
        /// </summary>
        public Quaternion quaternionValue => value is Quaternion parsedValue ? parsedValue : default;
        /// <summary>
        /// The value as a Color.
        /// </summary>
        public Color colorValue => value is Color parsedValue ? parsedValue : default;
        #endregion

        #region System types
        /// <summary>
        /// The value as a DateTime.
        /// </summary>
        public DateTime dateTimeValue => value is DateTime parsedValue ? parsedValue : default;
        #endregion

        #region Primitive arrays
        /// <summary>
        /// The value as a string array.
        /// </summary>
        public string[] stringArrayValue => value as string[];
        /// <summary>
        /// The value as an integer array.
        /// </summary>
        public int[] intArrayValue => value as int[];
        /// <summary>
        /// The value as a boolean array.
        /// </summary>
        public bool[] boolArrayValue => value as bool[];
        /// <summary>
        /// The value as a float array.
        /// </summary>
        public float[] floatArrayValue => value as float[];
        #endregion

        #region Nested variables (object)
        /// <summary>
        /// The value as a dictionary.
        /// </summary>
        public Dictionary<string, object> dictionaryValue => value as Dictionary<string, object>;
        #endregion
    }

    /// <summary>
    /// Request to check if a variable exists in the data store.
    /// </summary>
    [DocumentationCategory("Services/User World Data Store Service")]
    public class DataStoreHasVariableRequest : DataStoreOperationRequest
    {
        /// <summary>
        /// Whether the variable exists or not.
        /// </summary>
        public bool hasVariable;
    }

    /// <summary>
    /// Request to check if the data store has any variables.
    /// </summary>
    [DocumentationCategory("Services/User World Data Store Service")]
    public class DataStoreHasAnyVariableRequest : DataStoreOperationRequest
    {
        /// <summary>
        /// Whether the data store has any variables or not.
        /// </summary>
        public bool hasAnyVariable;
    }

    /// <summary>
    /// Request to dump all variables in the data store as a JSON string.
    /// </summary>
    [DocumentationCategory("Services/User World Data Store Service")]
    public class DataStoreDumpVariablesRequest : DataStoreOperationRequest
    {
        /// <summary>
        /// The current contents of the data store as a JSON string.
        /// </summary>
        public string json;
    }
}