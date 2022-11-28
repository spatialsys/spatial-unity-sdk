using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialSDKConfigWindow : EditorWindow
    {
        private static readonly string ACCESS_TOKEN_URL = $"https://{SpatialAPI.SPATIAL_ORIGIN}/account";

        private const string CONFIG_TAB_NAME = "config";
        private const string CONFIG_EXISTS_SELECTOR_NAME = "configExists";
        private const string CONFIG_NULL_SELECTOR_NAME = "configNull";

        private string _tab;
        private string _authToken;

        public static void OpenWindow(string startingTab)
        {
            SpatialSDKConfigWindow wnd = GetWindow<SpatialSDKConfigWindow>("SpatialSDK");
            wnd.minSize = new Vector2(640, 480);
            wnd.SetTab(startingTab);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/io.spatial.unitysdk/Editor/Scripts/GUI/ConfigWindow/SpatialSDKConfigWindow.uxml");
            VisualElement element = visualTree.Instantiate();
            root.Add(element);

            root.Query<Button>("accountButton").First().clicked += () => SetTab("account");
            root.Query<Button>("configButton").First().clicked += () => SetTab(CONFIG_TAB_NAME);
            root.Query<Button>("helpButton").First().clicked += () => SetTab("help");

            // Account
            root.Query<Button>("getToken").First().clicked += () => Application.OpenURL(ACCESS_TOKEN_URL);

            // Config
            root.Query<Button>("createConfig").First().clicked += CreateAndBindConfigAsset;

            // Help
            root.Query<Button>("gotoDocumentation").First().clicked += () => Application.OpenURL(UpgradeUtility.packageInfo.documentationUrl);
            root.Query<Button>("gotoSupport").First().clicked += () => Application.OpenURL("https://support.spatial.io/hc/en-us");
            root.Query<Button>("gotoDiscord").First().clicked += () => Application.OpenURL("https://discord.com/invite/spatial");

            if (PackageConfig.instance != null)
                root.Bind(new SerializedObject(PackageConfig.instance));

            _authToken = EditorUtility.GetSavedAuthToken();
            if (string.IsNullOrEmpty(_authToken))
            {
                _authToken = "";
            }
            root.Query<TextField>("authField").First().value = _authToken;
            root.Query<TextField>("authField").First().RegisterValueChangedCallback<string>((e) => SetAuthToken(e.newValue));
            UpdateAuthWarning();
        }

        private void Update()
        {
            // Constantly refresh every repaint since the config asset can be deleted while the window is open.
            if (_tab == CONFIG_TAB_NAME)
                UpdateConfigTabContents();
        }

        public void SetTab(string tab)
        {
            _tab = tab;

            // Tab contents
            rootVisualElement.Q("account").style.display = DisplayStyle.None;
            rootVisualElement.Q(CONFIG_TAB_NAME).style.display = DisplayStyle.None;
            rootVisualElement.Q("help").style.display = DisplayStyle.None;

            rootVisualElement.Q(tab).style.display = DisplayStyle.Flex;

            // There are two states for the config tab, which depends on whether the config exists or not.
            if (tab == CONFIG_TAB_NAME)
                UpdateConfigTabContents();

            // Tab buttons
            rootVisualElement.Q("accountButton").RemoveFromClassList("tabButtonSelected");
            rootVisualElement.Q("configButton").RemoveFromClassList("tabButtonSelected");
            rootVisualElement.Q("helpButton").RemoveFromClassList("tabButtonSelected");

            rootVisualElement.Q(tab + "Button").AddToClassList("tabButtonSelected");
        }

        private void UpdateConfigTabContents()
        {
            rootVisualElement.Q(CONFIG_EXISTS_SELECTOR_NAME).style.display = (PackageConfig.instance != null) ? DisplayStyle.Flex : DisplayStyle.None;
            rootVisualElement.Q(CONFIG_NULL_SELECTOR_NAME).style.display = (PackageConfig.instance == null) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void SetAuthToken(string token)
        {
            _authToken = token;
            EditorUtility.SaveAuthToken(token);
            UpdateAuthWarning();
        }

        private void UpdateAuthWarning()
        {
            bool validAuthToken = _authToken.StartsWith("sandbox_") && _authToken.Length > 16;
            rootVisualElement.Q("authWarning").style.display = validAuthToken ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void CreateAndBindConfigAsset()
        {
            PackageConfig config = EditorUtility.CreateOrGetConfigurationFile();
            rootVisualElement.Bind(new SerializedObject(config));
        }
    }
}
