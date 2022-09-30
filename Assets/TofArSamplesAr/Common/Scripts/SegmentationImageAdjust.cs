/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using UnityEngine;

namespace TofArARSamples
{
    public class SegmentationImageAdjust : MonoBehaviour
    {
        private int imageRotation = 0;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        void OnEnable()
        {
            TofArManager.OnScreenOrientationUpdated += OnScreenOrientationChanged;

            UpdateRotation();
        }

        private void OnDisable()
        {
            TofArManager.OnScreenOrientationUpdated -= OnScreenOrientationChanged;

            this.StopAllCoroutines();
        }


        private void UpdateRotation()
        {
            imageRotation = TofArManager.Instance.GetScreenOrientation();

            if (rectTransform != null)
            {
                rectTransform.localRotation = Quaternion.Euler(0f, 0f, imageRotation);
            }
        }

        private void OnScreenOrientationChanged(ScreenOrientation previousDeviceOrientation, ScreenOrientation newDeviceOrientation)
        {
            UpdateRotation();
        }
    }
}
