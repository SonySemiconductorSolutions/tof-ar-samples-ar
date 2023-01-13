/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Threading;
using TofAr.V0.Hand;
using TofAr.V0.Tof;
using UnityEngine;

namespace TofArSettings.Hand
{
    public class HandManagerController : ControllerBase
    {
        private SynchronizationContext context;
        private bool isStarted = false;
        private bool isPlaying = false;
        private bool restartStream = false;

        void Awake()
        {
            context = SynchronizationContext.Current;
            isStarted = TofArHandManager.Instance.autoStart;
        }

        protected void OnEnable()
        {
            TofArTofManager.OnStreamStarted += OnTofPlaybackStreamStarted;
            TofArTofManager.OnStreamStopped += OnTofPlaybackStreamStopped;
        }

        protected void OnDisable()
        {
            TofArTofManager.OnStreamStarted -= OnTofPlaybackStreamStarted;
            TofArTofManager.OnStreamStopped -= OnTofPlaybackStreamStopped;
        }

        /// <summary>
        /// Is the stream currently running if tof is on
        /// </summary>
        public bool IsStreamActive => isStarted && TofArTofManager.Instance.IsStreamActive;

        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            if (!isStarted)
            {
                isStarted = true;
                if (TofArTofManager.Instance.IsPlaying)
                {
                    TofArHandManager.Instance.StartPlayback();
                }
                else
                {
                    TofArHandManager.Instance.StartStream();
                }
                OnStreamStartStatusChanged(isStarted);
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
                TofArHandManager.Instance.StopStream();
                OnStreamStartStatusChanged(isStarted);
            }
        }

        /// <summary>
        /// Starts Hand stream after a short delay
        /// </summary>
        IEnumerator StartStreamCoroutine()
        {
            // Wait 1 frame when executing OnStreamStarted directly because it does not execute for only the first time
            yield return new WaitForEndOfFrame();

            StartStream();
        }

        private IEnumerator StartPlaybackStreamCoroutine()
        {
            yield return new WaitForEndOfFrame();

            TofArHandManager.Instance.StartPlayback();
            OnStreamStartStatusChanged?.Invoke(true);
            isPlaying = true;
        }

        /// <summary>
        /// Event that is called when Tof stream is started and status is changed
        /// </summary>
        public event ChangeToggleEvent OnStreamStartStatusChanged;

        /// <summary>
        /// Event that is called when Tof stream is started
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        /// <param name="depthTexture">Depth texture</param>
        /// <param name="confidenceTexture">Confidence texture</param>
        /// <param name="pointCloudData">PointCloud data</param>
        public void OnStreamStarted(object sender, Texture2D depthTexture,
            Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            context.Post((s) =>
            {
                StartCoroutine(StartStreamCoroutine());
            }, null);
        }

        /// <summary>
        /// Event that is called when Tof stream is ended
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        public void OnStreamStopped(object sender)
        {
            StopStream();
        }

        /// <summary>
        /// Event that is called when Tof playback stream is started
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        private void OnTofPlaybackStreamStarted(object sender, Texture2D depth, Texture2D conf, PointCloudData pc)
        {
            if (TofArTofManager.Instance.IsPlaying)
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
        /// Event that is called when Tof stream is stopped
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        private void OnTofPlaybackStreamStopped(object sender)
        {
            if (isPlaying)
            {
                isPlaying = false;
                OnStreamStopped(sender);
            }

            // may have to restart mesh stream
            if (restartStream)
            {
                restartStream = false;
                OnStreamStarted(sender, null, null, null);
            }
        }
    }
}
