/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Diagnostics;
using TofAr.V0;
using UnityEngine.Events;

namespace TofArSettings.General
{
    public class InternalSessionSettingsController : ControllerBase
    {
        /// <summary>
        /// Turn on/off Internal Session
        /// </summary>
        public bool OnOff
        {
            get
            {
                return onOff;
            }

            set
            {
                if (value != onOff)
                {
                    onOff = value;
                    SetProperty();
                    OnChangeInternalSessionSettings?.Invoke(onOff);
                }
            }
        }

        bool onOff = true;

        public UnityAction<bool> OnChangeInternalSessionSettings;


        protected override void Start()
        {
            TofArManager.Instance?.postInternalSessionStart.AddListener(OnInternalSessionStarted);
            TofArManager.Instance?.postInternalSessionStop.AddListener(OnInternalSessionStop);

            OnOff = TofArManager.Instance.InternalSessionStarted;

            base.Start();
        }

        private void OnDisable()
        {
            TofArManager.Instance?.postInternalSessionStart.RemoveListener(OnInternalSessionStarted);
            TofArManager.Instance?.postInternalSessionStop.RemoveListener(OnInternalSessionStop);
        }

        /// <summary>
        /// Apply internal session state property
        /// </summary>
        void SetProperty()
        {
            var mgr = TofArManager.Instance;
            if (!mgr)
            {
                return;
            }

            if (onOff)
            {
                mgr.StartInternalSession();
            }
            else
            {
                mgr.StopInternalSession();
            }
        }

        void OnInternalSessionStarted()
        {
            TofArManager.Logger.WriteLog(LogLevel.Debug, "INTERNAL SESSION ON");
            onOff = true;
            OnChangeInternalSessionSettings?.Invoke(onOff);
        }

        void OnInternalSessionStop()
        {
            TofArManager.Logger.WriteLog(LogLevel.Debug, "INTERNAL SESSION OFF");
            onOff = false;
            OnChangeInternalSessionSettings?.Invoke(onOff);
        }
    }
}
