using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Linq;

namespace SpatialSys.UnitySDK.Editor
{
    public class VisualScriptingTests
    {
        [SceneTest]
        public static void DenyAllVisualScripts(Scene scene)
        {
            if (SpatialValidator.validationContext != ValidationContext.Publishing)
            {
                return;
            }

            if (GameObject.FindObjectOfType<ScriptMachine>() != null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        null,
                        TestResponseType.Fail,
                        $"Scene {scene.name} contains a ScriptMachine. Visual Scripting is not allowed in published packages yet.",
                        "You will need to remove the script machines from your project, or wait until Visual Scripting is enabled for publishing."
                    )
                );
            }

            if (GameObject.FindObjectOfType<SceneVariables>() != null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        null,
                        TestResponseType.Fail,
                        $"Scene {scene.name} contains SceneVariable. Visual Scripting is not allowed in published packages yet.",
                        "You will need to remove the Scene Variables from your project, or wait until Visual Scripting is enabled for publishing."
                    )
                );
            }
        }

        [PackageTest]
        //TODO: currently this checks every graph asset in the project.
        //try to check dependencies on a per-package basis
        public static void CheckAllNodesValid(PackageConfig config)
        {
            foreach (ScriptGraphAsset graphAsset in EditorUtility.FindAssetsByType<ScriptGraphAsset>())
            {
                foreach (IUnit unit in graphAsset.graph.units)
                {
                    if (!NodeFilter.FilterNode(unit).isAllowed)
                    {
                        SpatialValidator.AddResponse(
                            new SpatialTestResponse(
                                graphAsset,
                                TestResponseType.Fail,
                                $"Script Graph {graphAsset.name} contains unsupported node: {unit.GetAnalyticsIdentifier().Identifier}")
                        );
                    }
                }
            }
        }

        [SceneTest]
        public static void ScanEmbeddedVisualScriptingGraphs(Scene scene)
        {
            var scriptMachines = GameObject.FindObjectsOfType<ScriptMachine>();

            foreach (ScriptMachine scriptMachine in scriptMachines)
            {
                if (scriptMachine.nest.source == GraphSource.Embed && scriptMachine.nest.graph != null)
                {
                    foreach (IUnit unit in scriptMachine.nest.graph.units)
                    {
                        if (!NodeFilter.FilterNode(unit).isAllowed)
                        {
                            SpatialValidator.AddResponse(
                                new SpatialTestResponse(
                                    scriptMachine,
                                    TestResponseType.Fail,
                                    $"Embedded Script Graph on: {scriptMachine.gameObject.name} contains unsupported node: {unit.GetAnalyticsIdentifier().Identifier}")
                            );
                        }
                    }
                }
            }
        }
    }
}
