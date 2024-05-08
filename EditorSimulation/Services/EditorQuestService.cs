using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorQuestService : IQuestService
    {
        private Dictionary<uint, IQuest> _quests = new();
        public EditorQuestService()
        {
            SpatialQuest[] spatialQuests = GameObject.FindObjectsOfType<SpatialQuest>();
            foreach (SpatialQuest spatialQuest in spatialQuests)
            {
                EditorQuest quest = new EditorQuest(spatialQuest.id, spatialQuest.questName, spatialQuest.description, spatialQuest.saveUserProgress, spatialQuest.tasksAreOrdered, spatialQuest.celebrateOnComplete);
                foreach (var spatialReward in spatialQuest.questRewards)
                {
                    if (spatialReward.type == RewardType.Badge)
                        quest.AddBadgeReward(spatialReward.id);
                    else
                        quest.AddItemReward(spatialReward.id, spatialReward.amount);
                }
                foreach (var spatialTask in spatialQuest.tasks)
                {
                    var task = quest.AddTask(spatialTask.id, spatialTask.name, spatialTask.type, spatialTask.progressSteps, spatialTask.taskMarkers);
                }
                _quests.Add(quest.id, quest);

                if (spatialQuest.startAutomatically)
                    currentQuestID = quest.id;
            }
            onCurrentQuestChanged?.Invoke(currentQuest);
        }

        private uint CreateQuestID()
        {
            uint id = 1;
            while (_quests.ContainsKey(id))
                id++;
            return id;
        }

        //--------------------------------------------------------------------------------------------------------------
        // IQuestService
        //--------------------------------------------------------------------------------------------------------------
        public IReadOnlyDictionary<uint, IQuest> quests => _quests;

        public uint currentQuestID { get; }
        public IQuest currentQuest => currentQuestID != 0 ? quests[currentQuestID] : null;

        public event IQuestService.QuestDelegate onCurrentQuestChanged;
        public event IQuestService.QuestDelegate onQuestAdded;
        public event IQuestService.QuestDelegate onQuestRemoved;

        public IQuest CreateQuest(string name, string description, bool startAutomatically, bool saveUserProgress, bool tasksAreOrdered, bool celebrateOnComplete)
        {
            EditorQuest quest = new EditorQuest(CreateQuestID(), name, description, saveUserProgress, tasksAreOrdered, celebrateOnComplete);
            _quests.Add(quest.id, quest);

            onQuestAdded?.Invoke(quest);

            return quest;
        }

    }
}