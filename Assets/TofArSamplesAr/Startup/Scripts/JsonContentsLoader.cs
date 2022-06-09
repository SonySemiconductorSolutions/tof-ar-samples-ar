/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;
using TofAr.V0;

namespace TofArSamples.Startup
{
    [RequireComponent(typeof(Text))]
    public class JsonContentsLoader : MonoBehaviour
    {
        private Text jsonValuesOutput;

        void Start()
        {
            jsonValuesOutput = GetComponent<Text>();
            DeviceCapabilityProperty capability = TofArManager.Instance.GetProperty<DeviceCapabilityProperty>();
            jsonValuesOutput.text = capability?.deviceAttributesString ?? "Json text load failed";
        }
    }
}
