/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using TofAr.V0.Color;
using TofAr.V0.Segmentation;
using TofArSettings.UI;
using UnityEngine;

namespace TofArSettings.Segmentation
{
    public class SegmentationRecPlayerController : RecPlayerController
    {
        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Segmentation;
        public override bool HasDropdown => true;

        private bool isStreamActive = false;
        private SkyDetectorSettingsProperty skyDetectorSettingsProperty;
        private HumanDetectorSettingsProperty humanDetectorSettingsProperty;

        protected override void Start()
        {
            var fnames = new List<string>();
            fnames.Add("-");
            fnames.Add(RecPlayerSettings.fnamePlayBackEstimate_color);
            FileNames = fnames.ToArray();

            base.Start();
        }

        protected override void PlayPrep_internal()
        {
            isStreamActive = false;

            var segmentationMgr = TofArSegmentationManager.Instance;
            if (segmentationMgr != null)
            {
                skyDetectorSettingsProperty = segmentationMgr.GetProperty<SkyDetectorSettingsProperty>();
                humanDetectorSettingsProperty = segmentationMgr.GetProperty<HumanDetectorSettingsProperty>();

                segmentationMgr.PauseStream();
            }
        }

        protected override bool Play_internal(string fileName)
        {
            var segmentationMgr = TofArSegmentationManager.Instance;
            if (!segmentationMgr)
            {
                return false;
            }

            if (fileName.Equals(RecPlayerSettings.fnamePlayBackEstimate_color))
            {
                var colorMgr = TofArColorManager.Instance;
                if (!colorMgr)
                {
                    return false;
                }

                if (!TofArColorManager.Instance.IsPlaying)
                {
                    return false;
                }

                segmentationMgr.UnpauseStream();

                if (segmentationMgr.IsStreamActive)
                {
                    isStreamActive = true;
                    segmentationMgr.StopStream();
                }

                segmentationMgr.SetProperty(skyDetectorSettingsProperty);
                segmentationMgr.SetProperty(humanDetectorSettingsProperty);

                segmentationMgr.StartPlayback();

                return true;
            }

            return false;
        }

        protected override void Pause_internal()
        {
            TofArSegmentationManager.Instance?.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArSegmentationManager.Instance?.UnpauseStream();
        }

        protected override void Stop_internal()
        {
            TofArSegmentationManager.Instance?.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            var segmentationMgr = TofArSegmentationManager.Instance;
            if (segmentationMgr != null)
            {
                if (isStreamActive)
                {
                    segmentationMgr.StartStream();
                    segmentationMgr.PauseStream();
                }

                segmentationMgr.UnpauseStream();

                segmentationMgr.SetProperty(skyDetectorSettingsProperty);
                segmentationMgr.SetProperty(humanDetectorSettingsProperty);
            }
        }

        protected override string[] GetFileNames(string dirPath)
        {
            return new string[] { "" };
        }
    }
}
