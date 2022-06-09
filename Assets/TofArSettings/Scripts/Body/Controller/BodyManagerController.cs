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
        private BodyRuntimeController runtimeController;

        protected void Awake()
        {
            runtimeController = FindObjectOfType<BodyRuntimeController>();
            runtimeController.OnChangeDetectorType += (index) =>
            {
                TofArBodyManager.Instance.StopStream();
                if (index > 0 && TofArTofManager.Instance.IsStreamActive)
                {
                    TofArBodyManager.Instance.StartStream();
                }
            };
        }

        void OnEnable()
        {
            TofArTofManager.OnStreamStarted += OnStreamStarted;
            TofArTofManager.OnStreamStopped += OnStreamStopped;
        }

        void OnDisable()
        {
            TofArTofManager.OnStreamStarted -= OnStreamStarted;
            TofArTofManager.OnStreamStopped -= OnStreamStopped;
        }

        /// <summary>
        /// Event that is called when Tof stream is started
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        /// <param name="depthTexture"></param>
        /// <param name="confidenceTexture"></param>
        /// <param name="pointCloudData"></param>
        void OnStreamStarted(object sender, Texture2D depthTexture,
            Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            // If the Body's dictionary is already selected, it will be started in conjunction with it
            if (runtimeController.DetectorTypeIndex > 0)
            {
                StartCoroutine(WaitAndStartBody());
            }
        }

        /// <summary>
        /// Event that is called when Tof stream is stopped
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        void OnStreamStopped(object sender)
        {
            TofArBodyManager.Instance.StopStream();
        }

        /// <summary>
        /// Call StartStream of Body
        /// </summary>
        IEnumerator WaitAndStartBody()
        {
            // If calling directly using OnStreamStarted, wait one frame as it does not execute for only the first time
            yield return null;
            TofArBodyManager.Instance.StartStream();
        }

        /// <summary>
        /// Stop Body Stream and start it again
        /// </summary>
        public void RestartStream()
        {
            if (runtimeController.DetectorTypeIndex > 0)
            {
                TofArBodyManager.Instance.StopStream();
                StartCoroutine(WaitAndStartBody());
            }
        }
    }
}
