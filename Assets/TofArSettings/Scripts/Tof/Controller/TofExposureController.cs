/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using SensCord;
using TofAr.V0;
using TofAr.V0.Tof;
using UnityEngine;

namespace TofArSettings.Tof
{
    public class TofExposureController : ImageExposureController
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

        protected override void GetProperty()
        {
            base.GetProperty();

            TofAr.V0.Tof.ExposureProperty prop = null;
            try
            {
                prop = TofArTofManager.Instance.GetProperty<TofAr.V0.Tof.ExposureProperty>();
            }
            catch (ApiException e)
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, TofAr.V0.Utils.FormatException(e));
            }
            if (prop == null)
            {
                return;
            }

            bool isChange = (autoExposure != prop.autoExposure ||
                exposureTime != prop.exposureTime ||
                TimeMin != prop.minExposureTime || TimeMax != prop.maxExposureTime);
            autoExposure = prop.autoExposure;
            exposureTime = prop.exposureTime / 1000;
            TimeMin = prop.minExposureTime / 1000;
            TimeMax = prop.maxExposureTime / 1000;

            if (exposureTime < TimeMin)
            {
                exposureTime = TimeMin;
            }

            // Notify if settings are changed
            if (isChange)
            {
                OnChangeProperty?.Invoke();
            }
        }

        protected override void Apply()
        {
            var prop = new TofAr.V0.Tof.ExposureProperty
            {
                autoExposure = AutoExposure,
                exposureTime = ExposureTime * 1000
            };

            TofArTofManager.Instance.SetProperty(prop);

            base.Apply();
        }
    }
}
