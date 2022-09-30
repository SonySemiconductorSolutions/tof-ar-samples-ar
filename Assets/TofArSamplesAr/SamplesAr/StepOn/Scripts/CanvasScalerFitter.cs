/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TofArARSamples.IceWater
{
    public class CanvasScalerFitter : MonoBehaviour
    {
        public int baseScale;

        void Start()
        {
            ChangeScaleFactor();
        }

        private void ChangeScaleFactor()
        {
            CanvasScaler canvasScaler = this.GetComponent<CanvasScaler>();
            int screenScale = System.Math.Max(Screen.height, Screen.width);

            canvasScaler.scaleFactor = (screenScale * canvasScaler.scaleFactor) / baseScale;
        }
    }
}
