/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Slam;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Slam
{
    public class SlamInfo : MonoBehaviour
    {
        Text txtPos, txtRot, txtAcc;

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
                if (ui.name.Contains("Pos"))
                {
                    txtPos = ui;
                }
                if (ui.name.Contains("Rot"))
                {
                    txtRot = ui;
                }
                if (ui.name.Contains("Accel"))
                {
                    txtAcc = ui;
                }
            }
        }

        void Update()
        {
            if (TofArSlamManager.Instance?.SlamData != null)
            {
                if (TofArSlamManager.Instance.CameraPoseSource == CameraPoseSource.Gyro)
                {
                    txtPos.text = "N/A for CameraPoseSource.Gyro";
                }
                else
                {
                    txtPos.text = $"{TofArSlamManager.Instance.SlamData.Data.Position:F3}";
                }
                txtRot.text = $"{TofArSlamManager.Instance.SlamData.Data.Rotation.eulerAngles:F3}";
                txtAcc.text = $"{TofArSlamManager.Instance.SlamData.Data.Acceleration:F3}";
            }
        }
    }
}
