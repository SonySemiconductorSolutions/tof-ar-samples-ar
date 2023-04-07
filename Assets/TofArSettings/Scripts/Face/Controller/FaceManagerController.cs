/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using TofAr.V0.Color;
using TofAr.V0.Tof;
using TofAr.V0.Face;
using UnityEngine;

namespace TofArSettings.Face
{
    public class FaceManagerController : ControllerBase
    {
        private FaceRuntimeController runtimeController;
        private FaceEstimator faceEstimator;

        protected void Awake()
        {
            faceEstimator = FindObjectOfType<FaceEstimator>();
        }

        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            if (!TofArFaceManager.Instance.IsStreamActive)
            {
                TofArFaceManager.Instance.StartStream();
                OnStreamStartStatusChanged?.Invoke(true);
            }
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        public void StopStream()
        {
            if (TofArFaceManager.Instance.IsStreamActive)
            {
                TofArFaceManager.Instance.StopStream();
                OnStreamStartStatusChanged?.Invoke(false);
            }
        }

        /// <summary>
        /// Stop Body stream and restart
        /// </summary>
        public void RestartStream()
        {
            StopStream();
            StartCoroutine(WaitAndStartFace());
        }

        /// <summary>
        /// Execute StartStream of Face
        /// </summary>
        IEnumerator WaitAndStartFace()
        {
            // Wait 1 frame when calling OnStreamStarted directly because it does not get executed for the first time only
            yield return null;
            StartStream();
        }

        public bool IsStreamActive()
        {
            return TofArFaceManager.Instance.IsStreamActive;
        }

        /// <summary>
        /// Event that is called when Tof stream is started and status is changed
        /// </summary>
        public event ChangeToggleEvent OnStreamStartStatusChanged;

        /// <summary>
        /// Event that is called when Color sream is started
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        /// <param name="colorTexture">Color texture</param>
        public void OnColorStreamStarted(object sender, Texture2D colorTexture)
        {
            if (TofArFaceManager.Instance.DetectorType == FaceDetectorType.External && faceEstimator.InputSourceType == InputSource.Color)
            {
                StartCoroutine(WaitAndStartFace());
            }
        }

        /// <summary>
        /// Event that is called when Tof sream is started
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        /// <param name="depthTexture"></param>
        /// <param name="confidenceTexture"></param>
        /// <param name="pointCloudData"></param>
        public void OnTofStreamStarted(object sender, Texture2D depthTexture, Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            if ((TofArFaceManager.Instance.DetectorType == FaceDetectorType.External && faceEstimator.InputSourceType == InputSource.Confidence))
            {
                StartCoroutine(WaitAndStartFace());
            }
        }

        /// <summary>
        /// Event that is called when Color stream is stopped
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        public void OnColorStreamStopped(object sender)
        {
            if (TofArFaceManager.Instance.DetectorType == FaceDetectorType.External && faceEstimator.InputSourceType == InputSource.Color)
            {
                StopStream();
            }
        }

        /// <summary>
        /// Event that is called when Tof stream is stopped
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        public void OnTofStreamStopped(object sender)
        {
            if ((TofArFaceManager.Instance.DetectorType == FaceDetectorType.External && faceEstimator.InputSourceType == InputSource.Confidence))
            {
                StopStream();
            }
        }
    }
}
