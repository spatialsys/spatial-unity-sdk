using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorAdService : IAdService
    {
        private GameObject _adGameObject;
        private CoroutineRunner _coroutineRunner;
        private Text _adText;

        public EditorAdService()
        {
            _adGameObject = new GameObject("[Spatial SDK] Ad Service");
            _adGameObject.SetActive(false);

            _coroutineRunner = _adGameObject.AddComponent<CoroutineRunner>();

            var canvas = _adGameObject.AddComponent<Canvas>();
            canvas.sortingOrder = 1000;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var bgImageGO = new GameObject("Background Image");
            bgImageGO.transform.SetParent(_adGameObject.transform);
            var image = bgImageGO.AddComponent<Image>();
            image.color = Color.black;
            image.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            image.type = Image.Type.Sliced;

            ExpandToFillParent(bgImageGO);

            var textGO = new GameObject("Text");
            textGO.transform.SetParent(_adGameObject.transform);
            _adText = textGO.AddComponent<Text>();
            _adText.text = "Ad simulated from editor. Type: ";
            _adText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            _adText.color = Color.white;
            _adText.fontSize = 50;
            _adText.alignment = TextAnchor.MiddleCenter;
            ExpandToFillParent(textGO);
        }

        private void ExpandToFillParent(GameObject go)
        {
            var rectTransform = go.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = new Vector2(0, 0);
            rectTransform.offsetMax = new Vector2(0, 0);
        }

        private IEnumerator ShowAdCoroutine(AdRequest request)
        {
            request.InvokeStartedEvent();
            _adText.text = "Ad simulated from editor. Type: " + request.adType;
            yield return new WaitForSeconds(5);
            request.InvokeCompletionEvent();
            _adGameObject.SetActive(false);
        }

        //-----------------------------------------------------------------------------------------------------
        // IAdService
        //-----------------------------------------------------------------------------------------------------

        public bool isSupported => true;

        public AdRequest RequestAd(SpatialAdType adType)
        {
            AdRequest request = new() { adType = adType };
            _adGameObject.SetActive(true);
            _coroutineRunner.StartCoroutine(ShowAdCoroutine(request));
            return request;
        }
    }
}