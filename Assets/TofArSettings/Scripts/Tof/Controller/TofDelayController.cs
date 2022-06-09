/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;

namespace TofArSettings.Tof
{
    public class TofDelayController : ImageDelayController
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            TofArTofManager.Instance.SetDefaultStreamDelay.AddListener(OnChangeDelay);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TofArTofManager.Instance?.SetDefaultStreamDelay.RemoveListener(OnChangeDelay);
        }

        protected override int GetDelay()
        {
            return TofArTofManager.Instance.StreamDelay;
        }

        protected override void SetDelay(int val)
        {
            TofArTofManager.Instance.StreamDelay = val;
        }
    }
}
