using System;
using System.Collections.Generic;
using System.IO;
using SpatialSys.UnitySDK;
using SpatialSys.UnitySDK.Editor;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorUserWorldDataStoreService : IUserWorldDataStoreService
    {
        private DataStoreState _dataStoreState;

        public EditorUserWorldDataStoreService()
        {
            string jsonState = LoadStateFromFile(ProjectConfig.defaultWorldID);
            _dataStoreState = DataStoreState.FromJSON(jsonState);
        }

        //--------------------------------------------------------------------------------------------------------------
        // Serialization
        //--------------------------------------------------------------------------------------------------------------

        private static readonly string DATASTORE_DIRECTORY = $"{Application.dataPath}/Spatial/DataStore";

        private static string GetPath(string worldID)
        {
            string filename = string.IsNullOrEmpty(worldID) ? "default" : worldID;

            return $"{DATASTORE_DIRECTORY}/{filename}.json";
        }

        private static string LoadStateFromFile(string worldID)
        {
            if (string.IsNullOrEmpty(worldID))
            {
                return null;
            }

            if (!Directory.Exists(DATASTORE_DIRECTORY))
            {
                Directory.CreateDirectory(DATASTORE_DIRECTORY);
                AssetDatabase.Refresh();
                return null;
            }

            if (!File.Exists(GetPath(worldID)))
            {
                return null;
            }

            // Read file
            StreamReader reader = new StreamReader(GetPath(worldID));
            var json = reader.ReadToEnd();
            reader.Close();

            return json;
        }

        private static void ClearStateFromFile(string worldID)
        {
            if (string.IsNullOrEmpty(worldID))
            {
                return;
            }

            string path = GetPath(worldID);
            if (File.Exists(path))
            {
                File.Delete(path);

                AssetDatabase.Refresh();
            }
        }

        private static void SaveStateToFile(string worldID, DataStoreState dataStore)
        {
            if (string.IsNullOrEmpty(worldID))
            {
                return;
            }
            string json = dataStore.ToJSON().ToString();

            // Write file
            StreamWriter writer = new StreamWriter(GetPath(worldID));
            writer.Write(json);
            writer.Close();

            AssetDatabase.Refresh();
        }

        //--------------------------------------------------------------------------------------------------------------
        // IUserWorldDataStoreService implementation
        //--------------------------------------------------------------------------------------------------------------

        public DataStoreGetVariableRequest GetVariable(string key, object defaultValue)
        {
            DataStoreGetVariableRequest request = new()
            {
                responseCode = DataStoreResponseCode.Ok,
                succeeded = true,
                value = defaultValue,
            };

            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    request.value = _dataStoreState.root;
                }
                else if (_dataStoreState.TryGetVariable(key, out object variable))
                {
                    request.value = variable;
                }
                else
                {
                    request.succeeded = false;
                    request.responseCode = DataStoreResponseCode.VariableDoesNotExist;
                }
            }
            catch (DataStoreException e)
            {
                Debug.LogError($"Error getting variable: {e.Message}");
                request.responseCode = e.code;
                request.succeeded = false;
            }

            request.InvokeCompletionEvent();
            return request;
        }

        public DataStoreOperationRequest SetVariable(string key, object value)
        {
            DataStoreOperationRequest request = new()
            {
                responseCode = DataStoreResponseCode.Ok,
                succeeded = true
            };

            // Don't allow key to be null or empty, since it overwrite everything and that may not be intentional
            if (string.IsNullOrEmpty(key))
            {
                request.succeeded = false;
                request.responseCode = DataStoreResponseCode.VariableKeyInvalid;
                Debug.LogError($"{nameof(IUserWorldDataStoreService)}: Cannot set DataStore variable when key is empty; ResponseCode: {request.responseCode}");
            }
            else
            {
                try
                {
                    _dataStoreState.SetVariable(key, value);
                    SaveStateToFile(ProjectConfig.defaultWorldID, _dataStoreState);
                }
                catch (DataStoreException e)
                {
                    request.succeeded = false;
                    request.responseCode = ConvertExceptionToCode(e);
                    Debug.LogError($"{nameof(IUserWorldDataStoreService)}: Failed to set DataStore variable '{key}'; ResponseCode: {request.responseCode}");
                }
            }

            request.InvokeCompletionEvent();
            return request;
        }

        public DataStoreOperationRequest DeleteVariable(string key)
        {
            DataStoreOperationRequest request = new()
            {
                responseCode = DataStoreResponseCode.Ok,
                succeeded = true
            };

            // Don't allow key to be null or empty, since it overwrite everything and that may not be intentional
            if (string.IsNullOrEmpty(key))
            {
                request.succeeded = false;
                request.responseCode = DataStoreResponseCode.VariableKeyInvalid;
                Debug.LogError($"{nameof(IUserWorldDataStoreService)}: Cannot delete DataStore variable when key is empty; ResponseCode: {request.responseCode}");
            }
            else
            {
                try
                {
                    _dataStoreState.DeleteVariable(key);
                    SaveStateToFile(ProjectConfig.defaultWorldID, _dataStoreState);
                }
                catch (DataStoreException e)
                {
                    request.succeeded = false;
                    request.responseCode = ConvertExceptionToCode(e);
                    Debug.LogError($"{nameof(IUserWorldDataStoreService)}: Failed to delete DataStore variable '{key}'; ResponseCode: {request.responseCode}");
                }
            }

            request.InvokeCompletionEvent();
            return request;
        }

        public DataStoreHasVariableRequest HasVariable(string key)
        {
            DataStoreHasVariableRequest request = new()
            {
                responseCode = DataStoreResponseCode.Ok,
                succeeded = true,
            };
            try
            {
                request.hasVariable = _dataStoreState.HasVariable(key);
            }
            catch (DataStoreException e)
            {
                request.succeeded = false;
                request.responseCode = ConvertExceptionToCode(e);
                Debug.LogError($"{nameof(IUserWorldDataStoreService)}: Failed to check if DataStore has variable '{key}'; ResponseCode: {request.responseCode}");
            }
            request.InvokeCompletionEvent();
            return request;
        }

        public DataStoreHasAnyVariableRequest HasAnyVariable()
        {
            DataStoreHasAnyVariableRequest request = new()
            {
                responseCode = DataStoreResponseCode.Ok,
                succeeded = true,
                hasAnyVariable = _dataStoreState.HasAnyVariable()
            };
            request.InvokeCompletionEvent();
            return request;
        }

        public DataStoreOperationRequest ClearAllVariables()
        {
            _dataStoreState.Clear();
            ClearStateFromFile(ProjectConfig.defaultWorldID);

            DataStoreOperationRequest request = new()
            {
                responseCode = DataStoreResponseCode.Ok,
            };
            request.InvokeCompletionEvent();
            return request;
        }

        public DataStoreDumpVariablesRequest DumpVariablesAsJSON()
        {
            DataStoreDumpVariablesRequest request = new()
            {
                responseCode = DataStoreResponseCode.Ok,
                json = _dataStoreState.ToJSON().ToString(),
            };
            request.InvokeCompletionEvent();
            return request;
        }

        private DataStoreResponseCode ConvertExceptionToCode(Exception e)
        {
            if (e is DataStoreException dsException)
                return dsException.code;

            return DataStoreResponseCode.InternalError;
        }
    }
}
