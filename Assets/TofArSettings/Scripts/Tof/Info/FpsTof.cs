/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Tof
{
    public class FpsTof : MonoBehaviour
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

            TofArTofManager.OnFrameArrived += TofFrameArrived;
        }

        void OnDisable()
        {
            TofArTofManager.OnFrameArrived -= TofFrameArrived;
        }

        void Start()
        {
            if (!TofArTofManager.Instance)
            {
                Debug.LogError("TofArTofManager is not found.");
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
            txtFps.text = $"{TofArTofManager.Instance.FrameRate:0.0}";

            if (calcTime)
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
        /// <param name="sender">TofArTofManager</param>
        void TofFrameArrived(object sender)
        {
            var manager = sender as TofArTofManager;
            if (manager == null || !calcTime)
            {
                return;
            }

            if (manager.ProcessTargets.processDepth)
            {
                totalProcessTime += manager.DepthData.ProcessTimeNs * nsToMs;
            }
            else if (manager.ProcessTargets.processConfidence)
            {
                totalProcessTime += manager.ConfidenceData.ProcessTimeNs * nsToMs;
            }
            else if (manager.ProcessTargets.processPointCloud)
            {
                totalProcessTime += manager.PointCloudData.ProcessTimeNs * nsToMs;
            }

            frameCount++;
        }
    }
}
