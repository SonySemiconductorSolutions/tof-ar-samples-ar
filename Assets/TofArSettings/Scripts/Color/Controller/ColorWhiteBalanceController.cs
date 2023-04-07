/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using TofAr.V0;
using TofAr.V0.Color;
using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Color
{
    public class ColorWhiteBalanceController : ControllerBase
    {
        bool autoWhiteBalance;
        public bool AutoWhiteBalance
        {
            get { return autoWhiteBalance; }
            set
            {
                if (value != AutoWhiteBalance)
                {
                    autoWhiteBalance = value;
                    SetProperty();
                    OnChangeAutoWhiteBalance?.Invoke(AutoWhiteBalance);
                }
            }
        }

        int index;
        public int Index
        {
            get { return index; }
            set
            {
                if (value != index && 0 <= value &&
                    value < WhiteBalanceList.Length)
                {
                    index = value;
                    SetProperty();
                    OnChange?.Invoke(Index);
                }
            }
        }

        public WhiteBalanceMode WhiteBalance
        {
            get { return WhiteBalanceList != null ? WhiteBalanceList[Index] : WhiteBalanceMode.Off; }
            set
            {
                if (value != WhiteBalance && WhiteBalanceList != null)
                {
                    Index = Utils.Find(value, WhiteBalanceList);
                }
            }
        }

        public WhiteBalanceMode[] WhiteBalanceList { get; private set; }

        public string[] WhiteBalanceNames { get; private set; }

        public UnityAction OnChangeProperty;

        public event ChangeToggleEvent OnChangeAutoWhiteBalance;

        public event ChangeIndexEvent OnChange;

        protected void Awake()
        {
            colorMgrCtrl = GetComponent<ColorManagerController>();
        }

        void OnEnable()
        {
            TofArColorManager.OnStreamStarted += OnStreamStarted;
        }

        void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnStreamStarted;
        }

        ColorManagerController colorMgrCtrl;

        protected override void Start()
        {
            WhiteBalanceList = new WhiteBalanceMode[] { WhiteBalanceMode.Incandescent,
                WhiteBalanceMode.Fluorescent, WhiteBalanceMode.Daylight,WhiteBalanceMode.CloudyDaylight };
            WhiteBalanceNames = new string[WhiteBalanceList.Length];
            for (int i = 0; i < WhiteBalanceList.Length; i++)
            {
                WhiteBalanceNames[i] = WhiteBalanceList[i].ToString();
            }

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
        /// Get WhiteBalance property
        /// </summary>
        void GetProperty()
        {
            var prop = TofArColorManager.Instance.GetProperty<WhiteBalanceModeProperty>();
            if (prop == null)
            {
                return;
            }
            bool isChange = (autoWhiteBalance != prop.autoWhiteBalance || WhiteBalance != prop.mode);

            autoWhiteBalance = prop.autoWhiteBalance;
            WhiteBalance = prop.mode;

            if (isChange)
            {
                OnChangeProperty?.Invoke();
            }
        }

        /// <summary>
        /// Apply WhiteBalance property to TofArColorManager
        /// </summary>
        void SetProperty()
        {
            var prop = new WhiteBalanceModeProperty
            {
                autoWhiteBalance = AutoWhiteBalance,
                mode = WhiteBalance
            };

            TofArManager.Logger.WriteLog(LogLevel.Debug, "Set WhiteBalanceModeProperty");
            try
            {
                TofArColorManager.Instance.SetProperty(prop);
            }
            catch (SensCord.ApiException e)
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, $"Failed to set WhiteBalanceMode. {e.Message}");
            }
        }

    }
}
