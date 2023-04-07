/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Threading;
using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class InfoGesture : HandInfo
    {
        UI.InfoFade uiInfoFadeLeft, uiInfoFadeRight, uiInfoFadeBoth;
        SynchronizationContext context;

        GestureController gestureCtrl;

        protected override void Awake()
        {
            base.Awake();

            gestureCtrl = FindObjectOfType<GestureController>();
        }

        void OnEnable()
        {
            TofArHandManager.OnGestureEstimated += GestureEstimated;
            ShowDisableText();
        }

        void OnDisable()
        {
            TofArHandManager.OnGestureEstimated -= GestureEstimated;
        }

        void Start()
        {
            foreach (var ui in GetComponentsInChildren<UI.InfoFade>())
            {
                if (ui.name.Contains("Left"))
                {
                    uiInfoFadeLeft = ui;
                }
                else if (ui.name.Contains("Right"))
                {
                    uiInfoFadeRight = ui;
                }
                else if (ui.name.Contains("Both"))
                {
                    uiInfoFadeBoth = ui;
                }
            }

            context = SynchronizationContext.Current;

            ShowDisableText();
        }

        /// <summary>
        /// Event that is called when gesture is detected
        /// </summary>
        /// <param name="sender">TofArHandManager</param>
        /// <param name="result">Gesture estimation result</param>
        void GestureEstimated(object sender, GestureResultProperty result)
        {
            var gestureIndex = result.gestureIndex;
            if (gestureIndex == GestureIndex.None ||
                gestureIndex == GestureIndex.Others)
            {
                return;
            }

            // Show
            context.Post((s) =>
            {
                string text = gestureIndex.ToString();
                if (result.gestureHand == GestureHand.LeftHand)
                {
                    uiInfoFadeLeft.InfoText = text;
                }
                else if (result.gestureHand == GestureHand.RightHand)
                {
                    uiInfoFadeRight.InfoText = text;
                }
                else if (result.gestureHand == GestureHand.BothHands)
                {
                    uiInfoFadeBoth.InfoText = text;
                }
            }, null);
        }

        /// <summary>
        /// Show when gesture is disabled
        /// </summary>
        void ShowDisableText()
        {
            if (gestureCtrl.OnOff)
            {
                return;
            }

            if (uiInfoFadeLeft && uiInfoFadeRight)
            {
                uiInfoFadeLeft.InfoText = "<color=red>Gesture is</color>"; ;
                uiInfoFadeRight.InfoText = "<color=red>disabled.</color>"; ;
                uiInfoFadeBoth.InfoText = "";
            }
        }
    }
}
