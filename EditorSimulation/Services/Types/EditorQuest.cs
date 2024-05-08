using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorQuest : IQuest
    {
        private List<IReward> _rewards = new();
        private List<IQuestTask> _tasks = new();

        public EditorQuest(uint id, string name, string description, bool saveUserProgress, bool tasksAreOrdered, bool celebrateOnComplete)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.saveUserProgress = saveUserProgress;
            this.tasksAreOrdered = tasksAreOrdered;
            this.celebrateOnComplete = celebrateOnComplete;
        }

        private void HandleTaskCompleted()
        {
            if (status == QuestStatus.InProgress && tasks.All(t => t.status == QuestStatus.Completed))
            {
                Complete();
            }
            else
            {
                if (tasksAreOrdered)
                {
                    foreach (IQuestTask task in tasks)
                    {
                        if (task.status == QuestStatus.None)
                        {
                            task.Start();
                            break;
                        }
                    }
                }
            }
        }

        public uint id { get; }

        public string name { get; }

        public string description { get; }

        public bool saveUserProgress { get; }

        public bool tasksAreOrdered { get; }

        public bool celebrateOnComplete { get; }

        public QuestStatus status { get; private set; }

        public IReadOnlyList<IReward> rewards => _rewards;

        public IReadOnlyList<IQuestTask> tasks => _tasks;

        public event Action onStarted;
        public event Action onCompleted;
#pragma warning disable 0067 // Disable "event is never used" warning
        public event Action onPreviouslyCompleted;
#pragma warning restore 0067
        public event Action onReset;
        public event IQuest.QuestTaskDelegate onTaskAdded;

        public IQuestTask AddTask(string name, QuestTaskType type, int progressSteps, GameObject[] taskMarkers)
        {
            return AddTask((uint)tasks.Count, name, type, progressSteps, taskMarkers);
        }

        public IQuestTask AddTask(uint id, string name, QuestTaskType type, int progressSteps, GameObject[] taskMarkers)
        {
            EditorQuestTask task = new EditorQuestTask(id, name, type, progressSteps, taskMarkers);
            task.onCompleted += HandleTaskCompleted;
            _tasks.Add(task);
            onTaskAdded?.Invoke(task);
            return task;
        }

        public IReward AddBadgeReward(string id)
        {
            EditorReward reward = new EditorReward(RewardType.Badge, id, 1);
            _rewards.Add(reward);
            return reward;
        }

        public IReward AddItemReward(string id, int amount)
        {
            EditorReward reward = new EditorReward(RewardType.Item, id, amount);
            _rewards.Add(reward);
            return reward;
        }

        public IQuestTask GetTaskByID(uint id)
        {
            return tasks.FirstOrDefault(t => t.id == id);
        }

        public void Start()
        {
            status = QuestStatus.InProgress;
            if (tasksAreOrdered && tasks.Count > 0)
            {
                tasks[0].Start();
            }
            onStarted?.Invoke();
        }

        public void Complete()
        {
            if (status != QuestStatus.InProgress)
            {
                return;
            }
            status = QuestStatus.Completed;
            foreach (IQuestTask task in tasks)
            {
                task.Complete();
            }
            onCompleted?.Invoke();
        }

        public void Reset()
        {
            status = QuestStatus.None;
            foreach (IQuestTask task in tasks)
            {
                (task as EditorQuestTask).Reset();
            }
            onReset?.Invoke();
        }

        public IQuest SetOnCompleted(Action onCompleted)
        {
            this.onCompleted += onCompleted;
            return this;
        }

        public IQuest SetOnPreviouslyCompleted(Action onPreviouslyCompleted)
        {
            this.onPreviouslyCompleted += onPreviouslyCompleted;
            return this;
        }

        public IQuest SetOnReset(Action onReset)
        {
            this.onReset += onReset;
            return this;
        }

        public IQuest SetOnStarted(Action onStarted)
        {
            this.onStarted += onStarted;
            return this;
        }
    }
}