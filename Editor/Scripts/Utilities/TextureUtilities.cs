using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TextureUtilities
{
    public enum TextureSize
    {
        _1 = 1,
        _2 = 2,
        _4 = 4,
        _8 = 8,
        _16 = 16,
        _32 = 32,
        _64 = 64,
        _128 = 128,
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096,
        _8192 = 8192,
    }
    public static TextureSize GetTextureSize(int size) => size switch {
        32 => TextureSize._32,
        64 => TextureSize._64,
        128 => TextureSize._128,
        256 => TextureSize._256,
        512 => TextureSize._512,
        1024 => TextureSize._1024,
        2048 => TextureSize._2048,
        4096 => TextureSize._4096,
        _ => GetNearestPowerOfTwo(size)
    };
    private static TextureSize GetNearestPowerOfTwo(int size)
    {
        int lower = 1;
        while (lower * 2 < size)
        {
            lower *= 2;
        }
        int upper = lower * 2;
        return (TextureSize)((size - lower < upper - size) ? lower : upper);
    }

    public enum Format
    {
        JPG,
        PNG,
    }

    [System.Serializable]
    public class ExportSettings
    {
        public string folderName = "Assets/TexturesProcessed/";
        public string fileName = "Texture";
        public Format format = Format.JPG;
        public string extension => format switch {
            Format.JPG => ".jpg",
            Format.PNG => ".png",
            _ => throw new NotImplementedException()
        };
        public string path
        {
            get
            {
                if (!folderName.EndsWith("/"))
                {
                    folderName += "/";
                }
                return folderName + fileName + extension;
            }
            set
            {
                folderName = Path.GetDirectoryName(value);
                fileName = Path.GetFileNameWithoutExtension(value);
                format = Path.GetExtension(value) == ".jpg" ? Format.JPG : Format.PNG;
            }
        }

        public TextureImporterType importerType = TextureImporterType.Default;
        public bool sRGBTexture = true;
        public int maxTextureSize = 1024;
        public bool hasAlpha = false;

        public TextureImporterFormat textureFormatStandalone = TextureImporterFormat.DXT5Crunched;
        public TextureImporterFormat textureFormatAndroid = TextureImporterFormat.ASTC_8x8;
        public TextureImporterFormat textureFormatIOS = TextureImporterFormat.ASTC_8x8;
        public TextureImporterFormat textureFormatWebGL = TextureImporterFormat.DXT5Crunched;
    }

    public static void SaveTextureToDisk(Texture2D texture, ExportSettings exportSettings, TextureImporterSettings textureImporterSettings = null, Dictionary<string, TextureImporterPlatformSettings> platformSettings = null)
    {
        try
        {
            SaveTextureToDiskInternal(texture, exportSettings);
            if (textureImporterSettings != null)
            {
                SetTextureSettings(exportSettings, textureImporterSettings, platformSettings);
            }
            else
            {
                SetTextureImportSettings(exportSettings);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private static void SaveTextureToDiskInternal(Texture2D texture, ExportSettings exportSettings)
    {
        if (exportSettings.format == Format.JPG && exportSettings.hasAlpha)
        {
            Debug.LogWarning("JPG format does not support alpha channel. Saving as PNG instead.");
            exportSettings.format = Format.PNG;
        }

        byte[] bytes;
        if (exportSettings.format == Format.JPG)
        {
            bytes = ImageConversion.EncodeToJPG(texture);
        }
        else
        {
            bytes = ImageConversion.EncodeToPNG(texture);
        }

        if (exportSettings.path.Length != 0)
        {
            if (bytes != null)
            {
                if (!Directory.Exists(exportSettings.folderName))
                {
                    Directory.CreateDirectory(exportSettings.folderName);
                }
                File.WriteAllBytes(exportSettings.path, bytes);
            }
        }
        else
        {
            Debug.LogError("Path is empty");
        }

        AssetDatabase.Refresh();

        Debug.Log("Texture Saved to: " + exportSettings.path);
    }

    private static void DuplicateTextureImporter(ExportSettings exportSettings, TextureImporter originalTextureImporter)
    {
        TextureImporterSettings settings = new TextureImporterSettings();
        originalTextureImporter.ReadTextureSettings(settings);
        TextureImporter newTextureImporter = (TextureImporter)TextureImporter.GetAtPath(exportSettings.path);
        newTextureImporter.SetTextureSettings(settings);

        EditorUtility.SetDirty(newTextureImporter);
        newTextureImporter.SaveAndReimport();
    }

    private static void SetTextureSettings(ExportSettings exportSettings, TextureImporterSettings textureImporterSettings, Dictionary<string, TextureImporterPlatformSettings> platformSettings)
    {
        TextureImporter newTextureImporter = (TextureImporter)TextureImporter.GetAtPath(exportSettings.path);
        newTextureImporter.SetTextureSettings(textureImporterSettings);
        foreach (KeyValuePair<string, TextureImporterPlatformSettings> platformSetting in platformSettings)
        {
            newTextureImporter.SetPlatformTextureSettings(platformSetting.Value);
        }

        EditorUtility.SetDirty(newTextureImporter);
        newTextureImporter.SaveAndReimport();
    }

    private static void SetTextureImportSettings(ExportSettings exportSettings)
    {
        TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(exportSettings.path);
        textureImporter.textureType = exportSettings.importerType;

        TextureImporterSettings settings = new TextureImporterSettings();
        textureImporter.ReadTextureSettings(settings);
        settings.sRGBTexture = exportSettings.sRGBTexture;
        textureImporter.SetTextureSettings(settings);

        textureImporter.textureCompression = TextureImporterCompression.CompressedLQ;
        textureImporter.crunchedCompression = true;
        // textureImporter.textureFormat = TextureImporterFormat.ASTC_8x8;

        textureImporter.SetPlatformTextureSettings(
            new TextureImporterPlatformSettings() {
                name = "Standalone",
                overridden = true,
                maxTextureSize = exportSettings.maxTextureSize,
                format = exportSettings.textureFormatStandalone,
                compressionQuality = 100,
                crunchedCompression = true,
                allowsAlphaSplitting = false
            }
        );

        textureImporter.SetPlatformTextureSettings(
            new TextureImporterPlatformSettings() {
                name = "Android",
                overridden = true,
                maxTextureSize = exportSettings.maxTextureSize,
                format = exportSettings.textureFormatAndroid,
                compressionQuality = 100,
                crunchedCompression = true,
                allowsAlphaSplitting = false
            }
        );

        textureImporter.SetPlatformTextureSettings(
            new TextureImporterPlatformSettings() {
                name = "iPhone",
                overridden = true,
                maxTextureSize = exportSettings.maxTextureSize,
                format = exportSettings.textureFormatIOS,
                compressionQuality = 100,
                crunchedCompression = true,
                allowsAlphaSplitting = false
            }
        );

        textureImporter.SetPlatformTextureSettings(
            new TextureImporterPlatformSettings() {
                name = "WebGL",
                overridden = true,
                maxTextureSize = exportSettings.maxTextureSize,
                format = exportSettings.textureFormatWebGL,
                compressionQuality = 100,
                crunchedCompression = true,
                allowsAlphaSplitting = false
            }
        );

        EditorUtility.SetDirty(textureImporter);
        textureImporter.SaveAndReimport();
    }

    public static Texture2D DuplicateTexture(Texture2D source, bool resizeToMultipleOfFour, bool linear = false)
    {
        int width = source.width % 4 != 0 && resizeToMultipleOfFour ? source.width + (4 - source.width % 4) : source.width;
        int height = source.height % 4 != 0 && resizeToMultipleOfFour ? source.height + (4 - source.height % 4) : source.height;
        RenderTexture rt = RenderTexture.GetTemporary(
            width: width,
            height: height,
            depthBuffer: 0,
            format: linear ? RenderTextureFormat.ARGB32 : RenderTextureFormat.Default,
            readWrite: linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB
        );

        Texture2D result = new Texture2D(
            width: width,
            height: height,
            textureFormat: linear ? TextureFormat.ARGB32 : TextureFormat.RGBA32,
            mipChain: false
        );

        Graphics.Blit(source, rt);
        result.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        result.Apply();

        RenderTexture.ReleaseTemporary(rt);

        return result;
    }

    public static TextureImporterFormat GetAdjustedTextureImporterFormat(TextureImporterFormat textureImporterFormat, bool hasAlpha, bool isHDR = false)
    {
        if (textureImporterFormat == TextureImporterFormat.DXT1 || textureImporterFormat == TextureImporterFormat.DXT5)
        {
            return hasAlpha || isHDR ? TextureImporterFormat.DXT5 : TextureImporterFormat.DXT1;
        }
        else if (textureImporterFormat == TextureImporterFormat.DXT1Crunched || textureImporterFormat == TextureImporterFormat.DXT5Crunched)
        {
            return hasAlpha || isHDR ? TextureImporterFormat.DXT5Crunched : TextureImporterFormat.DXT1Crunched;
        }
        return textureImporterFormat;
    }
}