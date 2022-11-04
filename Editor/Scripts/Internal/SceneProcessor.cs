using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.Callbacks;
using System;

namespace SpatialSys.UnitySDK
{
    public static class SceneProcessor
    {
        [PostProcessSceneAttribute(0)]
        public static void OnPostprocessScene()
        {
            if (Application.isPlaying ||
                EditorConfig.instance == null ||
                EditorConfig.instance.environmentVariants == null ||
                !Array.Exists(EditorConfig.instance.environmentVariants, (variant) => {
                    SceneAsset asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
                    return asset != null && asset == variant.scene;
                }))
            {
                return;
            }

            //users should not be adding environmentData to their scene. remove any
            EnvironmentData[] dataInScene = GameObject.FindObjectsOfType<EnvironmentData>();
            for (int i = 0; i < dataInScene.Length; i++)
            {
                UnityEngine.Object.DestroyImmediate(dataInScene[i]);
            }

            //add fresh environment data
            EnvironmentData data;
            GameObject g = new GameObject();
            g.name = "EnvironmentData";
            data = g.AddComponent<EnvironmentData>();

            //save components to environmentData
            data.seats = GameObject.FindObjectsOfType<SpatialSeatHotspot>();
            data.entrancePoints = GameObject.FindObjectsOfType<SpatialEntrancePoint>();
            data.triggerEvents = GameObject.FindObjectsOfType<SpatialTriggerEvent>();
            data.emptyFrames = GameObject.FindObjectsOfType<SpatialEmptyFrame>();
            data.avatarTeleporters = GameObject.FindObjectsOfType<SpatialAvatarTeleporter>();

            data.renderingVolumes = GameObject.FindObjectsOfType<UnityEngine.Rendering.Volume>();
        }
    }
}
