using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorLoggingService : ILoggingService
    {
        private string DictionaryToString(Dictionary<string, object> data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            string result = string.Empty;
            foreach (KeyValuePair<string, object> kvp in data)
            {
                result += $"{kvp.Key}: {kvp.Value}\n";
            }
            return result;
        }

        public void LogError(string message, Exception exception = null, Dictionary<string, object> data = null)
        {
            if (data != null)
            {
                message += "\n" + DictionaryToString(data);
            }
            Debug.LogError(message);
        }

        public void LogError(Exception exception, Dictionary<string, object> data = null)
        {
            string message = exception.Message;
            if (data != null)
            {
                message += "\n" + DictionaryToString(data);
            }
            Debug.LogError(message);
        }
    }
}