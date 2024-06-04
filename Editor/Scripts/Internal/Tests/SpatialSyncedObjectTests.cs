using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialSyncedObjectTests
    {
        [ComponentTest(typeof(SpatialSyncedObject))]
        public static void CheckSyncedObjectValidity(SpatialSyncedObject target)
        {
            if (!target.saveWithSpace) // saving not supported in the new component yet
            {
                var resp = new SpatialTestResponse(
                    target,
                    TestResponseType.Tip,
                    "Synced Objects are deprecated",
                    "We recommend using the new Network Object component instead of Synced Objects."
                );
                resp.SetAutoFix(isSafe: false, "Replace with Network Object. Note that this doesn't modify any Visual Scripting graphs.", t => {
                    (t as SpatialSyncedObject).ConvertToNetworkObject();
                });
                SpatialValidator.AddResponse(resp);
            }

            SpatialSyncedObject[] parentSyncedObjects = target.gameObject.GetComponentsInParent<SpatialSyncedObject>();

            // need to check if greater than one, since it's inclusive of itself
            if (parentSyncedObjects.Length > 1)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    target,
                    TestResponseType.Fail,
                    $"Synced Objects nested under another",
                    "Synced Objects cannot be under the hierarchy of another Synced Object."
                ));
            }
        }
    }
}