using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Service for managing Quests on the space.
    /// </summary>
    [DocumentationCategory("Services/Quest Service")]
    public interface IQuestService
    {
        /// <summary>
        /// Dictionary of all quests.
        /// </summary>
        IReadOnlyDictionary<uint, IQuest> quests { get; }

        /// <summary>
        /// ID of the current quest.
        /// </summary>
        uint currentQuestID { get; }

        /// <summary>
        /// Current quest.
        /// </summary>
        IQuest currentQuest { get; }

        /// <summary>
        /// Event called when the current quest changes.
        /// </summary>
        event QuestDelegate onCurrentQuestChanged;

        /// <summary>
        /// Event called when a quest is added.
        /// </summary>
        event QuestDelegate onQuestAdded;

        /// <summary>
        /// Event called when a quest is removed.
        /// </summary>
        event QuestDelegate onQuestRemoved;

        public delegate void QuestDelegate(IQuest quest);

        /// <summary>
        /// Creates a new quest.
        /// </summary>
        /// <param name="name">Name of the quest.</param>
        /// <param name="description">Description of the quest.</param>
        /// <param name="startAutomatically">Does quest start automatically?</param>
        /// <param name="saveUserProgress">Should progress be saved?</param>
        /// <param name="tasksAreOrdered">Should tasks be completed one at a time, sequentially?</param>
        /// <param name="celebrateOnComplete">If true, plays confetti VFX surrounding the local avatar upon quest completion</param>
        /// <returns></returns>
        IQuest CreateQuest(string name, string description, bool startAutomatically, bool saveUserProgress, bool tasksAreOrdered, bool celebrateOnComplete);
    }

    /// <summary>
    /// Basic interface Quest component
    /// </summary>
    [DocumentationCategory("Services/Quest Service")]
    public interface IQuest
    {
        /// <summary>
        /// Unique ID of the quest.
        /// </summary>
        uint id { get; }

        /// <summary>
        /// Name of the quest.
        /// </summary>
        string name { get; }

        /// <summary>
        /// Short description of what the quest objective is.
        /// </summary>
        string description { get; }

        /// <summary>
        /// If enabled, the user's progress will be saved to the cloud and restored when they rejoin the space. If disabled, the quest progress is reset on the next join.
        /// </summary>
        bool saveUserProgress { get; }

        /// <summary>
        /// If true, only the first task will be started and once it’s completed, the next task will start. If false, all tasks are automatically started when the quest completes.
        /// </summary>
        bool tasksAreOrdered { get; }

        /// <summary>
        /// If enabled, a confetti animation will play when the quest is completed.
        /// </summary>
        bool celebrateOnComplete { get; }

        /// <summary>
        /// Gets the status of the quest.
        /// </summary>
        QuestStatus status { get; }

        /// <summary>
        /// Rewards for completing the quest.
        /// </summary>
        IReadOnlyList<IReward> rewards { get; }

        /// <summary>
        /// Tasks that need to be completed to finish the quest.
        /// </summary>
        IReadOnlyList<IQuestTask> tasks { get; }

        /// <summary>
        /// Event called when this quest is started.
        /// </summary>
        event Action onStarted;

        /// <summary>
        /// Event called when this quest is completed.
        /// </summary>
        event Action onCompleted;

        /// <summary>
        /// Event that is triggered when the user loads into a space where a quest was previously completed. Only used when <see cref="ISpatialQuest.saveUserProgress"/>
        /// is set to true. This event allows you to “fast forward” any settings in the scene that should be enabled if a quest was previously completed.
        /// </summary>
        event Action onPreviouslyCompleted;

        /// <summary>
        /// Event called when this quest is reset.
        /// </summary>
        event Action onReset;

        /// <summary>
        /// Event called when a task is added to the quest.
        /// </summary>
        event QuestTaskDelegate onTaskAdded;

        public delegate void QuestTaskDelegate(IQuestTask task);

        /// <summary>
        /// Adds a task to the task list
        /// </summary>
        /// <param name="name">Name of the task</param>
        /// <param name="type">Task type</param>
        /// <param name="progressSteps">Progress steps</param>
        /// <param name="taskMarkers">Task markers</param>
        /// <returns>Created ISpatialQuestTask.</returns>
        IQuestTask AddTask(string name, QuestTaskType type, int progressSteps, GameObject[] taskMarkers);

        /// <summary>
        /// Adds a badge reward to the quest.
        /// </summary>
        /// <param name="id">Badge id.</param>
        /// <returns>Reward object</returns>
        IReward AddBadgeReward(string id);

        /// <summary>
        /// Adds an item reward to the quest.
        /// </summary>
        /// <param name="id">Badge id.</param>
        /// <returns>Reward object</returns>
        IReward AddItemReward(string id, int amount);

        /// <summary>
        /// Gets a task by ID.
        /// </summary>
        IQuestTask GetTaskByID(uint id);

        /// <summary>
        /// Starts the quest.
        /// </summary>
        void Start();

        /// <summary>
        /// Completes the quest.
        /// </summary>
        void Complete();

        /// <summary>
        /// Resets the quest (all tasks are reset and the quest status is marked as None)
        /// </summary>
        void Reset();

        /// <summary>
        /// Sets the onCompleted action and returns the quest.
        /// </summary>
        /// <param name="onCompleted">Event called when this quest is completed.</param>
        /// <returns>Quest object.</returns>
        IQuest SetOnCompleted(Action onCompleted);

        /// <summary>
        /// Sets the onPreviouslyCompleted action and returns the quest.
        /// </summary>
        /// <param name="onPreviouslyCompleted">Event that is triggered when the user loads into a space where a quest was previously completed.</param>
        /// <returns>Quest object.</returns>
        IQuest SetOnPreviouslyCompleted(Action onPreviouslyCompleted);

        /// <summary>
        /// Sets the onReset action and returns the quest.
        /// </summary>
        /// <param name="onReset">Event called when this quest is reset.</param>
        /// <returns>Quest object.</returns>
        IQuest SetOnReset(Action onReset);

        /// <summary>
        /// Sets the onStarted action and returns the quest.
        /// </summary>
        /// <param name="onStarted">Event called when this quest is started.</param>
        /// <returns>Quest object.</returns>
        IQuest SetOnStarted(Action onStarted);
    }

    /// <summary>
    /// Interface that represents a task for a ISpatialQuest.
    /// </summary>
    [DocumentationCategory("Services/Quest Service")]
    public interface IQuestTask
    {
        /// <summary>
        /// Unique ID of the task
        /// </summary>
        uint id { get; }

        /// <summary>
        /// Name of the task
        /// </summary>
        string name { get; }

        /// <summary>
        /// Type of task.
        /// </summary>
        QuestTaskType type { get; }

        /// <summary>
        /// The number of steps required to complete the task.
        /// </summary>
        int progressSteps { get; }

        /// <summary>
        /// The current progress of the task.
        /// </summary>
        int progress { get; set; }

        /// <summary>
        /// Task marker objects 
        /// </summary>
        GameObject[] taskMarkers { get; set; }

        /// <summary>
        /// Gets the status of the quest.
        /// </summary>
        QuestStatus status { get; }

        /// <summary>
        /// Event called when this task is started.
        /// </summary>
        event Action onStarted;

        /// <summary>
        /// Event called when this task is completed.
        /// </summary>
        event Action onCompleted;

        /// <summary>
        /// Event that is triggered when the user loads into a space where a task was previously completed. Only used when the quest's
        /// <see cref="ISpatialQuest.saveUserProgress"/> is set to true.
        /// This event allows you to “fast forward” any settings in the scene that should be enabled if a task was previously completed.
        /// </summary>
        event Action onPreviouslyCompleted;

        /// <summary>
        /// Starts the task.
        /// </summary>
        public void Start();

        /// <summary>
        /// Completes the task.
        /// </summary>
        void Complete();

        /// <summary>
        /// Sets the onCompleted action and returns the task.
        /// </summary>
        /// <param name="onCompleted">Event called when this task is completed.</param>
        /// <returns>Task object.</returns>
        IQuestTask SetOnCompleted(Action onCompleted);

        /// <summary>
        /// Sets the onPreviouslyCompleted action and returns the task.
        /// </summary>
        /// <param name="onPreviouslyCompleted">Event that is triggered when the user loads into a space where a task was previously completed.</param>
        /// <returns>Task object.</returns>
        IQuestTask SetOnPreviouslyCompleted(Action onPreviouslyCompleted);

        /// <summary>
        /// Sets the onStarted action and returns the task.
        /// </summary>
        /// <param name="onStarted">Event called when this task is started.</param>
        /// <returns>Task object.</returns>
        IQuestTask SetOnStarted(Action onStarted);
    }

    /// <summary>
    /// The status of the quest.
    /// </summary>
    [DocumentationCategory("Services/Quest Service")]
    public enum QuestStatus
    {
        /// <summary>
        /// Quest is not started.
        /// </summary>
        None = 0,

        /// <summary>
        /// Quest is in progress.
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Quest is completed.
        /// </summary>
        Completed = 2,
    }

    /// <summary>
    /// The type of task.
    /// </summary>
    [DocumentationCategory("Services/Quest Service")]
    public enum QuestTaskType
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

    /// <summary>
    /// Interface that represents a reward for a ISpatialQuest.
    /// </summary>
    [DocumentationCategory("Services/Quest Service")]
    public interface IReward
    {
        /// <summary>
        /// Type of reward.
        /// </summary>
        RewardType type { get; }

        /// <summary>
        /// Id of reward to add.
        /// </summary>
        string id { get; }

        /// <summary>
        /// Amount of items (if item type)
        /// </summary>
        int amount { get; }
    }

    /// <summary>
    /// The type of reward.
    /// </summary>
    [DocumentationCategory("Services/Quest Service")]
    public enum RewardType
    {
        /// <summary>
        /// Badge reward.
        /// </summary>
        Badge = 0,

        /// <summary>
        /// Item reward.
        /// </summary>
        Item = 1,
    }
}
