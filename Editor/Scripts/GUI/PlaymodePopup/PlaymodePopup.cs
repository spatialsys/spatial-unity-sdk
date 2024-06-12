using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpatialSys.UnitySDK.Editor
{
    public class PlaymodePopup : EditorWindow
    {
        private const string EDITOR_PREFS_KEY = "SpatialSys.UnitySDK.Editor.DontShowPlaymodePopup";
        private static PlaymodePopup _instance;

        private bool forceFocused = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ShowPopupOnStart()
        {
#if !SPATIAL_UNITYSDK_INTERNAL
            bool pref = EditorPrefs.GetBool(EDITOR_PREFS_KEY, false);
            if (!pref)
            {
                InitWindow(true);
            }
#endif
        }

        private void Update()
        {
            if (!this.forceFocused)
            {
                // Without this, the window sometimes doesnt paint when the game view is focused.
                // Seems like a unity or MacOS bug.
                Focus();
                this.forceFocused = true;
            }
            if (!Application.isPlaying)
            {
                Close();
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        public static void InitWindow(bool fromPlayInit)
        {
            if (_instance != null)
            {
                return;
            }
            PlaymodePopup window = CreateInstance<PlaymodePopup>();
            _instance = window;
            _instance.forceFocused = false;

            var uxml = EditorUtility.LoadAssetFromPackagePath<VisualTreeAsset>("Editor/Scripts/GUI/PlaymodePopup/PlaymodePopup.uxml");
            VisualElement ui = uxml.Instantiate();
            ui.Q<Label>("PackageTutorialLink").AddManipulator(new Clickable(() => {
                Application.OpenURL("https://toolkit.spatial.io/docs/packages");
            }));

            Toggle toggle = ui.Q<Toggle>("DontShowToggle");
            if (!fromPlayInit)
            {
                toggle.visible = false;
            }

            ui.Q<Button>("GotItButton").clicked += () => {
                if (fromPlayInit)
                {
                    EditorPrefs.SetBool(EDITOR_PREFS_KEY, toggle.value);
                }
                window.Close();
            };

            // Register a callback that we can use to re-evaluate the resolved size of the visualElement, then set the window rect.
            window.rootVisualElement.RegisterCallback<GeometryChangedEvent>(GeometryChangedCallback);
            window.rootVisualElement.Add(ui);

            //temporary rect to allow the VisualElement to compute correct size.
            window.position = new Rect(0, 0, 99999, 99999);
            window.ShowPopup();
            window.Focus();
            // Called in same frame. Wacky hack to create auto-sized popup windows.
            void GeometryChangedCallback(GeometryChangedEvent evt)
            {
                window.rootVisualElement.UnregisterCallback<GeometryChangedEvent>(GeometryChangedCallback);

                VisualElement container = ui.Q<VisualElement>("BG");
                Vector2 size = new Vector2(container.resolvedStyle.width, container.resolvedStyle.height);

                Rect main = EditorGUIUtility.GetMainWindowPosition();
                Rect pos = new Rect();
                pos.size = size;
                float centerWidth = (main.width - pos.width) * 0.5f;
                float centerHeight = (main.height - pos.height) * 0.5f;
                pos.x = main.x + centerWidth;
                pos.y = main.y + centerHeight * .25f;
                window.position = pos;
                window.Focus();
            }
        }
    }
}
