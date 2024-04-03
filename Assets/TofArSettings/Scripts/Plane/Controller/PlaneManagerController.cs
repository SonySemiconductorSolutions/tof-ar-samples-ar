/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using UnityEngine;
using TofAr.V0.Plane;
using TofAr.V0.Tof;

namespace TofArSettings.Plane
{
    public class PlaneManagerController : ControllerBase
    {
        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            if (!TofArPlaneManager.Instance.IsStreamActive && !TofArPlaneManager.Instance.IsPlaying)
            {
                if (TofArTofManager.Instance != null && TofArTofManager.Instance.IsPlaying)
                {
                    if (TofArPlaneManager.Instance.Stream != null)
                    {
                        TofArPlaneManager.Instance.StartPlayback();
                    }
                }
                else
                {
                    TofArPlaneManager.Instance.StartStream();
                }
                OnStreamStartStatusChanged?.Invoke(true);
            }
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        public void StopStream()
        {
            if (TofArPlaneManager.Instance.IsStreamActive)
            {
                if (TofArPlaneManager.Instance.IsPlaying)
                {
                    TofArPlaneManager.Instance.StopPlayback();
                }
                else
                {
                    TofArPlaneManager.Instance.StopStream();
                }
                OnStreamStartStatusChanged?.Invoke(false);
            }
        }

        /// <summary>
        /// Stop Body stream and restart
        /// </summary>
        public void RestartStream()
        {
            StopStream();
            StartCoroutine(WaitAndStartPlane());
        }

        /// <summary>
        /// Execute StartStream of Face
        /// </summary>
        IEnumerator WaitAndStartPlane()
        {
            // Wait 1 frame when calling OnStreamStarted directly because it does not get executed for the first time only
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
        /// <param name="depthTexture"></param>
        /// <param name="confidenceTexture"></param>
        /// <param name="pointCloudData"></param>
        public void OnStreamStarted(object sender, Texture2D depthTexture,
            Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            StartCoroutine(WaitAndStartPlane());
        }

        /// <summary>
        /// Event that is called when Tof stream is stopped
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        public void OnStreamStopped(object sender)
        {
            StopStream();
        }
    }
}
