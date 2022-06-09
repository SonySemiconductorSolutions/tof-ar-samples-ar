/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;
using UnityEngine;
using FrameRateProperty = TofAr.V0.Tof.FrameRateProperty;

namespace TofArSettings.Tof
{
    public class TofFpsRequestController : ImageFpsRequestController
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            TofArTofManager.OnStreamStarted += OnStreamStarted;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TofArTofManager.OnStreamStarted -= OnStreamStarted;
        }

        protected override void Start()
        {
            var mgr = TofArTofManager.Instance;

            // Save initial values
            defaultFrameRate = mgr.DesiredFrameRate;

            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (TofArTofManager.Instance)
            {
                // Reset request speed back to initial values
                Apply(defaultFrameRate);
            }
        }

        /// <summary>
        /// Event that is called when Tof stream is started
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        /// <param name="depthTexture">Depth texture</param>
        /// <param name="confidenceTexture">Confidence texture</param>
        /// <param name="pointCloudData">PointCloud data</param>
        void OnStreamStarted(object sender, Texture2D depthTexture,
            Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            GetProperty();
        }

        protected override float GetFrameRate()
        {
            return TofArTofManager.Instance.DesiredFrameRate;
        }

        protected override void GetProperty()
        {
            var frameRateRange = TofArTofManager.Instance.GetProperty<FrameRateRangeProperty>();
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
            TofArTofManager.Instance.SetProperty(prop);
        }
    }
}
