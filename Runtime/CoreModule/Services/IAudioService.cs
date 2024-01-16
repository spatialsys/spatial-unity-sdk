
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Service for playing audio.
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// Plays the given SFX with a cached AudioSource.
        /// </summary>
        /// <param name="sfx">Spfx clip to play</param>
        /// <param name="extraVolume">extra volume to add to the clip</param>
        /// <param name="extraPitch">extra pitch to add to the clip</param>
        void PlaySFX(SpatialSFX sfx, float extraVolume = 0, float extraPitch = 0);

        /// <summary>
        /// Plays the given SFX with a cached AudioSource at a given position.
        /// </summary>
        /// <param name="sfx">Spfx clip to play</param>
        /// <param name="position">position to play the clip at</param>
        /// <param name="extraVolume">extra volume to add to the clip</param>
        /// <param name="extraPitch">extra pitch to add to the clip</param>
        void PlaySFX(SpatialSFX sfx, Vector3 position, float extraVolume = 0, float extraPitch = 0);

        /// <summary>
        /// Plays the given SFX on the provided AudioSource.
        /// </summary>
        /// <param name="sfx">Spfx clip to play</param>
        /// <param name="source">AudioSource to play the clip on</param>
        /// <param name="extraVolume">extra volume to add to the clip</param>
        /// <param name="extraPitch">extra pitch to add to the clip</param>
        void PlaySFX(SpatialSFX sfx, AudioSource source, float extraVolume = 0, float extraPitch = 0);
    }
}
