using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorAudioService : IAudioService
    {
        public void PlaySFX(SpatialSFX sfx, float extraVolume = 0, float extraPitch = 0)
        {
        }

        public void PlaySFX(SpatialSFX sfx, Vector3 position, float extraVolume = 0, float extraPitch = 0)
        {
        }

        public void PlaySFX(SpatialSFX sfx, AudioSource source, float extraVolume = 0, float extraPitch = 0)
        {
            if (sfx && sfx.clips.Length > 0)
            {
                source.clip = sfx.clips[Random.Range(0, sfx.clips.Length)];
                if (source.clip != null)
                {
                    source.volume = Random.Range(sfx.volume.x, sfx.volume.y) + extraVolume;
                    source.pitch = Random.Range(sfx.pitch.x, sfx.pitch.y) + extraPitch;
                    source.spatialBlend = sfx.spatialBlend;
                    source.reverbZoneMix = sfx.reverbMix;
                    source.minDistance = sfx.rollOffMin;
                    source.maxDistance = sfx.rollOffMax;
                    source.rolloffMode = sfx.rolloff;
                    source.PlayOneShot(source.clip);
                }
            }
        }
    }
}