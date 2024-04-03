/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Slam;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Slam
{
    public class FpsSlam : MonoBehaviour
    {
        Text txtFps, txtMs;

        void Start()
        {
            if (!TofArSlamManager.Instance)
            {
                Debug.LogError("TofArSlamManager is not found.");
                enabled = false;
                return;
            }

            // Get UI
            foreach (var ui in GetComponentsInChildren<Text>())
            {
                if (ui.name.Contains("Fps"))
                {
                    txtFps = ui;
                }
            }
        }

        void Update()
        {
            txtFps.text = $"{TofArSlamManager.Instance.FrameRate:0.0}";
        }
    }
}