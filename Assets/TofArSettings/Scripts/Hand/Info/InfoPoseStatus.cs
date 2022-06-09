/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class InfoPoseStatus : HandInfo
    {
        UI.InfoLR uiLRInfo;
        PoseIndex poseLeft = PoseIndex.None;
        PoseIndex poseRight = PoseIndex.None;

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
            uiLRInfo.SetText($"{poseLeft,-11}", $"{poseRight,-11}");
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
            manager.HandData.GetPoseIndex(out poseLeft, out poseRight);
        }
    }
}
