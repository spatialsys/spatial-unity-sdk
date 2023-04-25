using UnityEditor.SceneManagement;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class GameObjectTests
    {
        [ComponentTest(typeof(Transform))]
        public static void WarnAgainstCustomTags(Transform target)
        {
            if (!EditorUtility.defaultTags.Contains(target.gameObject.tag))
            {
                SpatialTestResponse resp = new SpatialTestResponse(
                    target,
                    TestResponseType.Warning,
                    "GameObject has a custom tag",
                    "Spatial does not support custom tags."
                );

                resp.SetAutoFix(isSafe: true, "Sets the tag to 'Untagged'",
                    (target) => {
                        Component component = (Component)target;
                        GameObject go = component.gameObject;
                        go.tag = "Untagged";

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
