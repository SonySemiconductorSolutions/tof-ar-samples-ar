/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TofArARSamples.RockPaperScissors
{
    public class HederPanel : MonoBehaviour
    {
        private ScreenOrientation currentOrientation;

        public GameObject panel;
        public GameObject contentRoot;

        void Start()
        {
            VerticalAndHorizontalBranch();
        }

        void Update()
        {
            CheckChangeOrientation();
        }

        private void CheckChangeOrientation()
        {
            if (Screen.orientation != currentOrientation)
            {
                VerticalAndHorizontalBranch();
            }
        }

        private void VerticalAndHorizontalBranch()
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                SetRectTransform(-75, 150, 0);
            }
            else
            {
                SetRectTransform(-125, 250, -35);
            }

            currentOrientation = Screen.orientation;
        }

        private void SetRectTransform(int p_position_y,int p_size_y,int c_postion_y)
        {
            RectTransform p_rectTransform = panel.GetComponent<RectTransform>();

            Vector3 p_position = p_rectTransform.anchoredPosition3D;
            Vector2 p_size = p_rectTransform.sizeDelta;

            p_rectTransform.anchoredPosition3D = new Vector3(p_position.x, p_position_y, p_position.z);
            p_rectTransform.sizeDelta = new Vector2(p_size.x, p_size_y);

            RectTransform c_rectTransform = contentRoot.GetComponent<RectTransform>();

            Vector3 c_position = c_rectTransform.anchoredPosition3D;

            c_rectTransform.localPosition = new Vector3(c_position.x, c_postion_y, c_position.z);
        }
    }
}
