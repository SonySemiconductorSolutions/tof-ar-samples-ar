/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using TofAr.V0.Color;
using UnityEngine;

namespace TofArSettings.Color
{
    public class StabilizationController : ControllerBase
    {
        bool lens;
        public bool LensStabilization
        {
            get { return lens; }
            set
            {
                if (value != LensStabilization)
                {
                    lens = value;
                    SetProperty();
                    OnChange?.Invoke(LensStabilization);
                }
            }
        }

        public event ChangeToggleEvent OnChange;

        void OnEnable()
        {
            TofArColorManager.OnStreamStarted += OnStreamStarted;
        }

        void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnStreamStarted;
        }

        protected override void Start()
        {
            GetProperty();
            base.Start();
        }

        /// <summary>
        /// Event that is called when Color stream is started
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        /// <param name="colorTexture">Color texture</param>
        void OnStreamStarted(object sender, Texture2D colorTexture)
        {
            GetProperty();
        }

        /// <summary>
        /// Get Stabilization property
        /// </summary>
        void GetProperty()
        {
            var prop = TofArColorManager.Instance.GetProperty<StabilizationProperty>();
            LensStabilization = prop.lensStabilization;
        }

        /// <summary>
        /// Apply Stabilization property to TofArColorManager
        /// </summary>
        void SetProperty()
        {
            var prop = new StabilizationProperty
            {
                lensStabilization = LensStabilization
            };

            TofArManager.Logger.WriteLog(LogLevel.Debug, "Set LensStabilization");
            try
            {

                TofArColorManager.Instance.SetProperty(prop);
            }
            catch (SensCord.ApiException e)
            {
                TofAr.V0.TofArManager.Logger.WriteLog(TofAr.V0.LogLevel.Debug, $"Failed to set lens stabilization. {e.Message}");
            }

        }
    }
}
