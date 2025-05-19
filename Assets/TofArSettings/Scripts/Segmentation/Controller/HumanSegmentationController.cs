/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Segmentation.Human;

namespace TofArSettings.Segmentation
{
    public class HumanSegmentationController : ControllerBase
    {
        private HumanSegmentationDetector humanDetector;

        private void Awake()
        {
            humanDetector = FindAnyObjectByType<HumanSegmentationDetector>();
        }

        private bool humanSegmenationEnabled = false;
        public bool HumanSegmentationEnabled
        {
            get => humanSegmenationEnabled;
            set
            {
                if (humanSegmenationEnabled != value)
                {
                    humanSegmenationEnabled = value;
                    humanDetector.IsActive = IsHumanSegRunning;
                    OnHumanChange?.Invoke(humanSegmenationEnabled);
                }
            }
        }
        private bool notHumanSegmenationEnabled = false;
        public bool NotHumanSegmentationEnabled
        {
            get => notHumanSegmenationEnabled;
            set
            {
                if (notHumanSegmenationEnabled != value)
                {
                    notHumanSegmenationEnabled = value;
                    humanDetector.IsActive = IsHumanSegRunning;
                    OnNotHumanChange?.Invoke(notHumanSegmenationEnabled);
                }
            }
        }

        public bool IsHumanSegRunning { get => notHumanSegmenationEnabled || humanSegmenationEnabled; }

        public event ChangeToggleEvent OnHumanChange;

        public event ChangeToggleEvent OnNotHumanChange;
    }
}
