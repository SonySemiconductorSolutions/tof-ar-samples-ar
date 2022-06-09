/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class manages displaying or hiding GUI panel object of result menu.
    /// </summary>
    public class JugglingResultMenu : MonoBehaviour, IJugglingMenu
    {
        [SerializeField]
        private RectTransform panel;

        [SerializeField]
        private Text resultText;

        [SerializeField]
        private JugglingScoreManager scoreManager;

        /// <summary>
        /// displays the panel.
        /// </summary>
        public void OpenMenu()
        {
            panel.gameObject.SetActive(true);
            SetResultText();
        }

        /// <summary>
        /// hides the panel.
        /// </summary>
        public void CloseMenu()
        {
            panel.gameObject.SetActive(false);
        }

        /// <summary>
        /// displays scores as text.
        /// </summary>
        private void SetResultText()
        {
            scoreManager.GetScore(out int ballCount, out int catchCount);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Balls:{0}", ballCount);
            sb.AppendLine();
            sb.AppendFormat("Catch:{0}", catchCount);

            resultText.text = sb.ToString();
        }
    }
}
