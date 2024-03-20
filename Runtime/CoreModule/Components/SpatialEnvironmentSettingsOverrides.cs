using SpatialSys.UnitySDK.Internal;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Components")]
    [DisallowMultipleComponent]
    public class SpatialEnvironmentSettingsOverrides : SpatialComponentBase
    {
        public override string prettyName => "Environment Settings Overrides";
        public override string tooltip => "Use to override environment settings.";

        public override string documentationURL => "https://docs.spatial.io/components/environment-settings-overrides";

        protected override bool _limitOnePerScene => true;

        public EnvironmentSettings environmentSettings = new EnvironmentSettings();

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.15f, 0f, 0.4f);
            Gizmos.DrawCube(new Vector3(0f, environmentSettings.respawnLevelY, 0f), new Vector3(100000f, 0f, 100000f));
        }
#endif
    }
}
