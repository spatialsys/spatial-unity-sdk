using System;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SpatialSys.UnitySDK.Editor
{
    public enum TestResponseType
    {
        Fail,
        Warning,//won't block you from publishing, but indicates something is probably set up wrong.
    }

    public class SpatialTestResponse
    {
        public TestResponseType responseType;
        public string title;
        public string description;//optional

        public GlobalObjectId? targetObjectGlobalID;
        public UnityEngine.Object targetObject;
        public string scenePath;

        public bool hasAutoFix = false;
        public bool autoFixIsSafe = false;//auto fix can be applied without asking user. Some auto fixes might have unwanted side effects.
        Action<UnityEngine.Object> autoFixMethod;
        public string autoFixDescription;//describe what will happen

        public SpatialTestResponse(UnityEngine.Object targetObject, TestResponseType responseType, string title, string description = "")
        {
            this.responseType = responseType;
            this.title = title;
            this.description = description;
            this.targetObject = targetObject;
            if (targetObject != null)
            {
                targetObjectGlobalID = GlobalObjectId.GetGlobalObjectIdSlow(targetObject);
            }
        }

        public void SetAutoFix(bool isSafe, string description, Action<UnityEngine.Object> fix)
        {
            hasAutoFix = true;
            autoFixIsSafe = isSafe;
            autoFixDescription = description;
            autoFixMethod = fix;
        }

        public void InvokeFix()
        {
            if (!hasAutoFix)
            {
                return;
            }

            if (!string.IsNullOrEmpty(scenePath) && EditorSceneManager.GetActiveScene().path != scenePath)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }

            if (targetObject == null && targetObjectGlobalID.HasValue)
            {
                targetObject = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(targetObjectGlobalID.Value);
            }

            autoFixMethod.Invoke(targetObject);
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
