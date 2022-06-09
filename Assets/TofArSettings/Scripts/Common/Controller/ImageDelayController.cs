/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

namespace TofArSettings
{
    public abstract class ImageDelayController : ControllerBase
    {
        public int Delay
        {
            get
            {
                return GetDelay();
            }

            set
            {
                if (Delay != value && Min <= value && value <= Max)
                {
                    SetDelay(value);
                }
            }
        }

        public const int Min = 0;
        public const int Max = 8;
        public const int Step = 1;

        public ChangeIndexEvent OnChange;

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
        /// Get Delay from TofArXManager
        /// </summary>
        /// <returns>Delay</returns>
        protected virtual int GetDelay()
        {
            return 0;
        }

        /// <summary>
        /// Set Delay to TofArXManager
        /// </summary>
        /// <param name="val">Delay</param>
        protected virtual void SetDelay(int val)
        {
        }

        /// <summary>
        /// Event that is called when Delay is changed
        /// </summary>
        /// <param name="val">delay</param>
        protected void OnChangeDelay(int val)
        {
            OnChange?.Invoke(Delay);
        }
    }
}
