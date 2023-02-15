using System;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SpatialSys.UnitySDK.Editor
{
    public enum TestResponseType
    {
        Fail,
        Warning, // won't block you from publishing, but indicates something is probably set up wrong.
    }

    public class SpatialTestResponse
    {
        public TestResponseType responseType;
        public string title;
        public string description;//optional

        public GlobalObjectId? targetObjectGlobalID;
        public UnityEngine.Object targetObject;
        public string scenePath;

        public bool autoFixIsSafe = false; // "Unsafe" auto-fixes have unwanted side effects.
        public string autoFixDescription; // Describe what will happen to the user.

        public bool hasAutoFix => _autoFixMethod != null;
        public bool isSceneResponse => !string.IsNullOrEmpty(scenePath);

        private Action<UnityEngine.Object> _autoFixMethod = null;

        public SpatialTestResponse(UnityEngine.Object targetObject, TestResponseType responseType, string title, string description = "")
        {
            this.responseType = responseType;
            this.title = title;
            this.description = description;
            this.targetObject = targetObject;
            _autoFixMethod = null;

            if (targetObject != null)
            {
                targetObjectGlobalID = GlobalObjectId.GetGlobalObjectIdSlow(targetObject);
            }
        }

        public void SetAutoFix(bool isSafe, string description, Action<UnityEngine.Object> fix)
        {
            autoFixIsSafe = isSafe;
            autoFixDescription = description;
            _autoFixMethod = fix;
        }

        public void InvokeAutoFix()
        {
            if (!hasAutoFix)
            {
                return;
            }

            if (isSceneResponse && EditorSceneManager.GetActiveScene().path != scenePath)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }

            if (targetObject == null && targetObjectGlobalID.HasValue)
            {
                targetObject = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(targetObjectGlobalID.Value);
            }

            _autoFixMethod.Invoke(targetObject);
            if (targetObject != null)
            {
                Selection.activeObject = targetObject;
                UnityEditor.EditorUtility.SetDirty(targetObject);
                EditorSceneManager.SaveOpenScenes();
            }
        }

        public override string ToString()
        {
            return $"{responseType}: {title}; Description: {description}; Target: {targetObject}; Scene: {scenePath}";
        }
    }
}
