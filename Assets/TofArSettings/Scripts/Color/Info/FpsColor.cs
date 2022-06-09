/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Color;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Color
{
    public class FpsColor : MonoBehaviour
    {
        Text txt;

        void Start()
        {
            if (!TofArColorManager.Instance)
            {
                Debug.LogError("TofArColorManager is not found.");
                enabled = false;
                return;
            }

            // Get UI
            foreach (var ui in GetComponentsInChildren<Text>())
            {
                if (ui.name.Contains("Fps"))
                {
                    txt = ui;
                    break;
                }
            }
        }

        void Update()
        {
            txt.text = $"{TofArColorManager.Instance.FrameRate:0.0}";
        }
    }
}
