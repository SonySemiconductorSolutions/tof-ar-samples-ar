/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Plane;

namespace TofArSettings.Plane
{
    public class DetectIntervalController : ControllerBase
    {
        public int DetectInterval
        {
            get
            {
                return TofArPlaneManager.Instance.GetProperty<DetectIntervalProperty>().intervalFrames;
            }

            set
            {
                if (DetectInterval != value &&
                    Min <= value && value <= Max)
                {
                    TofArPlaneManager.Instance.SetProperty<DetectIntervalProperty>(new DetectIntervalProperty()
                    {
                        intervalFrames = value
                    });
                    OnChange?.Invoke(DetectInterval);
                }
            }
        }

        public const int Min = 1;
        public const int Max = 30;
        public const int Step = 1;

        public event ChangeIndexEvent OnChange;
    }
}