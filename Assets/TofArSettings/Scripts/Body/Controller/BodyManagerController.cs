/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using TofAr.V0.Body;
using TofAr.V0.Tof;
using UnityEngine;

namespace TofArSettings.Body
{
    public class BodyManagerController : ControllerBase
    {

        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            var badyMgr = TofArBodyManager.Instance;
            if (badyMgr && !TofArBodyManager.Instance.IsStreamActive && !TofArBodyManager.Instance.IsPlaying)
            {
                var tofMgr = TofArTofManager.Instance;
                if (tofMgr && TofArTofManager.Instance.IsPlaying)
                {
                    if (TofArBodyManager.Instance.Stream != null)
                    {
                        TofArBodyManager.Instance.StartPlayback();
                    }
                }
                else
                {
                    TofArBodyManager.Instance.StartStream();
                }
                OnStreamStartStatusChanged?.Invoke(true);
            }
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        public void StopStream()
        {
            var badyMgr = TofArBodyManager.Instance;
            if (badyMgr && TofArBodyManager.Instance.IsStreamActive)
            {
                if (TofArBodyManager.Instance.IsPlaying)
                {
                    TofArBodyManager.Instance.StopPlayback();
                }
                else
                {
                    TofArBodyManager.Instance.StopStream();
                }
                OnStreamStartStatusChanged?.Invoke(false);
            }
        }

        /// <summary>
        /// Stop Body Stream and start it again
        /// </summary>
        public void RestartStream()
        {
            StopStream();
            StartCoroutine(WaitAndStartBody());
        }

        /// <summary>
        /// Call StartStream of Body
        /// </summary>
        IEnumerator WaitAndStartBody()
        {
            // If calling directly using OnStreamStarted, wait one frame as it does not execute for only the first time
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
            StartCoroutine(WaitAndStartBody());
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
