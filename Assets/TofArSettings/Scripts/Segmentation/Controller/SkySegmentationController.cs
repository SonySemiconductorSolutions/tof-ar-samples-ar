/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Segmentation.Sky;

namespace TofArSettings.Segmentation
{
    public class SkySegmentationController : ControllerBase
    {
        private SkySegmentationDetector skyDetector;

        private void Awake()
        {
            skyDetector = FindAnyObjectByType<SkySegmentationDetector>();
        }

        private bool skySegmenationEnabled = false;
        public bool SkySegmentationEnabled
        {
            get => skySegmenationEnabled;
            set
            {
                if (skySegmenationEnabled != value)
                {
                    skySegmenationEnabled = value;
                    skyDetector.IsActive = IsSkySegRunning;
                    OnSkyChange?.Invoke(skySegmenationEnabled);
                }
            }
        }

        private bool notSkySegmenationEnabled = false;
        public bool NotSkySegmentationEnabled
        {
            get => notSkySegmenationEnabled;
            set
            {
                if (notSkySegmenationEnabled != value)
                {
                    notSkySegmenationEnabled = value;
                    skyDetector.IsActive = IsSkySegRunning;
                    OnNotSkyChange?.Invoke(notSkySegmenationEnabled);
                }
            }
        }

        public bool IsSkySegRunning { get => notSkySegmenationEnabled || skySegmenationEnabled; }

        public event ChangeToggleEvent OnSkyChange;

        public event ChangeToggleEvent OnNotSkyChange;
    }
}
