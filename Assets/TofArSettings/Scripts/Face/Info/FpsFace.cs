/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Face;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Face
{
    public class FpsFace : MonoBehaviour
    {
        /// <summary>
        /// Calculate/do not calculate the processing time based on the number of times OnFrameArrived is called and the time
        /// </summary>
        [SerializeField]
        bool calcTime = true;

        Text txtFps, txtMs;

        float totalProcessTime;
        float fromFpsMeasured;
        int frameCount;
        float avgProcessTime;

        private const float nsToMs = 0.000001f;

        void OnEnable()
        {
            totalProcessTime = 0f;
            fromFpsMeasured = 0f;
            frameCount = 0;
            avgProcessTime = 0f;

            TofArFaceManager.OnFrameArrived += FaceFrameArrived;
        }

        void OnDisable()
        {
            TofArFaceManager.OnFrameArrived -= FaceFrameArrived;
        }

        void Start()
        {
            if (!TofArFaceManager.Instance)
            {
                Debug.LogError("TofArFaceManager is not found.");
                enabled = false;
                return;
            }

            // Get UI
            foreach (var ui in GetComponentsInChildren<Text>())
            {
                if (ui.name.Contains("Fps"))
                {
                    txtFps = ui;
                }
                else if (ui.name.Contains("Ms"))
                {
                    txtMs = ui;
                }
            }
        }

        void Update()
        {
            txtFps.text = $"{TofArFaceManager.Instance.FrameRate:0.0}";

            if (calcTime && (TofArFaceManager.Instance.DetectorType == FaceDetectorType.External))
            {
                fromFpsMeasured += Time.deltaTime;
                if (fromFpsMeasured >= 1.0f)
                {
                    fromFpsMeasured = 0;
                    if (frameCount > 0)
                    {
                        avgProcessTime = totalProcessTime / frameCount;
                    }

                    frameCount = 0;
                    totalProcessTime = 0f;
                }

                txtMs.text = $"{avgProcessTime:0.0}";
            }
            else
            {
                txtMs.text = string.Empty;
            }
        }

        /// <summary>
        /// Event that is called when Hand data is updated
        /// </summary>
        /// <param name="sender">TofArFaceManager</param>
        void FaceFrameArrived(object sender)
        {
            var manager = sender as TofArFaceManager;
            if (manager == null || !calcTime)
            {
                return;
            }

            totalProcessTime += manager.FaceData.ProcessTimeNs * nsToMs;
            frameCount++;
        }
    }
}
