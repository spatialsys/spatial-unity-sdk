using System;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class ComponentTests
    {
        [ComponentTest(typeof(Component))]
        public static void TestValidComponent(Component target)
        {
            Type targetType = target.GetType();

            // Ignore this component if it has a [EditorOnly] attribute. These are automatically removed during scene build
            if (targetType.GetCustomAttributes(typeof(EditorOnlyAttribute), true).Length > 0)
                return;

            if (!ValidComponents.IsComponentTypeAllowedForPackageType(ProjectConfig.activePackage.packageType, targetType))
            {
                // Maybe do some type specific messages. For example reasure people that we have an event system active etc.
                SpatialTestResponse resp = new SpatialTestResponse(
                    target,
                    TestResponseType.Fail,
                    "Object has unsupported component type: " + targetType.ToString(),
                    "Certain components are not allowed. To fix this error remove the offending component from the object."
                );

                // TODO: not including this fix right now because it can fail if a component has a requirement (inputModule requires EventSystem etc.)
                /*
                resp.SetAutoFix(isSafe: false, "Deletes the component from the offending gameObject.",
                    (target) => {
                        GameObject g = ((Component)target).gameObject;
                        GameObject.DestroyImmediate(target);
                        //target will be null so we need to select an mark dirty here.
                        UnityEditor.Selection.activeObject = g;
                        UnityEditor.EditorUtility.SetDirty(g);
                        EditorSceneManager.SaveOpenScenes();
                    }
                );
                */

                SpatialValidator.AddResponse(resp);
            }
        }
    }
}
