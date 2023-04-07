/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
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
            foreach (var ui in GetComponentsInChildren<UI.InfoLR>())
            {
                if (ui.name.Contains("LR"))
                {
                    uiLRInfo = ui;
                }
            }
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
            if (manager.HandData.Data.handStatus != HandStatus.NoHand)
            {
                manager.HandData.GetPoseIndex(out poseLeft, out poseRight);
            }
            else
            {
                poseLeft = PoseIndex.None;
                poseRight = PoseIndex.None;
            }  
        }
    }
}
