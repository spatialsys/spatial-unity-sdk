using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpatialSys.UnitySDK
{
    [System.Serializable]
    public class SpatialEvent
    {
        [HideInInspector]
        public int id; // Set during sceneProcess
        public bool unityEventIsSynced;
        public UnityEvent unityEvent;
        public AnimatorEvent animatorEvent;
    }

    [System.Serializable]
    public class AnimatorEvent
    {
        public List<AnimatorEventEntry> events;

        [System.Serializable]
        public class AnimatorEventEntry
        {
            public Animator animator;
            [HideInInspector]
            public SpatialSyncedAnimator syncedAnimator;//set during sceneProcess
            public string parameter;
            [HideInInspector]
            public AnimatorControllerParameterType parameterType;//set during sceneProcess
            public OperationType operation;
            public float floatValue;
            public int intValue;
            public bool boolValue;
        }

        public enum OperationType
        {
            Set,
            Add,
            Multiply,
        }
    }
}
