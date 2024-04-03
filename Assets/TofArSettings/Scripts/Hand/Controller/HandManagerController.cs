/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
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

        void Awake()
        {
            context = SynchronizationContext.Current;
        }

        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            var handMgr = TofArHandManager.Instance;
            if (handMgr && !TofArHandManager.Instance.IsStreamActive && !TofArHandManager.Instance.IsPlaying)
            {
                var tofMgr = TofArTofManager.Instance;
                if (tofMgr && TofArTofManager.Instance != null && TofArTofManager.Instance.IsPlaying)
                {
                    if(TofArHandManager.Instance.Stream != null)
                    {
                        TofArHandManager.Instance.StartPlayback();
                    }
                }
                else
                {
                    TofArHandManager.Instance.StartStream();
                }

                OnStreamStartStatusChanged?.Invoke(true);
            }
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        public void StopStream()
        {
            var handMgr = TofArHandManager.Instance;
            if (handMgr && TofArHandManager.Instance.IsStreamActive)
            {
                if (TofArHandManager.Instance.IsPlaying)
                {
                    TofArHandManager.Instance.StopPlayback();
                }
                else
                {
                    TofArHandManager.Instance.StopStream();
                }
                OnStreamStartStatusChanged?.Invoke(false);
            }
        }

        /// <summary>
        /// Starts Hand stream after a short delay
        /// </summary>
        IEnumerator StartStreamCoroutine()
        {
            // Wait 1 frame when executing OnStreamStarted directly because it does not execute for only the first time
            yield return null;
            StartStream();
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
    }
}
