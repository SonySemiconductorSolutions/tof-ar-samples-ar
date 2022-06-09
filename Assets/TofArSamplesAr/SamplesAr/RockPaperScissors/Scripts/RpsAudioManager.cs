/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArARSamples.RockPaperScissors
{
    /// <summary>
    /// Audio management class for rock-paper-scissors app
    /// </summary>
    public class RpsAudioManager : MonoBehaviour
    {
        [SerializeField]
        private AudioClip[] englishAudioClips = null;

        [SerializeField]
        private AudioClip[] chineseAudioClips = null;

        [SerializeField]
        private AudioClip[] japaneseAudioClips = null;

        private AudioSource audioSource = null;

        void Awake()
        {
            audioSource = this.GetComponent<AudioSource>();
        }

        /// <summary>
        /// Play sound effects
        /// </summary>
        public void PlaySoundEffect(SoundEffect soundEffect, RpsAppProgressManager.AppLanguage appLanguage)
        {
            switch (appLanguage)
            {
                case RpsAppProgressManager.AppLanguage.english:
                    audioSource.PlayOneShot(englishAudioClips[(int)soundEffect]);
                    break;
                case RpsAppProgressManager.AppLanguage.chinese:
                    audioSource.PlayOneShot(chineseAudioClips[(int)soundEffect]);
                    break;
                case RpsAppProgressManager.AppLanguage.japanese:
                    audioSource.PlayOneShot(japaneseAudioClips[(int)soundEffect]);
                    break;
            }
        }

        /// <summary>
        /// Sound type
        /// </summary>
        public enum SoundEffect
        {
            jankenpon,
            ready,
            again,
            win,
            lose
        }
    }
}
