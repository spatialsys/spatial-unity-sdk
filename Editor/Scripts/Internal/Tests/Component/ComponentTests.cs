using System;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class ComponentTests
    {
        [ComponentTest(typeof(Component))]
        public static void TestValidComponent(Component target)
        {
            Type targetType = target.GetType();

            // This component is automatically removed during scene build
            if (targetType.IsEditorOnlyType())
                return;

            if (!ValidComponents.IsComponentTypeAllowedForPackageType(ProjectConfig.activePackage, targetType))
            {
                // Maybe do some type specific messages. For example reasure people that we have an event system active etc.
                SpatialTestResponse resp = new SpatialTestResponse(
                    target,
                    TestResponseType.Fail,
                    "Object has unsupported component type: " + targetType.ToString(),
                    "Certain components are not allowed. To fix this error remove the offending component from the object."
                );

                resp.SetAutoFix(isSafe: false, "Deletes the component, as well as any other components that depend on it, from the offending object",
                    (target) => {
                        Component component = (Component)target;
                        GameObject go = component.gameObject;
                        EditorUtility.RemoveComponentAndDependents(component);

                        UnityEditor.Selection.activeGameObject = go;
                        UnityEditor.EditorUtility.SetDirty(go);
                        EditorSceneManager.SaveOpenScenes();
                    }
                );

                SpatialValidator.AddResponse(resp);
            }
        }
    }
}
