﻿/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using TofArSettings;
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
            scRotCtrl = FindObjectOfType<ScreenRotateController>();

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

            if (scRotCtrl.IsPortrait)
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
