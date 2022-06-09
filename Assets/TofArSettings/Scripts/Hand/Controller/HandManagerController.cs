/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using TofAr.V0.Hand;
using TofAr.V0.Tof;
using UnityEngine;

namespace TofArSettings.Hand
{
    public class HandManagerController : ControllerBase
    {
        RecogModeController recogModeController;

        void Awake()
        {
            recogModeController = FindObjectOfType<RecogModeController>();

            // Stop if Hand dictionary is not selected, otherwise start
            recogModeController.OnChangeRecog += (index, conf) =>
            {
                if (index <= 0)
                {
                    TofArHandManager.Instance.StopStream();
                }
                else
                {
                    if (TofArTofManager.Instance.IsPlaying)
                    {
                        TofArHandManager.Instance.StartPlayback();
                    }
                    if (TofArTofManager.Instance.IsStreamActive)
                    {
                        TofArHandManager.Instance.StartStream();
                    }
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
        /// <param name="depthTexture">Depth texture</param>
        /// <param name="confidenceTexture">Confidence texture</param>
        /// <param name="pointCloudData">PointCloud data</param>
        void OnStreamStarted(object sender, Texture2D depthTexture,
            Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            // If Hand dictionary is selected, start in conjunction
            if (recogModeController.Index > 0)
            {
                StartCoroutine(WaitAndStartHand());
            }
        }

        /// <summary>
        /// Event that is called when Tof stream is ended
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        void OnStreamStopped(object sender)
        {
            TofArHandManager.Instance.StopStream();
        }

        /// <summary>
        /// Call StartStream of Hand
        /// </summary>
        IEnumerator WaitAndStartHand()
        {
            // Wait 1 frame when executing OnStreamStarted directly because it does not execute for only the first time
            yield return null;
            if (TofArTofManager.Instance.IsPlaying)
            {
                TofArHandManager.Instance.StartPlayback();
            }
            else
            {
                TofArHandManager.Instance.StartStream();
            }
        }

        /// <summary>
        /// Is the stream currently running if tof is on
        /// </summary>
        public bool IsStreamActive => recogModeController.Index > 0 && TofArTofManager.Instance.IsStreamActive;
    }
}
