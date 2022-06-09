/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Body;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Body
{
    public class FpsBody : MonoBehaviour
    {
        Text txtFps;

        void Start()
        {
            if (!TofArBodyManager.Instance)
            {
                Debug.LogError("TofArHandManager is not found.");
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
            txtFps.text = $"{TofArBodyManager.Instance.FrameRate:0.0}";
        }

    }
}
