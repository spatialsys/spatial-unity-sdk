using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    internal static class JSONSerializer
    {
        #region Serialization

        public static string Serialize(object obj)
        {
            Dictionary<string, string> typeInfo = new();

            JSONObject json = new JSONObject();
            json.Add("data", SerializeObject(obj, typeInfo, ""));
            json.Add("typeInfo", SerializeTypeInfo(typeInfo));

            return json.ToString();
        }

        private static JSONNode SerializeObject(object obj, Dictionary<string, string> typeInfo, string path)
        {
            if (obj == null)
            {
                typeInfo[path] = "String";
                return JSONNull.CreateOrGet();
            }
            else
            {
                typeInfo[path] = obj.GetType().Name;
            }

            if (obj is Dictionary<string, object> dict)
            {
                return SerializeDictionary(dict, typeInfo, path);
            }

            if (obj is string str)
            {
                return new JSONString(str);
            }

            // Value types
            if (obj is bool boolValue)
            {
                return new JSONBool(boolValue);
            }

            if (obj is int || obj is float || obj is double || obj is long || obj is decimal)
            {
                return new JSONNumber(Convert.ToDouble(obj));
            }

            // Unity types
            if (obj is Vector2 v2)
            {
                return CreateNumberArray(v2.x, v2.y);
            }

            if (obj is Vector3 v3)
            {
                return CreateNumberArray(v3.x, v3.y, v3.z);
            }

            if (obj is Vector4 v4)
            {
                return CreateNumberArray(v4.x, v4.y, v4.z, v4.w);
            }

            if (obj is Quaternion q)
            {
                return CreateNumberArray(q.x, q.y, q.z, q.w);
            }

            if (obj is Color c)
            {
                return CreateNumberArray(c.r, c.g, c.b, c.a);
            }

            // System types
            if (obj is DateTime dt)
            {
                return new JSONString(dt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            }

            // Arrays
            if (obj is string[] strArray)
            {
                return CreateArray(strArray, v => new JSONString(v));
            }

            if (obj is bool[] boolArray)
            {
                return CreateArray(boolArray, v => new JSONBool((bool)v));
            }

            if (obj is int[] intArray)
            {
                return CreateArray(intArray, v => new JSONNumber(v));
            }

            if (obj is float[] floatArray)
            {
                return CreateArray(floatArray, v => new JSONNumber(v));
            }

            throw new NotImplementedException($"Serialization of type {obj.GetType()} is not implemented");

        }

        private static JSONArray CreateArray<T>(T[] values, Func<T, JSONNode> serializeFunc)
        {
            JSONArray array = new JSONArray();
            foreach (var value in values)
            {
                array.Add(serializeFunc(value));
            }
            return array;
        }

        private static JSONArray CreateNumberArray(params float[] values)
        {
            return CreateArray(values, v => new JSONNumber(v));
        }

        private static JSONNode SerializeDictionary(Dictionary<string, object> dict, Dictionary<string, string> typeInfo, string path)
        {
            JSONNode json = new JSONObject();
            foreach (var kvp in dict)
            {
                json[kvp.Key] = SerializeObject(kvp.Value, typeInfo, $"{path}/{kvp.Key}");
            }
            return json;
        }

        private static JSONNode SerializeTypeInfo(Dictionary<string, string> dict)
        {
            JSONNode json = new JSONObject();
            foreach (var kvp in dict)
            {
                json[kvp.Key] = kvp.Value;
            }
            return json;
        }

        #endregion Serialization


        #region Desearialization

        public static Dictionary<string, object> Deserialize(string json)
        {
            JSONNode jsonNode = JSON.Parse(json);

            // Create typeinfo structure
            Dictionary<string, string> typeInfo = new();
            foreach (var kvp in jsonNode["typeInfo"])
            {
                typeInfo[kvp.Key] = kvp.Value;
            }

            // Deserialize data
            return DeserializeDictionary(jsonNode["data"], typeInfo, "");
        }

        private static Dictionary<string, object> DeserializeDictionary(JSONNode json, Dictionary<string, string> typeInfo, string path)
        {
            Dictionary<string, object> dict = new();
            foreach (var kvp in json)
            {
                dict[kvp.Key] = DeserializeValue(kvp.Value, typeInfo, $"{path}/{kvp.Key}");
            }
            return dict;
        }

        private static object DeserializeValue(JSONNode json, Dictionary<string, string> typeInfo, string path)
        {
            if (json.IsNull)
                return null;

            if (json.IsObject)
                return DeserializeDictionary(json, typeInfo, path);

            if (json.IsArray)
            {
                switch (typeInfo[path])
                {
                    case "String[]":
                        return DeserializeArray(json, j => j.Value);
                    case "Int32[]":
                        return DeserializeArray(json, j => j.AsInt);
                    case "Single[]":
                        return DeserializeArray(json, j => j.AsFloat);
                    case "Boolean[]":
                        return DeserializeArray(json, j => j.AsBool);
                    case "Quaternion":
                        return new Quaternion(json[0].AsFloat, json[1].AsFloat, json[2].AsFloat, json[3].AsFloat);
                    case "Vector2":
                        return new Vector2(json[0].AsFloat, json[1].AsFloat);
                    case "Vector3":
                        return new Vector3(json[0].AsFloat, json[1].AsFloat, json[2].AsFloat);
                    case "Vector4":
                        return new Vector4(json[0].AsFloat, json[1].AsFloat, json[2].AsFloat, json[3].AsFloat);
                    case "Color":
                        return new Color(json[0].AsFloat, json[1].AsFloat, json[2].AsFloat, json[3].AsFloat);
                }
            }

            if (json.IsString)
            {
                switch (typeInfo[path])
                {
                    case "String":
                        return json.Value;
                    case "DateTime":
                        return DateTime.Parse(json.Value);
                }
            }

            if (json.IsBoolean)
            {
                return json.AsBool;
            }

            if (json.IsNumber)
            {
                switch (typeInfo[path])
                {
                    case "Int32":
                        return json.AsInt;
                    case "Single":
                        return json.AsFloat;
                    case "Double":
                        return json.AsDouble;
                    case "Int64":
                        return json.AsLong;
                }
            }

            // If we're here then we have an unsupported type
            throw new NotImplementedException($"Deserialization of type {json.GetType()} is not implemented. Key: {path}");
        }

        private static T[] DeserializeArray<T>(JSONNode json, Func<JSONNode, T> deserializeFunc)
        {
            T[] array = new T[json.Count];
            for (int i = 0; i < json.Count; i++)
            {
                array[i] = deserializeFunc(json[i]);
            }
            return array;
        }

        #endregion

    }
}