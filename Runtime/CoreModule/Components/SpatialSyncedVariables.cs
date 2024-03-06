using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Spatial Components")]
    [AddComponentMenu("")]//hide from add component menu.
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Variables))]
    public class SpatialSyncedVariables : SpatialComponentBase
    {
        public override string prettyName => "Synced Variables";
        public override string tooltip => "Define which Visual Scripting Variables should be synced across clients.\nOnly bool, int, float, string, Vector2, and Vector3 types can be synced.";
        public override string documentationURL => null;

        [System.Serializable]
        public class Data
        {
            [HideInInspector]
            public byte id;
            public string name;
            public bool saveWithSpace;
            [HideInInspector]
            public byte syncRate = 10;

            public VariableDeclaration declaration { get; set; }
        }

        public List<Data> variableSettings = new List<Data>();
    }
}