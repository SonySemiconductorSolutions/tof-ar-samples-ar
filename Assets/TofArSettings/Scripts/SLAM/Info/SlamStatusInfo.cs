/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Slam;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Slam
{
    public class SlamStatusInfo : MonoBehaviour
    {
        Text txtStatus;

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
                if (ui.name.Contains("Status"))
                {
                    txtStatus = ui;
                }
            }
        }

        void Update()
        {
            if (TofArSlamManager.Instance.IsStreamActive)
            {
                if (TofArSlamManager.Instance?.SlamData != null)
                {
                    txtStatus.text = $"{TofArSlamManager.Instance.SlamData.Status}";
                }
            }
            else
            {
                txtStatus.text = $"None";
            }

        }
    }
}
