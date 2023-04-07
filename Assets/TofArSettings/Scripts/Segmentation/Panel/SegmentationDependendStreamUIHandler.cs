/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using TofAr.V0;
using TofAr.V0.Segmentation;
namespace TofArSettings
{
    public class SegmentationDependendStreamUIHandler : IDependendStreamUIHandler
    {
        private void OnEnable()
        {
            TofArSegmentationManager.OnStreamStarted += OnStreamStarted;
            TofArSegmentationManager.OnStreamStopped += OnStreamStopped;
        }

        private void OnDisable()
        {
            TofArSegmentationManager.OnStreamStarted -= OnStreamStarted;
            TofArSegmentationManager.OnStreamStopped -= OnStreamStopped;
        }

        private void OnStreamStarted(object sender)
        {
            OnStreamStatusChanged?.Invoke();
        }

        private void OnStreamStopped(object sender)
        {
            OnStreamStatusChanged?.Invoke();
        }

        override public void AddTofDependendStreams(List<IDependManager> managersDepend) { }

        override public void AddColorDependendStreams(List<IDependManager> managersDepend) {
            managersDepend.Add(TofArSegmentationManager.Instance);
        }
    }
}
