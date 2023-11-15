using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialSFX))]
    public class SpatialSFXEditor : SpatialComponentEditorBase
    {
        public override void DrawFields()
        {
            SpatialSFX targetComponent = target as SpatialSFX;
            if (targetComponent.mixerGroup == null)
            {
                SpatialGUIUtility.HelpBox("Mixer Group Required", SpatialGUIUtility.HelpSectionType.Error);
            }
            base.DrawFields();
        }
    }
}
