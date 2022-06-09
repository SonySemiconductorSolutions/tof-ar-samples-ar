/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;
using TofAr.V0;

namespace TofArSettings
{
    [RequireComponent(typeof(Text))]
    public class JsonInfo : MonoBehaviour
    {
        void Start()
        {
            if (TofArManager.Instance != null)
            {
                var configSource = TofArManager.Instance.GetConfigSource();

                SetText(configSource);
            }
        }

        /// <summary>
        /// Set text
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            var txt = GetComponent<Text>();
            txt.text = $"Config source : {text}";
        }

        /// <summary>
        /// Set text
        /// </summary>
        /// <param name="configSource">Device-specific config load source</param>
        public void SetText(ConfigSource configSource)
        {
            var txt = GetComponent<Text>();
            txt.text = $"Config source : {ConfigSourceToText(configSource)}";
        }

        private string ConfigSourceToText(ConfigSource configSource)
        {
            string configSourceDesc = "None";
            if (configSource == ConfigSource.Default)
            {
                configSourceDesc = "SDK Default Value";
            }
            else if (configSource == ConfigSource.LocalFile)
            {
                configSourceDesc = "Local File";
            }

            return configSourceDesc;
        }
    }
}
