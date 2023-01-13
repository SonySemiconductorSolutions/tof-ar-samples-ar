/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;

namespace TofArSettings.Tof
{
    public class ResortConfidenceController : ControllerBase
    {
        bool resort;
        public bool Resort
        {
            get { return resort; }
            set
            {
                if (resort != value)
                {
                    resort = value;
                    var prop = new DepthConfidenceProperty
                    {
                        depth16ConfidenceConvert = Resort
                    };

                    TofArTofManager.Instance.SetProperty(prop);

                    OnChange?.Invoke(Resort);
                }
            }
        }

        public event ChangeToggleEvent OnChange;

        protected override void Start()
        {
            try
            {
                var prop = TofArTofManager.Instance.GetProperty<DepthConfidenceProperty>();
                if (prop != null)
                {
                    resort = prop.depth16ConfidenceConvert;
                }
            }
            catch (SensCord.ApiException e)
            {
                TofAr.V0.TofArManager.Logger.WriteLog(TofAr.V0.LogLevel.Debug, $"Failed to get DepthConfidenceProperty. Message: {e.Message}");
            }
            base.Start();
        }
    }
}
