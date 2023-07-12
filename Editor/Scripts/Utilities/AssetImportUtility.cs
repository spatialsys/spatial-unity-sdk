using System.IO;
using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public class AssetImportProcessor : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            AssetImportUtility.TextureImportProcess((TextureImporter)assetImporter, assetPath);
        }

        // Use OnPostprocessAudio instead of OnPreprocessAudio, because we can't know the length of the audio file in OnPreprocessAudio
        private void OnPostprocessAudio(AudioClip audio)
        {
            if (assetImporter.importSettingsMissing) // Skip if it's already imported and has settings
            {
                AssetImportUtility.AudioImportProcess((AudioImporter)assetImporter, audio);
            }
        }

        private void OnPreprocessModel()
        {
            if (assetImporter.importSettingsMissing) // Skip if it's already imported and has settings
            {
                AssetImportUtility.ModelImportProcess((ModelImporter)assetImporter);
            }
        }
    }

    public class AssetImportUtility
    {
        [MenuItem("Spatial SDK/Utilities/Optimize Assets")]
        public static void OptimizeAllAssets()
        {
            bool confirmedDialog = UnityEditor.EditorUtility.DisplayDialog(
                title: "Optimize Assets: Change Compression Settings",
                message: "By optimizing asset compression settings, the build size can be reduced. Please note that this may result in visual changes if specific settings were previously used.",
                ok: "Apply",
                cancel: "Cancel"
            );

            if (confirmedDialog)
            {
                ReimportAssets("Assets");
            }
        }

        [MenuItem("Spatial SDK/Utilities/Optimize Assets (Folder)")]
        public static void OptimizeAssetsInFolder()
        {
            string path = UnityEditor.EditorUtility.OpenFolderPanel("Select Folder", "", "");
            if (path.Length != 0)
            {
                ReimportAssets(RemoveBefore(path, "Assets"));
            }
        }

        private static void ReimportAssets(string folder)
        {
            UnityEditor.EditorUtility.DisplayProgressBar("Reimporting assets, please wait...", "", 1f);

            // Textures
            // "keyword l: <label> t: <type>"
            // https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
            string[] textureGuids = AssetDatabase.FindAssets("t: texture2D", new[] { folder });
            foreach (string guid in textureGuids)
            {
                try
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    string extension = Path.GetExtension(assetPath);
                    if (extension == ".ttf" || extension == ".otf" || extension == ".asset") // Exclude font assets
                    {
                        continue;
                    }
                    TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                    TextureImportProcess(textureImporter, assetPath, forceProcess: true);
                    UnityEditor.EditorUtility.SetDirty(textureImporter);
                    textureImporter.SaveAndReimport();
                }
                catch (System.Exception e)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    Debug.LogError($"Error while reimporting texture ({assetPath}): " + e.Message);
                }
            }

            // Audios
            string[] audioGuids = AssetDatabase.FindAssets("t: audioClip", new[] { folder });
            foreach (string guid in audioGuids)
            {
                try
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    AudioImporter audioImporter = (AudioImporter)AudioImporter.GetAtPath(assetPath);
                    AudioClip audioClip = (AudioClip)AssetDatabase.LoadAssetAtPath(assetPath, typeof(AudioClip));
                    AudioImportProcess(audioImporter, audioClip);
                    UnityEditor.EditorUtility.SetDirty(audioImporter);
                    audioImporter.SaveAndReimport();
                }
                catch (System.Exception e)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    Debug.LogError($"Error while reimporting audio clip ({assetPath}): " + e.Message);
                }
            }

            // Models
            string[] modelGuids = AssetDatabase.FindAssets("t: GameObject", new[] { folder });
            foreach (string guid in modelGuids)
            {
                try
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    string extension = Path.GetExtension(assetPath);
                    if (extension == ".prefab") // Exclude prefabs
                    {
                        continue;
                    }
                    ModelImporter modelImporter = (ModelImporter)ModelImporter.GetAtPath(assetPath);

                    ModelImportProcess(modelImporter);
                    UnityEditor.EditorUtility.SetDirty(modelImporter);
                    modelImporter.SaveAndReimport();
                }
                catch (System.Exception e)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    Debug.LogError($"Error while reimporting model ({assetPath}): " + e.Message);
                }
            }

            UnityEditor.EditorUtility.ClearProgressBar();
        }

        private static string RemoveBefore(string value, string character)
        {
            int index = value.IndexOf(character);
            if (index > 0)
            {
                value = value.Substring(index);
            }
            return value;
        }

        public static void TextureImportProcess(TextureImporter textureImporter, string assetPath, bool forceProcess = false)
        {
            string assetPathLower = assetPath.ToLowerInvariant();
            bool isNormalMap = assetPathLower.Contains("_norm") || assetPathLower.Contains("_normal");
            bool isDataMap = assetPathLower.Contains("_metal") || assetPathLower.Contains("_metallic") || assetPathLower.Contains("_smoothness") || assetPathLower.Contains("_roughness") || assetPathLower.Contains("_ao");
            bool isLightmap = assetPathLower.Contains("lightmap") && assetPathLower.Contains("_light");
            bool isShadowMask = assetPathLower.Contains("lightmap") && assetPathLower.Contains("_shadowmask");
            bool isReflectionProbe = assetPathLower.Contains("reflectionprobe") || assetPathLower.Contains("reflection probe");

            string extension = Path.GetExtension(assetPathLower);
            bool isHDR = extension == ".hdr" || extension == ".exr";

            bool isSingleChannel = false;
            if (!textureImporter.importSettingsMissing)
            {
                isNormalMap |= textureImporter.textureType == TextureImporterType.NormalMap;
                isLightmap |= textureImporter.textureType == TextureImporterType.Lightmap;
                isShadowMask |= textureImporter.textureType == TextureImporterType.Shadowmask;
                isSingleChannel |= textureImporter.textureType == TextureImporterType.SingleChannel;
            }

            // Unity use alpha to encode HDR data
            bool needAlpha =
                textureImporter.DoesSourceTextureHaveAlpha() ||
                textureImporter.alphaSource == TextureImporterAlphaSource.FromGrayScale ||
                isNormalMap || isLightmap || isReflectionProbe || isHDR;

            TextureImporterFormat textureFormat = needAlpha ? TextureImporterFormat.DXT5Crunched : TextureImporterFormat.DXT1Crunched;
            // The size of CompressedDXT5 is smaller than CrunchedDXT5 for ReflectionProbe images for some reason
            if (isReflectionProbe)
            {
                textureFormat = TextureImporterFormat.DXT5;
            }
            if (isSingleChannel)
            {
                textureFormat = TextureImporterFormat.BC4; // Use RCompressedBC4 for SingleChannel
            }
            // Use ASTC12x12 for shadow mask. It's not very noticeable
            TextureImporterFormat textureFormatMobile = isShadowMask ? TextureImporterFormat.ASTC_12x12 : TextureImporterFormat.ASTC_8x8;

            int maxTextureSize = (isLightmap || isShadowMask || isHDR) ? 2048 : 1024; // HDR could be skybox

            // Set settings if it's never imported before
            if (textureImporter.importSettingsMissing || forceProcess)
            {
                textureImporter.textureCompression = TextureImporterCompression.CompressedLQ;
                textureImporter.crunchedCompression = true;
                textureImporter.compressionQuality = 100;
                textureImporter.isReadable = false;

                if (isNormalMap)
                {
                    textureImporter.textureType = TextureImporterType.NormalMap;
                }
                else if (isLightmap)
                {
                    textureImporter.textureType = TextureImporterType.Lightmap;
                }
                else if (isShadowMask)
                {
                    textureImporter.textureType = TextureImporterType.Shadowmask;
                }
                else if (isReflectionProbe)
                {
                    textureImporter.textureType = TextureImporterType.Default;
                    textureImporter.textureShape = TextureImporterShape.TextureCube;
                }

                if (isDataMap)
                {
                    textureImporter.sRGBTexture = false;
                }

                textureImporter.SetPlatformTextureSettings(GetTextureImporterSettings("Standalone", maxTextureSize, textureFormat));
                textureImporter.SetPlatformTextureSettings(GetTextureImporterSettings("WebGL", maxTextureSize, textureFormat));
                textureImporter.SetPlatformTextureSettings(GetTextureImporterSettings("Android", maxTextureSize, textureFormatMobile));
                textureImporter.SetPlatformTextureSettings(GetTextureImporterSettings("iPhone", maxTextureSize, textureFormatMobile));
            }
            // If it's already imported but doesn't have compression settings, set setting
            else
            {
                if (!textureImporter.GetPlatformTextureSettings("Standalone", out int _, out TextureImporterFormat _, out int _, out bool _))
                {
                    textureImporter.SetPlatformTextureSettings(GetTextureImporterSettings("Standalone", maxTextureSize, textureFormat));
                }
                if (!textureImporter.GetPlatformTextureSettings("WebGL", out int _, out TextureImporterFormat _, out int _, out bool _))
                {
                    textureImporter.SetPlatformTextureSettings(GetTextureImporterSettings("WebGL", maxTextureSize, textureFormat));
                }
                if (!textureImporter.GetPlatformTextureSettings("Android", out int _, out TextureImporterFormat _, out int _, out bool _))
                {
                    textureImporter.SetPlatformTextureSettings(GetTextureImporterSettings("Android", maxTextureSize, textureFormatMobile));
                }
                if (!textureImporter.GetPlatformTextureSettings("iPhone", out int _, out TextureImporterFormat _, out int _, out bool _))
                {
                    textureImporter.SetPlatformTextureSettings(GetTextureImporterSettings("iPhone", maxTextureSize, textureFormatMobile));
                }
            }
        }

        private static TextureImporterPlatformSettings GetTextureImporterSettings(string platformName, int maxTextureSize, TextureImporterFormat textureFormat)
        {
            return new TextureImporterPlatformSettings() {
                name = platformName,
                overridden = true,
                maxTextureSize = maxTextureSize,
                format = textureFormat,
                compressionQuality = 100
            };
        }

        public static void AudioImportProcess(AudioImporter audioImporter, AudioClip audio)
        {
            // This will half the size of the audio file
            // In most of cases, this is not noticeable
            audioImporter.forceToMono = true;

            // Unity won't wait for sounds to be fully loaded before starting the scene and this will improve the scene loading time
            audioImporter.loadInBackground = true;

            float length = audio.length;

            // DecompressedOnLoad: Takes memory but use less CPU
            // CompressedInMemory: Takes less memory but use CPU
            // Streaming: Takes least memory but use CPU
            // ADPCM: Takes disk space but use less CPU
            // Vorbis: Takes less disk space but use CPU
            AudioImporterSampleSettings sampleSettings = audioImporter.defaultSampleSettings;
            if (length < 3.0f) // Short sounds Like button sounds or footsteps)
            {
                sampleSettings.loadType = AudioClipLoadType.DecompressOnLoad;
                sampleSettings.compressionFormat = AudioCompressionFormat.ADPCM;
            }
            else if (length < 15.0f)
            {
                sampleSettings.loadType = AudioClipLoadType.CompressedInMemory;
                sampleSettings.compressionFormat = AudioCompressionFormat.Vorbis;
            }
            else // BGM or Ambient sound
            {
                sampleSettings.loadType = AudioClipLoadType.Streaming;
                sampleSettings.compressionFormat = AudioCompressionFormat.Vorbis;
            }
            audioImporter.defaultSampleSettings = sampleSettings;
        }

        public static void ModelImportProcess(ModelImporter modelImporter)
        {
            modelImporter.importBlendShapes = false;
            modelImporter.importCameras = false;
            modelImporter.importLights = false;
            modelImporter.meshCompression = ModelImporterMeshCompression.Medium;
            modelImporter.isReadable = false;
        }
    }
}
