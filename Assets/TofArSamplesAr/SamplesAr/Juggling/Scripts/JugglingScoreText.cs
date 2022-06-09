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
    /// This class displays score information as text.
    /// </summary>
    public class JugglingScoreText : MonoBehaviour
    {
        [SerializeField]
        private JugglingScoreManager scoreManager;

        [SerializeField]
        private Text text;

        private void Awake()
        {
            scoreManager.OnScoreUpdated += OnScoreUpdated;
        }

        private void OnDestroy()
        {
            scoreManager.OnScoreUpdated -= OnScoreUpdated;
        }

        /// <summary>
        /// called when the score is updated.
        /// </summary>
        /// <param name="ballCount"></param>
        /// <param name="catchCount"></param>
        private void OnScoreUpdated(int ballCount, int catchCount)
        {
            ReloadText(ballCount, catchCount);
        }

        /// <summary>
        /// updates the text description.
        /// </summary>
        /// <param name="ballCount"></param>
        /// <param name="catchCount"></param>
        private void ReloadText(int ballCount, int catchCount)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Balls:{0}", ballCount);
            sb.AppendLine();
            sb.AppendFormat("Catch:{0}", catchCount);

            text.text = sb.ToString();
        }
    }
}
