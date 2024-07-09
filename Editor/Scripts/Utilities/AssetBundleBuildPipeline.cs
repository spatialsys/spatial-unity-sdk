using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;

namespace SpatialSys.UnitySDK.Editor
{
    public static class AssetBundleBuildPipeline
    {
        public static ReturnCode Build(string outputPath, UnityEngine.BuildCompression compression, BuildTarget targetPlatform, out IBundleBuildResults results)
        {
            // Gets all asset bundles defined through the AssetDatabase API.
            // TODO: Replace this with a list of asset paths. Don't set asset bundle names in project anymore.
            BundleBuildContent buildContent = new(ContentBuildInterface.GenerateAssetBundleBuilds());

            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(targetPlatform);
            BundleBuildParameters parameters = new(targetPlatform, targetGroup, outputPath);
            parameters.UseCache = false;
            parameters.BundleCompression = compression;

            ReturnCode status = ContentPipeline.BuildAssetBundles(parameters, buildContent, out results);
            return status;
        }
    }
}
