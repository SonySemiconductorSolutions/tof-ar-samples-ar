/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Segmentation;
using TofAr.V0.Color;
using System.Threading;
using System.Collections;
using UnityEngine;

namespace TofArSettings.Segmentation
{
    public class SegmentationManagerController : ControllerBase
    {
        private SynchronizationContext context;
        private bool isStarted = false;
        private bool isPlaying = false;
        private bool restartStream = false;

        protected void Awake()
        {
            context = SynchronizationContext.Current;
            isStarted = TofArSegmentationManager.Instance.autoStart;
        }

        protected void OnEnable()
        {
            TofArColorManager.OnStreamStarted += OnColorPlaybackStreamStarted;
            TofArColorManager.OnStreamStopped += OnColorPlaybackStreamStopped;
        }

        protected void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnColorPlaybackStreamStarted;
            TofArColorManager.OnStreamStopped -= OnColorPlaybackStreamStopped;
        }

        public bool IsStreamActive()
        {
            return isStarted && TofArSegmentationManager.Instance.IsStreamActive;
        }

        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            if (!isStarted)
            {
                isStarted = true;
                if (TofArColorManager.Instance.IsPlaying)
                {
                    TofArSegmentationManager.Instance.StartPlayback();
                }
                else
                {
                    TofArSegmentationManager.Instance.StartStream();
                }
                OnStreamStartStatusChanged?.Invoke(isStarted);
            }
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        public void StopStream()
        {
            if (isStarted)
            {
                isStarted = false;
                TofArSegmentationManager.Instance.StopStream();
                OnStreamStartStatusChanged?.Invoke(isStarted);
            }
        }

        /// <summary>
        /// Starts segmentation stream after a short delay
        /// </summary>
        private IEnumerator StartStreamCoroutine()
        {
            yield return new WaitForEndOfFrame();

            StartStream();
        }

        private IEnumerator StartPlaybackStreamCoroutine()
        {
            yield return new WaitForEndOfFrame();

            TofArSegmentationManager.Instance.StartPlayback();
            OnStreamStartStatusChanged?.Invoke(true);
            isPlaying = true;
        }

        /// <summary>
        /// Event that is called when Tof stream is started and status is changed
        /// </summary>
        public event ChangeToggleEvent OnStreamStartStatusChanged;

        /// <summary>
        /// Event that is called when Color stream is started
        /// </summary>
        /// <param name="sender">TofArSegmentationManager</param>
        public void OnColorStreamStarted(object sender, UnityEngine.Texture2D tex)
        {
            context.Post((s) =>
            {
                StartCoroutine(StartStreamCoroutine());
            }, null);
        }

        /// <summary>
        /// Event that is called when Color stream is stopped
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        public void OnColorStreamStopped(object sender)
        {
            StopStream();
        }

        /// <summary>
        /// Event that is called when Color playback stream is started
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        private void OnColorPlaybackStreamStarted(object sender, Texture2D tex)
        {
            if (TofArColorManager.Instance.IsPlaying)
            {
                if (isStarted)
                {
                    restartStream = true;
                }

                context.Post((s) =>
                {
                    StartCoroutine(StartPlaybackStreamCoroutine());
                }, null);
            }
        }

        /// <summary>
        /// Event that is called when Color playback stream is stopped
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        private void OnColorPlaybackStreamStopped(object sender)
        {
            if (isPlaying)
            {
                isPlaying = false;
                OnColorStreamStopped(sender);
            }

            // may have to restart mesh stream
            if (restartStream)
            {
                restartStream = false;
                OnColorStreamStarted(sender, null);
            }
        }
    }
}
