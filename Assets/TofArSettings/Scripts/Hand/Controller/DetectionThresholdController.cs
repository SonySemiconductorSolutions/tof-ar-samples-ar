/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class DetectionThresholdController : ControllerBase
    {
        public double Threshold
        {
            get
            {
                return TofArHandManager.Instance.DetectionThreshold;
            }

            set
            {
                if (Threshold != value)
                {
                    TofArHandManager.Instance.DetectionThreshold = value;
                    OnChange?.Invoke((float)Threshold);
                }
            }
        }

        public event ChangeValueEvent OnChange;
    }
}
