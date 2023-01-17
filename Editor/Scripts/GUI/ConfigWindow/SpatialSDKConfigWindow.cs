using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using UnityEditor.SceneManagement;
using System;

namespace SpatialSys.UnitySDK.Editor
{
    [InitializeOnLoad]
    public class SpatialSDKConfigWindow : EditorWindow
    {
        private static readonly string ACCESS_TOKEN_URL = $"https://{SpatialAPI.SPATIAL_ORIGIN}/account";
        private static bool _isOpen = false;

        private const string CONFIG_TAB_NAME = "config";
        private const string PROJECT_CONFIG_NULL_ELEMENT_NAME = "configNull";
        private const string PROJECT_CONFIG_ELEMENT_NAME = "projectConfig";
        private const string PACKAGE_CONFIG_ELEMENT_NAME = "packageConfig";

        private string _tab;
        private string _authToken;

        // Config Tab Elements
        private DropdownField _configActivePackageDropdown;
        private EnumField _configCreatePackageTypeDropdown;
        private VisualElement _packageConfigSKUEmptyElement;
        private VisualElement _packageConfigSKUElement;

        // Issue Tab Elements
        private VisualTreeAsset _issueContainerTemplate;
        private VisualElement _issueListParent;
        private VisualElement _selectedIssue;
        private List<VisualElement> _errorIssues;
        private List<VisualElement> _warningIssues;
        private Dictionary<VisualElement, SpatialTestResponse> _elementToReponse;

        private VisualElement _issuesCountBlock;
        private Label _issuesCountTitle;
        private Label _issuesCountDescription;
        private Button _issuesRefreshButton;

        private VisualElement _issuesScrollBlock;

        private VisualElement _selectedIssueBlock;
        private Label _selectedIssueTextTitle;
        private Label _selectedIssueTextDescription;

        private VisualElement _autoFixBlock;
        private Label _autoFixDescription;

        private VisualElement _openSceneBlock;
        private Label _targetSceneName;
        private VisualElement _selectObjectBlock;
        private Label _targetObjectName;

        public static void OpenWindow(string startingTab)
        {
            SpatialSDKConfigWindow wnd = GetWindow<SpatialSDKConfigWindow>("Spatial Portal");
            wnd.minSize = new Vector2(640, 480);
            wnd.SetTab(startingTab);
        }

        static SpatialSDKConfigWindow()
        {
            if (_isOpen)
                EditorApplication.update += DelayedInitialize;
        }

        private void OnEnable()
        {
            _isOpen = true;
        }

        private void OnDisable()
        {
            _isOpen = false;
        }

        // Needed to refresh the UI bindings after recompile
        private static void DelayedInitialize()
        {
            EditorApplication.update -= DelayedInitialize;
            EditorWindow.GetWindow<SpatialSDKConfigWindow>().Refresh();
        }

        public void Refresh()
        {
            SetTab(_tab);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/io.spatial.unitysdk/Editor/Scripts/GUI/ConfigWindow/SpatialSDKConfigWindow.uxml");
            _issueContainerTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/io.spatial.unitysdk/Editor/Scripts/GUI/ConfigWindow/IssueElement/SpatialValidatorIssueElement.uxml");
            VisualElement element = visualTree.Instantiate();
            if (!EditorGUIUtility.isProSkin)
            {
                element.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/io.spatial.unitysdk/Editor/Scripts/GUI/ConfigWindow/SpatialSDKConfigWindowStyles_LightModeOverride.uss"));
            }
            root.Add(element);

            root.Query<Button>("accountButton").First().clicked += () => SetTab("account");
            root.Query<Button>("configButton").First().clicked += () => SetTab(CONFIG_TAB_NAME);
            root.Query<Button>("issuesButton").First().clicked += () => SetTab("issues");
            root.Query<Button>("helpButton").First().clicked += () => SetTab("help");

            // Account
            root.Query<Button>("getToken").First().clicked += () => Application.OpenURL(ACCESS_TOKEN_URL);
            root.Query<Button>("pasteToken").First().clicked += () => PasteAuthToken();

            // Config 
            root.Query<Button>("newConfigButton").First().clicked += () => {
                ProjectConfig.Create();
                root.Bind(new SerializedObject(ProjectConfig.activePackage));
            };
            _configActivePackageDropdown = root.Q<DropdownField>("packageConfigDropDown");
            _configActivePackageDropdown.RegisterValueChangedCallback(evt => {
                int index = _configActivePackageDropdown.choices.IndexOf(evt.newValue);
                ProjectConfig.activePackageIndex = index;
                UnityEditor.EditorUtility.SetDirty(ProjectConfig.instance);
                rootVisualElement.Bind(new SerializedObject(ProjectConfig.activePackage));
            });
            _configCreatePackageTypeDropdown = root.Q<EnumField>("createPackageTypeDropdown");
            root.Q<Button>("createPackageButton").clicked += () => {
                var package = ProjectConfig.AddNewPackage((PackageType)_configCreatePackageTypeDropdown.value);
                ProjectConfig.SetActivePackage(package);
            };
            root.Q<Button>("skuCopyButton").clicked += () => {
                EditorGUIUtility.systemCopyBuffer = ProjectConfig.activePackage.sku;
            };
            root.Q<Button>("publishPackageButton").clicked += () => {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    UnityEditor.EditorUtility.DisplayDialog("Publish Package", "Cannot publish package while in play mode.", "Ok");
                    return;
                }

                UpgradeUtility.PerformUpgradeIfNecessaryForTestOrPublish()
                    .Then(() => {
                        return BuildUtility.PackageForPublishing();
                    })
                    .Catch(exc => {
                        if (exc is RSG.PromiseCancelledException)
                            return;

                        UnityEditor.EditorUtility.DisplayDialog("Publishing Error", $"There was an unexpected error while publishing your space.\n\n{exc.Message}", "OK");
                        Debug.LogException(exc);
                    });
            };
            root.Q<Button>("deletePackageButton").clicked += () => {
                if (UnityEditor.EditorUtility.DisplayDialog("Delete Package", "Are you sure you want to delete this package?", "Yes", "No"))
                    ProjectConfig.RemovePackage(ProjectConfig.activePackage);
            };
            _packageConfigSKUEmptyElement = root.Q(PACKAGE_CONFIG_ELEMENT_NAME).Q("packageSKUEmpty");
            _packageConfigSKUElement = root.Q(PACKAGE_CONFIG_ELEMENT_NAME).Q("packageSKU");

            // Issues
            _issuesCountBlock = root.Q("issuesCountBlock");
            _issuesCountTitle = root.Query<Label>("issuesCountTitle").First();
            _issuesCountDescription = root.Query<Label>("issuesCountDescription").First();
            _issuesRefreshButton = root.Query<Button>("refreshButton").First();
            _issuesRefreshButton.clicked += () => RefreshIssuesButton();

            _issuesScrollBlock = root.Q("issuesScrollBlock");

            _selectedIssueBlock = root.Q("selectedIssueBlock");
            _selectedIssueTextTitle = _selectedIssueBlock.Query<Label>("title").First();
            _selectedIssueTextDescription = _selectedIssueBlock.Query<Label>("description").First();

            _autoFixBlock = root.Q("autoFixBlock");
            _autoFixDescription = root.Query<Label>("autoFixDescription").First();
            root.Query<Button>("autoFixButton").First().clicked += () => FixSelectedIssue();

            _openSceneBlock = root.Q("openSceneBlock");
            _targetSceneName = root.Query<Label>("targetSceneName").First();
            root.Query<Button>("openSceneButton").First().clicked += () => OpenSelectedIssueScene();
            _selectObjectBlock = root.Q("selectObjectBlock");
            _targetObjectName = root.Query<Label>("targetObjectName").First();
            root.Query<Button>("selectObjectButton").First().clicked += () => OpenSelectedIssueGameObject();

            // Help
            root.Query<Button>("gotoDocumentation").First().clicked += () => Application.OpenURL(UpgradeUtility.packageInfo.documentationUrl);
            root.Query<Button>("gotoSupport").First().clicked += () => Application.OpenURL("https://support.spatial.io/hc/en-us");
            root.Query<Button>("gotoDiscord").First().clicked += () => Application.OpenURL("https://discord.com/invite/spatial");

            _issueListParent = root.Q("issuesScroll");

            if (ProjectConfig.activePackage != null)
                root.Bind(new SerializedObject(ProjectConfig.activePackage));

            _authToken = EditorUtility.GetSavedAuthToken();
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
            {
                UpdateConfigTabContents();

                _configActivePackageDropdown.index = ProjectConfig.activePackageIndex;
            }

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
            rootVisualElement.Q(PROJECT_CONFIG_NULL_ELEMENT_NAME).style.display = (ProjectConfig.instance == null) ? DisplayStyle.Flex : DisplayStyle.None;
            rootVisualElement.Q(PROJECT_CONFIG_ELEMENT_NAME).style.display = (ProjectConfig.instance != null) ? DisplayStyle.Flex : DisplayStyle.None;
            rootVisualElement.Q(PACKAGE_CONFIG_ELEMENT_NAME).style.display = (ProjectConfig.activePackage != null) ? DisplayStyle.Flex : DisplayStyle.None;

            // Active config dropdown
            _configActivePackageDropdown.SetEnabled(ProjectConfig.hasPackages);
            if (_configActivePackageDropdown.enabledSelf)
            {
                Func<PackageConfig, string> getPackageDropdownLabel = (PackageConfig config) => config.packageName;

                // Update elements
                if (_configActivePackageDropdown.choices == null || _configActivePackageDropdown.choices.Count != ProjectConfig.packages.Count)
                    _configActivePackageDropdown.choices = ProjectConfig.packages.Select(getPackageDropdownLabel).ToList();
                _configActivePackageDropdown.index = ProjectConfig.activePackageIndex;

                // Update the selected label
                if (ProjectConfig.activePackage != null)
                    _configActivePackageDropdown.choices[ProjectConfig.activePackageIndex] = getPackageDropdownLabel(ProjectConfig.activePackage);
            }

            // Active Package
            if (ProjectConfig.activePackage != null)
            {
                _packageConfigSKUEmptyElement.style.display = (string.IsNullOrEmpty(ProjectConfig.activePackage.sku)) ? DisplayStyle.Flex : DisplayStyle.None;
                _packageConfigSKUElement.style.display = (string.IsNullOrEmpty(ProjectConfig.activePackage.sku)) ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        private void PasteAuthToken()
        {
            string clipboard = GUIUtility.systemCopyBuffer;

            if (string.IsNullOrWhiteSpace(clipboard) || !clipboard.StartsWith("sandbox"))
            {
                Debug.LogError("Invalid access token, try clicking copy to clipboard again. Pasted text: " + clipboard);
                return;
            }
            _authToken = clipboard;
            EditorUtility.SaveAuthToken(clipboard);
            UpdateAuthWarning();
            GUIUtility.systemCopyBuffer = "";
        }

        private void UpdateAuthWarning()
        {
            bool validAuthToken = _authToken.StartsWith("sandbox_") && _authToken.Length > 16;
            rootVisualElement.Q("notLoggedInBlock").style.display = validAuthToken ? DisplayStyle.None : DisplayStyle.Flex;
            rootVisualElement.Q("loggedInBlock").style.display = validAuthToken ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /////////////////////////////////////////
        //               Issues
        /////////////////////////////////////////

        private void RefreshIssuesButton()
        {
            SpatialValidator.RunTestsOnProject(ValidationContext.ManualRun);
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
                if (EditorStyles.label.normal.textColor.r < .5f)//are we in light mode?
                {
                    element.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/io.spatial.unitysdk/Editor/Scripts/GUI/ConfigWindow/SpatialValidatorWindow_LightModeOverride.uss"));
                }
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

            _issuesCountBlock.ClearClassList();
            _issuesRefreshButton.ClearClassList();
            _issuesCountBlock.AddToClassList("block_base");
            _issuesRefreshButton.AddToClassList("blockButton");
            if (_errorIssues.Count == 0 && _warningIssues.Count == 0)
            {
                _issuesCountTitle.text = "No issues found!";
                _issuesCountDescription.text = "You're good to go!";
                _issuesCountBlock.AddToClassList("block_green");
                _issuesRefreshButton.AddToClassList("blockButton_green");
                _issuesScrollBlock.style.display = DisplayStyle.None;
                _selectedIssueBlock.style.display = DisplayStyle.None;
            }
            else if (_errorIssues.Count > 0)
            {
                _issuesCountTitle.text = "<color=#FF4343>" + _errorIssues.Count.ToString() + " Errors</color> <color=#FFEA80>" + _warningIssues.Count.ToString() + " Warnings</color>";
                _issuesCountDescription.text = "You can't upload your environment to the sandbox or publish until the errors are resolved.";
                _issuesCountBlock.AddToClassList("block_red");
                _issuesRefreshButton.AddToClassList("blockButton_red");
                _issuesScrollBlock.style.display = DisplayStyle.Flex;
                _selectedIssueBlock.style.display = DisplayStyle.Flex;
            }
            else
            {
                _issuesCountTitle.text = "<color=#FFEA80>" + _warningIssues.Count.ToString() + " Warnings</color>";
                _issuesCountDescription.text = "Warnings will not prevent you from publishing, but you should still look to resolve them as your environment may not work as expected.";
                _issuesCountBlock.AddToClassList("block_yellow");
                _issuesRefreshButton.AddToClassList("blockButton_yellow");
                _issuesScrollBlock.style.display = DisplayStyle.Flex;
                _selectedIssueBlock.style.display = DisplayStyle.Flex;
            }

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

            _selectedIssueBlock.ClearClassList();
            _selectedIssueBlock.AddToClassList("block_base");

            if (element == null || !_elementToReponse.TryGetValue(element, out SpatialTestResponse response))
            {
                // No selection
                _selectedIssueTextTitle.text = "No issue selected";
                _selectedIssueTextDescription.text = "Choose an issue from the list above to see more information about it and execute auto fixes";
                _autoFixBlock.style.display = DisplayStyle.None;
                _selectObjectBlock.style.display = DisplayStyle.None;
                _openSceneBlock.style.display = DisplayStyle.None;
            }
            else
            {
                _selectedIssueBlock.AddToClassList(response.responseType == TestResponseType.Fail ? "block_red" : "block_yellow");
                _selectedIssueTextTitle.text = response.title;
                _selectedIssueTextDescription.text = response.description;

                _openSceneBlock.style.display = !string.IsNullOrEmpty(response.scenePath) ? DisplayStyle.Flex : DisplayStyle.None;
                _targetSceneName.text = response.scenePath;

                if (response.targetObject != null || response.targetObjectGlobalID.HasValue)
                {
                    _selectObjectBlock.style.display = DisplayStyle.Flex;
                    if (response.targetObject != null)
                    {
                        _targetObjectName.text = response.targetObject.name;
                    }
                    else
                    {
                        _targetObjectName.text = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(response.targetObjectGlobalID.Value).name;
                    }
                }
                else
                {
                    _selectObjectBlock.style.display = DisplayStyle.None;
                }

                if (response.hasAutoFix)
                {
                    _autoFixBlock.style.display = DisplayStyle.Flex;
                    _autoFixDescription.text = response.autoFixDescription;
                }
                else
                {
                    _autoFixBlock.style.display = DisplayStyle.None;
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
