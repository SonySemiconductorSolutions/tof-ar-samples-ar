/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using System.Text;
using System.Threading;
using TofAr.V0.Hand;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Hand
{
    public class StatusHistory : MonoBehaviour
    {
        /// <summary>
        /// Max display count
        /// </summary>
        public int MaxFrames = 30;

        /// <summary>
        /// Line height
        /// </summary>
        public float LineHeight { get; private set; }

        const string colorStyleStart = "<color=red>";
        const string colorStyleEnd = "</color>";

        List<string> statusTexts = new List<string>();
        StringBuilder sb = new StringBuilder();

        Text txt;
        SynchronizationContext context;

        GestureController gestureCtrl;

        void Awake()
        {
            txt = GetComponent<Text>();
            LineHeight = txt.preferredHeight;

            // Due to the possibility of event occuring before Start, get in Awake
            context = SynchronizationContext.Current;

            gestureCtrl = FindAnyObjectByType<GestureController>();
        }

        void OnEnable()
        {
            TofArHandManager.OnGestureEstimatedDefault += OnGestureEstimatedDefault;
            if (gestureCtrl)
            {
                gestureCtrl.OnChangeEnable += ShowDisableText;
            }
        }

        void OnDisable()
        {
            TofArHandManager.OnGestureEstimatedDefault -= OnGestureEstimatedDefault;
            if (gestureCtrl)
            {
                gestureCtrl.OnChangeEnable -= ShowDisableText;
            }
        }


        void Start()
        {
            ShowDisableText(gestureCtrl.OnOff);
        }

        /// <summary>
        /// Event that is called when Gesture information is obtained
        /// </summary>
        /// <param name="sender">TofArHandManager</param>
        /// <param name="result">Gesture recognition result</param>
        void OnGestureEstimatedDefault(object sender, GestureResultProperty result)
        {
            // Convert result to string
            string hand = (result.gestureHand == GestureHand.Unknown) ?
                "NoHand" : string.Empty;
            string gesture = string.Empty;
            string callback = string.Empty;
            string interpolated = string.Empty;
            if (result.gestureHand != GestureHand.Unknown)
            {
                string handType = string.Empty;
                if (result.gestureHand == GestureHand.LeftHand)
                {
                    handType = "Left";
                }
                else if (result.gestureHand == GestureHand.RightHand)
                {
                    handType = "Right";
                }
                else if (result.gestureHand == GestureHand.BothHands)
                {
                    handType = "Both";
                }

                gesture = $"{result.gestureIndex} ({handType})";
                callback = (result.isCallback) ? "[Callback]" : string.Empty;
                interpolated = (result.wasInterpolated) ? "[NoHand]" : string.Empty;
            }

            string text = $"{hand}{gesture}{callback} {interpolated}";

            // Emphasize when gesture is recognized
            text = (result.gestureIndex != GestureIndex.None &&
                result.gestureIndex != GestureIndex.Others) ?
                $"{colorStyleStart}{text}{colorStyleEnd}\n" : $"{text}\n";

            statusTexts.Add(text);
            if (statusTexts.Count > MaxFrames)
            {
                statusTexts.RemoveAt(0);
            }

            // Show
            context.Post((s) =>
            {
                for (int i = 0; i < statusTexts.Count; i++)
                {
                    sb.Append($"{i + 1:D2}: {statusTexts[i]}");
                }

                txt.text = sb.ToString();
                sb.Clear();
            }, null);
        }

        /// <summary>
        /// Show when gesture is disabled
        /// </summary>
        /// <param name="onOff">Enabled/Disabled</param>
        void ShowDisableText(bool onOff)
        {
            if (txt && !onOff)
            {
                txt.text = "Gesture is disabled.";
            }
        }
    }
}
