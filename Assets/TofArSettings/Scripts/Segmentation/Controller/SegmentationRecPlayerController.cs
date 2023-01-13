/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Linq;
using TofAr.V0;
using TofAr.V0.Segmentation;
using TofArSettings.UI;
using UnityEngine;

namespace TofArSettings.Segmentation
{
    public class SegmentationRecPlayerController : RecPlayerController
    {
        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Segmentation;

        public override bool HasDropdown => false;

        private SegmentationManagerController managerController;

        private void Awake()
        {
            managerController = FindObjectOfType<SegmentationManagerController>();
        }

        protected override void PlayPrep_internal()
        {
            TofArSegmentationManager.Instance.StopStream();
            TofArSegmentationManager.Instance.StopPlayback();
        }

        protected override bool Play_internal(string fileName)
        {
            if (managerController.IsStreamActive())
            {
                TofArSegmentationManager.Instance.StartPlayback();
            }
            return true;
        }

        protected override void Pause_internal()
        {
            Debug.Log("Segmentation Pause");
            TofArSegmentationManager.Instance.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArSegmentationManager.Instance.UnpauseStream();
            Debug.Log("Segmentation UnPause");
        }

        protected override void Stop_internal()
        {
            TofArSegmentationManager.Instance.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            if (managerController.IsStreamActive())
            {
                TofArSegmentationManager.Instance.StartStream();
            }
        }

        protected override string[] GetFileNames(string dirPath)
        {
            return new string[] { "" };
        }
    }
}
