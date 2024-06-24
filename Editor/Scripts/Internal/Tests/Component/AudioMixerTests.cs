using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace SpatialSys.UnitySDK.Editor
{
    public class AudioMixerTests
    {
        public const string MASTER_VOLUME_PARAM = "masterVolume";
        public const string SOUND_EFFECTS_VOLUME_PARAM = "soundEffectsVolume";
        public const string MUSIC_VOLUME_PARAM = "musicVolume";
        public const string AMBIENCE_VOLUME_PARAM = "ambienceVolume";
        public const string INTERFACE_VOLUME_PARAM = "interfaceVolume";
        public const string DIALOGUE_VOLUME_PARAM = "dialogueVolume";

        [ComponentTest(typeof(AudioSource))]
        public static void CheckSceneAudioSources(AudioSource target)
        {
            if (target.outputAudioMixerGroup == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        target,
                        SpatialValidator.runContext == ValidationRunContext.UploadingToSandbox ? TestResponseType.Warning : TestResponseType.Fail,
                        $"Audio Source on scene object \"{target.gameObject.name}\" is missing a mixer group.",
                        "All audio sources used must have a mixer group assigned."
                    )
                );
            }
        }

        [PackageTest]
        // TODO: We are scrubbing through all the dependencies for lots of things here.
        // At some point this should be abstracted into [DependencyTest]'s or something.
        // Maybe [ComponentTest] should scan dependencies... idk
        public static void CheckDependenciesForAudioIssues(PackageConfig config)
        {
            List<AudioMixer> mixers = new();
            List<SpatialSFX> sfx = new();
            List<GameObject> prefabs = new();

            foreach (UnityEngine.Object asset in config.assets)
            {
                foreach (string path in AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(asset), true))
                {
                    UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                    if (obj is AudioMixer mixer)
                    {
                        mixers.Add(mixer);
                    }
                    else if (obj is SpatialSFX sfxSource)
                    {
                        sfx.Add(sfxSource);
                    }
                    else if (obj is GameObject prefab)
                    {
                        prefabs.Add(prefab);
                    }
                }
            }

            //* Make sure all mixers have the required exposed parameters
            foreach (AudioMixer mixer in mixers)
            {
                List<string> missingParams = new List<string>();

                float f;
                if (!mixer.GetFloat(MASTER_VOLUME_PARAM, out f))
                    missingParams.Add(MASTER_VOLUME_PARAM);
                if (!mixer.GetFloat(MUSIC_VOLUME_PARAM, out f))
                    missingParams.Add(MUSIC_VOLUME_PARAM);
                if (!mixer.GetFloat(AMBIENCE_VOLUME_PARAM, out f))
                    missingParams.Add(AMBIENCE_VOLUME_PARAM);
                if (!mixer.GetFloat(SOUND_EFFECTS_VOLUME_PARAM, out f))
                    missingParams.Add(SOUND_EFFECTS_VOLUME_PARAM);
                if (!mixer.GetFloat(INTERFACE_VOLUME_PARAM, out f))
                    missingParams.Add(INTERFACE_VOLUME_PARAM);
                if (!mixer.GetFloat(DIALOGUE_VOLUME_PARAM, out f))
                    missingParams.Add(DIALOGUE_VOLUME_PARAM);

                if (missingParams.Count > 0)
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(
                            mixer,
                            SpatialValidator.runContext == ValidationRunContext.UploadingToSandbox ? TestResponseType.Warning : TestResponseType.Fail,
                            $"Audio Mixer {mixer.name} is missing required exposed parameters.",
$@"All audio mixers must have the following parameters:
- {MASTER_VOLUME_PARAM}, {SOUND_EFFECTS_VOLUME_PARAM}, {MUSIC_VOLUME_PARAM}, {AMBIENCE_VOLUME_PARAM}, {INTERFACE_VOLUME_PARAM}, {DIALOGUE_VOLUME_PARAM}

This audio mixer is missing the following:
- {string.Join(", ", missingParams)}"
                        )
                    );
                }
            }

            //* Make sure all SFX have a mixer group
            foreach (SpatialSFX sfxSource in sfx)
            {
                if (sfxSource.mixerGroup == null)
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(
                            sfxSource,
                            SpatialValidator.runContext == ValidationRunContext.UploadingToSandbox ? TestResponseType.Warning : TestResponseType.Fail,
                            $"Spatial SFX \"{sfxSource.name}\" is missing a mixer group.",
                            "All SFX assets used must have a mixer group assigned."
                        )
                    );
                }
            }

            //* Make sure all prefabs with audio sources have mixer groups
            foreach (GameObject prefab in prefabs)
            {
                foreach (AudioSource source in prefab.GetComponentsInChildren<AudioSource>(true))
                {
                    if (source.outputAudioMixerGroup == null)
                    {
                        SpatialValidator.AddResponse(
                            new SpatialTestResponse(
                                source,
                                SpatialValidator.runContext == ValidationRunContext.UploadingToSandbox ? TestResponseType.Warning : TestResponseType.Fail,
                                $"Audio Source on Prefab \"{prefab.name}\" is missing a mixer group.",
                                "All audio sources used must have a mixer group assigned."
                            )
                        );
                    }
                }
            }
        }
    }
}