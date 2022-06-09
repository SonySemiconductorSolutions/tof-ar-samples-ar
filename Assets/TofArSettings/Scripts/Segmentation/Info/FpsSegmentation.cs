/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Segmentation;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Segmentation
{
    public class FpsSegmentation : MonoBehaviour
    {
        Text txt;

        void Start()
        {
            if (!TofArSegmentationManager.Instance)
            {
                Debug.LogError("TofArSegmentationManager is not found.");
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
            txt.text = $"{TofArSegmentationManager.Instance.FrameRate:0.0}";
        }
    }
}
