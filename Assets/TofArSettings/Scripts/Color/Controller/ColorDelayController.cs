/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */
using TofAr.V0.Color;

namespace TofArSettings.Color
{
    public class ColorDelayController : ImageDelayController
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            TofArColorManager.Instance.SetDefaultStreamDelay.AddListener(OnChangeDelay);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TofArColorManager.Instance?.SetDefaultStreamDelay.RemoveListener(OnChangeDelay);
        }

        protected override int GetDelay()
        {
            return TofArColorManager.Instance.StreamDelay;
        }

        protected override void SetDelay(int val)
        {
            TofArColorManager.Instance.StreamDelay = val;
        }
    }
}
