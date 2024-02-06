using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using UnityEngine.Audio;


namespace SpatialSys.UnitySDK.Editor
{
    public static class AssetMenuItems
    {
        [MenuItem("Assets/Create/Spatial/Spatial Audio Mixer")]
        static void CreateAudioMixer()
        {
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            string pathToCurrentFolder = obj.ToString();
            AssetDatabase.CopyAsset("Packages/io.spatial.unitysdk/Runtime/Assets/DefaultToolkitMixer.mixer", pathToCurrentFolder + "/NewAudioMixer.mixer");
        }
    }
}