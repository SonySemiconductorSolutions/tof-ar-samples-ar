/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */
using TofAr.V0.Face;

namespace TofArSettings.Face
{
    public class InfoFaceStatus : FaceInfo
    {
        UI.Info uiInfo;
        TrackingState status = TrackingState.None;

        void OnEnable()
        {
            TofArFaceManager.OnFaceEstimated += FaceFrameArrived;
            TofArFaceManager.OnStreamStopped += OnStreamStopped;
        }

        void OnDisable()
        {
            TofArFaceManager.OnStreamStopped -= OnStreamStopped;
            TofArFaceManager.OnFaceEstimated -= FaceFrameArrived;
        }

        void Start()
        {
            uiInfo = GetComponent<UI.Info>();
        }

        void Update()
        {
            // Show
            uiInfo.InfoText = status.ToString();
        }

        /// <summary>
        /// Event that is called when Face data is updated
        /// </summary>
        /// <param name="faceResults"></param>
        void FaceFrameArrived(FaceResults faceResults)
        {
            // Get

            if (faceResults.results.Length > 0)
            {
                status = faceResults.results[0].trackingState;
            }
            else
            {
                status = TrackingState.None;
            }
        }

        private void OnStreamStopped(object sender)
        {
            status = TrackingState.None;
        }
    }
}
