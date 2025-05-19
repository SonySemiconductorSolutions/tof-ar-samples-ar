/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Color;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Color
{
    public class FpsColor : MonoBehaviour
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

            TofArColorManager.OnFrameArrived += ColorFrameArrived;
        }

        void OnDisable()
        {
            TofArColorManager.OnFrameArrived -= ColorFrameArrived;
        }

        void Start()
        {
            if (!TofArColorManager.Instance)
            {
                Debug.LogError("TofArColorManager is not found.");
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
            txtFps.text = $"{TofArColorManager.Instance.FrameRate:0.0}";

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
        void ColorFrameArrived(object sender)
        {
            var manager = sender as TofArColorManager;
            if (manager == null || !calcTime)
            {
                return;
            }

            totalProcessTime += manager.ColorData.ProcessTimeNs * nsToMs;

            frameCount++;
        }
    }
}
