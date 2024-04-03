/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;
using TofAr.V0.Plane;


namespace TofArSettings.Plane
{
    public class FpsPlane : MonoBehaviour
    {
        Text txt;

        void Start()
        {
            if (!TofArPlaneManager.Instance)
            {
                Debug.LogError("TofArPlaneManager is not found.");
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
            txt.text = $"{TofArPlaneManager.Instance.FrameRate:0.0}";
        }
    }
}
