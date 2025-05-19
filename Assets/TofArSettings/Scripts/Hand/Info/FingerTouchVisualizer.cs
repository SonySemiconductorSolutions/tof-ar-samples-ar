/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Hand
{
    public class FingerTouchVisualizer : MonoBehaviour
    {
        [SerializeField]
        HandStatus lrHand = HandStatus.RightHand;

        [SerializeField]
        UnityEngine.Color colorNormal = UnityEngine.Color.white;

        [SerializeField]
        UnityEngine.Color colorHighlight = UnityEngine.Color.red;

        FingerTouchController fingerTouchCtrl;
        Image[] imgFingers = new Image[5];

        void Start()
        {
            fingerTouchCtrl = FindAnyObjectByType<FingerTouchController>();
            fingerTouchCtrl.OnChangeTouchStates += ChangeAppearance;

            // Get UI
            var imgs = GetComponentsInChildren<Image>();
            string[] fingerNames = new string[]
            {
                "Thumb", "Index", "Middle", "Ring", "Pinky"
            };

            for (int i = 0; i < fingerNames.Length; i++)
            {
                string fingerName = fingerNames[i];
                for (int j = 0; j < imgs.Length; j++)
                {
                    var img = imgs[j];
                    if (img.name.Contains(fingerName))
                    {
                        imgFingers[i] = img;
                        break;
                    }
                }
            }

            ChangeAppearance(null);
        }

        /// <summary>
        /// Change appearance
        /// </summary>
        /// <param name="touchStates">Touch status</param>
        void ChangeAppearance(bool[] touchStates)
        {
            int startIndex = (lrHand == HandStatus.LeftHand) ? 0 : 5;
            for (int i = 0; i < 5; i++)
            {
                bool state = (touchStates == null) ? false : touchStates[startIndex + i];
                imgFingers[i].color = (state) ? colorHighlight : colorNormal;
            }
        }
    }
}
