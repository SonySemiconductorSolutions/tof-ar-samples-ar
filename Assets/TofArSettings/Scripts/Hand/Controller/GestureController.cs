/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class GestureController : ControllerBase
    {  
        bool onOff;
        public bool OnOff
        {
            get { return onOff; }
            set
            {
                if (onOff != value)
                {
                    onOff = value;
                    if (value)
                    {
                        TofArHandManager.Instance.StartGestureEstimation();
                    }
                    else
                    {
                        TofArHandManager.Instance.StopGestureEstimation();
                    }

                    OnChangeEnable?.Invoke(OnOff);
                }
            }
        }

        public event ChangeToggleEvent OnChangeEnable;

        protected override void Start()
        {
            onOff = TofArHandManager.Instance.autoStartGestureEstimation;
            base.Start();
        }
    }
}
