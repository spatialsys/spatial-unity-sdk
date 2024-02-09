using System;
using System.Linq;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorQuestTask : IQuestTask
    {
        public EditorQuestTask(uint id, string name, QuestTaskType type, int progressSteps, GameObject[] taskMarkers)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.progressSteps = progressSteps;
            this.taskMarkers = taskMarkers;
        }

        public void Reset()
        {
            status = QuestStatus.None;
            progress = 0;
        }

        //--------------------------------------------------------------------------------------------------------------
        // IQuestTask
        //--------------------------------------------------------------------------------------------------------------

        public uint id { get; }

        public string name { get; }

        public QuestTaskType type { get; }

        public int progressSteps { get; }

        public int progress { get; set; }

        public GameObject[] taskMarkers { get; set; }

        public QuestStatus status { get; private set; }

        public event Action onStarted;

        public event Action onCompleted;

#pragma warning disable 0067 // Disable "event is never used" warning
        public event Action onPreviouslyCompleted;
#pragma warning restore 0067

        public void Start()
        {
            status = QuestStatus.InProgress;
            onStarted?.Invoke();
        }

        public void Complete()
        {
            status = QuestStatus.Completed;
            progress = progressSteps;
            onCompleted?.Invoke();
        }

        public IQuestTask SetOnCompleted(Action onCompleted)
        {
            this.onCompleted += onCompleted;
            return this;
        }

        public IQuestTask SetOnPreviouslyCompleted(Action onPreviouslyCompleted)
        {
            this.onPreviouslyCompleted += onPreviouslyCompleted;
            return this;
        }

        public IQuestTask SetOnStarted(Action onStarted)
        {
            this.onStarted += onStarted;
            return this;
        }
    }
}