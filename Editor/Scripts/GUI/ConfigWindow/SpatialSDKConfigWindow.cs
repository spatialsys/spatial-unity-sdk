using RSG;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpatialSys.UnitySDK.Editor
{
    [InitializeOnLoad]
    public class SpatialSDKConfigWindow : EditorWindow
    {
        private static readonly string ACCESS_TOKEN_URL = $"https://{SpatialAPI.SPATIAL_ORIGIN}/studio/account";
        private string spatialStudioURL => $"https://{SpatialAPI.SPATIAL_ORIGIN}/studio/worlds/{ProjectConfig.defaultWorldID}";

        private const string SUPPORT_URL = "https://support.spatial.io/hc/en-us";
        private const string DISCORD_URL = "https://discord.com/invite/spatial";
        private const string FORUM_URL = "https://github.com/spatialsys/spatial-unity-sdk/discussions";

        private static bool _isOpen = false;

        public const string ACCOUNT_TAB_NAME = "account";
        public const string CONFIG_TAB_NAME = "config";
        public const string ISSUES_TAB_NAME = "issues";
        public const string UTILITIES_TAB_NAME = "utilities";
        public const string HELP_TAB_NAME = "help";

        private const string PROJECT_CONFIG_NULL_ELEMENT_NAME = "configNull";
        private const string PROJECT_CONFIG_ELEMENT_NAME = "projectConfig";
        private const string PACKAGE_CONFIG_ELEMENT_NAME = "packageConfig";

        private string _tab = CONFIG_TAB_NAME;
        private SpatialValidationSummary _issuesValidationSummary;

        // Config Tab Elements
        private DropdownField _configActivePackageDropdown;
        private DropdownField _configDefaultWorldSelectionDropdown;
        private EnumField _configCreatePackageTypeDropdown;
        private TextField _packageConfigName;
        private VisualElement _packageConfigSKUEmptyElement;
        private VisualElement _packageConfigSKUElement;
        private Label _configPackageType;
        private Button _publishPackageButton;

        // Issue Tab Elements
        private const string _issuesContainerTemplatePath = "Editor/Scripts/GUI/ConfigWindow/IssueElement/SpatialValidatorIssueElement.uxml";
        private VisualTreeAsset issueContainerTemplate
        {
            get
            {
                if (_issueContainerTemplate == null)
                {
                    _issueContainerTemplate = EditorUtility.LoadAssetFromPackagePath<VisualTreeAsset>(_issuesContainerTemplatePath);
                    if (_issueContainerTemplate == null)
                    {
                        Debug.LogError("Issue Template could not be found. Your Spatial SDK installation may be corrupted.");
                    }
                }
                return _issueContainerTemplate;
            }
        }
        private VisualTreeAsset _issueContainerTemplate;
        private VisualElement _issueListParent;
        private VisualElement _selectedIssue;
        private DropdownField _issuesActivePackageDropdown;
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

        public static SpatialSDKConfigWindow OpenWindow()
        {
            var window = GetWindow<SpatialSDKConfigWindow>("Spatial Portal");
            window.minSize = new Vector2(640, 480);
            return window;
        }

        public static void OpenWindow(string startingTab)
        {
            SpatialSDKConfigWindow window = OpenWindow();
            window.SetTab(startingTab);
        }

        public static void OpenIssuesTabWithSummary(SpatialValidationSummary validationSummary)
        {
            SpatialSDKConfigWindow window = OpenWindow();
            window._issuesValidationSummary = validationSummary;
            window.SetTab(ISSUES_TAB_NAME);
        }

        static SpatialSDKConfigWindow()
        {
            EditorApplication.update += DelayedInitialize;
        }

        private void OnEnable()
        {
            _isOpen = true;
        }

        private void OnDisable()
        {
            _isOpen = false;
            AuthUtility.onAuthStatusChanged -= HandleAuthStatusChanged;
        }

        // Needed to refresh the UI bindings after recompile
        private static void DelayedInitialize()
        {
            EditorApplication.update -= DelayedInitialize;

            if (_isOpen)
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
            var visualTree = EditorUtility.LoadAssetFromPackagePath<VisualTreeAsset>("Editor/Scripts/GUI/ConfigWindow/SpatialSDKConfigWindow.uxml");
            _issueContainerTemplate = EditorUtility.LoadAssetFromPackagePath<VisualTreeAsset>(_issuesContainerTemplatePath);
            VisualElement element = visualTree.Instantiate();
            if (!EditorGUIUtility.isProSkin)
            {
                element.styleSheets.Add(EditorUtility.LoadAssetFromPackagePath<StyleSheet>("Editor/Scripts/GUI/ConfigWindow/SpatialSDKConfigWindowStyles_LightModeOverride.uss"));
            }
            root.Add(element);

            root.Query<Button>("accountButton").ForEach(btn => btn.clicked += () => SetTab(ACCOUNT_TAB_NAME));
            root.Query<Button>("configButton").First().clicked += () => SetTab(CONFIG_TAB_NAME);
            root.Query<Button>("issuesButton").First().clicked += () => SetTab(ISSUES_TAB_NAME);
            root.Query<Button>("utilitiesButton").First().clicked += () => SetTab(UTILITIES_TAB_NAME);
            root.Query<Button>("helpButton").First().clicked += () => SetTab(HELP_TAB_NAME);

            // Account
            root.Query<Button>("getToken").First().clicked += () => Application.OpenURL(ACCESS_TOKEN_URL);
            root.Query<Button>("pasteToken").First().clicked += () => PasteAuthToken();
            root.Query<Button>("logout").First().clicked += AuthUtility.LogOut;

            // Config
            root.Query<Button>("openStudio").First().clicked += () => Application.OpenURL(spatialStudioURL);
            root.Query<Button>("newConfigButton").First().clicked += () => {
                ProjectConfig.Create();
                root.Bind(new SerializedObject(ProjectConfig.activePackageConfig));
            };
            _configActivePackageDropdown = root.Q<DropdownField>("packageConfigDropDown");
            _configActivePackageDropdown.RegisterValueChangedCallback(OnActivePackageDropdownValueChanged);
            _configDefaultWorldSelectionDropdown = root.Q<DropdownField>("defaultWorldDropDown");
            _configDefaultWorldSelectionDropdown.RegisterValueChangedCallback(OnDefaultWorldSelectionDropdownValueChanged);
            root.Q<Button>("refreshWorldsButton").clicked += () => {
                UpdateDefaultWorldSelectionDropdown();
                WorldUtility.FetchWorlds().Then(() => UpdateDefaultWorldSelectionDropdown());
            };
            root.Q<Button>("createWorldsButton").clicked += () => {
                SpatialAPI.CreateWorld()
                    .Then(resp => {
                        ProjectConfig.defaultWorldID = resp.id;
                        return WorldUtility.FetchWorlds();
                    })
                    .Then(() => UpdateDefaultWorldSelectionDropdown())
                    .Catch(ex => {
                        UnityEditor.EditorUtility.DisplayDialog("Failed to create new world", ex.Message, "OK");
                        Debug.LogError($"Failed to create new world: {ex.Message}");
                    });
            };
            _configCreatePackageTypeDropdown = root.Q<EnumField>("createPackageTypeDropdown");
            root.Q<Button>("createPackageButton").clicked += () => ProjectConfig.AddNewPackage((PackageType)_configCreatePackageTypeDropdown.value, makeActive: true);
            root.Q<Button>("skuCopyButton").clicked += () => {
                EditorGUIUtility.systemCopyBuffer = ProjectConfig.activePackageConfig.sku;
            };

            _publishPackageButton = root.Q<Button>("publishPackageButton");
            _publishPackageButton.clicked += () => {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    UnityEditor.EditorUtility.DisplayDialog("Unable to Publish", "Cannot publish package while in play mode.", "OK");
                    return;
                }

                if (UnityEditor.EditorUtility.DisplayDialog(
                    "Publishing Package",
                    $"You're about to upload '{ProjectConfig.activePackageConfig.packageName}' to Spatial for publishing.\n\nIt's strongly encouraged that you test this package in the Spatial sandbox, if you haven't done so already.",
                    "Continue",
                    "Cancel"))
                {
                    UpgradeUtility.PerformUpgradeIfNecessaryForTestOrPublish()
                        .Then(() => {
                            return BuildUtility.PackageForPublishing();
                        })
                        .Catch(exc => {
                            if (exc is RSG.PromiseCancelledException)
                                return;

                            UnityEditor.EditorUtility.DisplayDialog("Publishing Error", $"An unexpected error occurred while publishing your package.\n\n{exc.Message}", "OK");
                            Debug.LogException(exc);
                        });
                }
            };
            root.Q<Button>("deletePackageButton").clicked += () => {
                if (UnityEditor.EditorUtility.DisplayDialog("Delete Package", $"Are you sure you want to delete {ProjectConfig.activePackageConfig.packageName}?", "Yes", "No"))
                    ProjectConfig.RemovePackage(ProjectConfig.activePackageConfig);
            };

            _packageConfigName = root.Q(PACKAGE_CONFIG_ELEMENT_NAME).Query<TextField>("name");
            _packageConfigName.RegisterCallback<KeyUpEvent>(evt => {
                PopulatePackageDropdownLabels(_configActivePackageDropdown);
                PopulatePackageDropdownLabels(_issuesActivePackageDropdown);
            });
            _packageConfigSKUEmptyElement = root.Q(PACKAGE_CONFIG_ELEMENT_NAME).Q("packageSKUEmpty");
            _packageConfigSKUElement = root.Q(PACKAGE_CONFIG_ELEMENT_NAME).Q("packageSKU");
            _configPackageType = root.Q(PACKAGE_CONFIG_ELEMENT_NAME).Query<Label>("packageTypeValue").First();

            // Issues
            _issuesActivePackageDropdown = root.Q<DropdownField>("issuesPackageDropDown");
            _issuesActivePackageDropdown.RegisterValueChangedCallback(OnActivePackageDropdownValueChanged);
            _issuesCountBlock = root.Q("issuesCountBlock");
            _issuesCountTitle = root.Query<Label>("issuesCountTitle").First();
            _issuesCountDescription = root.Query<Label>("issuesCountDescription").First();
            _issuesRefreshButton = root.Query<Button>("refreshButton").First();
            _issuesRefreshButton.clicked += RefreshIssues;

            _issuesScrollBlock = root.Q("issuesScrollBlock");

            _selectedIssueBlock = root.Q("selectedIssueBlock");
            _selectedIssueTextTitle = _selectedIssueBlock.Query<Label>("title").First();
            _selectedIssueTextDescription = _selectedIssueBlock.Query<Label>("description").First();

            _autoFixBlock = root.Q("autoFixBlock");
            _autoFixDescription = root.Query<Label>("autoFixDescription").First();
            root.Query<Button>("autoFixButton").First().clicked += FixSelectedIssue;

            _openSceneBlock = root.Q("openSceneBlock");
            _targetSceneName = root.Query<Label>("targetSceneName").First();
            root.Query<Button>("openSceneButton").First().clicked += OpenSelectedIssueScene;
            _selectObjectBlock = root.Q("selectObjectBlock");
            _targetObjectName = root.Query<Label>("targetObjectName").First();
            root.Query<Button>("selectObjectButton").First().clicked += OpenSelectedIssueGameObject;

            // Utilities
            root.Query<Button>("optimizeAssets").First().clicked += () => {
                AssetImportUtility.OptimizeAllAssets();
            };
            root.Query<Button>("optimizeAssetsFolder").First().clicked += () => {
                AssetImportUtility.OptimizeAssetsInFolder();
            };
            root.Query<Toggle>("disableAssetProcessing").First().value = EditorPrefs.GetBool("DisableAssetProcessing", false);
            root.Query<Toggle>("disableAssetProcessing").First().RegisterValueChangedCallback(evt => {
                EditorPrefs.SetBool("DisableAssetProcessing", evt.newValue);
            });

            // Help
            root.Query<Button>("gotoDocumentation").First().clicked += () => Application.OpenURL(PackageManagerUtility.documentationUrl);
            root.Query<Button>("gotoSupport").First().clicked += () => Application.OpenURL(SUPPORT_URL);
            root.Query<Button>("gotoDiscord").First().clicked += () => Application.OpenURL(DISCORD_URL);
            root.Query<Button>("gotoForum").First().clicked += () => Application.OpenURL(FORUM_URL);

            _issueListParent = root.Q("issuesScroll");

            if (ProjectConfig.activePackageConfig != null)
                root.Bind(new SerializedObject(ProjectConfig.activePackageConfig));

            HandleAuthStatusChanged();
            AuthUtility.onAuthStatusChanged += HandleAuthStatusChanged;
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
            rootVisualElement.Q(ACCOUNT_TAB_NAME).style.display = DisplayStyle.None;
            rootVisualElement.Q(CONFIG_TAB_NAME).style.display = DisplayStyle.None;
            rootVisualElement.Q(ISSUES_TAB_NAME).style.display = DisplayStyle.None;
            rootVisualElement.Q(UTILITIES_TAB_NAME).style.display = DisplayStyle.None;
            rootVisualElement.Q(HELP_TAB_NAME).style.display = DisplayStyle.None;

            rootVisualElement.Q(tab).style.display = DisplayStyle.Flex;

            // There are two states for the config tab, which depends on whether the config exists or not.
            if (tab == CONFIG_TAB_NAME)
            {
                UpdateConfigTabContents();
            }
            else if (tab == ISSUES_TAB_NAME)
            {
                UpdateIssuesTabContents();
            }

            // Tab buttons
            rootVisualElement.Q("accountButton").RemoveFromClassList("tabButtonSelected");
            rootVisualElement.Q("configButton").RemoveFromClassList("tabButtonSelected");
            rootVisualElement.Q("issuesButton").RemoveFromClassList("tabButtonSelected");
            rootVisualElement.Q("utilitiesButton").RemoveFromClassList("tabButtonSelected");
            rootVisualElement.Q("helpButton").RemoveFromClassList("tabButtonSelected");

            rootVisualElement.Q(tab + "Button").AddToClassList("tabButtonSelected");
        }

        private void UpdateConfigTabContents()
        {
            UpdateActivePackageDropdown(_configActivePackageDropdown);
            UpdateDefaultWorldSelectionDropdown();

            PackageConfig packageConfig = ProjectConfig.activePackageConfig;
            rootVisualElement.Q(PROJECT_CONFIG_NULL_ELEMENT_NAME).style.display = (ProjectConfig.instance == null) ? DisplayStyle.Flex : DisplayStyle.None;
            rootVisualElement.Q(PROJECT_CONFIG_ELEMENT_NAME).style.display = (ProjectConfig.instance != null) ? DisplayStyle.Flex : DisplayStyle.None;
            rootVisualElement.Q(PACKAGE_CONFIG_ELEMENT_NAME).style.display = (packageConfig != null) ? DisplayStyle.Flex : DisplayStyle.None;

            // Active Package
            if (packageConfig != null)
            {
                _packageConfigSKUEmptyElement.style.display = (string.IsNullOrEmpty(packageConfig.sku)) ? DisplayStyle.Flex : DisplayStyle.None;
                _packageConfigSKUElement.style.display = (string.IsNullOrEmpty(packageConfig.sku)) ? DisplayStyle.None : DisplayStyle.Flex;
                _configPackageType.text = packageConfig.packageType.ToString();

                rootVisualElement.Q("spaceConfig").style.display = (packageConfig.packageType == PackageType.Space) ? DisplayStyle.Flex : DisplayStyle.None;
                rootVisualElement.Q("spaceTemplateConfig").style.display = (packageConfig.packageType == PackageType.SpaceTemplate) ? DisplayStyle.Flex : DisplayStyle.None;
                rootVisualElement.Q("avatarConfig").style.display = (packageConfig.packageType == PackageType.Avatar) ? DisplayStyle.Flex : DisplayStyle.None;
                rootVisualElement.Q("avatarAnimationConfig").style.display = (packageConfig.packageType == PackageType.AvatarAnimation) ? DisplayStyle.Flex : DisplayStyle.None;
                rootVisualElement.Q("prefabObjectConfig").style.display = (packageConfig.packageType == PackageType.PrefabObject) ? DisplayStyle.Flex : DisplayStyle.None;
                rootVisualElement.Q("avatarAttachmentConfig").style.display = (packageConfig.packageType == PackageType.AvatarAttachment) ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private void UpdateActivePackageDropdown(DropdownField dropdown)
        {
            if (ProjectConfig.hasPackages)
            {
                dropdown.SetEnabled(true);

                // Update elements
                if (dropdown.choices == null || dropdown.choices.Count != ProjectConfig.packages.Count)
                {
                    PopulatePackageDropdownLabels(dropdown);
                }

                dropdown.index = ProjectConfig.activePackageIndex;
            }
            else
            {
                dropdown.SetEnabled(false);
                dropdown.choices = new List<string>() { "None (Create New Package)" };
                dropdown.index = 0;
            }
        }

        private void PopulatePackageDropdownLabels(DropdownField dropdown)
        {
            Func<PackageConfig, string> getPackageDropdownLabel = (PackageConfig config) => $"{config.packageType} - {config.packageName}";
            dropdown.choices = ProjectConfig.packages.Select(getPackageDropdownLabel).ToList();

            // Modify all duplicate package names from the dropdown choices so they're all unique, otherwise the duplicates won't appear unless the duplicates are renamed manually.
            // select duplicate package labels as keys, value is a counter for packages without SKU
            Dictionary<string, int> duplicates = dropdown.choices.GroupBy(c => c)
                                                                .Where(group => group.Count() > 1)
                                                                .Select(group => group.Key)
                                                                .ToDictionary(c => c, c => 0);

            if (duplicates.Count == 0)
                return;

            for (int i = 0; i < dropdown.choices.Count; i++)
            {
                string choice = dropdown.choices[i];
                if (!duplicates.ContainsKey(choice))
                    continue;

                if (string.IsNullOrEmpty(ProjectConfig.packages[i].sku))
                {
                    // if there's no SKU to append, use a number instead.
                    if (duplicates[choice] > 0)
                    {
                        dropdown.choices[i] += $" ({duplicates[choice]})";
                    }
                    duplicates[choice]++;
                }
                else
                {
                    // append sku
                    dropdown.choices[i] += $" ({ProjectConfig.packages[i].sku})";
                }
            }
        }

        private void OnActivePackageDropdownValueChanged(ChangeEvent<string> evt)
        {
            var dropdownField = evt.target as DropdownField;
            int index = dropdownField.choices.IndexOf(evt.newValue);
            ProjectConfig.activePackageIndex = index;
            UnityEditor.EditorUtility.SetDirty(ProjectConfig.instance);
            rootVisualElement.Bind(new SerializedObject(ProjectConfig.activePackageConfig));
            UpdateIssuesTabContents();
        }

        private void UpdateDefaultWorldSelectionDropdown()
        {
            _configDefaultWorldSelectionDropdown.SetEnabled(!WorldUtility.isFetchingWorlds && WorldUtility.worlds.Length > 0);
            _configDefaultWorldSelectionDropdown.choices = WorldUtility.worlds.Select(w => $"{w.displayName} ({w.id})").ToList();

            int index = System.Array.FindIndex(WorldUtility.worlds, w => w.id == ProjectConfig.defaultWorldID);
            if (index >= 0)
            {
                _configDefaultWorldSelectionDropdown.index = index;
            }
            else
            {
                if (!AuthUtility.isAuthenticated)
                {
                    _configDefaultWorldSelectionDropdown.choices.Add("(Not Logged In)");
                }
                else if (string.IsNullOrEmpty(ProjectConfig.defaultWorldID))
                {
                    _configDefaultWorldSelectionDropdown.choices.Add($"None (Create New World)");
                }
                else
                {
                    _configDefaultWorldSelectionDropdown.choices.Add($"Invalid World ({ProjectConfig.defaultWorldID})");
                }

                _configDefaultWorldSelectionDropdown.index = _configDefaultWorldSelectionDropdown.choices.Count - 1;
            }
        }

        private void OnDefaultWorldSelectionDropdownValueChanged(ChangeEvent<string> evt)
        {
            var dropdownField = evt.target as DropdownField;
            int index = dropdownField.choices.IndexOf(evt.newValue);
            if (index >= 0 && index < WorldUtility.worlds.Length)
                ProjectConfig.defaultWorldID = WorldUtility.worlds[index].id;
        }

        private void PasteAuthToken()
        {
            string clipboard = GUIUtility.systemCopyBuffer;

            if (string.IsNullOrWhiteSpace(clipboard))
            {
                UnityEditor.EditorUtility.DisplayDialog("System clipboard is empty", "Unable to paste the access token since the system clipboard is empty. Try copying the access token to your clipboard again.", "OK");
                return;
            }

            AuthUtility.LogInWithDialogPopup(clipboard);

            if (GUIUtility.systemCopyBuffer == clipboard)
                GUIUtility.systemCopyBuffer = null;
        }

        private void HandleAuthStatusChanged()
        {
            VisualElement loggedInBlock = rootVisualElement.Q("loggedInBlock");

            rootVisualElement.Query("notLoggedInBlock").ForEach(block => {
                block.style.display = !AuthUtility.isAuthenticated && !AuthUtility.isAuthenticating ? DisplayStyle.Flex : DisplayStyle.None;
            });
            rootVisualElement.Q("loggingInBlock").style.display = AuthUtility.isAuthenticating ? DisplayStyle.Flex : DisplayStyle.None;

            if (AuthUtility.isAuthenticated)
            {
                loggedInBlock.style.display = DisplayStyle.Flex;
                loggedInBlock.Q<TextElement>("loggedInAsText").text = $"as {AuthUtility.userAlias}";

                WorldUtility.FetchWorlds().Then(() => UpdateDefaultWorldSelectionDropdown());
            }
            else
            {
                loggedInBlock.style.display = DisplayStyle.None;

                WorldUtility.ClearWorlds();
                UpdateDefaultWorldSelectionDropdown();
            }
        }

        /////////////////////////////////////////
        //               Issues
        /////////////////////////////////////////

        private void RefreshIssues()
        {
            SpatialValidator.RunTestsOnPackage(ValidationRunContext.ManualRun)
                .Then(validationSummary => {
                    _issuesValidationSummary = validationSummary;
                    UpdateIssuesTabContents();
                });
        }

        private void UpdateIssuesTabContents()
        {
            UpdateActivePackageDropdown(_issuesActivePackageDropdown);

            _issueListParent.Clear();
            _selectedIssue = null;
            SelectIssue(null);

            _warningIssues = new List<VisualElement>();
            _errorIssues = new List<VisualElement>();
            _elementToReponse = new Dictionary<VisualElement, SpatialTestResponse>();

            foreach (SpatialTestResponse response in SpatialValidator.allResponses)
            {
                VisualElement element = issueContainerTemplate.Instantiate();
                if (!EditorGUIUtility.isProSkin)
                {
                    element.styleSheets.Add(EditorUtility.LoadAssetFromPackagePath<StyleSheet>("Editor/Scripts/GUI/ConfigWindow/SpatialValidatorWindow_LightModeOverride.uss"));
                }
                // It's important to note that when this tree is instantiated it has an extra "root" element that we can't really see inside the UI-builder.
                // So below, "issueContainer" != element, it's a child of this extra root element... 

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
            bool showSelectedIssue = false;

            if (_issuesValidationSummary == null || _issuesValidationSummary.targetPackage != ProjectConfig.activePackageConfig)
            {
                // The results aren't applicable to the current package, or the results aren't valid anymore.
                _issuesCountTitle.text = "Check for issues";
                _issuesCountDescription.text = "Click the button below to get started!";
            }
            else if (_errorIssues.Count == 0 && _warningIssues.Count == 0)
            {
                _issuesCountTitle.text = "No issues found";
                _issuesCountDescription.text = "You're good to go!";
                _issuesCountBlock.AddToClassList("block_green");
                _issuesRefreshButton.AddToClassList("blockButton_green");
            }
            else if (_errorIssues.Count > 0)
            {
                _issuesCountTitle.text = "<color=#FF4343>" + _errorIssues.Count.ToString() + " Error(s)</color> <color=#FFEA80>" + _warningIssues.Count.ToString() + " Warning(s)</color>";
                _issuesCountDescription.text = "You can't upload your package to the sandbox or publish until the errors are resolved.";
                _issuesCountBlock.AddToClassList("block_red");
                _issuesRefreshButton.AddToClassList("blockButton_red");
                showSelectedIssue = true;
            }
            else
            {
                _issuesCountTitle.text = "<color=#FFEA80>" + _warningIssues.Count.ToString() + " Warning(s)</color>";
                _issuesCountDescription.text = "Warnings will not prevent you from publishing, but you should still look to resolve them as your package may not work as expected.";
                _issuesCountBlock.AddToClassList("block_yellow");
                _issuesRefreshButton.AddToClassList("blockButton_yellow");
                showSelectedIssue = true;
            }

            if (showSelectedIssue)
            {
                if (_errorIssues.Count > 0)
                {
                    SelectIssue(_errorIssues[0]);
                }
                else if (_warningIssues.Count > 0)
                {
                    SelectIssue(_warningIssues[0]);
                }
            }

            _issuesScrollBlock.style.display = showSelectedIssue ? DisplayStyle.Flex : DisplayStyle.None;
            _selectedIssueBlock.style.display = showSelectedIssue ? DisplayStyle.Flex : DisplayStyle.None;
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
                _selectedIssueTextDescription.text = EditorUtility.TruncateTextForUI(response.description);

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
                response.InvokeAutoFix(); // This might switch the scene.

                // Re-run tests to refresh UI
                RefreshIssues();
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

                if (response.targetObject == null)
                    return;
            }

            Selection.activeObject = response.targetObject;
            EditorGUIUtility.PingObject(response.targetObject);
        }
    }
}
