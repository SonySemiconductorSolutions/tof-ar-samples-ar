/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArSettings.Hand
{
    public class HandGestureInfoController : ControllerBase
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
                    handInfo.ShowGesture = value;
                }
            }
        }

        public bool[] GestureNotify
        {
            get
            {
                return handInfo?.GestureNotify ?? null;
            }
        }

        public void SetGestureNotify(int index, bool val)
        {
            if (handInfo != null)
            {
                handInfo.GestureNotify[index] = val;
            }
        }
    }
}
