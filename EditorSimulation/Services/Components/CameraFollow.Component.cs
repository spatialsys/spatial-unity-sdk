using System;
using System.Linq;
using UnityEngine;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class CameraFollow : MonoBehaviour
    {
        public new Camera camera;
        public Transform target;
        public Vector3 offset;
        public Vector3 lookAtOffset;
        public float smoothSpeed = 0.125f;
        public Quaternion cameraRotation = Quaternion.identity;

        private void Start()
        {
            var depth = FindObjectsOfType<Camera>().Max(c => c.depth);
            camera.depth = depth + 1;
        }

        private void Update()
        {
            // Check for mouse drag
            if (Input.GetMouseButton(0))
            {
                float rotX = Input.GetAxis("Mouse X") * 5;
                float rotY = Input.GetAxis("Mouse Y") * 5;
                cameraRotation *= Quaternion.Euler(-rotY, rotX, 0);
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            // Smooth rotate around center point (target)
            Vector3 localPosition = transform.position - target.position;
            Vector3 localTargetPosition = cameraRotation * offset;
            Vector3 smoothedLocalPosition = Vector3.Slerp(localPosition, localTargetPosition, smoothSpeed);
            transform.position = target.position + smoothedLocalPosition;

            transform.LookAt(target.position + lookAtOffset);
        }
    }
}