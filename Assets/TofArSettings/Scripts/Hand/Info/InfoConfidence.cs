/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using UnityEngine;

namespace TofArSettings.Hand
{
    public class InfoConfidence : HandInfo
    {
        UI.InfoLR uiLRInfo;
        double leftHandConfidence = 0;
        double rightHandConfidence = 0;

        void OnEnable()
        {
            TofArHandManager.OnFrameArrived += HandFrameArrived;
        }

        void OnDisable()
        {
            TofArHandManager.OnFrameArrived -= HandFrameArrived;
        }

        void Start()
        {
            uiLRInfo = GetComponent<UI.InfoLR>();
        }

        void Update()
        {
            // Show
            uiLRInfo.SetText($"{leftHandConfidence:F3}", $"{rightHandConfidence:F3}");
        }

        /// <summary>
        /// Event that is called when Hand data is updated
        /// </summary>
        /// <param name="sender">TofArHandManager</param>
        void HandFrameArrived(object sender)
        {
            var manager = sender as TofArHandManager;
            if (manager == null)
            {
                return;
            }

            // Get
            var data = manager.HandData.Data;
            this.leftHandConfidence = data.leftHandConfidence;
            this.rightHandConfidence = data.rightHandConfidence;
        }
    }
}
