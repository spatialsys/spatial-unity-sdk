using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialNetworkVariablesTests
    {
        [ComponentTest(typeof(SpatialNetworkVariables))]
        public static void CheckNetworkVariablesValidity(SpatialNetworkVariables target)
        {
            Variables variables = target.GetComponent<Variables>();

            var matchingDeclarations = variables.declarations.Where(x => target.variableSettings.Any(y => y.name == x.name)).ToArray();

            if (target.variableSettings.Count != matchingDeclarations.Length)
            {
                SpatialNetworkVariables.Data[] nonExisting = target.variableSettings.Where(x => !variables.declarations.Any(y => y.name == x.name)).ToArray();

                if (nonExisting.Length > 0)
                {
                    SpatialValidator.AddResponse(new SpatialTestResponse(
                        target,
                        TestResponseType.Fail,
                        $"Network Variables contains undefined variable: {nonExisting[0].name}",
                        "For network variables to be valid, it must have matching variables defined in the `Variables` component."
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
                        $"Network Variables contains null variable declaration: {declaration.name}",
                        "For network variables to be valid, it must be a `bool`, `byte`, `int`, `float`, `double`, `long`, `string`, `Vector2`, `Vector3`, `Color32`, `int[]` (AotList)."
                    ));
                    continue;
                }

                Type type = Type.GetType(declaration.typeHandle.Identification);
                if (!SpatialNetworkVariablesEditor.IsSyncable(declaration))
                {
                    SpatialValidator.AddResponse(new SpatialTestResponse(
                        target,
                        TestResponseType.Fail,
                        $"Network Variables contains invalid variable type: {type.Name}",
                        "For network variables to be valid, it must be a `bool`, `byte`, `int`, `float`, `double`, `long`, `string`, `Vector2`, `Vector3`, `Color32`, `int[]` (AotList)."
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
                            $"Network Variables contains duplicate variable id: {variable.id} for variable {variable.name}",
                            "For network variables to be valid, each variable must have a unique id. You can fix this by toggling the synced/notsynced button, which will generate a new id."
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