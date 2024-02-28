using SpatialSys.UnitySDK.Internal;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DisallowMultipleComponent]
    public class SpatialRenderPipelineSettingsOverrides : SpatialComponentBase
    {
        public override string prettyName => "RenderPipeline Settings Overrides";
        public override string tooltip => "Use to override render pipeline settings.";

        public override string documentationURL => "https://docs.spatial.io/components/render-pipeline-settings-overrides";

        protected override bool _limitOnePerScene => true;

        public bool overrideSettings = false;
        public RenderPipelineSettings renderPipelineSettings = new RenderPipelineSettings();
    }
}
