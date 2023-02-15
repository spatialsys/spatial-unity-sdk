using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialGUIUtility
    {
        private const string TEXTURE_PATH = "Packages/io.spatial.unitysdk/Editor/Textures";

        public static Texture2D LoadGUITexture(string name, bool hasLightVariant = false)
        {
            if (!EditorGUIUtility.isProSkin && hasLightVariant)
                name = name.Insert(name.LastIndexOf('.'), "-Light");

            string path = System.IO.Path.Combine(TEXTURE_PATH, name);
            Texture2D tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
            return tex;
        }

        public static Sprite LoadSprite(string name)
        {
            string path = System.IO.Path.Combine(TEXTURE_PATH, name);
            return AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
        }
    }
}
