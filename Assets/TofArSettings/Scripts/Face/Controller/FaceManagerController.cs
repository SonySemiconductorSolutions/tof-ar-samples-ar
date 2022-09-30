/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
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

            runtimeController = FindObjectOfType<FaceRuntimeController>();
            runtimeController.OnChangeDetectorType += (index) =>
            {
                TofArFaceManager.Instance.StopStream();
                if (index > 0)
                {
                    bool streamActive = true;
                    if (TofArFaceManager.Instance.DetectorType == FaceDetectorType.External)
                    {
                        switch (faceEstimator.InputSourceType)
                        {
                            case InputSource.Confidence:
                                streamActive = TofArTofManager.Instance?.IsStreamActive == true;
                                break;
                            default:
                                streamActive = TofArColorManager.Instance?.IsStreamActive == true;
                                break;
                        }
                    }

                    if (streamActive)
                    {
                        TofArFaceManager.Instance.StartStream();
                    }

                }
            };
        }

        void OnEnable()
        {
            TofArTofManager.OnStreamStarted += OnTofStreamStarted;
            TofArTofManager.OnStreamStopped += OnTofStreamStopped;

            TofArColorManager.OnStreamStarted += OnColorStreamStarted;
            TofArColorManager.OnStreamStopped += OnColorStreamStopped;
        }

        void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
            TofArColorManager.OnStreamStopped -= OnColorStreamStopped;

            TofArTofManager.OnStreamStarted -= OnTofStreamStarted;
            TofArTofManager.OnStreamStopped -= OnTofStreamStopped;
        }

        /// <summary>
        /// Event that is called when Color sream is started
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        /// <param name="colorTexture">Color texture</param>
        private void OnColorStreamStarted(object sender, Texture2D colorTexture)
        {
            if (TofArFaceManager.Instance.DetectorType == FaceDetectorType.External && faceEstimator.InputSourceType == InputSource.Color)
            {
                StartStream();
            }
        }

        /// <summary>
        /// Event that is called when Tof sream is started
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        /// <param name="depthTexture"></param>
        /// <param name="confidenceTexture"></param>
        /// <param name="pointCloudData"></param>
        private void OnTofStreamStarted(object sender, Texture2D depthTexture, Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            if ((TofArFaceManager.Instance.DetectorType == FaceDetectorType.External && faceEstimator.InputSourceType == InputSource.Confidence))
            {
                StartStream();
            }
        }

        /// <summary>
        /// Event that is called when Color stream is stopped
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        void OnColorStreamStopped(object sender)
        {
            if (TofArFaceManager.Instance.DetectorType == FaceDetectorType.External && faceEstimator.InputSourceType == InputSource.Color)
            {
                TofArFaceManager.Instance.StopStream();
            }
        }

        /// <summary>
        /// Event that is called when Tof stream is stopped
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        void OnTofStreamStopped(object sender)
        {
            if ((TofArFaceManager.Instance.DetectorType == FaceDetectorType.External && faceEstimator.InputSourceType == InputSource.Confidence))
            {
                TofArFaceManager.Instance.StopStream();
            }
        }

        private void StartStream()
        {
            if (runtimeController.DetectorTypeIndex > 0)
            {
                StartCoroutine(WaitAndStartFace());
            }
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
