/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Tof
{
    public class FpsTof : MonoBehaviour
    {
        Text txt;

        void Start()
        {
            if (!TofArTofManager.Instance)
            {
                Debug.LogError("TofArTofManager is not found.");
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
            txt.text = $"{TofArTofManager.Instance.FrameRate:0.0}";
        }
    }
}
