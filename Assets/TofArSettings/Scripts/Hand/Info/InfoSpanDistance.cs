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
    public class InfoSpanDistance : HandInfo
    {
        UI.InfoLR uiLRInfo;
        float spanDistLeft = 0;
        float spanDistRight = 0;

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
            uiLRInfo.SetText($"{spanDistLeft:F1}", $"{spanDistRight:F1}");
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
            spanDistLeft = CalcSpanDistance(HandStatus.LeftHand,
                data.handStatus, data.featurePointsLeft);
            spanDistRight = CalcSpanDistance(HandStatus.RightHand,
                data.handStatus, data.featurePointsRight);
        }

        /// <summary>
        /// Calculate distance between thumb and pinky
        /// </summary>
        /// <param name="leftOrRight">Left/Right hand</param>
        /// <param name="handStatus">Recognized hand</param>
        /// <param name="points">Hand feature points</param>
        /// <returns>Distance</returns>
        float CalcSpanDistance(HandStatus leftOrRight, HandStatus handStatus,
            Vector3[] points)
        {
            if (handStatus == HandStatus.BothHands || handStatus == leftOrRight)
            {
                Vector3 thumbTip = points[(int)HandPointIndex.ThumbTip];
                Vector3 pinkyTip = points[(int)HandPointIndex.PinkyTip];

                return (thumbTip - pinkyTip).magnitude * 1000;
            }

            return float.NaN;
        }
    }
}
