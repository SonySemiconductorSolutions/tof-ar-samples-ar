/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TofArARSamples.StepOn
{
    public class SettingsPanel : MonoBehaviour
    {
        public void Show()
        {
            this.gameObject.SetActive(!this.gameObject.activeSelf);
        }

    }
}
