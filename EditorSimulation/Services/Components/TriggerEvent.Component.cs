using System;
using UnityEngine;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class TriggerEventComponent : MonoBehaviour
    {
        public SpatialTriggerEvent triggerEvent;

        private void OnTriggerEnter(Collider collider)
        {
            TriggerEvent(triggerEvent.onEnterEvent);
        }

        private void OnTriggerExit(Collider collider)
        {
            TriggerEvent(triggerEvent.onExitEvent);
        }

        private void TriggerEvent(SpatialEvent spatialEvent)
        {
            spatialEvent.runtimeEvent?.Invoke();

            if (spatialEvent.hasUnityEvent)
            {
                spatialEvent.unityEvent.Invoke();
            }

            if (spatialEvent.hasAnimatorEvent)
            {
                foreach (AnimatorEvent.AnimatorEventEntry animatorEventEntry in spatialEvent.animatorEvent.events)
                {
                    switch (animatorEventEntry.parameterType)
                    {
                        case AnimatorControllerParameterType.Trigger:
                            animatorEventEntry.animator.SetTrigger(animatorEventEntry.parameter);
                            break;

                        case AnimatorControllerParameterType.Bool:
                            animatorEventEntry.animator.SetBool(animatorEventEntry.parameter, animatorEventEntry.boolValue);
                            break;

                        case AnimatorControllerParameterType.Float:
                            switch (animatorEventEntry.operation)
                            {
                                case AnimatorEvent.OperationType.Set:
                                    animatorEventEntry.animator.SetFloat(animatorEventEntry.parameter, animatorEventEntry.floatValue);
                                    break;
                                case AnimatorEvent.OperationType.Multiply:
                                    animatorEventEntry.animator.SetFloat(animatorEventEntry.parameter, animatorEventEntry.animator.GetFloat(animatorEventEntry.parameter) * animatorEventEntry.floatValue);
                                    break;
                                case AnimatorEvent.OperationType.Add:
                                    animatorEventEntry.animator.SetFloat(animatorEventEntry.parameter, animatorEventEntry.animator.GetFloat(animatorEventEntry.parameter) + animatorEventEntry.floatValue);
                                    break;
                            }
                            break;

                        case AnimatorControllerParameterType.Int:
                            switch (animatorEventEntry.operation)
                            {
                                case AnimatorEvent.OperationType.Set:
                                    animatorEventEntry.animator.SetInteger(animatorEventEntry.parameter, animatorEventEntry.intValue);
                                    break;
                                case AnimatorEvent.OperationType.Multiply:
                                    animatorEventEntry.animator.SetInteger(animatorEventEntry.parameter, animatorEventEntry.animator.GetInteger(animatorEventEntry.parameter) * animatorEventEntry.intValue);
                                    break;
                                case AnimatorEvent.OperationType.Add:
                                    animatorEventEntry.animator.SetInteger(animatorEventEntry.parameter, animatorEventEntry.animator.GetInteger(animatorEventEntry.parameter) + animatorEventEntry.intValue);
                                    break;
                            }
                            break;
                    }
                }
            }


            if (spatialEvent.hasQuestEvent)
            {
                foreach (var questEvent in spatialEvent.questEvent.events)
                {
                    if (SpatialBridge.questService.quests.TryGetValue(questEvent.questID, out IQuest quest))
                    {
                        switch (questEvent.questEventType)
                        {
                            case QuestEvent.QuestEventType.Unset:
                                break;
                            case QuestEvent.QuestEventType.StartQuest:
                                quest.Start();
                                break;
                            case QuestEvent.QuestEventType.ResetQuest:
                                quest.Reset();
                                break;
                            case QuestEvent.QuestEventType.AddTaskProgress:
                                var task = quest.GetTaskByID(questEvent.taskID);
                                if (task != null)
                                    task.progress++;
                                break;
                            case QuestEvent.QuestEventType.CompleteTask:
                                quest.GetTaskByID(questEvent.taskID)?.Complete();
                                break;
                        }
                    }
                }
            }
        }
    }
}