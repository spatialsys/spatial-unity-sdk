using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialQuestTests
    {
        [ComponentTest(typeof(SpatialQuest))]
        public static void CheckQuestValidity(SpatialQuest target)
        {
            if (string.IsNullOrEmpty(target.questName) || string.IsNullOrEmpty(target.description))
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    target,
                    TestResponseType.Fail,
                    "Quest does not have a name or description.",
                    "For this quest to be valid, it must be given a name and description"
                ));
            }

            if (target.tasks.Length == 0)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    target,
                    TestResponseType.Fail,
                    "Quest does not have any tasks.",
                    "Create quest tasks and assign them to this quest."
                ));
            }
            else
            {
                // TODO: if a quest only has one task, then it shouldn't need a name and description?
                bool allTasksHaveNamesAndDescriptions = target.tasks.All(task => !string.IsNullOrEmpty(task.name));
                if (!allTasksHaveNamesAndDescriptions)
                {
                    SpatialValidator.AddResponse(new SpatialTestResponse(
                        target,
                        TestResponseType.Fail,
                        "One or more quest tasks do not have a name",
                        "For all quest tasks to be valid, they must be given a name."
                    ));
                }
            }
        }

        [SceneTest]
        public static void CheckOnlyOneQuestShouldStartAutomatically(Scene scene)
        {
            SpatialQuest[] questsInScene = Object.FindObjectsOfType<SpatialQuest>(true);

            SpatialQuest[] autoStartQuests = questsInScene.Where(quest => quest.startAutomatically).ToArray();
            if (autoStartQuests.Length > 1)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    autoStartQuests[0],
                    TestResponseType.Fail,
                    "More than one quest is set to start automatically.",
                    "Only one quest should be set to start automatically. Find the `SpatialQuest` components in your scene and make sure only one has `Start Automatically` checked."
                ));
            }
        }

        [SceneTest]
        public static void CheckAllSpatialQuestEvents(Scene scene)
        {
            Dictionary<SpatialEvent, Component> allEvents = new Dictionary<SpatialEvent, Component>();

            // Find all SpatialEvents in the scene
            SpatialQuest[] questsInScene = Object.FindObjectsOfType<SpatialQuest>(true);
            foreach (SpatialQuest quest in questsInScene)
            {
                allEvents.Add(quest.onStartedEvent, quest);
                allEvents.Add(quest.onCompletedEvent, quest);
                allEvents.Add(quest.onResetEvent, quest);

                foreach (SpatialQuest.Task task in quest.tasks)
                {
                    allEvents.Add(task.onStartedEvent, quest);
                    allEvents.Add(task.onCompletedEvent, quest);
                }
            }
            SpatialTriggerEvent[] triggersInScene = Object.FindObjectsOfType<SpatialTriggerEvent>(true);
            foreach (SpatialTriggerEvent trigger in triggersInScene)
            {
                allEvents.Add(trigger.onEnterEvent, trigger);
                allEvents.Add(trigger.onExitEvent, trigger);
            }
            SpatialPointOfInterest[] poisInScene = Object.FindObjectsOfType<SpatialPointOfInterest>(true);
            foreach (SpatialPointOfInterest poi in poisInScene)
            {
                allEvents.Add(poi.onTextDisplayedEvent, poi);
            }

            // Lookup for validity check
            Dictionary<uint, SpatialQuest> questIdToQuest = questsInScene.ToDictionary(quest => quest.id, quest => quest);

            // Validate all "Quest" events
            Dictionary<SpatialEvent, Component> questEvents = allEvents.Where(kvp => kvp.Key.questEvent.events?.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            foreach (KeyValuePair<SpatialEvent, Component> kvp in questEvents)
            {
                SpatialEvent e = kvp.Key;
                Component associatedComponent = kvp.Value;
                foreach (QuestEvent.QuestEventEntry questEvent in e.questEvent.events)
                {
                    // Check if Quest exists
                    if (!questIdToQuest.ContainsKey(questEvent.questID))
                    {
                        SpatialValidator.AddResponse(new SpatialTestResponse(
                            associatedComponent,
                            TestResponseType.Fail,
                            $"Quest event is pointing to a quest that does not exist ({associatedComponent.gameObject.GetPath()})",
                            $"Find the object at path `{associatedComponent.gameObject.GetPath()}` and make sure that the quest event is pointing to a valid quest."
                        ));
                    }
                    // If quest exists, then check if the event type for that quest is valid
                    else if (questEvent.questEventType == QuestEvent.QuestEventType.Unset)
                    {
                        SpatialValidator.AddResponse(new SpatialTestResponse(
                            associatedComponent,
                            TestResponseType.Fail,
                            $"Quest event is not set to a valid event type ({associatedComponent.gameObject.GetPath()})",
                            $"Find the object at path `{associatedComponent.gameObject.GetPath()}` and make sure that the quest event is set to a valid event type. The event type is currently set to {nameof(QuestEvent.QuestEventType.Unset)}."
                        ));
                    }
                    // If quest and event type are good, and the event requires a task to be assigned, then check if task is valid
                    else if (QuestEvent.QuestEventHasTaskParam(questEvent.questEventType))
                    {
                        // Check if task exists
                        SpatialQuest quest = questIdToQuest[questEvent.questID];
                        HashSet<uint> taskIds = quest.tasks.Select(task => task.id).ToHashSet();
                        if (!taskIds.Contains(questEvent.taskID))
                        {
                            SpatialValidator.AddResponse(new SpatialTestResponse(
                                associatedComponent,
                                TestResponseType.Fail,
                                $"Quest event is pointing to a task that does not exist ({associatedComponent.gameObject.GetPath()})",
                                $"Find the object at path `{associatedComponent.gameObject.GetPath()}` and make sure that the quest event is pointing to a valid task."
                            ));
                        }
                        // Check that the event type is valid for the task type
                        else
                        {
                            SpatialQuest.Task task = quest.tasks.First(t => t.id == questEvent.taskID);
                            if (questEvent.questEventType == QuestEvent.QuestEventType.CompleteTask && task.type != SpatialQuest.TaskType.Check)
                            {
                                SpatialValidator.AddResponse(new SpatialTestResponse(
                                    associatedComponent,
                                    TestResponseType.Fail,
                                    $"Quest event `{nameof(QuestEvent.QuestEventType.CompleteTask)}` can only be used on Quest Tasks that are of type `{nameof(SpatialQuest.TaskType.Check)}`",
                                    $"You will either need to change the Quest Task Type to `{nameof(SpatialQuest.TaskType.Check)}`, or change the event type on the object at path `{associatedComponent.gameObject.GetPath()}`"
                                ));
                            }
                            else if (questEvent.questEventType == QuestEvent.QuestEventType.AddTaskProgress && task.type != SpatialQuest.TaskType.ProgressBar)
                            {
                                SpatialValidator.AddResponse(new SpatialTestResponse(
                                    associatedComponent,
                                    TestResponseType.Fail,
                                    $"Quest event `{nameof(QuestEvent.QuestEventType.AddTaskProgress)}` can only be used on Quest Tasks that are of type `{nameof(SpatialQuest.TaskType.ProgressBar)}`",
                                    $"You will either need to change the Quest Task Type to `{nameof(SpatialQuest.TaskType.ProgressBar)}`, or change the event type on the object at path `{associatedComponent.gameObject.GetPath()}`"
                                ));
                            }
                        }
                    }
                }
            }
        }
    }
}
