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
    public class Info : MonoBehaviour
    {
        /// <summary>
        /// Title
        /// </summary>
        public string TitleText
        {
            get { return txtTitle.text; }
            set
            {
                txtTitle.text = $"{value} :";
            }
        }

        /// <summary>
        /// Display text
        /// </summary>
        public virtual string InfoText
        {
            get { return txtInfo.text; }
            set { txtInfo.text = value; }
        }

        Text txtTitle;
        protected Text txtInfo;

        void Awake()
        {
            // Get UI
            foreach (var ui in GetComponentsInChildren<Text>())
            {
                if (ui.name.Contains("Title"))
                {
                    txtTitle = ui;
                }
                else if (ui.name.Contains("Txt"))
                {
                    txtInfo = ui;
                }
            }
        }
    }
}
