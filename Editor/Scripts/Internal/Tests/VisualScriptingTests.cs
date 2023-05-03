using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEditor;
using SpatialSys.UnitySDK.VisualScripting;

namespace SpatialSys.UnitySDK.Editor
{
    public class VisualScriptingTests
    {
        [PackageTest]
        //TODO: currently this checks every graph asset in the project.
        //try to check dependencies on a per-package basis
        public static void CheckAllMacroGraphNodesValid(PackageConfig config)
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

        [PackageTest]
        public static void CheckAllDependenciesEmbeddedNodes(PackageConfig config)
        {
            List<GameObject> allDependencies = new List<GameObject>();
            foreach (UnityEngine.Object asset in config.assets)
            {
                allDependencies.AddRange(AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(asset))
                    .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
                    .Where(obj => obj != null));
            }
            foreach (GameObject dependency in allDependencies)
            {
                dependency.GetComponentsInChildren<ScriptMachine>(true).ToList().ForEach((scriptMachine) => {
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
                });
            }
        }

        [SceneTest]
        public static void CheckSceneEmbeddedNodes(Scene scene)
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

        private static List<Type> _blockedPrefabNodes = new List<Type>(){
            typeof(SendNetworkEventByte),
            typeof(SendNetworkEventToActorByteNode),
        };

        [PackageTest(PackageType.PrefabObject)]
        public static void BlockNetworkEventsInPrefabs(PackageConfig config)
        {
            //embedded scripts
            List<GameObject> allDependencies = new List<GameObject>();
            foreach (UnityEngine.Object asset in config.assets)
            {
                allDependencies.AddRange(AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(asset))
                    .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
                    .Where(obj => obj != null));
            }
            foreach (GameObject dependency in allDependencies)
            {
                foreach (ScriptMachine scriptMachine in dependency.GetComponentsInChildren<ScriptMachine>(true))
                {
                    if (scriptMachine.nest.source == GraphSource.Embed && scriptMachine.nest.graph != null)
                    {
                        foreach (IUnit unit in scriptMachine.nest.graph.units)
                        {
                            if (_blockedPrefabNodes.Contains(unit.GetType()))
                            {
                                SpatialValidator.AddResponse(
                                    new SpatialTestResponse(
                                        scriptMachine,
                                        TestResponseType.Fail,
                                        $"Embedded Script Graph on: {scriptMachine.gameObject.name} contains Spatial node that is not supported on prefabs: {unit.GetAnalyticsIdentifier().Identifier}")
                                );
                            }
                        }
                    }
                }
            }

            //script files
            List<ScriptGraphAsset> allScriptGraphAssets = new List<ScriptGraphAsset>();
            foreach (UnityEngine.Object asset in config.assets)
            {
                allScriptGraphAssets.AddRange(AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(asset))
                    .Select(AssetDatabase.LoadAssetAtPath<ScriptGraphAsset>)
                    .Where(obj => obj != null));
            }
            foreach (ScriptGraphAsset scriptGraphAsset in allScriptGraphAssets)
            {
                foreach (IUnit unit in scriptGraphAsset.graph.units)
                {
                    if (_blockedPrefabNodes.Contains(unit.GetType()))
                    {
                        SpatialValidator.AddResponse(
                            new SpatialTestResponse(
                                scriptGraphAsset,
                                TestResponseType.Fail,
                                $"Script Graph {scriptGraphAsset.name} is used in {config.packageName} and contains Spatial node that is not supported on prefabs: {unit.GetAnalyticsIdentifier().Identifier}")
                        );
                    }
                }
            }
        }
    }
}
