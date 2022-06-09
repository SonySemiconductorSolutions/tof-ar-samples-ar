/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Color;
using FrameRateProperty = TofAr.V0.Color.FrameRateProperty;

namespace TofArSettings.Color
{
    public class ColorFpsRequestController : ImageFpsRequestController
    {
        protected override void Start()
        {
            var mgr = TofArColorManager.Instance;

            // Save initial values
            defaultFrameRate = mgr.DesiredFrameRate;

            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (TofArColorManager.Instance)
            {
                // Reset request speed  to initial values
                Apply(defaultFrameRate);
            }
        }

        protected override float GetFrameRate()
        {
            return TofArColorManager.Instance.DesiredFrameRate;
        }

        protected override void GetProperty()
        {
            var frameRateRange = TofArColorManager.Instance.GetProperty<FrameRateRangeProperty>();
            bool isChange = (Min != frameRateRange.minimumFrameRate ||
                Max != frameRateRange.maximumFrameRate);
            Min = frameRateRange.minimumFrameRate;
            Max = frameRateRange.maximumFrameRate;

            base.GetProperty();

            if (isChange)
            {
                OnChangeRange?.Invoke();
            }
        }

        protected override void SetProperty(float val)
        {
            base.SetProperty(val);

            var prop = new FrameRateProperty() { desiredFrameRate = val };
            TofArColorManager.Instance.SetProperty(prop);
        }
    }
}
