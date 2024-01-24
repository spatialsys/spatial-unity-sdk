using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    // Helper class used to run coroutines in the editor. By placing this class on a shared file, Unity
    // doesn't detect this file as an 'Editor' file and lets us attach it to a GameObject.
    public class CoroutineRunner: MonoBehaviour
    {
    }
    public class EditorAudioService : IAudioService
    {
        private Queue<AudioSource> _audioSources = new Queue<AudioSource>();
        private List<AudioSource> _activeAudioSources = new List<AudioSource>();
        private CoroutineRunner _coroutineRunner;

        private Transform _audioServiceTransform;

        public EditorAudioService()
        {
            GameObject g = new GameObject();
            g.name = "[Spatial SDK] Audio Service";
            _audioServiceTransform = g.transform;
            _coroutineRunner = g.AddComponent<CoroutineRunner>();

            // Ensure there's an audio listener
            if (GameObject.FindObjectsOfType<AudioListener>().Length == 0)
            {
                g.AddComponent<AudioListener>();
            }
        }

        private AudioSource CreateAudioSource()
        {
            GameObject g = new GameObject();
            g.name = "[Spatial SDK] Audio Source";
            g.transform.parent = _audioServiceTransform;
            return g.AddComponent<AudioSource>();
        }

        private void PlaySFXInternal(SpatialSFX sfx, Vector3 position, AudioSource source, float extraVolume, float extraPitch, bool isPooled)
        {
            if (sfx.clips.Length == 0)
            {
                return;
            }

            source.clip = sfx.clips[Random.Range(0, sfx.clips.Length)];
            if (source.clip == null)
            {
                if (isPooled)
                {
                    _audioSources.Enqueue(source);
                }
                return;
            }

            source.volume = Random.Range(sfx.volume.x, sfx.volume.y) + extraVolume;
            source.pitch = Random.Range(sfx.pitch.x, sfx.pitch.y) + extraPitch;
            source.spatialBlend = sfx.spatialBlend;
            source.reverbZoneMix = sfx.reverbMix;
            source.minDistance = sfx.rollOffMin;
            source.maxDistance = sfx.rollOffMax;
            source.rolloffMode = sfx.rolloff;

            if (isPooled)
            {
                source.mute = false;
                source.loop = false;
                source.transform.position = position;
                source.Play();
                _coroutineRunner.StartCoroutine(ReturnAudioSource(source, source.clip.length));
                _activeAudioSources.Add(source);
            }
            else
            {
                source.PlayOneShot(source.clip);
            }
        }

        private IEnumerator ReturnAudioSource(AudioSource source, float clipLength)
        {
            yield return new WaitForSeconds(clipLength);
            _audioSources.Enqueue(source);
            _activeAudioSources.Remove(source);
        }

        //-----------------------------------------------------------------------------------------
        // Public API
        //-----------------------------------------------------------------------------------------

        public void PlaySFX(SpatialSFX sfx, float extraVolume = 0, float extraPitch = 0)
        {
            PlaySFX(sfx, Vector3.zero, extraVolume, extraPitch);
        }

        public void PlaySFX(SpatialSFX sfx, Vector3 position, float extraVolume = 0, float extraPitch = 0)
        {
            AudioSource source;
            if (_audioSources.Count == 0)
            {
                source = CreateAudioSource();
            }
            else
            {
                source = _audioSources.Dequeue();
            }
            source.outputAudioMixerGroup = sfx.mixerGroup;
            PlaySFXInternal(sfx, position, source, extraVolume, extraPitch, true);
        }

        public void PlaySFX(SpatialSFX sfx, AudioSource source, float extraVolume = 0, float extraPitch = 0)
        {
            PlaySFXInternal(sfx, Vector3.zero, source, extraVolume, extraPitch, false);
        }
    }
}