/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings
{
    [RequireComponent(typeof(Text))]
    public class UnityEditorInfo : MonoBehaviour
    {
        void Start()
        {
            var txt = GetComponent<Text>();
            txt.text = $"Unity Editor version : {Application.unityVersion}";
        }
    }
}
