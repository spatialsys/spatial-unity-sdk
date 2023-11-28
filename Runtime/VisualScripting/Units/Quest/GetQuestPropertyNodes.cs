using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    public enum SpatialQuestStatus
    {
        None = 0,
        InProgress = 1,
        Completed = 2,
    }

    public enum SpatialQuestTaskType
    {
        Check = 0,
        ProgressBar = 1,
    }

    [UnitTitle("Spatial Quest: Get Quest Name")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Get Quest Name")]
    [UnitCategory("Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class GetSpatialQuestNameNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }
        [DoNotSerialize]
        public ValueOutput name { get; private set; }


        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();

            name = ValueOutput<string>(nameof(name), (f) => f.GetValue<SpatialQuest>(questRef).questName);
        }
    }

    [UnitTitle("Spatial Quest: Get Quest Description")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Get Quest Description")]
    [UnitCategory("Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class GetSpatialQuestDescriptionNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }
        [DoNotSerialize]
        public ValueOutput description { get; private set; }

        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();

            description = ValueOutput<string>(nameof(description), (f) => f.GetValue<SpatialQuest>(questRef).description);
        }
    }

    [UnitTitle("Spatial Quest: Get Task Count")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Get Task Count")]
    [UnitCategory("Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class GetSpatialQuestTaskCountNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }
        [DoNotSerialize]
        public ValueOutput count { get; private set; }


        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();

            count = ValueOutput<int>(nameof(count), (f) => f.GetValue<SpatialQuest>(questRef).tasks.Length);
        }
    }

    [UnitTitle("Spatial Quest: Get Tasks Are Ordered")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Get Tasks Are Ordered")]
    [UnitCategory("Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class GetSpatialQuestTasksAreOrderedNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }
        [DoNotSerialize]
        public ValueOutput areOrdered { get; private set; }


        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();

            areOrdered = ValueOutput<bool>(nameof(areOrdered), (f) => f.GetValue<SpatialQuest>(questRef).tasksAreOrdered);
        }
    }

    [UnitTitle("Spatial Quest: Get Task Type")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Get Task Type")]
    [UnitCategory("Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class GetSpatialQuestTaskTypeNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }
        [DoNotSerialize]
        public ValueInput taskID { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput taskType { get; private set; }

        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
            taskID = ValueInput<uint>(nameof(taskID), 0);

            taskType = ValueOutput<SpatialQuestTaskType>(nameof(taskType), (f) => {
                var q = f.GetValue<SpatialQuest>(questRef);
                var id = f.GetValue<uint>(taskID);
                foreach (var task in q.tasks)
                {
                    if (task.id == id)
                    {
                        return (SpatialQuestTaskType)task.type;
                    }
                }
                return SpatialQuestTaskType.Check;
            });
        }
    }

    [UnitTitle("Spatial Quest: Get Task Progress Steps")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Get Task Progress Steps")]
    [UnitCategory("Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class GetSpatialQuestTaskProgressStepsNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }
        [DoNotSerialize]
        public ValueInput taskID { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput taskProgressSteps { get; private set; }

        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
            taskID = ValueInput<uint>(nameof(taskID), 0);

            taskProgressSteps = ValueOutput<int>(nameof(taskProgressSteps), (f) => {
                var q = f.GetValue<SpatialQuest>(questRef);
                var id = f.GetValue<uint>(taskID);
                foreach (var task in q.tasks)
                {
                    if (task.id == id)
                    {
                        return task.progressSteps;
                    }
                }
                return 0;
            });
        }
    }

    [UnitTitle("Spatial Quest: Get Quest Status")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Get Quest Status")]
    [UnitCategory("Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class GetSpatialQuestStatusNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput status { get; private set; }

        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();

            status = ValueOutput<SpatialQuestStatus>(nameof(status), (f) =>
                (SpatialQuestStatus)SpatialBridge.GetQuestStatus?.Invoke(f.GetValue<SpatialQuest>(questRef))
            );
        }
    }

    [UnitTitle("Spatial Quest: Get Task Progress")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Get Task Progress")]
    [UnitCategory("Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class GetSpatialQuestTaskProgressNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }
        [DoNotSerialize]
        public ValueInput taskID { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput taskProgress { get; private set; }

        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
            taskID = ValueInput<uint>(nameof(taskID), 0);

            taskProgress = ValueOutput<int>(nameof(taskProgress), (f) =>
                SpatialBridge.GetQuestTaskProgress?.Invoke(f.GetValue<SpatialQuest>(questRef), f.GetValue<uint>(taskID)) ?? 0
            );
        }
    }
    [UnitTitle("Spatial Quest: Get Task Status")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Get Task Status")]
    [UnitCategory("Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class GetSpatialQuestTaskStatusNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }
        [DoNotSerialize]
        public ValueInput taskID { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput taskStatus { get; private set; }

        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
            taskID = ValueInput<uint>(nameof(taskID), 0);

            taskStatus = ValueOutput<SpatialQuestStatus>(nameof(taskStatus), (f) =>
                (SpatialQuestStatus)SpatialBridge.GetQuestTaskStatus?.Invoke(f.GetValue<SpatialQuest>(questRef), f.GetValue<uint>(taskID))
            );
        }
    }

    [UnitTitle("Spatial Quest: Get Quest By Id")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Get Quest By Id")]
    [UnitCategory("Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class GetSpatialQuestByIdNode : Unit
    {
        [DoNotSerialize]
        public ValueInput questId { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput questRef { get; private set; }

        protected override void Definition()
        {
            questId = ValueInput<uint>(nameof(questId), 0);

            questRef = ValueOutput<SpatialQuest>(nameof(questRef), (f) =>
                GameObject.FindObjectsOfType<SpatialQuest>().FirstOrDefault(q => q.id == f.GetValue<uint>(questId))
            );
        }
    }
}