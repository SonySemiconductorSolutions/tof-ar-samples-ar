/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings
{
    [RequireComponent(typeof(Text))]
    public class VersionInfo : MonoBehaviour
    {
        void Start()
        {
            if (TofArManager.Instance != null)
            {
                var txt = GetComponent<Text>();
                txt.text = $"ToF AR version : v{TofArManager.Instance.Version}";
            }
        }

        /// <summary>
        /// Set text
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            var txt = GetComponent<Text>();
            txt.text = $"ToF AR version : v{text}";
        }
    }
}
