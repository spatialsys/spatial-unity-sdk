using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Quest: On Quest Started")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("On Quest Started")]
    [UnitCategory("Events\\Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class SpatialQuestEventOnStarted : EventUnit<SpatialQuest>
    {
        public static string eventName = "SpatialQuestOnStarted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialQuest args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SpatialQuest>(questRef) == args)
            {
                return true;
            }
            return false;
        }
    }

    [UnitTitle("Spatial Quest: On Quest Completed")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("On Quest Completed")]
    [UnitCategory("Events\\Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class SpatialQuestEventOnCompleted : EventUnit<SpatialQuest>
    {
        public static string eventName = "SpatialQuestOnCompleted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialQuest args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SpatialQuest>(questRef) == args)
            {
                return true;
            }
            return false;
        }
    }

    [UnitTitle("Spatial Quest: On Quest Previously Completed")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("On Quest Previously Completed")]
    [UnitCategory("Events\\Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class SpatialQuestEventOnPreviouslyCompleted : EventUnit<SpatialQuest>
    {
        public static string eventName = "SpatialQuestOnPreviouslyCompleted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialQuest args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SpatialQuest>(questRef) == args)
            {
                return true;
            }
            return false;
        }
    }

    [UnitTitle("Spatial Quest: On Quest Reset")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("On Quest Reset")]
    [UnitCategory("Events\\Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class SpatialQuestEventOnReset : EventUnit<SpatialQuest>
    {
        public static string eventName = "SpatialQuestOnReset";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialQuest args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SpatialQuest>(questRef) == args)
            {
                return true;
            }
            return false;
        }
    }

    [UnitTitle("Spatial Quest: On Task Started")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("On Task Started")]
    [UnitCategory("Events\\Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class SpatialQuestEventOnTaskStarted : EventUnit<(SpatialQuest, uint)>
    {
        public static string eventName = "SpatialQuestOnTaskStarted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }

        [DoNotSerialize]
        public ValueInput taskID { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
            taskID = ValueInput<uint>(nameof(taskID), 0);
        }

        protected override bool ShouldTrigger(Flow flow, (SpatialQuest, uint) args)
        {
            var taskId = flow.GetValue<uint>(taskID);
            if (flow.GetValue<SpatialQuest>(questRef) == args.Item1 && taskId == args.Item2)
            {
                return true;
            }
            return false;
        }
    }

    [UnitTitle("Spatial Quest: On Task Completed")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("On Task Completed")]
    [UnitCategory("Events\\Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class SpatialQuestEventOnTaskCompleted : EventUnit<(SpatialQuest, uint)>
    {
        public static string eventName = "SpatialQuestOnTaskCompleted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }

        [DoNotSerialize]
        public ValueInput taskID { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
            taskID = ValueInput<uint>(nameof(taskID), 0);
        }

        protected override bool ShouldTrigger(Flow flow, (SpatialQuest, uint) args)
        {
            var taskId = flow.GetValue<uint>(taskID);
            if (flow.GetValue<SpatialQuest>(questRef) == args.Item1 && taskId == args.Item2)
            {
                return true;
            }
            return false;
        }
    }

    [UnitTitle("Spatial Quest: On Task Previously Completed")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("On Task Previously Completed")]
    [UnitCategory("Events\\Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class SpatialQuestEventOnTaskPreviouslyCompleted : EventUnit<(SpatialQuest, uint)>
    {
        public static string eventName = "SpatialQuestOnTaskPreviouslyCompleted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }

        [DoNotSerialize]
        public ValueInput taskID { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
            taskID = ValueInput<uint>(nameof(taskID), 0);
        }

        protected override bool ShouldTrigger(Flow flow, (SpatialQuest, uint) args)
        {
            var taskId = flow.GetValue<uint>(taskID);
            if (flow.GetValue<SpatialQuest>(questRef) == args.Item1 && taskId == args.Item2)
            {
                return true;
            }
            return false;
        }
    }

    [UnitTitle("Spatial Quest: On Any Task Started")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("On Any Task Started")]
    [UnitCategory("Events\\Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class SpatialQuestEventOnAnyTaskStarted : EventUnit<SpatialQuest>
    {
        public static string eventName = "SpatialQuestOnAnyTaskStarted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialQuest args)
        {
            if (flow.GetValue<SpatialQuest>(questRef) == args)
            {
                return true;
            }
            return false;
        }
    }

    [UnitTitle("Spatial Quest: On Any Task Completed")]
    [UnitSurtitle("Spatial Quest")]
    [UnitShortTitle("On Any Task Completed")]
    [UnitCategory("Events\\Spatial\\Quest")]
    [TypeIcon(typeof(SpatialQuest))]
    public class SpatialQuestEventOnAnyTaskCompleted : EventUnit<SpatialQuest>
    {
        public static string eventName = "SpatialQuestOnAnyTaskCompleted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            questRef = ValueInput<SpatialQuest>(nameof(questRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialQuest args)
        {
            if (flow.GetValue<SpatialQuest>(questRef) == args)
            {
                return true;
            }
            return false;
        }
    }
}