/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using UnityEngine.Events;

namespace TofArSettings
{
    public abstract class ImageExposureController : ControllerBase
    {
        protected bool autoExposure;
        public bool AutoExposure
        {
            get { return autoExposure; }
            set
            {
                if (autoExposure != value)
                {
                    autoExposure = value;
                    Apply();

                    OnChangeAuto?.Invoke(AutoExposure);
                }
            }
        }

        protected long exposureTime;
        public long ExposureTime
        {
            get { return exposureTime; }
            set
            {
                if (exposureTime != value && TimeMin <= value && value <= TimeMax)
                {
                    exposureTime = value;
                    Apply();

                    OnChangeTime?.Invoke(ExposureTime);
                }
            }
        }

        public long TimeMin { get; protected set; }
        public long TimeMax { get; protected set; }
        public const long TimeStep = 1;

        public UnityAction OnChangeProperty;

        public event ChangeToggleEvent OnChangeAuto;

        /// <summary>
        /// Event that is called when a value of type long is changed
        /// </summary>
        /// <param name="val">Value of type long</param>
        public delegate void ChangeLongEvent(long val);

        /// <summary>
        /// Event that is called when ExposureTime is changed
        /// </summary>
        public event ChangeLongEvent OnChangeTime;

        /// <summary>
        /// Executed when script is enabled (Unity standard function)
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// Executed when script is disabled (Unity standard function)
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// Called after application startup (after Awake) (Unity standard function)
        /// </summary>
        protected override void Start()
        {
            GetProperty();
            base.Start();
        }

        /// <summary>
        /// Get Exposure property
        /// </summary>
        protected virtual void GetProperty()
        {
        }

        /// <summary>
        /// Apply Exposure property
        /// </summary>
        protected virtual void Apply()
        {
            // Child class is called first
            TofArManager.Logger.WriteLog(LogLevel.Debug, "Set ExposureModeProperty");
        }
    }
}
