/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using UnityEngine;

namespace TofArSettings.UI
{
    public class InfoFade : Info
    {
        /// <summary>
        /// Display text
        /// </summary>
        public override string InfoText
        {
            set
            {
                base.InfoText = value;

                // Show
                alpha = -1;
                StartCoroutine(Show());
            }
        }

        /// <summary>
        /// Show time (Unit: s)
        /// </summary>
        public float ShowTime = 1;

        /// <summary>
        /// Fade in time (Unit: s)
        /// </summary>
        public float FadeInTime = 0.2f;

        /// <summary>
        /// Fade out time (Unit: s)
        /// </summary>
        public float FadeOutTime = 0.2f;

        CanvasGroup canvasGroup;
        float alpha = 0;

        void Start()
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        /// <summary>
        /// Show
        /// </summary>
        IEnumerator Show()
        {
            // Wait for one frame to stop the previous display
            yield return null;

            alpha = 0;
            canvasGroup.alpha = alpha;

            // Fade in
            while (alpha < 1)
            {
                // Stop animation when newly displayed
                if (alpha < 0)
                {
                    yield break;
                }

                alpha += (Time.deltaTime / FadeInTime);
                if (alpha >= 1)
                {
                    alpha = 1;
                }

                canvasGroup.alpha = alpha;
                yield return null;
            }

            // Show
            yield return new WaitForSeconds(ShowTime);

            // Fade out
            while (alpha > 0)
            {
                // Stop animation when newly displayed
                if (alpha < 0)
                {
                    yield break;
                }

                alpha -= (Time.deltaTime / FadeOutTime);
                if (alpha < 0)
                {
                    alpha = 0;
                }

                canvasGroup.alpha = alpha;
                yield return null;
            }
        }
    }
}
