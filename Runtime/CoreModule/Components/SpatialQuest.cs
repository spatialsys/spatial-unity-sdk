using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSys.UnitySDK.Internal;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Core/Components")]
    [DisallowMultipleComponent]
    public class SpatialQuest : SpatialComponentBase
    {
        public const int LATEST_VERSION = 2;
        public const int MAX_NAME_LENGTH = 50;
        public const int MAX_DESCRIPTION_LENGTH = 500;

        public override string prettyName => "Quest";
        public override string tooltip => $"This component describes a quest and its tasks.";

        public override string documentationURL => "https://toolkit.spatial.io/docs/economy/quests-and-rewards";
        public override bool isExperimental => true;

        [ReadOnly]
        public uint id;
        [HideInInspector]
        public int version = LATEST_VERSION;

        public string questName;
        [TextArea(1, 3)]
        public string description;
        public bool startAutomatically = false;
        [Tooltip("If enabled, the user's progress will be saved to the cloud and restored when they rejoin the space. If disabled, the quest progress is reset on the next join.")]
        public bool saveUserProgress = true;
        public bool tasksAreOrdered = true;

        [Tooltip("Plays a confetti animation when the quest is completed")]
        public bool celebrateOnComplete = true;
        [Tooltip("Affects VR devices only. If true, toast messages will not be displayed when the quest or its tasks are completed.")]
        public bool hideToastMessageOnComplete = false;
        public Reward[] questRewards;
        public Task[] tasks = new Task[0];

        public SpatialEvent onStartedEvent;
        public SpatialEvent onCompletedEvent;
        public SpatialEvent onPreviouslyCompleted;
        public SpatialEvent onResetEvent;

        [Serializable]
        [DocumentationCategory("Core/Components")]
        public class Reward
        {
            public RewardType type;
            public string id;
            public int amount = 1; // only used for item reward type
        }

        [Serializable]
        [DocumentationCategory("Core/Components")]
        public class Task
        {
            [HideInInspector]
            public uint id;
            public string name;
            public QuestTaskType type;
            public int progressSteps;
            public GameObject[] taskMarkers;
            public SpatialEvent onStartedEvent;
            public SpatialEvent onCompletedEvent;
            public SpatialEvent onPreviouslyCompleted;

            //------------------------------------------------------------------------------------------------------------
            // Spatial Bridge shortcuts
            //------------------------------------------------------------------------------------------------------------
            // Helper variables
            internal uint _questID;
            private IQuestTask _task => SpatialBridge.questService.quests[_questID].tasks.First(t => t.id == id);

            /// <summary>
            /// Gets the status of the task.
            /// </summary>
            public QuestStatus status => _task?.status ?? QuestStatus.None;

            /// <summary>
            /// The current progress of the task.
            /// </summary>
            public int progress { get => _task?.progress ?? 0; set => _task.progress = value; }

            /// <summary>
            /// Starts the task.
            /// </summary>
            public void StartTask()
            {
                _task?.Start();
            }

            /// <summary>
            /// Completes the task.
            /// </summary>
            public void CompleteTask()
            {
                _task?.Complete();
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            UpgradeDataIfNecessary();

            // Check if a new instance or a duplicate was just made
            if (id == 0 || (Event.current != null && Event.current.type == EventType.ExecuteCommand && (Event.current.commandName == "Duplicate" || Event.current.commandName == "Paste")))
            {
                id = FindObjectsOfType<SpatialQuest>(includeInactive: true).Select(quest => quest.id).Max() + 1;
            }

            // Assign unique IDs to tasks if they don't have one or if it's repeated
            List<uint> ids = new List<uint>();
            for (var i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].id == 0 || ids.Contains(tasks[i].id))
                {
                    tasks[i].id = tasks.Select(task => task.id).Max() + 1;

                    // If the name is empty or repeated, assign a default name
                    if (string.IsNullOrEmpty(tasks[i].name) || (i > 0 && tasks[i].name == tasks[i - 1].name))
                        tasks[i].name = $"Task {i + 1}";
                }

                ids.Add(tasks[i].id);
            }
        }

        private void UpgradeDataIfNecessary()
        {
            if (version == LATEST_VERSION)
                return;

            if (version == 1)
            {
                saveUserProgress = false; // for any v1 quests, we default to not saving progress
                version = 2;
            }
        }
#endif

        //------------------------------------------------------------------------------------------------------------
        // Spatial Bridge shortcuts
        //------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The quest associated with this component.
        /// </summary>
        public IQuest quest => SpatialBridge.questService.quests[id];

        /// <summary>
        /// The current status of the quest.
        /// </summary>
        public QuestStatus status => quest?.status ?? QuestStatus.None;

        private void Awake()
        {
            // Initialize tasks with quest id for runtime helper functions.
            foreach (var t in tasks)
            {
                t._questID = id;
            }
        }

        /// <summary>
        /// Starts the quest.
        /// </summary>
        public void StartQuest()
        {
            quest?.Start();
        }

        /// <summary>
        /// Completes the quest.
        /// </summary>
        public void CompleteQuest()
        {
            quest?.Complete();
        }

        /// <summary>
        /// Resets the quest (all tasks are reset and the quest status is marked as None)
        /// </summary>
        public void ResetQuest()
        {
            quest?.Reset();
        }
    }
}
