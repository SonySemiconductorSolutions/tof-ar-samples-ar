/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using TofAr.V0;

namespace TofArSettings.General
{
    public class DepthFilteringController : ControllerBase
    {
        int modeIndex;
        public int ModeIndex
        {
            get { return modeIndex; }
            set
            {
                if (value != modeIndex && 0 <= value &&
                    value < DepthFilteringModeList.Length)
                {
                    DepthFilteringMode = DepthFilteringModeList[value];
                }
            }
        }

        public AvFoundationDepthFiltering DepthFilteringMode
        {
            get
            {
                var platformConfigProperty = TofArManager.Instance.GetProperty<PlatformConfigurationProperty>();
                return platformConfigProperty.platformConfigurationIos.depthFiltering;
            }

            set
            {
                var platformConfigProperty = TofArManager.Instance.GetProperty<PlatformConfigurationProperty>();
                var api = platformConfigProperty.platformConfigurationIos.depthFiltering;
                if (value != api)
                {
                    modeIndex = Utils.Find(value, DepthFilteringModeList);
                    platformConfigProperty.platformConfigurationIos.depthFiltering = value;
                    TofArManager.Instance.SetProperty(platformConfigProperty);
                    //managerController.RestartStream();
                    OnChangeDepthFilteringMode?.Invoke(ModeIndex);
                }
            }
        }

        public AvFoundationDepthFiltering[] DepthFilteringModeList { get; private set; }
        string[] depthFilteringModeNames = new string[0];
        public string[] DepthFilteringModeNames
        {
            get { return depthFilteringModeNames; }
        }

        /// <summary>
        /// Event that is called when CameraApi list is updated
        /// </summary>
        /// <param name="list">CameraApi name list</param>
        /// <param name="mode1Index">CameraApi index</param>
        public delegate void UpdateDepthFilteringModeListEvent(string[] list,
            int modeIndex);
        public event UpdateDepthFilteringModeListEvent OnUpdateDepthFilteringModeList;

        public ChangeIndexEvent OnChangeDepthFilteringMode;

        protected override void Start()
        {
            base.Start();
            UpdateDepthFilteringModeList();
        }

        /// <summary>
        /// Update CameraApi list
        /// </summary>
        void UpdateDepthFilteringModeList()
        {
            // Get RuntimeMode list
            DepthFilteringModeList = (AvFoundationDepthFiltering[])Enum.GetValues(typeof(AvFoundationDepthFiltering));
            if (DepthFilteringModeList.Length != depthFilteringModeNames.Length)
            {
                Array.Resize(ref depthFilteringModeNames, DepthFilteringModeList.Length);
            }

            for (int i = 0; i < DepthFilteringModeList.Length; i++)
            {
                depthFilteringModeNames[i] = DepthFilteringModeList[i].ToString();
            }

            // Get intial values of RuntimeMode1
            var platformConfigProperty = TofArManager.Instance.GetProperty<PlatformConfigurationProperty>();
            var api = platformConfigProperty.platformConfigurationIos.depthFiltering;
            modeIndex = Utils.Find(api, DepthFilteringModeList);
            if (modeIndex < 0)
            {
                modeIndex = 0;
            }

            OnUpdateDepthFilteringModeList?.Invoke(DepthFilteringModeNames, ModeIndex);
        }
    }
}
