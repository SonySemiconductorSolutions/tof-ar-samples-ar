/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2025 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using TofAr.V0.Color;

using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Color
{
    public class ColorDirectFrameCaptureController : ControllerBase
    {
        bool directFrameCaptureEnabled;
        public bool DirectFrameCaptureEnabled
        {
            get { return directFrameCaptureEnabled; }
            set
            {
                if (value != DirectFrameCaptureEnabled)
                {
                    directFrameCaptureEnabled = value;
                    SetValue();
                    OnChangeDirectFrameCaptureEnabled?.Invoke(DirectFrameCaptureEnabled);
                }
            }
        }

        public UnityAction OnChangeProperty;

        public event ChangeToggleEvent OnChangeDirectFrameCaptureEnabled;

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
            GetValue();
            base.Start();
        }

        /// <summary>
        /// Event that is called when Color stream is started
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        /// <param name="colorTexture">Color texture</param>
        void OnStreamStarted(object sender, Texture2D colorTexture)
        {
            GetValue();
        }

        /// <summary>
        /// Get DirectFrameCaptureEnabled
        /// </summary>
        void GetValue()
        {
            var value = TofArColorManager.Instance.DirectFrameCaptureEnabled;
            bool isChange = (directFrameCaptureEnabled != value);
            directFrameCaptureEnabled = value;

            // Notify when properties are changed
            if (isChange)
            {
                OnChangeProperty?.Invoke();
            }
        }

        /// <summary>
        /// Apply DirectFrameCaptureEnabled to TofArColorManager
        /// </summary>
        void SetValue()
        {
            var value = DirectFrameCaptureEnabled;

            TofArManager.Logger.WriteLog(LogLevel.Debug, "Set DirectFrameCaptureEnabled");
            try
            {
                TofArColorManager.Instance.DirectFrameCaptureEnabled = value;
            }
            catch (SensCord.ApiException e)
            {
                TofAr.V0.TofArManager.Logger.WriteLog(TofAr.V0.LogLevel.Debug, $"Failed to set direct frame capture enabled. {e.Message}");
            }
        }
    }
}
