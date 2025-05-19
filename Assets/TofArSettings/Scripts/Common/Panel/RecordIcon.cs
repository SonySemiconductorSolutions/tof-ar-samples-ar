/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArSettings.UI
{
    public class RecordIcon : MonoBehaviour
    {
        // Offset value for right align in horizontal display
        public int offset;

        private float initPosX;

        private ScreenRotateController scRotCtrl;

        private void Awake()
        {
            initPosX = this.GetComponent<RectTransform>().anchoredPosition.x;
            scRotCtrl = FindAnyObjectByType<ScreenRotateController>();

            scRotCtrl.OnRotateScreen += OnRotateScreen;
        }

        private void OnDestroy()
        {
            if (scRotCtrl)
            {
                scRotCtrl.OnRotateScreen -= OnRotateScreen;
            }
        }

        /// <summary>
        /// Adjust position to match screen orientation
        /// </summary>
        /// <param name="ori">Screen orientation</param>
        void OnRotateScreen(ScreenOrientation ori)
        {
            RectTransform rectTransform = this.GetComponent<RectTransform>();

            if (scRotCtrl.IsPortraitScreen)
            {
                rectTransform.anchoredPosition = new Vector2(initPosX, rectTransform.anchoredPosition.y);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(initPosX - offset, rectTransform.anchoredPosition.y);
            }
        }

    }
}
