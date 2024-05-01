using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
// using Newtonsoft.Json;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    internal class DataStoreException : Exception
    {
        public DataStoreResponseCode code;
        public DataStoreException(DataStoreResponseCode code, string message) : base(message)
        {
            this.code = code;
        }
    }

    internal class DataStoreState
    {
        public const char PATH_SEPARATOR = '/';
        public const char PATH_SEPARATOR_TYPEINFO = '.';
        public const int MAX_VARIABLE_NESTED_DEPTH = 10;
        public const int MAX_VARIABLE_NAME_LENGTH = 64;
        public const int MAX_PATH_LENGTH = 400;

        public static readonly Regex VALID_VARIABLE_NAME_REGEX = new Regex(@"^(?![0-9])((?!__).)[\w]*$(?<!__)$"); // a-z, A-Z, 0-9, _ and not starting with a number, and not starting or ending with __
        public static readonly Regex VALID_PATH_REGEX = new Regex(@"^[\w/]+$");

        public static readonly HashSet<Type> SUPPORTED_TYPES = new HashSet<Type> {
            typeof(string),
            typeof(bool),
            typeof(int),
            typeof(float),
            typeof(double),
            typeof(long),
            typeof(decimal),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(Color),
            typeof(DateTime),
            typeof(string[]),
            typeof(bool[]),
            typeof(int[]),
            typeof(float[]),
            typeof(Dictionary<string, object>), // object value should also match one of the supported types
        };

        private Dictionary<string, object> _root = new Dictionary<string, object>();

        public Dictionary<string, object> root => _root;

        private string[] ValidatePath(string relativePath)
        {
            // Misc validity checks
            if (string.IsNullOrEmpty(relativePath))
                throw new DataStoreException(DataStoreResponseCode.VariableKeyInvalid, $"Variable key is null or empty");
            if (relativePath.Length > MAX_PATH_LENGTH)
                throw new DataStoreException(DataStoreResponseCode.VariableKeyTooLong, $"Variable key exceeds maximum length of {MAX_PATH_LENGTH}");
            if (!VALID_PATH_REGEX.IsMatch(relativePath))
                throw new DataStoreException(DataStoreResponseCode.VariableKeyInvalidCharacters, $"Variable key contains invalid characters");

            string[] pathParts = relativePath.Trim(PATH_SEPARATOR).Split(PATH_SEPARATOR);

            // Misc validity checks
            if (pathParts.Length > MAX_VARIABLE_NESTED_DEPTH)
                throw new DataStoreException(DataStoreResponseCode.VariableDepthTooDeep, $"Variable exceeds maximum nested depth of {MAX_VARIABLE_NESTED_DEPTH}");
            if (pathParts.Any(part => part.Length == 0))
                throw new DataStoreException(DataStoreResponseCode.VariableKeyInvalid, $"Variable key contains empty parts");
            if (pathParts.Any(pathParts => pathParts.Length > MAX_VARIABLE_NAME_LENGTH))
                throw new DataStoreException(DataStoreResponseCode.VariableNameTooLong, $"Variable name exceeds maximum allowed length of {MAX_VARIABLE_NAME_LENGTH} characters");

            return pathParts;
        }

        public bool TryGetVariable(string relativePath, out object variable)
        {
            string[] pathParts = ValidatePath(relativePath);

            object currentObj = _root;
            foreach (string path in pathParts)
            {
                currentObj = FindChild(currentObj, path);
                if (currentObj == null)
                    break;
            }
            variable = DeepCopy(currentObj);
            return variable != null;
        }

        private object FindChild(object parent, string name)
        {
            var parentDict = parent as Dictionary<string, object>;

            if (parentDict != null && parentDict.TryGetValue(name, out object child))
                return child;

            return null;
        }

        private object DeepCopy(object obj)
        {
            if (obj == null)
                return null;

            Type objType = obj.GetType();

            if (objType == typeof(string) || objType == typeof(bool) || objType == typeof(int) || objType == typeof(float) || objType == typeof(double) || objType == typeof(long) || objType == typeof(decimal))
            {
                return obj;
            }
            else if (objType == typeof(Vector2) || objType == typeof(Vector3) || objType == typeof(Vector4) || objType == typeof(Quaternion) || objType == typeof(Color) || objType == typeof(DateTime))
            {
                return obj;
            }
            else if (objType == typeof(string[]))
            {
                return ((string[])obj).ToArray();
            }
            else if (objType == typeof(bool[]))
            {
                return ((bool[])obj).ToArray();
            }
            else if (objType == typeof(int[]))
            {
                return ((int[])obj).ToArray();
            }
            else if (objType == typeof(float[]))
            {
                return ((float[])obj).ToArray();
            }
            else if (objType == typeof(Dictionary<string, object>))
            {
                Dictionary<string, object> dict = (Dictionary<string, object>)obj;
                Dictionary<string, object> newDict = new Dictionary<string, object>();
                foreach (var kvp in dict)
                {
                    newDict[kvp.Key] = DeepCopy(kvp.Value);
                }
                return newDict;
            }
            else
            {
                throw new DataStoreException(DataStoreResponseCode.UnsupportedValueType, $"The provided value type is not supported");
            }
        }

        public void SetVariable(string path, object value)
        {
            // Analyze variable value
            Type valueType = value?.GetType();
            if (valueType != null && !SUPPORTED_TYPES.Contains(valueType))
            {
                throw new DataStoreException(DataStoreResponseCode.UnsupportedValueType, $"The provided value type is not supported");
            }

            // object existingValue;
            // if (TryGetChildVariable(path, out existingValue))
            // {
            //     if (existingValue.GetType() != valueType)
            //     {
            //         throw new DataStoreException(DataStoreResponseCode.UnsupportedValueType, $"The provided value type does not match the existing value type");
            //     }
            // }

            string[] pathParts = ValidatePath(path);

            Dictionary<string, object> currentDict = _root;

            // Make sure all parent paths are dictionaries
            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                if (currentDict.ContainsKey(pathParts[i]) && currentDict[pathParts[i]] is Dictionary<string, object> dict)
                {
                    currentDict = dict;
                }
                else
                {
                    Dictionary<string, object> newDict = new();
                    currentDict[pathParts[i]] = newDict;
                    currentDict = newDict;
                }
            }

            // Assign value
            currentDict[pathParts[pathParts.Length - 1]] = value;
        }

        public void DeleteVariable(string path)
        {
            string[] pathParts = ValidatePath(path);

            Dictionary<string, object> currentDict = _root;

            // Make sure all parent paths are dictionaries
            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                if (currentDict.ContainsKey(pathParts[i]) && currentDict[pathParts[i]] is Dictionary<string, object> dict)
                {
                    currentDict = dict;
                }
                else
                {
                    return;
                }
            }

            // Delete value
            currentDict.Remove(pathParts[pathParts.Length - 1]);
        }

        public bool HasVariable(string path)
        {
            return TryGetVariable(path, out _);
        }

        public bool HasAnyVariable()
        {
            return _root.Count > 0;
        }

        public void Clear()
        {
            _root.Clear();
        }

        //--------------------------------------------------------------------------------------------------------------
        // JSON Serialization
        //--------------------------------------------------------------------------------------------------------------

        public string ToJSON()
        {
            return JSONSerializer.Serialize(_root);
        }

        public static DataStoreState FromJSON(string json)
        {
            DataStoreState dataStore = new();

            if (!string.IsNullOrEmpty(json))
            {
                dataStore._root = JSONSerializer.Deserialize(json);
            }
            return dataStore;
        }
    }
}