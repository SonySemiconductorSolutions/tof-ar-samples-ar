/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class GestureFramesForDetectNoHandsController : ControllerBase
    {
        public int FramesForDetectNoHands
        {
            get
            {
                return TofArHandManager.Instance.FramesForDetectNoHands;
            }

            set
            {
                if (FramesForDetectNoHands != value &&
                    Min <= value && value <= Max)
                {
                    TofArHandManager.Instance.FramesForDetectNoHands = value;
                    OnChange?.Invoke(FramesForDetectNoHands);
                }
            }
        }

        public const int Min = 0;
        public const int Max = 15;
        public const int Step = 1;

        public event ChangeIndexEvent OnChange;
    }
}
