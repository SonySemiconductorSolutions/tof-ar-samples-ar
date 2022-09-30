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

namespace TofArSettings.Segmentation
{
    public class SegmentationManagerController : ControllerBase
    {
        private SynchronizationContext context;

        protected void Awake()
        {
            isStarted = TofArSegmentationManager.Instance.autoStart;

            context = SynchronizationContext.Current;
        }

        protected void OnEnable()
        {
            TofArColorManager.OnStreamStarted += OnColorStreamStarted;
            TofArColorManager.OnStreamStopped += OnColorStreamStopped;
        }

        protected void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
            TofArColorManager.OnStreamStopped -= OnColorStreamStopped;
        }

        public bool IsStreamActive()
        {
            return TofArSegmentationManager.Instance.IsStreamActive;
        }

        private bool isStarted = false;

        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            isStarted = true;
            if (TofArColorManager.Instance.IsStreamActive)
            {
                var colorFormat = TofArColorManager.Instance.GetProperty<FormatConvertProperty>();
                if (colorFormat.format != ColorFormat.BGR)
                {
                    TofArSegmentationManager.Instance.StartStream();
                }    
            }
        }

        /// <summary>
        /// Starts segmentation stream after a short delay
        /// </summary>
        private IEnumerator StartStreamCoroutine()
        {
            yield return new UnityEngine.WaitForEndOfFrame();
            TofArSegmentationManager.Instance.StartStream();
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        public void StopStream()
        {
            isStarted = false;
            if (TofArSegmentationManager.Instance.IsStreamActive)
            {
                TofArSegmentationManager.Instance.StopStream();
            }
        }

        /// <summary>
        /// Event that is called when Color stream is started
        /// </summary>
        /// <param name="sender">TofArSegmentationManager</param>
        void OnColorStreamStarted(object sender, UnityEngine.Texture2D tex)
        {
            var colorFormat = TofArColorManager.Instance.GetProperty<FormatConvertProperty>();
            if (colorFormat.format != ColorFormat.BGR)
            {
                if (isStarted)
                {
                    context.Post((s) =>
                    {
                        StartCoroutine(StartStreamCoroutine());
                    }, null);
                }
            }
        }

        /// <summary>
        /// Event that is called when Color stream is stopped
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        void OnColorStreamStopped(object sender)
        {
            if (isStarted)
            {
                TofArSegmentationManager.Instance.StopStream();
            }
        }
    }
}
