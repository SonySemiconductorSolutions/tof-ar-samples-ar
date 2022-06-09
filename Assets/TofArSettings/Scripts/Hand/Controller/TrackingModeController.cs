/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class TrackingModeController : ControllerBase
    {
        public bool OnOff
        {
            get
            {
                return TofArHandManager.Instance.TrackingMode;
            }

            set
            {
                if (OnOff != value)
                {
                    TofArHandManager.Instance.TrackingMode = value;
                    OnChange?.Invoke(OnOff);
                }
            }
        }

        public event ChangeToggleEvent OnChange;
    }
}
