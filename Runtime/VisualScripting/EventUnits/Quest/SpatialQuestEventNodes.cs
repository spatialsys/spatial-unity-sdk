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
        private const string EVENT_HOOK_ID = "SpatialQuestOnStarted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialQuest quest)
        {
            EventBus.Trigger(EVENT_HOOK_ID, quest);
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
        private const string EVENT_HOOK_ID = "SpatialQuestOnCompleted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialQuest quest)
        {
            EventBus.Trigger(EVENT_HOOK_ID, quest);
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
        private const string EVENT_HOOK_ID = "SpatialQuestOnPreviouslyCompleted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialQuest quest)
        {
            EventBus.Trigger(EVENT_HOOK_ID, quest);
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
        private const string EVENT_HOOK_ID = "SpatialQuestOnReset";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialQuest quest)
        {
            EventBus.Trigger(EVENT_HOOK_ID, quest);
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
        private const string EVENT_HOOK_ID = "SpatialQuestOnTaskStarted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }

        [DoNotSerialize]
        public ValueInput taskID { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialQuest quest, uint taskID)
        {
            EventBus.Trigger(EVENT_HOOK_ID, (quest, taskID));
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
        private const string EVENT_HOOK_ID = "SpatialQuestOnTaskCompleted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }

        [DoNotSerialize]
        public ValueInput taskID { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialQuest quest, uint taskID)
        {
            EventBus.Trigger(EVENT_HOOK_ID, (quest, taskID));
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
        private const string EVENT_HOOK_ID = "SpatialQuestOnTaskPreviouslyCompleted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }

        [DoNotSerialize]
        public ValueInput taskID { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialQuest quest, uint taskID)
        {
            EventBus.Trigger(EVENT_HOOK_ID, (quest, taskID));
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
        private const string EVENT_HOOK_ID = "SpatialQuestOnAnyTaskStarted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialQuest quest)
        {
            EventBus.Trigger(EVENT_HOOK_ID, quest);
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
        private const string EVENT_HOOK_ID = "SpatialQuestOnAnyTaskCompleted";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput questRef { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialQuest quest)
        {
            EventBus.Trigger(EVENT_HOOK_ID, quest);
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