using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class UnityExtensions
    {
        public static string GetPath(this GameObject obj, Transform relativeRoot = null)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            GetPathRecursiveInternal(obj.transform, stringBuilder, relativeRoot);
            return stringBuilder.ToString();
        }

        private static void GetPathRecursiveInternal(Transform t, System.Text.StringBuilder stringBuilder, Transform root)
        {
            if (t.parent != null && t.parent != root)
                GetPathRecursiveInternal(t.parent, stringBuilder, root);

            stringBuilder.AppendFormat("/{0}", t.gameObject.name);
        }

        /// <summary>
        /// Modifies importer settings to ensure thumbnail is ready to be encoded and uploaded, then applies the changes.
        /// </summary>
        public static void ApplySettingsForThumbnailEncoding(this TextureImporter importer, bool useTransparency, int maxTextureSize = -1)
        {
            // Required for encoding.
            importer.isReadable = true;
            TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();

            if (maxTextureSize < 0)
                maxTextureSize = defaultSettings.maxTextureSize;

            // Ensure all supported platforms use the same texture settings
            SetThumbnailTextureImporterPlatformSettings(importer, maxTextureSize, useTransparency, platform: defaultSettings.name);
            SetThumbnailTextureImporterPlatformSettings(importer, maxTextureSize, useTransparency, platform: "WebGL");
            SetThumbnailTextureImporterPlatformSettings(importer, maxTextureSize, useTransparency, platform: "Standalone");
            SetThumbnailTextureImporterPlatformSettings(importer, maxTextureSize, useTransparency, platform: "iPhone");
            SetThumbnailTextureImporterPlatformSettings(importer, maxTextureSize, useTransparency, platform: "Android");

            importer.SaveAndReimport();
        }

        private static void SetThumbnailTextureImporterPlatformSettings(TextureImporter importer, int maxTextureSize, bool useTransparency, string platform)
        {
            importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings() {
                name = platform,
                overridden = true,
                maxTextureSize = maxTextureSize,
                // Allow for encoding to both PNG/JPG.
                format = useTransparency ? TextureImporterFormat.RGBA32 : TextureImporterFormat.RGB24,
                // Compressed textures cannot be encoded.
                textureCompression = TextureImporterCompression.Uncompressed
            });
        }

        /// <summary>
        /// Returns true if the texture format supports an alpha channel for transparency.
        /// </summary>
        public static bool IsTransparentFormat(this TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.Alpha8:
                case TextureFormat.ARGB4444:
                case TextureFormat.RGBA4444:
                case TextureFormat.ARGB32:
                case TextureFormat.RGBA32:
                case TextureFormat.BGRA32:
                case TextureFormat.RGBA64:
                case TextureFormat.DXT5:
                case TextureFormat.DXT5Crunched:
                case TextureFormat.RGBAHalf:
                case TextureFormat.RGBAFloat:
                case TextureFormat.PVRTC_RGBA2:
                case TextureFormat.PVRTC_RGBA4:
                case TextureFormat.ETC2_RGBA1:
                case TextureFormat.ETC2_RGBA8:
                case TextureFormat.ETC2_RGBA8Crunched:
                case TextureFormat.ASTC_4x4:
                case TextureFormat.ASTC_HDR_4x4:
                case TextureFormat.ASTC_5x5:
                case TextureFormat.ASTC_HDR_5x5:
                case TextureFormat.ASTC_6x6:
                case TextureFormat.ASTC_HDR_6x6:
                case TextureFormat.ASTC_8x8:
                case TextureFormat.ASTC_HDR_8x8:
                case TextureFormat.ASTC_10x10:
                case TextureFormat.ASTC_HDR_10x10:
                case TextureFormat.ASTC_12x12:
                case TextureFormat.ASTC_HDR_12x12:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the texture is in a transparent format and has at least one transparent/semi-transparent pixel.
        /// </summary>
        public static bool HasTransparency(this Texture2D texture)
        {
            if (!texture.format.IsTransparentFormat())
                return false;

            NativeArray<Color32> pixelData = texture.GetRawTextureData<Color32>();
            for (int i = 0; i < pixelData.Length; i++)
            {
                if (pixelData[i].a != byte.MaxValue)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a value ranging from 0 to 1 representing how much of the texture is composed of fully transparent pixels.
        /// This function can be used to check for a transparent background, and is fundamentally different from HasTransparency() above.
        /// This ensures that the user doesn't circumvent the check by adding a few transparent pixels.
        /// </summary>
        public static float GetTransparentBackgroundRatio(this Texture2D texture)
        {
            if (!texture.format.IsTransparentFormat())
                return 0f;

            int pixelCount = texture.width * texture.height;
            int transparentPixels = 0;

            NativeArray<Color32> pixelData = texture.GetRawTextureData<Color32>();
            for (int i = 0; i < pixelData.Length; i++)
            {
                if (pixelData[i].a == 0)
                    transparentPixels++;
            }

            return Mathf.Clamp01(transparentPixels / (float)pixelCount);
        }
    }
}
