using System;
using System.Linq;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DisallowMultipleComponent]
    public class SpatialQuest : SpatialComponentBase
    {
        public const int LATEST_VERSION = 1;

        public override string prettyName => "Quest";
        public override string tooltip => $"This components describes a quest and its tasks.";

        public override string documentationURL => "https://docs.spatial.io/quests";
        public override bool isExperimental => true;

        [ReadOnly]
        public uint id;
        [HideInInspector]
        public int version = LATEST_VERSION;

        public string questName;
        public string description;
        public bool startAutomatically = false;
        public bool tasksAreOrdered = true;

        [Tooltip("Plays a confetti animation when the quest is completed")]
        public bool celebrateOnComplete = true;
        public Reward[] questRewards;
        public Task[] tasks = new Task[0];

        public SpatialEvent onStartedEvent;
        public SpatialEvent onCompletedEvent;
        public SpatialEvent onResetEvent;

        [Serializable]
        public class Reward
        {
            public RewardType type;
            public string id;
        }

        public enum RewardType
        {
            Badge = 0,
        }

        [Serializable]
        public class Task
        {
            [HideInInspector]
            public uint id;

            public string name;
            public TaskType type;
            public int progressSteps;

            public SpatialEvent onStartedEvent;
            public SpatialEvent onCompletedEvent;
        }

        // NOTE: If any new types are introduced here, make sure to also add validation checks for them
        public enum TaskType
        {
            /// <summary>
            /// One time only. Eg. "Go to this point of interest" or "Talk to X"
            /// Meant to be completed using Quest.CompleteTask
            /// </summary>
            Check = 0,
            /// <summary>
            /// Several times. Eg. "Interact with x 5 times" or "Collect 3 swords"
            /// Meant to be completed using Quest.AddTaskProgress the amount of times specified in the progress variable
            /// </summary>
            ProgressBar = 1,
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            // Check if a new instance or a duplicate was just made
            if (id == 0 || (Event.current != null && Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "Duplicate"))
            {
                id = FindObjectsOfType<SpatialQuest>(includeInactive: true).Select(quest => quest.id).Max() + 1;
            }

            // If a new task is added, unity will essentially duplicate the last item in the array so we need to clear it
            if (tasks.Length >= 2)
            {
                // Check if a duplicate was just made
                Task beforeLastTask = tasks[tasks.Length - 2];
                Task lastTask = tasks[tasks.Length - 1];
                if (beforeLastTask.id == lastTask.id)
                {
                    var newTask = new Task();
                    newTask.name = $"Task {tasks.Length}";
                    tasks[tasks.Length - 1] = newTask;
                }
            }

            // Assign unique IDs to tasks if they don't have one
            foreach (Task task in tasks)
            {
                if (task.id == 0)
                    task.id = tasks.Select(task => task.id).Max() + 1;
            }
        }
#endif
    }
}
