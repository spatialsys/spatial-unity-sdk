using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExportSettings = TextureUtilities.ExportSettings;
using TextureSize = TextureUtilities.TextureSize;

public class TextureOptimizer : EditorWindow
{
    [System.Flags]
    public enum Extension
    {
        None = 0,
        PNG = 1 << 0,
        TIF = 1 << 1,
        TGA = 1 << 2,
        PSD = 1 << 3,
        EXR = 1 << 4, // Lightmaps in most cases
        HDR = 1 << 5,
        AllExceptLightmap = PNG | TIF | TGA | PSD | HDR,
        All = PNG | TIF | TGA | PSD | EXR | HDR,
    }
    private static Extension GetExtensionFromString(string extension) => extension switch {
        ".png" => Extension.PNG,
        ".tif" => Extension.TIF,
        ".tga" => Extension.TGA,
        ".psd" => Extension.PSD,
        ".exr" => Extension.EXR,
        ".hdr" => Extension.HDR,
        _ => Extension.None,
    };
    private static bool IsTargetExtension(string extension) => (_targetExtension & GetExtensionFromString(extension)) != 0;
    private static bool NeedConvert(Texture2D texture, string extension, bool hasAlpha) =>
        _resizeTextures || // resize
        _resizeToMultipleOfFour && (texture.width % 4 != 0 || texture.height % 4 != 0) || // multiple of 4
        IsTargetExtension(extension) &&
        (!hasAlpha && extension != ".jpg" || hasAlpha && extension != ".png");

    private static bool IsFontAsset(string extension) => extension == ".asset" || extension == ".ttf" || extension == ".otf";

    private static readonly string[] PLATFORM_NAMES = new string[] { "Standalone", "Android", "iOS", "WebGL" };
    private static Dictionary<string, TextureImporterPlatformSettings> _platformSettings = new Dictionary<string, TextureImporterPlatformSettings>();

    private const string DIALOG_TITLE = "Backup Required";
    private const string DIALOG_MESSAGE = "Please backup your project before proceeding.\nThis will replace selected textures to JPG or PNG and you will lose the original textures.";
    private const int MAX_TEXTURE_COUNT_FOLDOUT = 128;

    // Settings for Disk Formats
    private static List<Texture2D> _targetTextures = new List<Texture2D>();
    private static bool _resizeTextures = false;
    private static bool _saveOriginalTextures = false;
    private static bool _resizeToMultipleOfFour = true;
    private static ExportSettings _exportSettings = new ExportSettings();
    private static Extension _targetExtension = Extension.AllExceptLightmap;

    // Settings for Importer
    private bool _showTextureImporterSettings = false;
    private static TextureSize _maxImportTextureSize = TextureSize._1024;
    private static bool _applyMaxImportTextureSizePerPlatform = false;
    private static TextureSize _maxImportTextureSizeStandalone = TextureSize._1024;
    private static TextureSize _maxImportTextureSizeAndroid = TextureSize._1024;
    private static TextureSize _maxImportTextureSizeIOS = TextureSize._1024;
    private static TextureSize _maxImportTextureSizeWebGL = TextureSize._1024;
    private static TextureImporterFormat _textureFormatStandalone = TextureImporterFormat.DXT5Crunched;
    private static TextureImporterFormat _textureFormatAndroid = TextureImporterFormat.ASTC_8x8;
    private static TextureImporterFormat _textureFormatIOS = TextureImporterFormat.ASTC_8x8;
    private static TextureImporterFormat _textureFormatWebGL = TextureImporterFormat.DXT5Crunched;

    private static EditorWindow _editorWindow;
    private Vector2 _scrollPos;

    private string _texturesFolder = "Assets";
    private bool _showTextures = true;


    public static void OptimizeAllTexturesInProject()
    {
        bool confirmedDialog = UnityEditor.EditorUtility.DisplayDialog(
            title: DIALOG_TITLE,
            message: DIALOG_MESSAGE,
            ok: "Proceed",
            cancel: "Cancel"
        );

        if (confirmedDialog)
        {
            OptimizeAllTexturedInProjectImmediate();
        }
    }

    public static void OptimizeAllTexturedInProjectImmediate()
    {
        _targetTextures = GetTexturesFromFolder("Assets");
        _saveOriginalTextures = false;
        ConvertTextures(_targetTextures);
        _targetTextures = null;
    }

    public static void OptimizeTextures(List<Texture2D> textures, bool resize = false, bool saveOriginals = false, bool resizeToMultipleOfFour = true)
    {
        _targetTextures = textures;
        _resizeTextures = resize;
        _saveOriginalTextures = saveOriginals;
        _resizeToMultipleOfFour = resizeToMultipleOfFour;
        ConvertTextures(_targetTextures);
        _targetTextures = null;
    }

    [MenuItem("Spatial SDK/Utilities/Texture Optimizer", priority = 2000)]
    public static void OpenTextureOptimizerWindow()
    {
        ShowWindow();
    }

    [MenuItem("Assets/Spatial Texture Optimizer (Selected)", priority = 2000)]
    public static void ProcessTexturesMenuItem()
    {
        _targetTextures = GetTexturesFromSelection();
        ShowWindow();
    }

    private static void ShowWindow()
    {
        _editorWindow = GetWindow(typeof(TextureOptimizer), utility: false, title: "Texture Optimizer");
    }

    private static List<Texture2D> GetTexturesFromSelection()
    {
        List<Texture2D> selectedTextures = new List<Texture2D>();
        foreach (Texture2D tex in Selection.GetFiltered<Texture2D>(SelectionMode.Assets))
        {
            string extension = Path.GetExtension(AssetDatabase.GetAssetPath(tex));
            extension = extension.ToLower();
            if (tex != null && !IsFontAsset(extension))
            {
                selectedTextures.Add(tex);
            }
        }

        // Get textures from folder selected
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets))
        {
            selectedTextures.AddRange(GetTexturesFromFolder(AssetDatabase.GetAssetPath(obj)));
        }
        return selectedTextures;
    }

    private void OnInspectorUpdate()
    {
        if (!_editorWindow)
            _editorWindow = GetWindow(typeof(TextureOptimizer), false);
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
        GUILayout.Label("Texture Optimizer", EditorStyles.boldLabel);
        GUILayout.Label("Convert selected textures to JPG for non-transparent textures or PNG for transparent textures to reduce the package size. This helps keep the package size under our 500 MB limit.", EditorStyles.wordWrappedLabel);
        if (EditorGUILayout.LinkButton("Documentation"))
        {
            Application.OpenURL("https://docs.spatial.io/texture-optimizer");
        }
        GUILayout.Space(10f);


        // Select Textures
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Space(5f);
        GUILayout.Label("Select Textures", EditorStyles.boldLabel);

        GUILayout.Space(5f);
        GUILayout.BeginHorizontal();
        _texturesFolder = EditorGUILayout.TextField("Textures Folder", _texturesFolder);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string path = UnityEditor.EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
            if (path.Length != 0)
            {
                _texturesFolder = RemoveBefore(path, "Assets");
            }
        }
        GUILayout.EndHorizontal();

        // Buttons
        GUILayout.Space(5f);
        if (GUILayout.Button("Select Textures from Folder"))
        {
            _targetTextures = GetTexturesFromFolder(_texturesFolder);
            if (_targetTextures.Count > MAX_TEXTURE_COUNT_FOLDOUT)
            {
                _showTextures = false;
            }
        }
        GUILayout.Space(2f);
        if (GUILayout.Button("Select Textures from Selection"))
        {
            _targetTextures = GetTexturesFromSelection();
        }
        GUILayout.Space(2f);
        if (GUILayout.Button("Deselect Textures"))
        {
            _targetTextures = null;
        }
        GUILayout.Space(10f);
        GUILayout.EndVertical();


        // ExportSettings (Disk Texture Formats)
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Space(5f);
        GUIContent contentDiskTextureFormats = new GUIContent("Texture File Settings", "Settings for saving textures to disk.");
        GUILayout.Label(contentDiskTextureFormats, EditorStyles.boldLabel);

        GUILayout.Space(5f);
        GUILayout.BeginHorizontal();
        GUIContent contentResizeTextures = new GUIContent("Resize Textures", "Resize textures if width/height is larger than the selected size.");
        _resizeTextures = EditorGUILayout.Toggle(contentResizeTextures, _resizeTextures);
        EditorGUI.BeginDisabledGroup(!_resizeTextures);
        _exportSettings.maxTextureSize = (int)(TextureSize)EditorGUILayout.EnumPopup("", (TextureSize)_exportSettings.maxTextureSize);
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        GUIContent contentSaveOriginalTextures = new GUIContent("Save Copies of Originals", "The original textures will be saved if checked.");
        _saveOriginalTextures = EditorGUILayout.Toggle(contentSaveOriginalTextures, _saveOriginalTextures);

        GUIContent contentResizeToMultipleOfFour = new GUIContent("Resize to Multiple of 4", "Only textures with width/height being multiple of 4 can be compressed to Crunch format.");
        _resizeToMultipleOfFour = EditorGUILayout.Toggle(contentResizeToMultipleOfFour, _resizeToMultipleOfFour);

        // Just copying the settings from the original texture and not using these settings
        // GUILayout.Space(5f);
        // _exportSettings.textureFormatStandalone = (TextureImporterFormat)EditorGUILayout.EnumPopup("Texture Format Standalone", _exportSettings.textureFormatStandalone);
        // _exportSettings.textureFormatAndroid = (TextureImporterFormat)EditorGUILayout.EnumPopup("Texture Format Android", _exportSettings.textureFormatAndroid);
        // _exportSettings.textureFormatIOS = (TextureImporterFormat)EditorGUILayout.EnumPopup("Texture Format iOS", _exportSettings.textureFormatIOS);
        // _exportSettings.textureFormatWebGL = (TextureImporterFormat)EditorGUILayout.EnumPopup("Texture Format WebGL", _exportSettings.textureFormatWebGL);

        // Target Extension
        GUILayout.Space(5f);
        GUILayout.BeginHorizontal();
        GUIContent contentTargetExtension = new GUIContent("Target Extension", "Textures will be optimized only if they have the selected extension.");
        GUILayout.Label(contentTargetExtension);
        _targetExtension = (Extension)EditorGUILayout.EnumFlagsField(_targetExtension);
        GUILayout.EndHorizontal();

        // Button - Convert Textures
        bool hasTargetTextures = _targetTextures != null && _targetTextures.Count > 0;
        GUILayout.Space(10f);
        EditorGUI.BeginDisabledGroup(!hasTargetTextures);
        if (GUILayout.Button("Optimize Textures"))
        {
            bool confirmedDialog = UnityEditor.EditorUtility.DisplayDialog(
                title: DIALOG_TITLE,
                message: DIALOG_MESSAGE,
                ok: "Proceed",
                cancel: "Cancel"
            );

            if (confirmedDialog)
            {
                ConvertTextures(_targetTextures);
                if (!_saveOriginalTextures)
                {
                    _targetTextures = null;
                }
            }
        }
        EditorGUI.EndDisabledGroup();
        if (!hasTargetTextures)
        {
            GUILayout.Label("No textures selected.");
        }

        GUILayout.Space(10f);
        GUILayout.EndVertical();


        // ExportSettings (GPU Texture Formats)
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Space(5f);
        GUIContent contentGPUTextureFormats = new GUIContent("Texture Import Settings Modifier", "Settings for importing textures to Unity. This will affect to the runtime memory usage and loading time.");
        _showTextureImporterSettings = EditorGUILayout.Foldout(_showTextureImporterSettings, contentGPUTextureFormats, true, EditorStyles.foldout);
        if (_showTextureImporterSettings)
        {
            GUILayout.Space(5f);
            GUILayout.BeginHorizontal();
            if (_applyMaxImportTextureSizePerPlatform)
            {
                GUILayout.BeginVertical(GUILayout.Width(240));
                _maxImportTextureSizeStandalone = (TextureSize)EditorGUILayout.EnumPopup("Max Size For Standalone", _maxImportTextureSizeStandalone, GUILayout.Width(240));
                _maxImportTextureSizeAndroid = (TextureSize)EditorGUILayout.EnumPopup("Max Size For Android", _maxImportTextureSizeAndroid, GUILayout.Width(240));
                _maxImportTextureSizeIOS = (TextureSize)EditorGUILayout.EnumPopup("Max Size For iOS", _maxImportTextureSizeIOS, GUILayout.Width(240));
                _maxImportTextureSizeWebGL = (TextureSize)EditorGUILayout.EnumPopup("Max Size For WebGL", _maxImportTextureSizeWebGL, GUILayout.Width(240));
                GUILayout.EndVertical();
            }
            else
            {
                _maxImportTextureSize = (TextureSize)EditorGUILayout.EnumPopup("Max Size", _maxImportTextureSize, GUILayout.Width(240));
            }
            GUILayout.Space(5f);
            _applyMaxImportTextureSizePerPlatform = EditorGUILayout.ToggleLeft("Per Platform", _applyMaxImportTextureSizePerPlatform, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);
            _textureFormatStandalone = (TextureImporterFormat)EditorGUILayout.EnumPopup("Format Standalone", _textureFormatStandalone);
            _textureFormatAndroid = (TextureImporterFormat)EditorGUILayout.EnumPopup("Format Android", _textureFormatAndroid);
            _textureFormatIOS = (TextureImporterFormat)EditorGUILayout.EnumPopup("Format iOS", _textureFormatIOS);
            _textureFormatWebGL = (TextureImporterFormat)EditorGUILayout.EnumPopup("Format WebGL", _textureFormatWebGL);

            if (!(_textureFormatStandalone == TextureImporterFormat.DXT5Crunched ||
                    _textureFormatStandalone == TextureImporterFormat.DXT1Crunched ||
                    _textureFormatStandalone == TextureImporterFormat.BC7) ||
                !(_textureFormatAndroid == TextureImporterFormat.ASTC_8x8 ||
                    _textureFormatAndroid == TextureImporterFormat.ASTC_6x6 ||
                    _textureFormatAndroid == TextureImporterFormat.ASTC_4x4) ||
                !(_textureFormatIOS == TextureImporterFormat.ASTC_8x8 ||
                    _textureFormatIOS == TextureImporterFormat.ASTC_6x6 ||
                    _textureFormatIOS == TextureImporterFormat.ASTC_4x4) ||
                !(_textureFormatWebGL == TextureImporterFormat.DXT5Crunched ||
                    _textureFormatWebGL == TextureImporterFormat.DXT1Crunched))
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Reset to Recommended Format", GUILayout.Width(220)))
                {
                    _textureFormatStandalone = TextureImporterFormat.DXT5Crunched;
                    _textureFormatAndroid = TextureImporterFormat.ASTC_8x8;
                    _textureFormatIOS = TextureImporterFormat.ASTC_8x8;
                    _textureFormatWebGL = TextureImporterFormat.DXT5Crunched;
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10f);
            EditorGUI.BeginDisabledGroup(!hasTargetTextures);
            if (GUILayout.Button("Change TextureImporter Settings"))
            {
                bool confirmedDialog = UnityEditor.EditorUtility.DisplayDialog(
                    title: "TextureImporter Settings",
                    message: "This will change the TextureImporter settings for the selected textures.",
                    ok: "Proceed",
                    cancel: "Cancel"
                );

                if (confirmedDialog)
                {
                    ChangeTextureImporterSettings(_targetTextures);
                    if (!_saveOriginalTextures)
                    {
                        _targetTextures = null;
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
            if (!hasTargetTextures)
            {
                GUILayout.Label("No textures selected.");
            }
        }
        GUILayout.Space(10f);
        GUILayout.EndVertical();


        // Selected Textures
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Space(5f);

        int targetTextureCount = _targetTextures == null ? 0 : _targetTextures.Count;
        GUILayout.Label($"Selected Textures ({targetTextureCount})", EditorStyles.boldLabel);
        GUILayout.Space(5);
        if (targetTextureCount > 0)
        {
            if (_targetTextures.Count > MAX_TEXTURE_COUNT_FOLDOUT)
            {
                _showTextures = EditorGUILayout.Foldout(_showTextures, "Selected Textures");
            }
            else
            {
                _showTextures = true;
            }

            if (_showTextures)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                foreach (Texture2D tex in _targetTextures)
                {
                    GUILayout.BeginHorizontal();
                    string path = AssetDatabase.GetAssetPath(tex);
                    string extension = Path.GetExtension(path);
                    TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(path);
                    if (textureImporter == null)
                    {
                        // Debug.LogError($"TextureImporter is null for {tex.name}");
                        continue;
                    }
                    bool hasAlpha = textureImporter.DoesSourceTextureHaveAlpha();
                    bool needConvert = NeedConvert(tex, extension, hasAlpha);

                    EditorGUI.BeginDisabledGroup(!needConvert);
                    EditorGUILayout.ObjectField(tex, typeof(Texture2D), false, GUILayout.Width(100));
                    GUILayout.Label(extension, GUILayout.Width(30));
                    GUILayout.Label(hasAlpha ? "Alpha" : "No Alpha", GUILayout.Width(60));
                    if (tex.width % 4 == 0 && tex.height % 4 == 0) // multiple of 4
                    {
                        GUILayout.Label($"{tex.width}x{tex.height}", GUILayout.Width(200));
                    }
                    else
                    {
                        GUILayout.Label($"{tex.width}x{tex.height} (Not multiple of 4)", GUILayout.Width(200));
                    }
                    EditorGUI.EndDisabledGroup();

                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
        }
        else
        {
            GUILayout.Label("No textures selected.");
        }

        GUILayout.Space(10f);
        GUILayout.EndVertical();


        GUILayout.Space(100);
        if (_editorWindow)
        {
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }

    private static List<Texture2D> GetTexturesFromFolder(string folder)
    {
        // "keyword l: <label> t: <type>"
        // https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
        List<Texture2D> textures = new List<Texture2D>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string extension = Path.GetExtension(path);
            extension = extension.ToLower();
            Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
            if (tex != null && !IsFontAsset(extension))
            {
                textures.Add(tex);
            }
        }
        return textures;
    }


    private static void ConvertTextures(List<Texture2D> textures)
    {
        try
        {
            for (int i = 0; i < textures.Count; i++)
            {
                Texture2D textureOriginal = textures[i];
                string path = AssetDatabase.GetAssetPath(textureOriginal);
                string folder = Path.GetDirectoryName(path) + "/";
                string textureName = textureOriginal.name;
                string extension = Path.GetExtension(path);

                EditorUtility.DisplayProgressBar("Converting Textures, please wait...", $"{textureOriginal.name}\n{path}", (float)i / textures.Count);

                TextureImporter originalTextureImporter = (TextureImporter)TextureImporter.GetAtPath(path);
                if (originalTextureImporter == null)
                {
                    Debug.LogError($"TextureImporter is null for {textureOriginal.name}");
                    continue;
                }

                bool hasAlpha = originalTextureImporter.DoesSourceTextureHaveAlpha();
                if (!NeedConvert(textureOriginal, extension, hasAlpha)) // still need convert if not multiple of 4
                {
                    Debug.Log($"Skipping {textureName} as it is already in the target format.");
                    continue;
                }

                // ExportSettings per texture (Some of the settings are not used, TextureImporterSettings are used instead)
                _exportSettings.folderName = folder;
                _exportSettings.fileName = textureName;
                _exportSettings.format = hasAlpha ? TextureUtilities.Format.PNG : TextureUtilities.Format.JPG;
                _exportSettings.importerType = originalTextureImporter.textureType;
                _exportSettings.sRGBTexture = originalTextureImporter.sRGBTexture;
                _exportSettings.maxTextureSize = _resizeTextures ? _exportSettings.maxTextureSize : 4096;
                _exportSettings.hasAlpha = hasAlpha;

                // Store original settings to copy
                TextureImporterSettings originalTextureImporterSettings = new TextureImporterSettings();
                originalTextureImporter.ReadTextureSettings(originalTextureImporterSettings);

                // Store each platform settings to copy
                _platformSettings.Clear();
                foreach (string platformName in PLATFORM_NAMES)
                {
                    _platformSettings.Add(platformName, originalTextureImporter.GetPlatformTextureSettings(platformName));
                }

                // Set TextureImporter settings to default before copying it.
                // Because we duplicate the texture using Graphics.Blit, the result can be different by the settings.
                // i.e. texture format, sRGB, maxTextureSize, etc.
                // TextureType (Default)
                TextureImporterType originalTextureType = originalTextureImporter.textureType;
                originalTextureImporter.textureType = TextureImporterType.Default;
                // sRGB (true)
                bool originalsRGB = originalTextureImporter.sRGBTexture;
                originalTextureImporter.sRGBTexture = true;
                // MaxTextureSize (4096)
                string platform = EditorUserBuildSettings.activeBuildTarget.ToString();
                int originalMaxTextureSize = 4096;
                bool hasPlatformSettings = originalTextureImporter.GetPlatformTextureSettings(platform, out originalMaxTextureSize, out TextureImporterFormat textureFormat, out int compressionQuality, out bool etc1AlphaSplitEnabled);
                int targetMaxTextureSize = _resizeTextures ? _exportSettings.maxTextureSize : 4096;
                if (hasPlatformSettings)
                {
                    originalTextureImporter.SetPlatformTextureSettings(
                        new TextureImporterPlatformSettings() {
                            name = platform,
                            overridden = true,
                            maxTextureSize = targetMaxTextureSize,
                            format = TextureImporterFormat.RGBA32,
                            compressionQuality = 100
                        });
                }
                else
                {
                    originalMaxTextureSize = originalTextureImporter.maxTextureSize;
                    originalTextureImporter.maxTextureSize = targetMaxTextureSize;
                }
                EditorUtility.SetDirty(originalTextureImporter);
                originalTextureImporter.SaveAndReimport();

                // Duplicate texture
                Texture2D newTexture = TextureUtilities.DuplicateTexture(textureOriginal, _resizeToMultipleOfFour);

                // Save New Texture to Disc
                TextureUtilities.SaveTextureToDisk(newTexture, _exportSettings, originalTextureImporterSettings, _platformSettings);

                if (_saveOriginalTextures)
                {
                    // Set back to original settings
                    originalTextureImporter.textureType = originalTextureType;
                    originalTextureImporter.sRGBTexture = originalsRGB;
                    if (hasPlatformSettings)
                    {
                        originalTextureImporter.SetPlatformTextureSettings(
                            new TextureImporterPlatformSettings() {
                                name = platform,
                                overridden = true,
                                maxTextureSize = originalMaxTextureSize,
                                format = textureFormat,
                                compressionQuality = compressionQuality
                            });
                    }
                    else
                    {
                        originalTextureImporter.maxTextureSize = originalMaxTextureSize;
                    }
                    EditorUtility.SetDirty(originalTextureImporter);
                    originalTextureImporter.SaveAndReimport();
                }

                string newExtension = _exportSettings.format == TextureUtilities.Format.PNG ? ".png" : ".jpg";
                if (extension != newExtension) // new extension
                {
                    // Get GUID of .meta file and replace it
                    string metaFilePath = path + ".meta";
                    string newGUID = GUID.Generate().ToString();
                    string originalTextureGUID = GetAndReplaceTextureGUID(metaFilePath, newGUID);

                    // Replace GUID in .meta file
                    string newMetaPath = _exportSettings.path + ".meta";
                    ReplaceTextureGUID(newMetaPath, originalTextureGUID);

                    if (!_saveOriginalTextures)
                    {
                        // Delete original texture
                        AssetDatabase.DeleteAsset(path);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"TextureOptimizer Exception: {e}");
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    private static string GetTextureGUID(string metaFilePath)
    {
        // Find the line containing the GUID
        string[] metaLines = File.ReadAllLines(metaFilePath);
        int guidLineIndex = Array.FindIndex(metaLines, line => line.StartsWith("guid:"));
        return guidLineIndex != -1 ? metaLines[guidLineIndex].Substring(6) : null;
    }

    private static void ReplaceTextureGUID(string metaFilePath, string newGUID)
    {
        // string metaContent = File.ReadAllText(metaPath);
        // metaContent = metaContent.Replace(metaGUID, newGUID);
        // File.WriteAllText(metaPath, metaContent);

        // Find the line containing the GUID
        string[] metaLines = File.ReadAllLines(metaFilePath);
        int guidLineIndex = Array.FindIndex(metaLines, line => line.StartsWith("guid:"));
        if (guidLineIndex != -1)
        {
            metaLines[guidLineIndex] = "guid: " + newGUID;
            File.WriteAllLines(metaFilePath, metaLines);
        }
    }

    private static string GetAndReplaceTextureGUID(string metaFilePath, string newGUID)
    {
        // Find the line containing the GUID
        string[] metaLines = File.ReadAllLines(metaFilePath);
        int guidLineIndex = Array.FindIndex(metaLines, line => line.StartsWith("guid:"));
        string originalGUID = guidLineIndex != -1 ? metaLines[guidLineIndex].Substring(6) : null;
        if (guidLineIndex != -1)
        {
            metaLines[guidLineIndex] = "guid: " + newGUID;
            File.WriteAllLines(metaFilePath, metaLines);
        }
        return originalGUID;
    }

    private static void ChangeTextureImporterSettings(List<Texture2D> textures)
    {
        int currentTexIndex = 0; // For Progress Bar
        try
        {
            foreach (Texture2D texture in textures)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                string folder = Path.GetDirectoryName(path) + "/";
                string textureName = texture.name;
                string extension = Path.GetExtension(path).ToLower();

                EditorUtility.DisplayProgressBar("Applying new TextureImporter settings, please wait...", $"{texture.name}\n{path}", (float)currentTexIndex++ / textures.Count);

                TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(path);

                bool isHDR = extension == ".hdr" || extension == ".exr";

                // Unity use alpha to encode HDR data
                bool hasAlpha =
                    textureImporter.DoesSourceTextureHaveAlpha() ||
                    textureImporter.alphaSource == TextureImporterAlphaSource.FromGrayScale ||
                    isHDR;

                // Get settings
                TextureSize textureSizeStandalone = _applyMaxImportTextureSizePerPlatform ? _maxImportTextureSizeStandalone : _maxImportTextureSize;
                TextureSize textureSizeAndroid = _applyMaxImportTextureSizePerPlatform ? _maxImportTextureSizeAndroid : _maxImportTextureSize;
                TextureSize textureSizeIOS = _applyMaxImportTextureSizePerPlatform ? _maxImportTextureSizeIOS : _maxImportTextureSize;
                TextureSize textureSizeWebGL = _applyMaxImportTextureSizePerPlatform ? _maxImportTextureSizeWebGL : _maxImportTextureSize;

                TextureImporterFormat textureFormatStandalone = TextureUtilities.GetAdjustedTextureImporterFormat(_textureFormatStandalone, hasAlpha, isHDR);
                TextureImporterFormat textureFormatAndroid = TextureUtilities.GetAdjustedTextureImporterFormat(_textureFormatAndroid, hasAlpha, isHDR);
                TextureImporterFormat textureFormatIOS = TextureUtilities.GetAdjustedTextureImporterFormat(_textureFormatIOS, hasAlpha, isHDR);
                TextureImporterFormat textureFormatWebGL = TextureUtilities.GetAdjustedTextureImporterFormat(_textureFormatWebGL, hasAlpha, isHDR);

                // Apply settings
                textureImporter.maxTextureSize = (int)_maxImportTextureSize;
                textureImporter.SetPlatformTextureSettings(GetTextureImporterSettings("Standalone", (int)textureSizeStandalone, textureFormatStandalone));
                textureImporter.SetPlatformTextureSettings(GetTextureImporterSettings("Android", (int)textureSizeAndroid, textureFormatAndroid));
                textureImporter.SetPlatformTextureSettings(GetTextureImporterSettings("iOS", (int)textureSizeIOS, textureFormatIOS));
                textureImporter.SetPlatformTextureSettings(GetTextureImporterSettings("WebGL", (int)textureSizeWebGL, textureFormatWebGL));

                UnityEditor.EditorUtility.SetDirty(textureImporter);
                textureImporter.SaveAndReimport();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        EditorUtility.ClearProgressBar();
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

    private string RemoveBefore(string value, string character)
    {
        int index = value.IndexOf(character);
        if (index > 0)
        {
            value = value.Substring(index);
        }
        return value;
    }
}
