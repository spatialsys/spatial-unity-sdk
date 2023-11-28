using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Audio;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK
{
    [CreateAssetMenu(fileName = "newSFX", menuName = "Spatial/SFX", order = 1)]
    [TypeIcon(typeof(SFXIcon))]
    public class SpatialSFX : SpatialScriptableObjectBase
    {
        public override string prettyName => "Spatial SFX";
        public override string tooltip => "Define a reusable and randomized sound effect.";
        public override string documentationURL => "https://docs.spatial.io/spatial-sfx";
        public override bool isExperimental => false;

        [SerializeField]
        private AudioMixerGroup _mixerGroup;
        [SerializeField]
        private AudioClip[] _clips;
        [Space(4)]
        [MinMax(1f, 1f, 0f, 1f)]
        [SerializeField]
        private Vector2 _volume = new Vector2(1f, 1f);

        [Space(8)]
        [MinMax(1f, 1f, 0f, 2f)]
        [SerializeField]
        private Vector2 _pitch = new Vector2(1f, 1f);
        [Space(16)]
        [Header("3D Sound Settings")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _spatialBlend = 1f;
        [Range(0f, 1f)]
        [SerializeField]
        private float _reverbMix = 1f;
        [SerializeField]
        private AudioRolloffMode _rolloff = AudioRolloffMode.Linear;
        [SerializeField]
        private float _rollOffMin = 5f;
        [SerializeField]
        private float _rollOffMax = 200f;

        public AudioMixerGroup mixerGroup { get { return _mixerGroup; } }
        public AudioClip[] clips { get { return (AudioClip[])_clips.Clone(); } }
        public Vector2 volume { get { return _volume; } }
        public Vector2 pitch { get { return _pitch; } }
        public float spatialBlend { get { return _spatialBlend; } }
        public float reverbMix { get { return _reverbMix; } }
        public AudioRolloffMode rolloff { get { return _rolloff; } }
        public float rollOffMin { get { return _rollOffMin; } }
        public float rollOffMax { get { return _rollOffMax; } }

        //These methods are used by VS. If needed, they must be deprecated, not deleted.
        public void Play(Vector3 position)
        {
            Play(position, 0f, 0f);
        }

        public void Play(Vector3 position, float extraVolume, float extraPitch)
        {
            SpatialBridge.PlaySpatialSFXPosition?.Invoke(this, position, extraVolume, extraPitch);
        }

        public void Play(AudioSource source)
        {
            Play(source, 0f, 0f);
        }

        public void Play(AudioSource source, float extraVolume, float extraPitch)
        {
            SpatialBridge.PlaySpatialSFXSource?.Invoke(this, source, extraVolume, extraPitch);
        }

        public AudioClip GetRandomClip()
        {
            if (_clips.Length == 0)
            {
                return null;
            }
            return _clips[Random.Range(0, _clips.Length)];
        }

        public float GetRandomVolume()
        {
            return Random.Range(_volume.x, _volume.y);
        }

        public float GetRandomPitch()
        {
            return Random.Range(_pitch.x, _pitch.y);
        }
    }
}
