using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Spatial Components")]
    [DisallowMultipleComponent]
    public class SpatialVirtualCamera : SpatialComponentBase
    {
        public override string prettyName => "Virtual Camera";
        public override string tooltip => "Defines a camera that will override the default spatial camera. If multiple virtual cameras are present the camera with the highest priority will be chosen.";
        public override string documentationURL => "https://docs.spatial.io/virtual-camera";

        [SerializeField]
        private int _priority;
        public int priority
        {
            get => _priority;
            set
            {
                //clamping to -1000 to insure it's at least higher prio than spatial internal cameras.
                _priority = Mathf.Max(-1000, value);
                UpdateProperties();
            }
        }
        [SerializeField]
        private float _fieldOfView;
        public float fieldOfView
        {
            get => _fieldOfView;
            set
            {
                _fieldOfView = value;
                UpdateProperties();
            }
        }
        [SerializeField]
        private float _nearClipPlane;
        public float nearClipPlane
        {
            get => _nearClipPlane;
            set
            {
                _nearClipPlane = Mathf.Max(0.1f, value);
                UpdateProperties();
            }
        }
        [SerializeField]
        private float _farClipPlane;
        public float farClipPlane
        {
            get => _farClipPlane;
            set
            {
                _farClipPlane = value;
                UpdateProperties();
            }
        }

        //Doing this private callback thing to avoid a public method that shows up in VS
        private Action valueChangedCallback;

        private void Reset()
        {
            priority = 0;
            fieldOfView = 60f;
            nearClipPlane = 0.1f;
            farClipPlane = 5000f;
        }

        private void Start()
        {
            valueChangedCallback = SpatialBridge.spatialComponentService.InitializeVirtualCamera(this);
        }

        private void UpdateProperties()
        {
            valueChangedCallback?.Invoke();
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(1f, 1f, 1f, .5f);
            Gizmos.DrawFrustum(Vector3.zero, fieldOfView, farClipPlane, nearClipPlane, Screen.width / (float)Screen.height);
        }
    }
}
