using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using UnityEditor.SceneManagement;

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

        private VisualTreeAsset _issueContainerTemplate;
        private VisualElement _issueListParent;
        private VisualElement _selectedIssue;
        private List<VisualElement> _errorIssues;
        private List<VisualElement> _warningIssues;
        private Dictionary<VisualElement, SpatialTestResponse> _elementToReponse;

        private VisualElement _selectedIssueText;
        private Label _selectedIssueTextTitle;
        private Label _selectedIssueTextDescription;

        private VisualElement _safeFixSection;
        private Label _safeFixDescription;
        private VisualElement _unsafeFixSection;
        private Label _unsafeFixDescription;

        private Button _openSceneButton;
        private Button _openObjectButton;

        public static void OpenWindow(string startingTab)
        {
            SpatialSDKConfigWindow wnd = GetWindow<SpatialSDKConfigWindow>("Spatial Portal");
            wnd.minSize = new Vector2(640, 480);
            wnd.SetTab(startingTab);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/io.spatial.unitysdk/Editor/Scripts/GUI/ConfigWindow/SpatialSDKConfigWindow.uxml");
            _issueContainerTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/io.spatial.unitysdk/Editor/Scripts/GUI/ConfigWindow/IssueElement/SpatialValidatorIssueElement.uxml");
            VisualElement element = visualTree.Instantiate();
            root.Add(element);

            root.Query<Button>("accountButton").First().clicked += () => SetTab("account");
            root.Query<Button>("configButton").First().clicked += () => SetTab(CONFIG_TAB_NAME);
            root.Query<Button>("issuesButton").First().clicked += () => SetTab("issues");
            root.Query<Button>("helpButton").First().clicked += () => SetTab("help");

            // Account
            root.Query<Button>("getToken").First().clicked += () => Application.OpenURL(ACCESS_TOKEN_URL);

            // Config
            root.Query<Button>("createConfig").First().clicked += CreateAndBindConfigAsset;

            // Issues
            root.Query<Button>("refreshButton").First().clicked += () => RefreshIssuesButton();
            _selectedIssueText = root.Q("selectedIssueText");
            _selectedIssueTextTitle = _selectedIssueText.Query<Label>("title").First();
            _selectedIssueTextDescription = _selectedIssueText.Query<Label>("description").First();
            _openSceneButton = root.Query<Button>("openSceneButton").First();
            _openSceneButton.clicked += () => OpenSelectedIssueScene();
            _openObjectButton = root.Query<Button>("selectObjectButton").First();
            _openObjectButton.clicked += () => OpenSelectedIssueGameObject();

            // These are different elements because swapping the style would have been harder. I would need to apply style to multiple elements :(
            _safeFixSection = root.Q("safeAutoFixSection");
            _unsafeFixSection = root.Q("unsafeAutoFixSection");
            _safeFixDescription = root.Query<Label>("safeAutoFixDescription").First();
            _unsafeFixDescription = root.Query<Label>("unsafeAutoFixDescription").First();
            root.Query<Button>("safeAutoFixButton").First().clicked += () => FixSelectedIssue();
            root.Query<Button>("unsafeAutoFixButton").First().clicked += () => FixSelectedIssue();

            // Help
            root.Query<Button>("gotoDocumentation").First().clicked += () => Application.OpenURL(UpgradeUtility.packageInfo.documentationUrl);
            root.Query<Button>("gotoSupport").First().clicked += () => Application.OpenURL("https://support.spatial.io/hc/en-us");
            root.Query<Button>("gotoDiscord").First().clicked += () => Application.OpenURL("https://discord.com/invite/spatial");

            _issueListParent = root.Q("issuesScroll");

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
            rootVisualElement.Q("issues").style.display = DisplayStyle.None;
            rootVisualElement.Q("help").style.display = DisplayStyle.None;

            rootVisualElement.Q(tab).style.display = DisplayStyle.Flex;

            // There are two states for the config tab, which depends on whether the config exists or not.
            if (tab == CONFIG_TAB_NAME)
                UpdateConfigTabContents();

            if (tab == "issues")
            {
                LoadIssues();
            }

            // Tab buttons
            rootVisualElement.Q("accountButton").RemoveFromClassList("tabButtonSelected");
            rootVisualElement.Q("configButton").RemoveFromClassList("tabButtonSelected");
            rootVisualElement.Q("issuesButton").RemoveFromClassList("tabButtonSelected");
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

        /////////////////////////////////////////
        //               Issues
        /////////////////////////////////////////

        private void RefreshIssuesButton()
        {
            SpatialValidator.RunTestsOnProject();
            LoadIssues();
        }

        private void LoadIssues()
        {
            _issueListParent.Clear();
            _selectedIssue = null;
            SelectIssue(null);
            List<SpatialTestResponse> responses = SpatialValidator.allResponses;
            _warningIssues = new List<VisualElement>();
            _errorIssues = new List<VisualElement>();
            _elementToReponse = new Dictionary<VisualElement, SpatialTestResponse>();

            foreach (SpatialTestResponse response in responses)
            {
                VisualElement element = _issueContainerTemplate.Instantiate();
                // It's important to note that when this tree is instantiated it has an extra "root" element that we can't really see inside the UI-builder.
                // So bellow, "issueContainer" != element, it's a child of this extra root element... 

                element.Q(response.responseType == TestResponseType.Fail ? "errorIcon" : "warningIcon").style.display = DisplayStyle.Flex;
                _issueListParent.Add(element);

                Button button = element.Query<Button>("issueContainer").First();
                button.AddToClassList(response.responseType == TestResponseType.Fail ? "issueContainerError" : "issueContainerWarning");
                button.clicked += () => SelectIssue(button);
                button.Query<Label>("issueLabel").First().text = response.title;

                _elementToReponse.Add(button, response);

                if (response.responseType == TestResponseType.Fail)
                {
                    _errorIssues.Add(button);
                }
                else
                {
                    _warningIssues.Add(button);
                }
            }

            rootVisualElement.Query<Button>("errorsButton").First().text = _errorIssues.Count.ToString() + " Errors";
            rootVisualElement.Query<Button>("warningsButton").First().text = _warningIssues.Count.ToString() + " Warnings";

            if (_errorIssues.Count > 0)
            {
                SelectIssue(_errorIssues[0]);
            }
            else if (_warningIssues.Count > 0)
            {
                SelectIssue(_warningIssues[0]);
            }
        }

        private void SelectIssue(VisualElement element)
        {
            if (element != null)
            {
                if (_selectedIssue != null)
                {
                    _selectedIssue.RemoveFromClassList("issueContainerSelected");
                }
                element.AddToClassList("issueContainerSelected");
                _selectedIssue = element;
            }

            _selectedIssueText.ClearClassList();

            if (element == null || !_elementToReponse.TryGetValue(element, out SpatialTestResponse response))
            {
                // No selection
                _selectedIssueText.AddToClassList("selectedIssueGray");
                _selectedIssueTextTitle.text = "No issue selected";
                _selectedIssueTextDescription.text = "Choose an issue from the list above to see more information about it and execute auto fixes";
                _unsafeFixSection.style.display = DisplayStyle.None;
                _safeFixSection.style.display = DisplayStyle.None;
                _openObjectButton.style.display = DisplayStyle.None;
                _openSceneButton.style.display = DisplayStyle.None;
            }
            else
            {
                _selectedIssueText.AddToClassList(response.responseType == TestResponseType.Fail ? "selectedIssueRed" : "selectedIssueYellow");
                _selectedIssueTextTitle.text = response.title;
                _selectedIssueTextDescription.text = response.description;

                _openSceneButton.style.display = !string.IsNullOrEmpty(response.scenePath) ? DisplayStyle.Flex : DisplayStyle.None;

                if (response.targetObject != null || response.targetObjectGlobalID.HasValue)
                {
                    _openObjectButton.style.display = DisplayStyle.Flex;
                }
                else
                {
                    _openObjectButton.style.display = DisplayStyle.None;
                }

                if (response.hasAutoFix)
                {
                    _unsafeFixSection.style.display = response.autoFixIsSafe ? DisplayStyle.None : DisplayStyle.Flex;
                    _safeFixSection.style.display = response.autoFixIsSafe ? DisplayStyle.Flex : DisplayStyle.None;
                    _unsafeFixDescription.text = response.autoFixDescription;
                    _safeFixDescription.text = response.autoFixDescription;
                }
                else
                {
                    _unsafeFixSection.style.display = DisplayStyle.None;
                    _safeFixSection.style.display = DisplayStyle.None;
                }
            }
        }

        private void FixSelectedIssue()
        {
            if (_elementToReponse.TryGetValue(_selectedIssue, out SpatialTestResponse response))
            {
                response.InvokeFix();//this might switch the scene.
                if (response.responseType == TestResponseType.Fail)
                {
                    _errorIssues.Remove(_selectedIssue);
                    rootVisualElement.Query<Button>("errorsButton").First().text = _errorIssues.Count.ToString() + " Errors";
                }
                else
                {
                    _warningIssues.Remove(_selectedIssue);
                    rootVisualElement.Query<Button>("warningsButton").First().text = _warningIssues.Count.ToString() + " Warnings";
                }
                _elementToReponse.Remove(_selectedIssue);
                _issueListParent.Remove(_selectedIssue.parent);
                if (_elementToReponse.Keys.Count > 0)
                {
                    SelectIssue(_elementToReponse.Keys.ElementAt(0));
                }
                else
                {
                    SelectIssue(null);
                }
            }
        }

        private void OpenSelectedIssueScene()
        {
            if (!_elementToReponse.TryGetValue(_selectedIssue, out SpatialTestResponse response))
            {
                return;
            }

            if (EditorSceneManager.GetActiveScene().path == response.scenePath || string.IsNullOrEmpty(response.scenePath))
            {
                return;
            }
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene(response.scenePath, OpenSceneMode.Single);
        }

        private void OpenSelectedIssueGameObject()
        {
            if (!_elementToReponse.TryGetValue(_selectedIssue, out SpatialTestResponse response))
            {
                return;
            }
            OpenSelectedIssueScene();

            if (response.targetObject == null)
            {
                response.targetObject = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(response.targetObjectGlobalID.Value);
            }
            if (response.targetObject == null)
            {
                return;
            }

            Selection.activeObject = response.targetObject;
        }
    }
}
