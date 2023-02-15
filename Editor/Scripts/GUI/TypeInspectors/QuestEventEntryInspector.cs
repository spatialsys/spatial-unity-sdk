using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomPropertyDrawer(typeof(QuestEvent.QuestEventEntry))]
    public class QuestEventEntryInspector : UnityEditor.PropertyDrawer
    {
        private static QuestDataCache _questDataCache = new QuestDataCache();
        private bool _initialized;

        public class QuestDataCache
        {
            public QuestData[] questData;
            public Dictionary<uint, QuestData> questsByID;
            public string[] questDropDownOptions;
            public int[] questDropDownValues; // quest id

            public class QuestData
            {
                public SpatialQuest quest;
                public Dictionary<uint, int> taskIndexByID;
                public string[] taskDropDownOptions;
                public int[] taskDropDownValues; // task id
            }

            public void Initialize()
            {
                SpatialQuest[] quests = Object.FindObjectsOfType<SpatialQuest>(true);

                questData = new QuestData[quests.Length];
                questsByID = new Dictionary<uint, QuestData>();
                questDropDownOptions = new string[quests.Length + 1];
                questDropDownOptions[0] = "Select a Quest...";
                questDropDownValues = new int[quests.Length + 1];
                questDropDownValues[0] = 0;
                for (int i = 0; i < quests.Length; i++)
                {
                    SpatialQuest quest = quests[i];
                    QuestData data = new QuestData();
                    data.quest = quest;
                    if (!questsByID.ContainsKey(quest.id))
                        questsByID.Add(quest.id, data);

                    questDropDownOptions[i + 1] = $"{quests[i].questName} [Quest ID: {quests[i].id}]";
                    questDropDownValues[i + 1] = (int)quests[i].id;

                    data.taskDropDownOptions = new string[quest.tasks.Length + 1];
                    data.taskDropDownOptions[0] = "Select a Task...";
                    data.taskDropDownValues = new int[quest.tasks.Length + 1];
                    data.taskDropDownValues[0] = 0;
                    for (int j = 0; j < quest.tasks.Length; j++)
                    {
                        data.taskDropDownOptions[j + 1] = $"{quest.tasks[j].name} [Task ID: {quest.tasks[j].id}]";
                        data.taskDropDownValues[j + 1] = (int)quest.tasks[j].id;
                    }
                    questData[i] = data;
                }
            }
        }

        private void InitializeIfNecessary()
        {
            if (_initialized)
                return;

            _initialized = true;
            _questDataCache.Initialize();
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            InitializeIfNecessary();

            SerializedProperty questID = property.FindPropertyRelative(nameof(QuestEvent.QuestEventEntry.questID));
            SerializedProperty questEventType = property.FindPropertyRelative(nameof(QuestEvent.QuestEventEntry.questEventType));
            SerializedProperty taskID = property.FindPropertyRelative(nameof(QuestEvent.QuestEventEntry.taskID));

            EditorGUI.BeginProperty(rect, label, property);
            GUILayout.BeginHorizontal();
            {
                float columnWidth = 0.333f;
                bool questExists = _questDataCache.questsByID.ContainsKey((uint)questID.intValue);

                // Quest ID
                Rect elementRect = new Rect(rect.x, rect.y, rect.width * columnWidth, rect.height);
                int oldQuestID = questID.intValue;
                int newQuestID = EditorGUI.IntPopup(elementRect, oldQuestID, _questDataCache.questDropDownOptions, _questDataCache.questDropDownValues);
                if (newQuestID != oldQuestID)
                {
                    questID.intValue = newQuestID;
                    UnityEditor.EditorUtility.SetDirty(property.serializedObject.targetObject);
                }

                // Quest Event Type
                elementRect = new Rect(rect.x + rect.width * columnWidth, rect.y, rect.width * columnWidth, rect.height);
                var oldQuestEventType = (QuestEvent.QuestEventType)questEventType.intValue;
                var newQuestEventType = (QuestEvent.QuestEventType)EditorGUI.EnumPopup(elementRect, (QuestEvent.QuestEventType)oldQuestEventType);
                if (newQuestEventType != oldQuestEventType)
                {
                    questEventType.intValue = (int)newQuestEventType;
                    UnityEditor.EditorUtility.SetDirty(property.serializedObject.targetObject);
                }

                // Quest Task ID
                if (QuestEvent.QuestEventHasTaskParam(newQuestEventType) && newQuestID != 0 && questExists)
                {
                    elementRect = new Rect(rect.x + rect.width * columnWidth * 2, rect.y, rect.width * columnWidth, rect.height);
                    QuestDataCache.QuestData questData = _questDataCache.questsByID[(uint)newQuestID];
                    int oldTaskID = taskID.intValue;
                    int newTaskID = EditorGUI.IntPopup(elementRect, oldTaskID, questData.taskDropDownOptions, questData.taskDropDownValues);
                    if (newTaskID != oldTaskID)
                    {
                        taskID.intValue = (int)newTaskID;
                        UnityEditor.EditorUtility.SetDirty(property.serializedObject.targetObject);
                    }
                }
            }
            GUILayout.EndHorizontal();
            EditorGUI.EndProperty();
        }
    }
}