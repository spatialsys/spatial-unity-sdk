using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Reflection;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialNetworkObjectTests
    {
        [ComponentTest(typeof(SpatialNetworkObject))]
        public static void CheckSyncedObjectValidity(SpatialNetworkObject target)
        {
            // If this is on a prefab, check that there is a NetworkObject component on the root object
            if (target.transform.parent != null && target.gameObject.scene.name == null)
            {
                if (target.transform.root.GetComponent<SpatialNetworkObject>() == null)
                {
                    SpatialValidator.AddResponse(new SpatialTestResponse(
                        target,
                        TestResponseType.Fail,
                        "NetworkObject is not on the root object",
                        "For prefabs with network objects to be valid, the SpatialNetworkObject component must be on the root object."
                    ));
                }
            }

            if (target.syncFlags.HasFlag(NetworkObjectSyncFlags.Rigidbody) && target.GetComponent<Rigidbody>() == null)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    target,
                    TestResponseType.Warning,
                    "NetworkObject has Rigidbody sync enabled but no Rigidbody component",
                    "For network objects with Rigidbody sync enabled, a Rigidbody component must be present."
                ));
            }

            if (target.GetComponentInChildren<SpatialSyncedObject>() != null)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    target,
                    TestResponseType.Fail,
                    "NetworkObject contains a SpatialSyncedObject component",
                    "SpatialNetworkObject and SpatialSyncedObject components cannot be used together."
                ));
            }
            else if (target.GetComponentInParent<SpatialSyncedObject>() != null)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    target,
                    TestResponseType.Fail,
                    "NetworkObject contains a SpatialSyncedObject component in parent",
                    "SpatialNetworkObject and SpatialSyncedObject components cannot be used together."
                ));
            }

            // NetworkVariable type support validation
            if (target.behaviours.Length > 0)
            {
                foreach (SpatialNetworkBehaviour behaviour in target.behaviours)
                {
                    if (behaviour == null)
                        continue;

                    FieldInfo[] fields = behaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (FieldInfo fieldInfo in fields)
                    {
                        if (typeof(INetworkVariable).IsAssignableFrom(fieldInfo.FieldType))
                        {
                            Type valueType = fieldInfo.FieldType.GetGenericArguments()[0];
                            if (!INetworkVariable.CURRENTLY_SUPPORTED_TYPES.Contains(valueType))
                            {
                                SpatialValidator.AddResponse(new SpatialTestResponse(
                                    behaviour,
                                    TestResponseType.Fail,
                                    $"NetworkBehavior {behaviour.GetType().Name} contains unsupported NetworkVariable type: {valueType.Name}",
                                    "For NetworkVariables to be valid, it must be a `bool`, `byte`, `int`, `float`, `double`, `long`, `string`, `Vector2`, `Vector3`, `Color32`, or `int[]`."
                                ));
                            }
                        }
                    }
                }
            }
        }
    }
}