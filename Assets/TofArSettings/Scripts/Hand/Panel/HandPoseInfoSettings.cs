/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Hand
{
    public class HandPoseInfoSettings : UI.SettingsBase
    {
        [Header("Default value")]

        [SerializeField]
        private bool show = true;

        private UI.ItemSlider poseFrame;
        private HandPoseInfoController poseInfoCtrl;

        protected virtual void Awake()
        {
            poseInfoCtrl = GetComponent<HandPoseInfoController>();
            poseInfoCtrl.enabled = true;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
            MakeUIShow,
            MakeUIPoseFrame
            };
            controllers.Add(poseInfoCtrl);

            ChangeShow(show);

            base.Start();
        }

        void MakeUIShow()
        {
            settings.AddItem("Show", show, ChangeShow);
        }

        void MakeUIPoseFrame()
        {
            poseFrame = settings.AddItem("Pose Frame", 1, 10, 1, poseInfoCtrl.PoseFrame, ChangePoseFrame);
            poseFrame.IsNotifyImmediately = true;
        }

        void ChangeShow(bool val)
        {
            this.show = val;
            poseInfoCtrl.Show = this.show;
        }

        void ChangePoseFrame(float val)
        {
            poseInfoCtrl.PoseFrame = (int)val;
        }
    }
}
