using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialSyncedVariablesTests
    {
        [ComponentTest(typeof(SpatialSyncedVariables))]
        public static void CheckSyncedVariablesValidity(SpatialSyncedVariables target)
        {
            Variables variables = target.GetComponent<Variables>();

            var matchingDeclarations = variables.declarations.Where(x => target.variableSettings.Any(y => y.name == x.name)).ToArray();

            if (target.variableSettings.Count != matchingDeclarations.Length)
            {
                SpatialSyncedVariables.Data[] nonExisting = target.variableSettings.Where(x => !variables.declarations.Any(y => y.name == x.name)).ToArray();

                if (nonExisting.Length > 0)
                {
                    SpatialValidator.AddResponse(new SpatialTestResponse(
                        target,
                        TestResponseType.Fail,
                        $"Synced Variables contains undefined variable: {nonExisting[0].name}",
                        "For synced variables to be valid, it must have matching variables defined in the `Variables` component."
                    ));
                }
            }

            foreach (var declaration in matchingDeclarations)
            {
                if (declaration.typeHandle.Identification == null)
                {
                    SpatialValidator.AddResponse(new SpatialTestResponse(
                        target,
                        TestResponseType.Fail,
                        $"Synced Variables contains null variable declaration: {declaration.name}",
                        "For synced variables to be valid, it must be a bool, int, float, string, Vector2, or Vector3."
                    ));
                    continue;
                }

                Type type = Type.GetType(declaration.typeHandle.Identification);
                bool invalidType = type != typeof(bool) &&
                    type != typeof(int) &&
                    type != typeof(float) &&
                    type != typeof(string) &&
                    type != typeof(Vector2) &&
                    type != typeof(Vector3);

                if (invalidType)
                {
                    SpatialValidator.AddResponse(new SpatialTestResponse(
                        target,
                        TestResponseType.Fail,
                        $"Synced Variables contains invalid variable type: {type.Name}",
                        "For synced variables to be valid, it must be a bool, int, float, string, Vector2, or Vector3."
                    ));
                }
            }

            // Check that all variables have unique ids
            if (target.version > 0)
            {
                HashSet<byte> ids = new HashSet<byte>();
                foreach (var variable in target.variableSettings)
                {
                    if (ids.Contains(variable.id))
                    {
                        SpatialValidator.AddResponse(new SpatialTestResponse(
                            target,
                            TestResponseType.Fail,
                            $"Synced Variables contains duplicate variable id: {variable.id} for variable {variable.name}",
                            "For synced variables to be valid, each variable must have a unique id. You can fix this by toggling the synced/notsynced button, which will generate a new id."
                        ));
                    }
                    else
                    {
                        ids.Add(variable.id);
                    }
                }
            }
        }
    }
}