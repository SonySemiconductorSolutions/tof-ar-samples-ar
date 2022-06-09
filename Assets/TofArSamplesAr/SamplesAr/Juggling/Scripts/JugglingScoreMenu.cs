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

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class manages displaying or hiding GUI panel object of result menu.
    /// </summary>
    public class JugglingScoreMenu : MonoBehaviour, IJugglingMenu
    {
        [SerializeField]
        private RectTransform panel;

        [SerializeField]
        private JugglingScoreManager scoreManager;

        /// <summary>
        /// displays the panel.
        /// </summary>
        public void OpenMenu()
        {
            panel.gameObject.SetActive(true);
        }

        /// <summary>
        /// hides the panel.
        /// </summary>
        public void CloseMenu()
        {
            panel.gameObject.SetActive(false);
        }
    }
}
