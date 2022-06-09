/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using TofAr.V0.Color;
using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Color
{
    public class FocusController : ControllerBase
    {
        bool autoFocus;
        public bool AutoFocus
        {
            get { return autoFocus; }
            set
            {
                if (value != AutoFocus)
                {
                    autoFocus = value;
                    SetProperty();
                    OnChangeAutoFocus?.Invoke(AutoFocus);
                }
            }
        }

        float dist;
        public float Distance
        {
            get { return dist; }
            set
            {
                if (value != Distance && DistMin <= value && value <= DistMax)
                {
                    dist = value;
                    SetProperty();
                    OnChangeDist?.Invoke(Distance);
                }
            }
        }

        public float DistMin { get; private set; }
        public float DistMax { get; private set; }
        public const float DistStep = 0.1f;

        public UnityAction OnChangeProperty;

        public event ChangeToggleEvent OnChangeAutoFocus;

        public event ChangeValueEvent OnChangeDist;

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
        /// Get ColorFormat property
        /// </summary>
        void GetProperty()
        {
            var prop = TofArColorManager.Instance.GetProperty<FocusModeProperty>();
            bool isChange = (autoFocus != prop.autoFocus || dist != prop.distance ||
                DistMin != prop.minDistance || DistMax != prop.maxDistance);
            autoFocus = prop.autoFocus;
            DistMin = prop.minDistance;
            DistMax = prop.maxDistance;

            // Clamp values as values may be set outside of the range
            dist = prop.distance;
            if (dist < DistMin)
            {
                dist = DistMin;
            }
            else if (DistMax < dist)
            {
                dist = DistMax;
            }

            // Notify when properties are changed
            if (isChange)
            {
                OnChangeProperty?.Invoke();
            }
        }

        /// <summary>
        /// Apply FocusMode property to TofArColorManager
        /// </summary>
        void SetProperty()
        {
            var prop = new FocusModeProperty
            {
                autoFocus = AutoFocus,
                distance = Distance
            };

            TofArManager.Logger.WriteLog(LogLevel.Debug, "Set FocusModeProperty");
            try
            {
                TofArColorManager.Instance.SetProperty(prop);
            }
            catch (SensCord.ApiException e)
            {
                TofAr.V0.TofArManager.Logger.WriteLog(TofAr.V0.LogLevel.Debug, $"Failed to set focus mode. {e.Message}");
            }
        }
    }
}
