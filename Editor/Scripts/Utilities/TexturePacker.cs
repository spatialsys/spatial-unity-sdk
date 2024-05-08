using System;
using System.IO;
using UnityEngine;
using TextureSize = TextureUtilities.TextureSize;
using ExportSettings = TextureUtilities.ExportSettings;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TexturePacker : EditorWindow
{
    private const string PACKER_SHADER_NAME = "Hidden/SpatialSys/Utilities/TexturePacker";
    private static Material _blitMaterial;

    public enum Channel
    {
        R = 0,
        G = 1,
        B = 2,
        A = 3,
    }

    public enum Format
    {
        JPG,
        PNG,
    }

    public class TextureChannelInfo
    {
        public Texture2D texture = null;
        public Channel channel = Channel.R;
        public float defaultValue = 1f;
        public bool invert = false;
    }

    public TextureChannelInfo redChannelInfo = new TextureChannelInfo();
    public TextureChannelInfo greenChannelInfo = new TextureChannelInfo();
    public TextureChannelInfo blueChannelInfo = new TextureChannelInfo();
    public TextureChannelInfo alphaChannelInfo = new TextureChannelInfo();
    public bool includeAlpha = false;
    private bool _includeAlphaCache = false;
    private Texture2D _redTextureCache;
    private Texture2D _greenTextureCache;
    private Texture2D _blueTextureCache;
    private Texture2D _alphaTextureCache;
    private bool _isTextureSizeNonSquare =>
        (redChannelInfo.texture != null && redChannelInfo.texture.width != redChannelInfo.texture.height) ||
        (greenChannelInfo.texture != null && greenChannelInfo.texture.width != greenChannelInfo.texture.height) ||
        (blueChannelInfo.texture != null && blueChannelInfo.texture.width != blueChannelInfo.texture.height) ||
        (alphaChannelInfo.texture != null && alphaChannelInfo.texture.width != alphaChannelInfo.texture.height);

    // Export Settings
    public TextureSize textureSize = TextureSize._1024;
    public TextureSize textureSizeHeight = TextureSize._1024;
    public bool sRGB = false;
    public Format format = Format.JPG;
    public string textureNameSuffix = "_Packed";
    public string exportPath = "Assets/Texture_Packed.jpg";
    public string exportPathCache = "Assets/Texture_Packed.jpg";

    // EditorWindow
    private static EditorWindow _editorWindow;
    private Vector2 _scrollPos;

#if UNITY_EDITOR
    [MenuItem("Spatial SDK/Utilities/Texture Packer", priority = 2500)]
    public static void ShowWindow()
    {
        _editorWindow = GetWindow(typeof(TexturePacker), false);
    }

    [MenuItem("Assets/Spatial Texture Packer", priority = 2500)]
    public static void ShowWindowRightClick()
    {
        _editorWindow = GetWindow(typeof(TexturePacker), false);
    }

    private void OnInspectorUpdate()
    {
        if (!_editorWindow)
            _editorWindow = GetWindow(typeof(TexturePacker), false);
    }

    private void OnGUI()
    {
        if (_editorWindow)
        {
            GUILayout.BeginArea(new Rect(0, 0, _editorWindow.position.size.x, _editorWindow.position.size.y));
            GUILayout.BeginVertical();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, true, GUILayout.ExpandHeight(true));
        }

        // Title, Description
        GUILayout.Label("Texture Packer", EditorStyles.boldLabel);
        GUILayout.Label("Combine multiple textures into a single texture.", EditorStyles.label);
        if (EditorGUILayout.LinkButton("Documentation"))
        {
            Application.OpenURL("https://docs.spatial.io/texture-packer");
        }
        GUILayout.Space(10f);


        // Texture Channels
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Texture Channels", EditorStyles.boldLabel);
        OnGUIDrawTextureChannel("Red", redChannelInfo, ref _redTextureCache);
        OnGUIDrawTextureChannel("Green", greenChannelInfo, ref _greenTextureCache);
        OnGUIDrawTextureChannel("Blue", blueChannelInfo, ref _blueTextureCache);
        GUILayout.BeginVertical(EditorStyles.helpBox);
        includeAlpha = EditorGUILayout.Toggle("Include Alpha", includeAlpha);
        if (includeAlpha)
        {
            OnGUIDrawTextureChannel("Alpha", alphaChannelInfo, ref _alphaTextureCache, useVerticalLayout: false);
        }
        GUILayout.EndVertical();
        GUILayout.EndVertical(); // Texture Channels


        // Export Settings
        GUILayout.Space(10f);
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Export Settings", EditorStyles.boldLabel);

        // Texture Size
        GUILayout.Space(5f);
        GUILayout.BeginHorizontal();
        GUIContent contentTextureSize = new GUIContent(_isTextureSizeNonSquare ? "Texture Size Width" : "Texture Size");
        textureSize = (TextureSize)EditorGUILayout.EnumPopup(contentTextureSize, textureSize);
        // Get size from texture
        EditorGUI.BeginDisabledGroup(!redChannelInfo.texture);
        if (GUILayout.Button("R", GUILayout.Width(20)))
        {
            GetSizeFromTexture(redChannelInfo.texture);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!greenChannelInfo.texture);
        if (GUILayout.Button("G", GUILayout.Width(20)))
        {
            GetSizeFromTexture(greenChannelInfo.texture);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!blueChannelInfo.texture);
        if (GUILayout.Button("B", GUILayout.Width(20)))
        {
            GetSizeFromTexture(blueChannelInfo.texture);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!includeAlpha || !alphaChannelInfo.texture);
        if (GUILayout.Button("A", GUILayout.Width(20)))
        {
            GetSizeFromTexture(alphaChannelInfo.texture);
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();
        if (_isTextureSizeNonSquare)
        {
            textureSizeHeight = (TextureSize)EditorGUILayout.EnumPopup("Texture Size Height:", textureSizeHeight);
        }

        // sRGB
        GUIContent contentSRGB = new GUIContent("sRGB", "Disable sRGB by default because we assume the texture will be used as a mask.");
        GUILayout.BeginHorizontal();
        sRGB = EditorGUILayout.Toggle(contentSRGB, sRGB, GUILayout.Width(170));
        GUILayout.Label("(Disable sRGB for mask textures (metallic, smoothness, ao, etc))", EditorStyles.miniLabel);
        GUILayout.EndHorizontal();

        // Format
        if (includeAlpha != _includeAlphaCache)
        {
            _includeAlphaCache = includeAlpha;
            format = includeAlpha ? Format.PNG : Format.JPG;
        }
        EditorGUI.BeginDisabledGroup(includeAlpha);
        format = (Format)EditorGUILayout.EnumPopup("Format:", format);
        EditorGUI.EndDisabledGroup();

        // Path
        textureNameSuffix = EditorGUILayout.TextField("Suffix:", textureNameSuffix);

        GUILayout.BeginHorizontal();
        exportPath = EditorGUILayout.TextField("Path:", exportPath);

        // Copy path from texture
        EditorGUI.BeginDisabledGroup(!redChannelInfo.texture);
        if (GUILayout.Button("R", GUILayout.Width(20)))
        {
            exportPath = GetNewTexturePath(redChannelInfo.texture);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!greenChannelInfo.texture);
        if (GUILayout.Button("G", GUILayout.Width(20)))
        {
            exportPath = GetNewTexturePath(greenChannelInfo.texture);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!blueChannelInfo.texture);
        if (GUILayout.Button("B", GUILayout.Width(20)))
        {
            exportPath = GetNewTexturePath(blueChannelInfo.texture);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!includeAlpha || !alphaChannelInfo.texture);
        if (GUILayout.Button("A", GUILayout.Width(20)))
        {
            exportPath = GetNewTexturePath(alphaChannelInfo.texture);
        }
        EditorGUI.EndDisabledGroup();

        // Correct Extension
        if (exportPath != exportPathCache)
        {
            CorrectExtension(ref exportPath);
            exportPathCache = exportPath;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical(); // Export Settings


        // Button - Generate Texture
        GUILayout.Space(10f);
        GUILayout.BeginVertical(EditorStyles.helpBox);
        if (GUILayout.Button("Generate Texture"))
        {
            EditorUtility.DisplayProgressBar("Packing Textures, please wait...", "", 1f);
            try
            {
                int width = (int)textureSize;
                int height = _isTextureSizeNonSquare ? (int)textureSizeHeight : width;
                PackTexture(redChannelInfo, greenChannelInfo, blueChannelInfo, alphaChannelInfo, width, height);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            EditorUtility.ClearProgressBar();
        }
        GUILayout.EndVertical();

        GUILayout.Space(100);
        if (_editorWindow)
        {
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }

    private void OnGUIDrawTextureChannel(string title, TextureChannelInfo textureChannelInfo, ref Texture2D textureCache, bool useVerticalLayout = true)
    {
        if (useVerticalLayout)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
        }
        textureChannelInfo.texture = (Texture2D)EditorGUILayout.ObjectField(title, textureChannelInfo.texture, typeof(Texture2D), false);
        if (!textureChannelInfo.texture)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("No image found, use slider to set value");
            textureChannelInfo.defaultValue = EditorGUILayout.Slider(textureChannelInfo.defaultValue, 0f, 1f);
            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string nonSquareRed = textureChannelInfo.texture.width != textureChannelInfo.texture.height ? " (Non-Square) " : "";
            GUILayout.Label($"{nonSquareRed}({textureChannelInfo.texture.width}x{textureChannelInfo.texture.height})", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
            textureChannelInfo.channel = (Channel)EditorGUILayout.EnumPopup("Channel:", textureChannelInfo.channel);
            textureChannelInfo.invert = EditorGUILayout.Toggle("Invert", textureChannelInfo.invert);
        }
        if (textureChannelInfo.texture != textureCache)
        {
            textureCache = textureChannelInfo.texture;
            if (textureChannelInfo.texture)
            {
                exportPath = GetNewTexturePath(textureChannelInfo.texture);
                textureSize = TextureUtilities.GetTextureSize(textureChannelInfo.texture.width);
                if (_isTextureSizeNonSquare)
                {
                    textureSizeHeight = TextureUtilities.GetTextureSize(textureChannelInfo.texture.height);
                }
            }
        }
        if (useVerticalLayout)
        {
            GUILayout.EndVertical();
        }
    }

    private void GetSizeFromTexture(Texture2D texture)
    {
        textureSize = TextureUtilities.GetTextureSize(texture.width);
        if (_isTextureSizeNonSquare)
        {
            textureSizeHeight = TextureUtilities.GetTextureSize(texture.height);
        }
    }

    private string GetNewTexturePath(Texture2D originalTexture)
    {
        string path = AssetDatabase.GetAssetPath(originalTexture);
        path = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path);
        path += textureNameSuffix;
        path += format == Format.JPG ? ".jpg" : ".png";
        return path;
    }

    private void CorrectExtension(ref string path)
    {
        string targetExtension = format == Format.JPG ? ".jpg" : ".png";
        if (!path.EndsWith(targetExtension))
        {
            // remove extension
            path = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path);
            path += targetExtension;
        }
    }

    private void PackTexture(
        TextureChannelInfo redChannelInfo,
        TextureChannelInfo greenChannelInfo,
        TextureChannelInfo blueChannelInfo,
        TextureChannelInfo alphaChannelInfo,
        int width,
        int height)
    {
        ///------------------------------------------------------------------
        /// Set Texture Importer before copying it. (To have the same result)
        ///------------------------------------------------------------------
        string platform = EditorUserBuildSettings.activeBuildTarget.ToString();
        TextureImporter textureImporter_R = null;
        TextureImporter textureImporter_G = null;
        TextureImporter textureImporter_B = null;
        TextureImporter textureImporter_A = null;
        TextureImporterSettings textureImporterSettings_R = new TextureImporterSettings();
        TextureImporterSettings textureImporterSettings_G = new TextureImporterSettings();
        TextureImporterSettings textureImporterSettings_B = new TextureImporterSettings();
        TextureImporterSettings textureImporterSettings_A = new TextureImporterSettings();
        TextureImporterPlatformSettings textureImporterPlatformSettings_R = new TextureImporterPlatformSettings();
        TextureImporterPlatformSettings textureImporterPlatformSettings_G = new TextureImporterPlatformSettings();
        TextureImporterPlatformSettings textureImporterPlatformSettings_B = new TextureImporterPlatformSettings();
        TextureImporterPlatformSettings textureImporterPlatformSettings_A = new TextureImporterPlatformSettings();

        // Get Texture Importer Settings
        if (redChannelInfo.texture != null)
        {
            GetTextureImporterSettings(redChannelInfo.texture, out textureImporter_R, out textureImporterSettings_R, out textureImporterPlatformSettings_R);
        }
        if (greenChannelInfo.texture != null)
        {
            GetTextureImporterSettings(greenChannelInfo.texture, out textureImporter_G, out textureImporterSettings_G, out textureImporterPlatformSettings_G);
        }
        if (blueChannelInfo.texture != null)
        {
            GetTextureImporterSettings(blueChannelInfo.texture, out textureImporter_B, out textureImporterSettings_B, out textureImporterPlatformSettings_B);
        }
        if (includeAlpha && alphaChannelInfo.texture != null)
        {
            GetTextureImporterSettings(alphaChannelInfo.texture, out textureImporter_A, out textureImporterSettings_A, out textureImporterPlatformSettings_A);
        }

        // Set Texture Importer Settings To Copy
        if (redChannelInfo.texture != null)
        {
            SetTextureImporterSettingsToCopy(textureImporter_R);
        }
        if (greenChannelInfo.texture != null)
        {
            SetTextureImporterSettingsToCopy(textureImporter_G);
        }
        if (blueChannelInfo.texture != null)
        {
            SetTextureImporterSettingsToCopy(textureImporter_B);
        }
        if (includeAlpha && alphaChannelInfo.texture != null)
        {
            SetTextureImporterSettingsToCopy(textureImporter_A);
        }


        ///------------------------------------------------------------------
        /// Blit Texture
        ///------------------------------------------------------------------
        Texture2D tex = GenerateTexture(redChannelInfo, greenChannelInfo, blueChannelInfo, alphaChannelInfo, width, height);

        ExportSettings exportSettings = new ExportSettings();
        exportSettings.path = exportPath;
        exportSettings.sRGBTexture = sRGB;
        exportSettings.maxTextureSize = (int)textureSize;
        exportSettings.hasAlpha = includeAlpha;
        exportSettings.textureFormatStandalone = TextureUtilities.GetAdjustedTextureImporterFormat(exportSettings.textureFormatStandalone, includeAlpha);
        exportSettings.textureFormatWebGL = TextureUtilities.GetAdjustedTextureImporterFormat(exportSettings.textureFormatWebGL, includeAlpha);
        TextureUtilities.SaveTextureToDisk(tex, exportSettings);


        ///------------------------------------------------------------------
        /// Set back to original settings after copying
        ///------------------------------------------------------------------
        if (redChannelInfo.texture != null)
        {
            RevertTextureImporterSettings(textureImporter_R, textureImporterSettings_R, textureImporterPlatformSettings_R);
        }
        if (greenChannelInfo.texture != null)
        {
            RevertTextureImporterSettings(textureImporter_G, textureImporterSettings_G, textureImporterPlatformSettings_G);
        }
        if (blueChannelInfo.texture != null)
        {
            RevertTextureImporterSettings(textureImporter_B, textureImporterSettings_B, textureImporterPlatformSettings_B);
        }
        if (includeAlpha && alphaChannelInfo.texture != null)
        {
            RevertTextureImporterSettings(textureImporter_A, textureImporterSettings_A, textureImporterPlatformSettings_A);
        }
    }

    private void GetTextureImporterSettings(Texture texture, out TextureImporter textureImporter, out TextureImporterSettings textureImporterSettings, out TextureImporterPlatformSettings textureImporterPlatformSettings)
    {
        string platform = EditorUserBuildSettings.activeBuildTarget.ToString();

        // Get Texture Importer
        string path = AssetDatabase.GetAssetPath(texture);
        textureImporter = (TextureImporter)TextureImporter.GetAtPath(path);

        // Get Texture Importer Settings
        textureImporterSettings = new TextureImporterSettings();
        textureImporter.ReadTextureSettings(textureImporterSettings);

        // Get Texture Importer Platform Settings
        textureImporterPlatformSettings = textureImporter.GetPlatformTextureSettings(platform);
    }

    private void SetTextureImporterSettingsToCopy(TextureImporter textureImporter)
    {
        string platform = EditorUserBuildSettings.activeBuildTarget.ToString();

        textureImporter.textureType = TextureImporterType.Default;
        textureImporter.sRGBTexture = false; // Convert in Linear color space
        textureImporter.maxTextureSize = 4096;
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        textureImporter.SetPlatformTextureSettings(
            new TextureImporterPlatformSettings() {
                name = platform,
                overridden = true,
                maxTextureSize = 4096,
                format = TextureImporterFormat.RGBA32,
                compressionQuality = 100
            }
        );
        textureImporter.SaveAndReimport();
    }

    private void RevertTextureImporterSettings(TextureImporter textureImporter, TextureImporterSettings textureImporterSettings, TextureImporterPlatformSettings textureImporterPlatformSettings)
    {
        textureImporter.SetTextureSettings(textureImporterSettings);
        textureImporter.SetPlatformTextureSettings(textureImporterPlatformSettings);
        textureImporter.SaveAndReimport();
    }

    private Texture2D GenerateTexture(
        TextureChannelInfo redChannelInfo,
        TextureChannelInfo greenChannelInfo,
        TextureChannelInfo blueChannelInfo,
        TextureChannelInfo alphaChannelInfo,
        int width,
        int height)
    {
        // Blit in Linear color space
        RenderTexture blitRenderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        blitRenderTexture.useMipMap = false;
        blitRenderTexture.filterMode = FilterMode.Bilinear;

        Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, mipCount: 1, linear: true);

        if (_blitMaterial == null)
        {
            _blitMaterial = new Material(Shader.Find(PACKER_SHADER_NAME));
        }

        if (redChannelInfo.texture != null)
        {
            _blitMaterial.SetTexture("_RedTex", redChannelInfo.texture);
        }
        if (greenChannelInfo.texture != null)
        {
            _blitMaterial.SetTexture("_GreenTex", greenChannelInfo.texture);
        }
        if (blueChannelInfo.texture != null)
        {
            _blitMaterial.SetTexture("_BlueTex", blueChannelInfo.texture);
        }
        if (alphaChannelInfo.texture != null)
        {
            _blitMaterial.SetTexture("_AlphaTex", alphaChannelInfo.texture);
        }

        _blitMaterial.SetInt("_RedChannel", (redChannelInfo.texture != null) ? (int)redChannelInfo.channel : -1);
        _blitMaterial.SetInt("_GreenChannel", (greenChannelInfo.texture != null) ? (int)greenChannelInfo.channel : -1);
        _blitMaterial.SetInt("_BlueChannel", (blueChannelInfo.texture != null) ? (int)blueChannelInfo.channel : -1);
        _blitMaterial.SetInt("_AlphaChannel", (alphaChannelInfo.texture != null) ? (int)alphaChannelInfo.channel : -1);

        _blitMaterial.SetFloat("_RedDefault", redChannelInfo.defaultValue);
        _blitMaterial.SetFloat("_GreenDefault", greenChannelInfo.defaultValue);
        _blitMaterial.SetFloat("_BlueDefault", blueChannelInfo.defaultValue);
        _blitMaterial.SetFloat("_AlphaDefault", alphaChannelInfo.defaultValue);

        _blitMaterial.SetInt("_RedInvert", redChannelInfo.invert ? 1 : 0);
        _blitMaterial.SetInt("_GreenInvert", greenChannelInfo.invert ? 1 : 0);
        _blitMaterial.SetInt("_BlueInvert", blueChannelInfo.invert ? 1 : 0);
        _blitMaterial.SetInt("_AlphaInvert", alphaChannelInfo.invert ? 1 : 0);

        Graphics.Blit(new Texture2D(1, 1), blitRenderTexture, _blitMaterial, 0);
        // Graphics.CopyTexture fails to encoding to jpg/png
        // Graphics.CopyTexture(blitRenderTexture, result);
        result.ReadPixels(new Rect(0, 0, blitRenderTexture.width, blitRenderTexture.height), 0, 0);
        result.Apply();

        blitRenderTexture.Release();

        return result;
    }
#endif
}
