using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SpatialSys.UnitySDK
{
    [DisallowMultipleComponent]
    public class SpatialEnvironmentSettingsOverrides : SpatialComponentBase
    {
        public override string prettyName => "Environment Settings Overrides";
        public override string tooltip => "Use to override environment settings.";

        public override string documentationURL => "https://docs.spatial.io/components/environment-settings-overrides";

        protected override bool _limitOnePerScene => true;

        public EnvironmentSettings environmentSettings = new EnvironmentSettings();
    }
}
