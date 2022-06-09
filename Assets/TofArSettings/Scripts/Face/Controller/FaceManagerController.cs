/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using TofAr.V0.Color;
using TofAr.V0.Face;
using UnityEngine;

namespace TofArSettings.Face
{
    public class FaceManagerController : ControllerBase
    {
        private FaceRuntimeController runtimeController;

        protected void Awake()
        {
            runtimeController = FindObjectOfType<FaceRuntimeController>();
            runtimeController.OnChangeDetectorType += (index) =>
            {
                TofArFaceManager.Instance.StopStream();
                if (index > 0 && TofArColorManager.Instance.IsStreamActive)
                {
                    TofArFaceManager.Instance.StartStream();
                }
            };
        }

        void OnEnable()
        {
            TofArColorManager.OnStreamStarted += OnStreamStarted;
            TofArColorManager.OnStreamStopped += OnStreamStopped;
        }

        void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnStreamStarted;
            TofArColorManager.OnStreamStopped -= OnStreamStopped;
        }

        /// <summary>
        /// Event that is called when Color sream is started
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        /// <param name="colorTexture">Color texture</param>
        private void OnStreamStarted(object sender, Texture2D colorTexture)
        {
            if (runtimeController.DetectorTypeIndex > 0)
            {
                StartCoroutine(WaitAndStartFace());
            }
        }

        /// <summary>
        /// Event that is called when Color stream is stopped
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        void OnStreamStopped(object sender)
        {
            TofArFaceManager.Instance.StopStream();
        }

        /// <summary>
        /// Execute StartStream of Face
        /// </summary>
        IEnumerator WaitAndStartFace()
        {
            // Wait 1 frame when calling OnStreamStarted directly because it does not get executed for the first time only
            yield return null;
            TofArFaceManager.Instance.StartStream();
        }

        /// <summary>
        /// Stop Body stream and restart
        /// </summary>
        public void RestartStream()
        {
            if (runtimeController.DetectorTypeIndex > 0)
            {
                TofArFaceManager.Instance.StopStream();
                StartCoroutine(WaitAndStartFace());
            }
        }
    }
}
