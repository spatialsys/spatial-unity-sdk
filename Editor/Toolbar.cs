using UnityEngine;
using UnityEditor;
using UnityToolbarExtender;

namespace SpatialSys.UnitySDK.Editor
{
    [InitializeOnLoad]
    public class Toolbar
    {
        static Toolbar()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            GUI.skin.button.fontSize = 11;
            GUI.skin.button.fixedHeight = 20;
            GUI.color = Color.white * 0.75f;
            GUI.contentColor = Color.white * 1.19f;

            if (GUILayout.Button(new GUIContent("▶️ Test Space", "Builds the space for testing in the Spatial web app")))
            {
                BuildUtility.BuildForPlaySpace();
            }

            if (GUILayout.Button(new GUIContent("▲ Publish Space", "Uploads all the source assets to Spatial where it will be compiled for all platforms")))
            {
                UnityEditor.EditorUtility.DisplayDialog("Publish Space", "This feature is not yet ready", "OK");
                BuildUtility.PackageForPublishing();
            }

            GUI.color = Color.white;
            GUILayout.Space(10);
        }
    }
}