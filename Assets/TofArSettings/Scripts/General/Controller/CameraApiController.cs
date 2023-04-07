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
    public class CameraApiController : ControllerBase
    {
        int apiIndex;
        public int ApiIndex
        {
            get { return apiIndex; }
            set
            {
                if (value != apiIndex && 0 <= value &&
                    value < CameraApiList.Length)
                {
                    CameraApi = CameraApiList[value];
                }
            }
        }

        public IosCameraApi CameraApi
        {
            get
            {
                var platformConfigProperty = TofArManager.Instance.GetProperty<PlatformConfigurationProperty>();
                return platformConfigProperty.platformConfigurationIos.cameraApi;
            }

            set
            {
                var platformConfigProperty = TofArManager.Instance.GetProperty<PlatformConfigurationProperty>();
                var api = platformConfigProperty.platformConfigurationIos.cameraApi;
                if (value != api)
                {
                    apiIndex = Utils.Find(value, CameraApiList);
                    platformConfigProperty.platformConfigurationIos.cameraApi = value;
                    TofArManager.Instance.SetProperty(platformConfigProperty);
                    //managerController.RestartStream();
                    OnChangeApi?.Invoke(ApiIndex);
                }
            }
        }

        public IosCameraApi[] CameraApiList { get; private set; }
        string[] apiNames = new string[0];
        public string[] ApiNames
        {
            get { return apiNames; }
        }

        /// <summary>
        /// Event that is called when CameraApi list is updated
        /// </summary>
        /// <param name="list">CameraApi name list</param>
        /// <param name="mode1Index">CameraApi index</param>
        public delegate void UpdateCameraApiListEvent(string[] list,
            int apiIndex);
        public event UpdateCameraApiListEvent OnUpdateCameraApiList;

        public ChangeIndexEvent OnChangeApi;

        protected override void Start()
        {
            base.Start();
            UpdateCameraApiList();
        }

        /// <summary>
        /// Update CameraApi list
        /// </summary>
        void UpdateCameraApiList()
        {
            // Get RuntimeMode list
            CameraApiList = (IosCameraApi[])Enum.GetValues(typeof(IosCameraApi));
            if (CameraApiList.Length != apiNames.Length)
            {
                Array.Resize(ref apiNames, CameraApiList.Length);
            }

            for (int i = 0; i < CameraApiList.Length; i++)
            {
                apiNames[i] = CameraApiList[i].ToString();
            }

            // Get intial values of RuntimeMode1
            var platformConfigProperty = TofArManager.Instance.GetProperty<PlatformConfigurationProperty>();
            var api = platformConfigProperty.platformConfigurationIos.cameraApi;
            apiIndex = Utils.Find(api, CameraApiList);
            if (apiIndex < 0)
            {
                apiIndex = 0;
            }

            OnUpdateCameraApiList?.Invoke(ApiNames, ApiIndex);
        }
    }
}
