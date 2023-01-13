/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Face;

namespace TofArSettings.Face
{
    public class FacialExpressionController : ControllerBase
    {
        TofArFacialExpressionEstimator facialExpressionEstimator;

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
                        facialExpressionEstimator.StartGestureEstimation();
                    }
                    else
                    {
                        facialExpressionEstimator.StopGestureEstimation();
                    }

                    OnChangeEnable?.Invoke(OnOff);
                }
            }
        }

        public event ChangeToggleEvent OnChangeEnable;

        protected void Awake()
        {
            facialExpressionEstimator = FindObjectOfType<TofArFacialExpressionEstimator>();
        }

        protected override void Start()
        {
            onOff = facialExpressionEstimator.autoStart;

            base.Start();
        }

    }
}
