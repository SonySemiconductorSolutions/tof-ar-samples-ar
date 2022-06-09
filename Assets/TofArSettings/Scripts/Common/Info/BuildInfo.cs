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
    public class BuildInfo : MonoBehaviour
    {
        const string fileName = "BuildDateTime";

        void Start()
        {
            var txt = GetComponent<Text>();
            var asset = Resources.Load<TextAsset>(fileName);
            string str = (asset) ? asset.text : "-";
            txt.text = $"Build Version : {str}";
        }
    }
}
