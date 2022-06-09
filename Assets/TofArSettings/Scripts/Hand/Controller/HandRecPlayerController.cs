/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Linq;
using TofAr.V0;
using TofAr.V0.Hand;
using TofArSettings.UI;
using UnityEngine;

namespace TofArSettings.Hand
{
    public class HandRecPlayerController : RecPlayerController
    {
        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Hand;
        public override bool HasDropdown => false;

        private HandManagerController handManagerController;

        protected void Awake()
        {
            handManagerController = FindObjectOfType<HandManagerController>();
        }

        protected override void PlayPrep_internal()
        {
            TofArHandManager.Instance.StopStream();
            TofArHandManager.Instance.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            if (handManagerController.IsStreamActive)
            {
                TofArHandManager.Instance.StartStream();
            }
        }

        protected override bool Play_internal(string fileName)
        {
            if (handManagerController.IsStreamActive)
            {
                TofArHandManager.Instance.StartPlayback();
            }
            return true;
        }

        protected override void Pause_internal()
        {
            Debug.Log("Hand Pause");
            TofArHandManager.Instance.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArHandManager.Instance.UnpauseStream();
            Debug.Log("Hand UnPause");
        }

        protected override void Stop_internal()
        {
            TofArHandManager.Instance.StopPlayback();
        }

        protected override string[] GetFileNames(string dirPath)
        {
            return new string[] { "" };
        }
    }
}
