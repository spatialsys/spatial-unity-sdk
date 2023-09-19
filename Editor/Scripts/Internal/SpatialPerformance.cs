using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine.Profiling;
using System.Linq;
using System.IO;
using System;

namespace SpatialSys.UnitySDK.Editor
{
    public class PackageSizeResponse
    {
        public string packageName;
        public string packagePath;
        public int packageSizeMB;
    }

    public class PerformanceResponse
    {
        private const int m = 1000000;
        private const int k = 1000;

        public static readonly int MAX_SUGGESTED_VERTS = 500 * k;
        public static readonly int MAX_SUGGESTED_UNIQUE_MATERIALS = 75;
        public static readonly int MAX_SUGGESTED_SHARED_TEXTURE_MB = 256;
        public static readonly int MAX_SUGGESTED_COLLIDER_VERTS = 75 * k;
        public static readonly int MAX_SUGGESTED_AUDIO_MB = 16;

        public string sceneName;
        public string scenePath;

        public bool hasLightmaps => lightmapTextureMB > 0;
        public bool hasLightprobes;
        public bool hasReflectionProbes;

        public int lightmapTextureMB;
        public int verts;
        public int uniqueVerts;
        public int uniqueMaterials;
        public int materialTextureMB;
        public int meshColliderVerts;
        public int realtimeLights;
        public int reflectionProbeMB;//textures
        public int graphicTextureMB;
        public int audioMB;

        // Per asset data about their size
        public IReadOnlyList<Tuple<string, int>> meshVertCounts;
        public IReadOnlyList<Tuple<string, int>> meshColliderVertCounts;
        public IReadOnlyList<Tuple<string, float>> textureMemorySizesMB;

        public int sharedTextureMB => materialTextureMB + lightmapTextureMB + graphicTextureMB + reflectionProbeMB;

        //how long it took to analyze the scene.
        //used in sceneVitals to auto adjust the refresh rate.
        public long responseMilliseconds;

        public float vertPercent => (float)verts / MAX_SUGGESTED_VERTS;
        public float uniqueMaterialsPercent => (float)uniqueMaterials / MAX_SUGGESTED_UNIQUE_MATERIALS;
        public float sharedTexturePercent => (float)sharedTextureMB / MAX_SUGGESTED_SHARED_TEXTURE_MB;
        public float meshColliderVertPercent => (float)meshColliderVerts / MAX_SUGGESTED_COLLIDER_VERTS;
        public float audioPercent => (float)audioMB / MAX_SUGGESTED_AUDIO_MB;
    }

    public class SpatialPerformance
    {
        // WIP
        public static void GetActiveScenePackageResponseSlow()
        {
            //todo to be removed. Counting files one by one is faster that packaging the scene.
            PackageSizeResponse response = new PackageSizeResponse();
            string tempOutputPath = "Temp/SpatialPackage.unitypackage";
            BuildUtility.PackageActiveScene(tempOutputPath);
            FileInfo packageInfo = new FileInfo(tempOutputPath);
            response.packageSizeMB = (int)packageInfo.Length / 1024 / 1024;
            Debug.LogError("estimated package size: " + response.packageSizeMB + "MB");

            string scenePath = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
            string[] assetPaths = AssetDatabase.GetDependencies(scenePath, true);
            assetPaths.Append(scenePath);

            long bytes = 0;
            foreach (string assetPath in assetPaths)
            {
                Type type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
                bytes += Profiler.GetRuntimeMemorySizeLong(asset);
                //todo we want to estimate the build size of assets here, not runtime size.
            }
        }

        //Takes usually 1ms or less on sample scene
        public static PerformanceResponse GetActiveScenePerformanceResponse()
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var scene = EditorSceneManager.GetActiveScene();

            List<Tuple<string, int>> meshVertCounts = new List<Tuple<string, int>>();
            List<Tuple<string, int>> meshColliderVertCounts = new List<Tuple<string, int>>();
            List<Tuple<string, float>> textureSizesMB = new List<Tuple<string, float>>();

            PerformanceResponse response = new PerformanceResponse();
            response.sceneName = scene.name;
            response.scenePath = scene.path;
            response.meshVertCounts = meshVertCounts;
            response.meshColliderVertCounts = meshColliderVertCounts;
            response.textureMemorySizesMB = textureSizesMB;

            // Count lightmaps size
            long bytes = 0;
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            foreach (LightmapData lightmap in lightmaps)
            {
                if (lightmap.lightmapColor != null)
                {
                    long sizeInBytes = Profiler.GetRuntimeMemorySizeLong(lightmap.lightmapColor);
                    bytes += sizeInBytes;
                    textureSizesMB.Add(new Tuple<string, float>(AssetDatabase.GetAssetPath(lightmap.lightmapColor), sizeInBytes / 1024f / 1024f));
                }
                if (lightmap.lightmapDir != null)
                {
                    long sizeInBytes = Profiler.GetRuntimeMemorySizeLong(lightmap.lightmapDir);
                    bytes += sizeInBytes;
                    textureSizesMB.Add(new Tuple<string, float>(AssetDatabase.GetAssetPath(lightmap.lightmapDir), sizeInBytes / 1024f / 1024f));
                }
                if (lightmap.shadowMask != null)
                {
                    long sizeInBytes = Profiler.GetRuntimeMemorySizeLong(lightmap.shadowMask);
                    bytes += sizeInBytes;
                    textureSizesMB.Add(new Tuple<string, float>(AssetDatabase.GetAssetPath(lightmap.shadowMask), sizeInBytes / 1024f / 1024f));
                }
            }
            response.lightmapTextureMB = (int)bytes / 1024 / 1024;

            // Count scene object sizes
            List<Texture> foundMaterialTextures = new List<Texture>();
            List<Texture> foundGraphicTextures = GameObject.FindObjectsOfType<Graphic>(true).Select(g => g.mainTexture).ToList();
            List<Material> materials = new List<Material>();
            List<Mesh> foundMeshes = new List<Mesh>();
            Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                //look for materials and textures
                materials.AddRange(renderer.sharedMaterials);
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null)
                    {
                        continue;
                    }
                    foreach (var texName in material.GetTexturePropertyNames())
                    {
                        var tex = material.GetTexture(texName);
                        if (tex != null)
                        {
                            foundMaterialTextures.Add(tex);
                        }
                    }
                }

                // look for sprite textures
                if (renderer is SpriteRenderer)
                {
                    SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
                    if (spriteRenderer.sprite != null && spriteRenderer.sprite.texture != null)
                    {
                        foundGraphicTextures.Add(spriteRenderer.sprite.texture);
                    }
                }

                // look for mesh
                if (renderer is MeshRenderer)
                {
                    MeshFilter filter = renderer.GetComponent<MeshFilter>();
                    if (filter != null && filter.sharedMesh != null)
                    {
                        foundMeshes.Add(filter.sharedMesh);
                        response.verts += filter.sharedMesh.vertexCount;
                    }
                }
                else if (renderer is SkinnedMeshRenderer)
                {
                    SkinnedMeshRenderer skinned = renderer as SkinnedMeshRenderer;
                    if (skinned.sharedMesh != null)
                    {
                        foundMeshes.Add(skinned.sharedMesh);
                        response.verts += skinned.sharedMesh.vertexCount;
                    }
                }
                else if (renderer is BillboardRenderer)
                {
                    response.verts += 4;
                }
            }
            IEnumerable<Mesh> uniqueMeshes = foundMeshes.Distinct();
            meshVertCounts.AddRange(uniqueMeshes.Select(m => new Tuple<string, int>(AssetDatabase.GetAssetPath(m), m.vertexCount)));
            response.uniqueVerts = uniqueMeshes.Distinct().Sum(m => m.vertexCount);
            response.uniqueMaterials = materials.FindAll(m => m != null).Select(m => m.name).Distinct().Count();

            // Count skybox material
            Material skyboxMaterial = RenderSettings.skybox;
            if (skyboxMaterial != null)
            {
                foreach (var texName in skyboxMaterial.GetTexturePropertyNames())
                {
                    var tex = skyboxMaterial.GetTexture(texName);
                    if (tex != null)
                    {
                        foundMaterialTextures.Add(tex);
                    }
                }
            }

            // Count material texture sizes
            bytes = 0;
            foreach (Texture texture in foundMaterialTextures.Distinct())
            {
                long sizeInBytes = Profiler.GetRuntimeMemorySizeLong(texture);
                bytes += sizeInBytes;
                textureSizesMB.Add(new Tuple<string, float>(AssetDatabase.GetAssetPath(texture), sizeInBytes / 1024f / 1024f));
            }
            response.materialTextureMB = (int)(bytes / 1024f / 1024f);

            // Count graphic texture sizes
            bytes = 0;
            foreach (Texture texture in foundGraphicTextures.Distinct())
            {
                if (texture == null)
                {
                    continue;
                }
                long sizeInBytes = Profiler.GetRuntimeMemorySizeLong(texture);
                bytes += sizeInBytes;

                string texturePath = AssetDatabase.GetAssetPath(texture);
                if (!string.IsNullOrEmpty(texturePath))
                {
                    textureSizesMB.Add(new Tuple<string, float>(texturePath, sizeInBytes / 1024f / 1024f));
                }
            }
            response.graphicTextureMB = (int)(bytes / 1024f / 1024f);

            // Count mesh collider vertices
            MeshCollider[] meshColliders = GameObject.FindObjectsOfType<MeshCollider>(true);
            foreach (MeshCollider meshCollider in meshColliders)
            {
                if (meshCollider.sharedMesh != null)
                {
                    response.meshColliderVerts += meshCollider.sharedMesh.vertexCount;
                    meshColliderVertCounts.Add(new Tuple<string, int>(GetGameObjectPath(meshCollider.gameObject), meshCollider.sharedMesh.vertexCount));
                }
            }

            // Look for light / reflection probes
            LightProbeGroup[] lightProbeGroups = GameObject.FindObjectsOfType<LightProbeGroup>(true);
            foreach (LightProbeGroup lightProbeGroup in lightProbeGroups)
            {
                if (lightProbeGroup.probePositions.Length > 0)
                {
                    response.hasLightprobes = true;
                    break;
                }
            }
            if (GameObject.FindObjectsOfType<ReflectionProbe>(true).Length > 0)
            {
                response.hasReflectionProbes = true;
            }

            // Look for lights
            Light[] lights = GameObject.FindObjectsOfType<Light>(true);
            response.realtimeLights = lights.Where(l => l.lightmapBakeType != LightmapBakeType.Baked).Count();

            bytes = 0;
            ReflectionProbe[] reflectionProbes = GameObject.FindObjectsOfType<ReflectionProbe>(true);
            foreach (ReflectionProbe probe in reflectionProbes)
            {
                if (probe.mode == UnityEngine.Rendering.ReflectionProbeMode.Baked && probe.texture != null)
                {
                    bytes += Profiler.GetRuntimeMemorySizeLong(probe.texture);
                }
                //realtime probes are currently disabled... but leaving this incase we enable them down the road.
                else if (probe.mode == UnityEngine.Rendering.ReflectionProbeMode.Realtime)
                {
                    bytes += probe.resolution * probe.resolution * 3;
                }
            }
            response.reflectionProbeMB = (int)(bytes / 1024 / 1024);

            // Look for audio
            bytes = 0;
            List<AudioClip> audioClips = new();
            foreach (string path in AssetDatabase.GetDependencies(scene.path, true))
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                if (obj is AudioClip)
                {
                    AudioClip audioClip = obj as AudioClip;
                    audioClips.Add(audioClip);
                }
            }

            foreach (AudioClip audioClip in audioClips.Distinct())
            {
                long sizeInBytes = Profiler.GetRuntimeMemorySizeLong(audioClip);
                bytes += sizeInBytes;
            }

            response.audioMB = (int)(bytes / 1024 / 1024);

            // Sort by size descending
            meshVertCounts.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            meshColliderVertCounts.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            textureSizesMB.Sort((a, b) => b.Item2.CompareTo(a.Item2));

            timer.Stop();
            response.responseMilliseconds = timer.ElapsedMilliseconds;

            return response;
        }

        private static string GetGameObjectPath(GameObject obj)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            GetGameObjectRecursive(obj.transform, stringBuilder);
            return stringBuilder.ToString();
        }

        private static void GetGameObjectRecursive(Transform t, System.Text.StringBuilder stringBuilder)
        {
            if (t.parent != null)
                GetGameObjectRecursive(t.parent, stringBuilder);

            stringBuilder.AppendFormat("/{0}", t.gameObject.name);
        }

        [SceneTest]
        public static void TestScenePerformance(Scene scene)
        {
            if (SpatialValidator.validationContext == ValidationContext.UploadingToSandbox)
                return;

            PerformanceResponse response = GetActiveScenePerformanceResponse();

            if (response.meshColliderVertPercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"Scene {scene.name} has a lot of high density mesh colliders ({response.meshColliderVerts}/{PerformanceResponse.MAX_SUGGESTED_COLLIDER_VERTS}).",
                    "You should try to use primitives or low density meshes for colliders where possible. "
                    + "High density collision geometry will impact the performance of your space.\n"

                    + "Here's a list of all objects with high density mesh colliders:\n - " + string.Join("\n - ", response.meshColliderVertCounts.Take(30).Select(m => $"{m.Item2} - {m.Item1}"))
                ));
            }

            //skipping reflection probe warning. Not everyone will benefit much from them.

            if (response.vertPercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"Scene {scene.name} has too many vertices.",
                    "The scene has too many high detail models. It is recommended that you stay within the suggested limits or your asset may not perform well on all platforms.\n"
                    + "Here's a list of all objects with high vertex counts:\n - " + string.Join("\n - ", response.meshVertCounts.Take(30).Select(m => $"{m.Item2} - {m.Item1}"))
                ));
            }

            if (response.uniqueMaterialsPercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"Scene {scene.name} has many unique materials.",
                    $"It is encouraged for scenes to limit unique materials to around {PerformanceResponse.MAX_SUGGESTED_UNIQUE_MATERIALS}. "
                    + "The more unique materials you have, the less likely it is that your asset will perform well on all platforms. "
                    + "Look into texture atlasing techniques to share textures and materials across multiple separate objects."
                ));
            }

            if (response.sharedTexturePercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"Scene {scene.name} has too many shared textures.",
                    $"Space Templates are limited to {PerformanceResponse.MAX_SUGGESTED_SHARED_TEXTURE_MB} MB of shared textures. "
                    + "High memory usage can cause application crashes on lower end devices. It is highly recommended that you stay within the suggested limits. "
                    + "Compressing your textures will help reduce their size.\n"
                    + "Here's a list of all textures used by the scene:\n - " + string.Join("\n - ", response.textureMemorySizesMB.Take(40).Select(m => $"{m.Item2:0.00}MB - {m.Item1}"))
                ));
            }

            if (response.audioPercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"The audio clips in scene {scene.name} are using too much memory. ",
                    $"It is recommended that you limit your scene to {PerformanceResponse.MAX_SUGGESTED_AUDIO_MB} MB of audio memory. "
                    + "Converting audio files to .ogg and mono can help reduce its size."
                ));
            }
        }
    }
}
