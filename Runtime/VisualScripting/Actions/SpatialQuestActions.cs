using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Quest: Start Quest")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Start Quest")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialQuest))]
    public class StartSpatialQuestNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }

        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.StartQuest.Invoke(f.GetValue<SpatialQuest>(questRef));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
    [UnitTitle("Spatial Quest: Complete Quest")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Complete Quest")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialQuest))]
    public class CompleteSpatialQuestNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }

        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.CompleteQuest.Invoke(f.GetValue<SpatialQuest>(questRef));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
    [UnitTitle("Spatial Quest: Reset Quest")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Reset Quest")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialQuest))]
    public class ResetSpatialQuestNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }

        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.ResetQuest.Invoke(f.GetValue<SpatialQuest>(questRef));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial Quest: Start Task")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Start Task")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialQuest))]
    public class StartSpatialQuestTaskNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }
        [DoNotSerialize]
        public ValueInput taskID { get; private set; }

        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
            taskID = ValueInput<uint>(nameof(taskID), 0);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.StartQuestTask.Invoke(f.GetValue<SpatialQuest>(questRef), f.GetValue<uint>(taskID));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial Quest: Complete Task")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Complete Task")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialQuest))]
    public class CompleteSpatialQuestTaskNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }
        [DoNotSerialize]
        public ValueInput taskID { get; private set; }

        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
            taskID = ValueInput<uint>(nameof(taskID), 0);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.CompleteQuestTask.Invoke(f.GetValue<SpatialQuest>(questRef), f.GetValue<uint>(taskID));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
    [UnitTitle("Spatial Quest: Add Task Progress")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("Add Task Progress")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialQuest))]
    public class SpatialQuestAddTaskProgressNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput questRef { get; private set; }
        [DoNotSerialize]
        public ValueInput taskID { get; private set; }

        protected override void Definition()
        {
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
            taskID = ValueInput<uint>(nameof(taskID), 0);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.AddQuestTaskProgress.Invoke(f.GetValue<SpatialQuest>(questRef), f.GetValue<uint>(taskID));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
