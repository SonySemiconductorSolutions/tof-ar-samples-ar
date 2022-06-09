/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using TofAr.V0;
using UnityEngine;
using TofArSettings;

namespace TofArSamples.Startup
{
    public class AppInfoPanel : MonoBehaviour
    {
        private VersionInfo versionInfo;
        private JsonInfo jsonInfo;
        private DeviceInfo deviceInfo;

        private void Awake()
        {
            versionInfo = GetComponentInChildren<VersionInfo>();
            jsonInfo = GetComponentInChildren<JsonInfo>();
            deviceInfo = GetComponentInChildren<DeviceInfo>();
        }

        /// <summary>
        /// Set each information to Text
        /// </summary>
        public void SetInfo()
        {
            if (versionInfo)
            {
                string version = TofArManager.Instance.Version;
                versionInfo.SetText(version);
            }

            if (jsonInfo)
            {
                ConfigSource configSource = TofArManager.Instance.GetConfigSource();
                jsonInfo.SetText(configSource);
            }

            if (deviceInfo)
            {
                string modelName = TofArManager.Instance.GetProperty<DeviceCapabilityProperty>().modelName;
                deviceInfo.SetText(modelName);
            }
        }
    }
}
