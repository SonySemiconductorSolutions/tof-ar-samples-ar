/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArSettings.Hand
{
    public class HandPoseInfoController : ControllerBase
    {
        [SerializeField]
        private HandPoseInfo handInfo;

        private void Awake()
        {
            if (handInfo == null)
            {
                handInfo = FindObjectOfType<HandPoseInfo>();
            }
        }

        public bool Show
        {
            set
            {
                if (handInfo != null)
                {
                    handInfo.ShowPose = value;
                }
            }
        }

        public int PoseFrame
        {
            get
            {
                return handInfo?.PoseFrame ?? 0;
            }
            set
            {
                handInfo.PoseFrame = value;
            }
        }
    }
}
