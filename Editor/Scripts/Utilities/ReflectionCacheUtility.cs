using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class ReflectionCacheUtility
    {
        private static Dictionary<Assembly, Type[]> _assemblyToExportedTypes = null;
        public static Type[] GetExportedTypesCached(this Assembly assembly)
        {
            // Dynamically generated assemblies do not support exported types.
            if (assembly.IsDynamic)
                return new Type[0] { };

            if (_assemblyToExportedTypes == null)
                _assemblyToExportedTypes = new Dictionary<Assembly, Type[]>();

            Type[] result;
            if (_assemblyToExportedTypes.TryGetValue(assembly, out Type[] types))
            {
                result = types;
            }
            else
            {
                result = assembly.GetExportedTypes();
                _assemblyToExportedTypes[assembly] = result;
            }

            return result;
        }

        private static Dictionary<string, Type> _fullNameToType = null;
        public static Type GetTypeFromFullNameCached(string fullName)
        {
            if (_fullNameToType == null)
            {
                _fullNameToType = new Dictionary<string, Type>();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (Assembly assembly in assemblies)
                {
                    foreach (Type type in assembly.GetExportedTypesCached())
                    {
                        _fullNameToType[type.FullName] = type;
                    }
                }
            }
            return (_fullNameToType.TryGetValue(fullName, out Type foundType)) ? foundType : null;
        }
    }
}
