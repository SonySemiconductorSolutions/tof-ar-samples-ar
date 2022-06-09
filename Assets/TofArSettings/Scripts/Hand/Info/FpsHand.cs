/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Hand
{
    public class FpsHand : MonoBehaviour
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

        void OnEnable()
        {
            totalProcessTime = 0f;
            fromFpsMeasured = 0f;
            frameCount = 0;
            avgProcessTime = 0f;

            TofArHandManager.OnFrameArrived += HandFrameArrived;
        }

        void OnDisable()
        {
            TofArHandManager.OnFrameArrived -= HandFrameArrived;
        }

        void Start()
        {
            if (!TofArHandManager.Instance)
            {
                Debug.LogError("TofArHandManager is not found.");
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
            txtFps.text = $"{TofArHandManager.Instance.FrameRate:0.0}";

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
        }

        /// <summary>
        /// Event that is called when Hand data is updated
        /// </summary>
        /// <param name="sender">TofArHandManager</param>
        void HandFrameArrived(object sender)
        {
            var manager = sender as TofArHandManager;
            if (manager == null || !calcTime)
            {
                return;
            }

            totalProcessTime += manager.HandData.Data.processTime;
            frameCount++;
        }
    }
}
