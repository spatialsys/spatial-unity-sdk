// Copied and adapted from https://gist.github.com/yasirkula/d8fa2fb5f22aefcc7a232f6feeb91db7
// Licensed under MIT License https://yasirkula.itch.io/unity3d

// The MIT License (MIT)
// Copyright 2020 Süleyman Yasir KULA
// All rights reserved.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace SpatialSys.UnitySDK.Editor
{
    public class ShaderStripper : IPreprocessShaders
    {
        private static bool SKIP_TIER_3_ON_MOBILE_AND_WEB = true;

        // To skip lower LOD
        private HashSet<string> _urpShaders = new HashSet<string>();

        // https://docs.unity3d.com/ScriptReference/Rendering.PassType.html
        private static readonly PassType[] SKIPPED_SHADER_PASS_TYPE = new PassType[]
        {
            PassType.Deferred,
            PassType.Meta,
            PassType.MotionVectors,
        };

        private static readonly ShaderKeyword[] SKIPPED_VARIANTS_COMMON = new ShaderKeyword[]
        {
            // Built-in shader defines
            // https://docs.unity3d.com/ScriptReference/Rendering.BuiltinShaderDefine.html
            new ShaderKeyword("UNITY_NO_DXT5nm"), // Spatial platforms support dxt5nm textures
            new ShaderKeyword("UNITY_ENABLE_REFLECTION_BUFFERS"), // deferred reflection
            new ShaderKeyword("UNITY_FRAMEBUFFER_FETCH_AVAILABLE"),
            new ShaderKeyword("UNITY_ENABLE_NATIVE_SHADOW_LOOKUPS"),
            new ShaderKeyword("UNITY_USE_DITHER_MASK_FOR_ALPHABLENDED_SHADOWS"), // there's no way to use semi-transparent shadows.
            new ShaderKeyword("SHADER_API_DESKTOP"),
            // https://docs.unity3d.com/Manual/graphics-tiers.html
            new ShaderKeyword("UNITY_HARDWARE_TIER1"), // platforms that only support OpenGL ES 2.0
            new ShaderKeyword("UNITY_HARDWARE_TIER3"), // desktop platforms
            new ShaderKeyword("UNITY_COLORSPACE_GAMMA"), // not using gamma space
            new ShaderKeyword("UNITY_LIGHT_PROBE_PROXY_VOLUME"), // not using LPPV
            new ShaderKeyword("UNITY_LIGHTMAP_FULL_HDR"), // not using HDR lightmap (High Quality of Lightmap Encoding mode)
            new ShaderKeyword("UNITY_VIRTUAL_TEXTURING"), // not using RVT
            new ShaderKeyword("UNITY_PRETRANSFORM_TO_DISPLAY_ORIENTATION"), // not using Vulkan API
            new ShaderKeyword("UNITY_UNIFIED_SHADER_PRECISION_MODEL"), // not using unified shader precision model
        };

        // Lit, SimpleLit and Unlit
        private static readonly ShaderKeyword[] SKIPPED_VARIANTS_URP_SHADERS = new ShaderKeyword[]
        {
            new ShaderKeyword("DOTS_INSTANCING_ON"),
            new ShaderKeyword("_DBUFFER_MRT1"),
            new ShaderKeyword("_DBUFFER_MRT2"),
            new ShaderKeyword("_DBUFFER_MRT3"),
            new ShaderKeyword("DEBUG_DISPLAY"), // Debug shaders should be stripped by editor settings, but make sure
        };

        private static readonly ShaderKeyword[] SKIPPED_VARIANTS_LIT = new ShaderKeyword[]
        {
            new ShaderKeyword("_MAIN_LIGHT_SHADOWS_CASCADE"), // not using cascade shadows
            new ShaderKeyword("_MAIN_LIGHT_SHADOWS_SCREEN"), // not using Screen Space Shadows
            new ShaderKeyword("_ADDITIONAL_LIGHT_SHADOWS"), // not using additional light shadows
        };

        public static bool ContainsKeyword(ShaderKeywordSet keywordSet, ShaderKeyword[] keywords)
        {
            foreach (ShaderKeyword keyword in keywords)
            {
                if (keywordSet.IsEnabled(keyword))
                {
                    return true;
                }
            }
            return false;
        }

        public int callbackOrder { get { return 0; } }

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            // Don't strip essential shaders
            string shaderName = shader.name;
            if (shaderName.StartsWith("Hidden/") || shaderName.StartsWith("Unlit/"))
                return;

            // Skip specific shader passes.
            if (SKIPPED_SHADER_PASS_TYPE.Contains(snippet.passType))
            {
                data.Clear();
            }

            bool isLitShader = shaderName.Contains("Universal Render Pipeline/Lit");
            bool isSimpleLitShader = shaderName.Contains("Universal Render Pipeline/Simple Lit");
            bool isUnlitShader = shaderName.Contains("Universal Render Pipeline/Unlit");
            bool isUrpShader = isLitShader || isSimpleLitShader || isUnlitShader;
            if (isUrpShader)
            {
                string fullShaderName = shaderName + snippet.passName + snippet.passType + snippet.shaderType;
                if (_urpShaders.Contains(fullShaderName))
                {
                    // Only include the shader the first time we see it.
                    // If we see the same shader name/pass/passtype/shadertype, then assume it's the same but a different LOD.
                    // Unfortunately, shader.maximumLOD always seems to be -1 so we have to use this hack.
                    data.Clear();
                }
                else
                {
                    _urpShaders.Add(fullShaderName);
                }
            }

            // Skip variants
            for (int i = data.Count - 1; i >= 0; --i)
            {
                bool skip = ContainsKeyword(data[i].shaderKeywordSet, SKIPPED_VARIANTS_COMMON);
                skip |= SKIP_TIER_3_ON_MOBILE_AND_WEB && data[i].graphicsTier == GraphicsTier.Tier3;

                if (isUrpShader)
                {
                    skip |= ContainsKeyword(data[i].shaderKeywordSet, SKIPPED_VARIANTS_URP_SHADERS);
                }

                if (isLitShader)
                {
                    skip |= ContainsKeyword(data[i].shaderKeywordSet, SKIPPED_VARIANTS_LIT);
                }

                if (skip)
                {
                    // Skip this shader variant.
                    data.RemoveAt(i);
                    continue;
                }
            }
        }
    }
}
