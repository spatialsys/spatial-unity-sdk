using System.Linq;
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
        public QuestEvent questEvent;

        public bool isSyncedEvent => (unityEventIsSynced || animatorEvent.events.Any(e => e.syncedAnimator != null));
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

    [System.Serializable]
    public class QuestEvent
    {
        public List<QuestEventEntry> events;

        [System.Serializable]
        public class QuestEventEntry
        {
            public uint questID;
            public QuestEventType questEventType;
            public uint taskID;  // Only used if quest event is "Task" type
        }

        public static bool QuestEventHasTaskParam(QuestEventType questEventType)
        {
            return (questEventType == QuestEventType.AddTaskProgress || questEventType == QuestEventType.CompleteTask);
        }

        public enum QuestEventType
        {
            Unset = 0,
            StartQuest = 1,
            ResetQuest = 2,
            AddTaskProgress = 3, // Task is auto-completed if progress is >= task.progressSteps
            CompleteTask = 4,
        }
    }
}
