using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Core/Components")]
    [AddComponentMenu("")]//hide from add component menu.
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Variables))]
    public class SpatialSyncedVariables : SpatialComponentBase
    {
        public override string prettyName => "Synced Variables";
        public override string tooltip => "Define which Visual Scripting Variables should be synced across clients.\nOnly bool, int, float, string, Vector2, and Vector3 types can be synced.";
        public override string documentationURL => null;

        private const int LATEST_VERSION = 1;
        [HideInInspector]
        public int version = 0;

        [System.Serializable]
        [DocumentationCategory("Core/Components")]
        public class Data
        {
            public byte id;
            public string name;
            public bool saveWithSpace;
            [HideInInspector]
            public byte syncRate = 10;

            public VariableDeclaration declaration { get; set; }
        }

        public List<Data> variableSettings = new List<Data>();

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            UpgradeDataIfNecessary();
        }

        public void UpgradeDataIfNecessary()
        {
            // Don't upgrade at runtime in editor
            if (Application.isPlaying)
                return;

            if (version == LATEST_VERSION)
                return;

            // Version 0 was the initial version; Converting to V1 is just about pre-calculating the id
            if (version == 0)
            {
                byte id = 0;
                foreach (var variable in variableSettings)
                    variable.id = id++;

                version = 1;
            }

            // Add future upgrade paths here
        }
#endif

        public byte GenerateUniqueVariableID()
        {
            byte id = 0;
            while (variableSettings.Any(v => v.id == id))
                id++;
            return id;
        }
    }
}