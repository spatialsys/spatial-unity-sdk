using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        public static readonly int MAX_VERTS = 500 * k;
        public static readonly int MAX_UNIQUE_MATERIALS = 50;
        public static readonly int MAX_SHARED_TEXTURE_MB = 200;
        public static readonly int MAX_COLLIDER_VERTS = 75 * k;

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

        public int sharedTextureMB => materialTextureMB + lightmapTextureMB + reflectionProbeMB;

        //how long it took to analyze the scene.
        //used in sceneVitals to auto adjust the refresh rate.
        public float responseMiliseconds;

        public float vertPercent => (float)verts / MAX_VERTS;
        public float uniqueMaterialsPercent => (float)uniqueMaterials / MAX_UNIQUE_MATERIALS;
        public float sharedTexturePercent => (float)sharedTextureMB / MAX_SHARED_TEXTURE_MB;
        public float meshColliderVertPercent => (float)meshColliderVerts / MAX_COLLIDER_VERTS;
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

            PerformanceResponse response = new PerformanceResponse();
            response.sceneName = scene.name;
            response.scenePath = scene.path;

            // Count lightmaps size
            long bytes = 0;
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            foreach (LightmapData lightmap in lightmaps)
            {
                if (lightmap.lightmapColor != null)
                {
                    bytes += Profiler.GetRuntimeMemorySizeLong(lightmap.lightmapColor);
                }
                if (lightmap.lightmapDir != null)
                {
                    bytes += Profiler.GetRuntimeMemorySizeLong(lightmap.lightmapDir);
                }
                if (lightmap.shadowMask != null)
                {
                    bytes += Profiler.GetRuntimeMemorySizeLong(lightmap.shadowMask);
                }
            }
            response.lightmapTextureMB = (int)bytes / 1024 / 1024;

            // Count scene object sizes
            List<Texture> foundTextures = new List<Texture>();
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
                            foundTextures.Add(tex);
                        }
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

            response.uniqueVerts = foundMeshes.Distinct().Sum(m => m.vertexCount);
            response.uniqueMaterials = materials.FindAll(m => m != null).Select(m => m.name).Distinct().Count();

            // Count texture sizes
            bytes = 0;
            foreach (Texture texture in foundTextures.Distinct())
            {
                bytes += Profiler.GetRuntimeMemorySizeLong(texture);
            }
            response.materialTextureMB = (int)(bytes / 1024 / 1024);

            // Count mesh collider tris
            MeshCollider[] meshColliders = GameObject.FindObjectsOfType<MeshCollider>(true);
            foreach (MeshCollider meshCollider in meshColliders)
            {
                if (meshCollider.sharedMesh != null)
                {
                    response.meshColliderVerts += meshCollider.sharedMesh.vertexCount;
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

            timer.Stop();
            response.responseMiliseconds = timer.ElapsedMilliseconds;

            return response;
        }

        [SceneTest]
        public static void TestScenePerformance(Scene scene)
        {
            if (SpatialValidator.validationContext == ValidationContext.Testing)
                return;

            PerformanceResponse response = GetActiveScenePerformanceResponse();


            if (response.meshColliderVertPercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"Scene {scene.name} has a lot of high density mesh colliders.",
                    "You should try to use primitives or low density meshes for colliders where possible. High density collision gemoetry will impact the performance of your environment."
                ));
            }

            if (!response.hasLightmaps)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"Scene {scene.name} does not have lightmaps.",
                    "It is highly reccomended that you bake lightmaps in each scene. This will greatly improve the fidelity of your environment."
                ));
            }

            if (!response.hasLightprobes)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Warning,
                    $"Scene {scene.name} does not have light probes.",
                    "It is highly reccomended that you bake light probes in each scene. This will allow avatars to interact with the baked lights in your environment properly."
                ));
            }

            //skipping reflection probe warning. Not everyone will benefit much from them.

            if (response.vertPercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Fail,
                    $"Scene {scene.name} has too many vertices.",
                    "The scene has too many high detail models. You will need to reduce the total vertex count in your scene before you can publish."
                ));
            }

            if (response.uniqueMaterialsPercent > 1f)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    null,
                    TestResponseType.Fail,
                    $"Scene {scene.name} has too many unique materials.",
                    $"Environments are limited to {PerformanceResponse.MAX_UNIQUE_MATERIALS} unique materials. You will need to reduce the number of unique materials in your scene before you can publish."
                ));
            }
        }
    }
}
