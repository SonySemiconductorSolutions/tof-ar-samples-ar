/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using SensCord;
using UnityEngine.Events;

namespace TofArSettings
{
    public abstract class ImageFpsRequestController : ControllerBase
    {
        public float FrameRate
        {
            get { return GetFrameRate(); }
            set
            {
                if (value != FrameRate && Min <= value && value <= Max)
                {
                    Apply(value);

                    OnChangeFrameRate?.Invoke(FrameRate);
                }
            }
        }

        public float Min { get; protected set; }
        public float Max { get; protected set; }
        public const int Step = 1;

        public UnityAction OnChangeRange;

        public event ChangeValueEvent OnChangeFrameRate;

        protected float defaultFrameRate;

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
            // Child class is called first
            GetProperty();

            base.Start();
        }

        /// <summary>
        /// Called when object is destroyed (Unity standard function)
        /// </summary>
        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        /// Get FrameRate from TofArXManager
        /// </summary>
        /// <returns>FrameRate</returns>
        protected virtual float GetFrameRate()
        {
            return 0;
        }

        /// <summary>
        /// Get property from TofArXManager
        /// </summary>
        protected virtual void GetProperty()
        {
            // Child class is called first
            // Clamp is value is outside of range
            if (FrameRate < Min)
            {
                FrameRate = Min;
            }
            else if (Max < FrameRate)
            {
                FrameRate = Max;
            }
        }

        /// <summary>
        /// Set FrameRate to TofArXManager
        /// </summary>
        /// <param name="val">FrameRate</param>
        protected virtual void SetProperty(float val)
        {
        }

        /// <summary>
        /// Apply the request speed change
        /// </summary>
        /// <param name="val">Property</param>
        protected void Apply(float val)
        {
            try
            {
                SetProperty(val);
            }
            catch (ApiException e)
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug,
                    $"{e.GetType().Name} : {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
