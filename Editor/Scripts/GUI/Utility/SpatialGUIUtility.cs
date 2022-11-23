using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialGUIUtility
    {
        public static Texture2D LoadGUITexture(string name, bool hasLightVariant = false)
        {
            if (!EditorGUIUtility.isProSkin && hasLightVariant)
                name = name.Insert(name.LastIndexOf('.'), "-Light");

            string path = System.IO.Path.Combine("Packages/io.spatial.unitysdk/Editor/Textures", name);
            Texture2D tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
            return tex;
        }

        //--------------------------------------------------------------------------------------------------------------
        // Auto Foldout
        //--------------------------------------------------------------------------------------------------------------

        private static Dictionary<string, bool> _foldoutLookup = new Dictionary<string, bool>();

        public static void AutoFoldout(string menuID, string title, System.Action drawFunc, bool savePref = false, bool openByDefault = true)
        {
            menuID = "AUTO_FOLDOUT_" + menuID;
            bool isOpen = openByDefault;

            // Load from prefs or local
            if (savePref)
            {
                if (EditorPrefs.HasKey(menuID))
                {
                    isOpen = EditorPrefs.GetBool(menuID);
                }
                else
                {
                    EditorPrefs.SetBool(menuID, isOpen);
                }
            }
            else
            {
                if (_foldoutLookup.ContainsKey(menuID))
                {
                    isOpen = _foldoutLookup[menuID];
                }
                else
                {
                    _foldoutLookup.Add(menuID, isOpen);
                }
            }

            // Toolbar button
            GUILayout.BeginHorizontal();
            isOpen = GUILayout.Toggle(isOpen, title, Styles.foldoutHeader);
            GUILayout.EndHorizontal();

            // Save pref
            if (savePref)
            {
                EditorPrefs.SetBool(menuID, isOpen);
            }
            else
            {
                _foldoutLookup[menuID] = isOpen;
            }

            // Draw box for foldout
            if (!isOpen)
                return;

            // Draw the layout
            GUILayout.BeginHorizontal(Styles.foldoutContentBox);
            GUILayout.BeginVertical();
            drawFunc();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        public static class Styles
        {
            public static GUIStyle foldoutHeader;
            public static GUIStyle foldoutContentBox;

            static Styles()
            {
                // This is kind of odd. All the dark skins are in EditorSkin.Scene
                //  and the lighter versions are in EditorSkin.Inspector
                GUISkin skin;
                if (EditorGUIUtility.isProSkin)
                {
                    skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
                }
                else
                {
                    skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
                }

                // Auto Foldout
                foldoutHeader = EditorStyles.toolbarButton;
                foldoutContentBox = new GUIStyle();
                foldoutContentBox.border = new RectOffset(3, 3, 2, 2);
                foldoutContentBox.normal.background = SpatialGUIUtility.LoadGUITexture("GUI/FoldoutContentBox.png");
                foldoutContentBox.margin = new RectOffset(0, 0, 0, 0);
                foldoutContentBox.padding = new RectOffset(0, 0, 4, 4);
            }
        }
    }
}
