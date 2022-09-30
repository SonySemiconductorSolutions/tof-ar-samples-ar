/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using UnityEngine;

namespace TofArSettings.Hand
{
    public class InfoDistance : HandInfo
    {
        UI.InfoLR uiLRInfo;
        Vector3[] pointsLeft, pointsRight;
        bool handAvailable = false;

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
            uiLRInfo.LeftText = MakeDistText(pointsLeft);
            uiLRInfo.RightText = MakeDistText(pointsRight);
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
            pointsLeft = manager.HandData.Data.featurePointsLeft;
            pointsRight = manager.HandData.Data.featurePointsRight;

            handAvailable = manager.HandData.Data.handStatus != HandStatus.NoHand;
        }

        /// <summary>
        /// Make display text
        /// </summary>
        /// <param name="points">featurePoints</param>
        /// <returns>Text</returns>
        string MakeDistText(Vector3[] points)
        {
            float dist = 0;
            if (points != null && handAvailable)
            {
                dist = points[(int)HandPointIndex.HandCenter].magnitude;
            }

            return $"{dist,-4:0.00}";
        }
    }
}
