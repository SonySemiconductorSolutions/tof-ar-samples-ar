/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
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
        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            var segmentationMgr = TofArSegmentationManager.Instance;
            if (segmentationMgr && !TofArSegmentationManager.Instance.IsStreamActive && !TofArSegmentationManager.Instance.IsPlaying)
            {
                var colorMgr = TofArColorManager.Instance;
                if (colorMgr && TofArColorManager.Instance.IsPlaying)
                {
                    if (TofArSegmentationManager.Instance.Stream != null)
                    {
                        TofArSegmentationManager.Instance.StartPlayback();
                    }
                }
                else
                {
                    TofArSegmentationManager.Instance.StartStream();
                }
                OnStreamStartStatusChanged?.Invoke(true);
            }
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        public void StopStream()
        {
            var segmentationMgr = TofArSegmentationManager.Instance;
            if (segmentationMgr && TofArSegmentationManager.Instance.IsStreamActive)
            {
                if (TofArSegmentationManager.Instance.IsPlaying)
                {
                    TofArSegmentationManager.Instance.StopPlayback();
                }
                else
                {
                    TofArSegmentationManager.Instance.StopStream();
                }
                OnStreamStartStatusChanged?.Invoke(false);
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
            StartCoroutine(StartStreamCoroutine());
        }

        /// <summary>
        /// Event that is called when Color stream is stopped
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        public void OnColorStreamStopped(object sender)
        {
            StopStream();
        }
    }
}
