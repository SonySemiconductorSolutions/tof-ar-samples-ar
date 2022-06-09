/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class InfoHandStatus : HandInfo
    {
        UI.Info uiInfo;
        HandStatus status = HandStatus.NoHand;

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
            uiInfo = GetComponent<UI.Info>();
        }

        void Update()
        {
            // Show
            uiInfo.InfoText = status.ToString();
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
            status = manager.HandData.Data.handStatus;
        }
    }
}
