/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class GestureIntervalFrameNotRecogController : ControllerBase
    {
        public int IntervalFrameNotRecognized
        {
            get
            {
                return TofArHandManager.Instance.IntervalFramesNotRecognized;
            }

            set
            {
                if (IntervalFrameNotRecognized != value &&
                    Min <= value && value <= Max)
                {
                    TofArHandManager.Instance.IntervalFramesNotRecognized = value;
                    OnChange?.Invoke(IntervalFrameNotRecognized);
                }
            }
        }

        public const int Min = 0;
        public const int Max = 15;
        public const int Step = 1;

        public event ChangeIndexEvent OnChange;
    }
}
