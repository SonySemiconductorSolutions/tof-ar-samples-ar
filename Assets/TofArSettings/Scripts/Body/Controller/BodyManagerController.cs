/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
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
        private bool isStarted = false;

        protected void Awake()
        {
            isStarted = TofArBodyManager.Instance.autoStart;
        }

        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            if (!isStarted)
            {
                isStarted = true;
                TofArBodyManager.Instance.StartStream();
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
                TofArBodyManager.Instance.StopStream();
                OnStreamStartStatusChanged(isStarted);
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
