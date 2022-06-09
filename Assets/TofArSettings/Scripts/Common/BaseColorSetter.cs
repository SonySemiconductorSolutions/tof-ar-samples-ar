/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class BaseColorSetter : MonoBehaviour
    {
        private void Awake()
        {
            SetColorImage();
        }

        private void SetColorImage()
        {
            Image image = this.gameObject.GetComponent<Image>();

            BaseColor baseColor = Resources.Load<BaseColor>("BaseColor");

            if (baseColor != null)
            {
                image.color = baseColor.color;
            }
        }
    }
}
