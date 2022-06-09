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
    public class InfoLR : MonoBehaviour
    {
        public string TitleText
        {
            get { return txtTitle.text; }
            set
            {
                txtTitle.text = $"{value} :";
            }
        }

        public string LeftText
        {
            get { return txtLeft.text; }
            set { txtLeft.text = value; }
        }

        public string RightText
        {
            get { return txtRight.text; }
            set { txtRight.text = value; }
        }

        Text txtTitle, txtLeft, txtRight;

        void Awake()
        {
            // Get UI
            foreach (var ui in GetComponentsInChildren<Text>())
            {
                if (ui.name.Contains("Title"))
                {
                    txtTitle = ui;
                }
                else if (ui.name.Contains("Left"))
                {
                    txtLeft = ui;
                }
                else if (ui.name.Contains("Right"))
                {
                    txtRight = ui;
                }
            }
        }

        /// <summary>
        /// Set strings (both left and right)
        /// </summary>
        /// <param name="left">Left info</param>
        /// <param name="right">Right info</param>
        public void SetText(string left, string right)
        {
            LeftText = left;
            RightText = right;
        }
    }
}
