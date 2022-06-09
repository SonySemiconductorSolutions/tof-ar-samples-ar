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
using TofAr.V0.Hand;
using TofAr.V0.Face;
using System.Text;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class displays the information of juggling scene.
    /// </summary>
    public class JugglingInformationText : MonoBehaviour
    {
        //controller
        [SerializeField]
        private JugglingAppController appController;

        [SerializeField]
        private JugglingFaceController faceController;

        [SerializeField]
        private JugglingHandController handController;

        [SerializeField]
        private JugglingHand leftHand, rightHand;

        [SerializeField]
        private RectTransform jugglingInfo_parent;

        [SerializeField]
        private Text text;

        [Header("Show Information Text")]
        [SerializeField]
        private bool showText;

        private float throwPower;
        private float handFrameRate;
        private float faceFrameRate;

        private void OnEnable()
        {
            leftHand.OnBallThrew += OnBallThrew;
            rightHand.OnBallThrew += OnBallThrew;
        }

        private void OnDisable()
        {
            leftHand.OnBallThrew -= OnBallThrew;
            rightHand.OnBallThrew -= OnBallThrew;
        }

        private void Start()
        {
            ReloadText();
        }

        private void LateUpdate()
        {
            ReloadText();
        }

        /// <summary>
        /// called when the ball is thrown.
        /// </summary>
        /// <param name="throwPower">magnitude when the player threw the ball</param>
        private void OnBallThrew(float throwPower)
        {
            this.throwPower = throwPower;
        }

        /// <summary>
        /// displays the information in this scene as text.
        /// </summary>
        private void ReloadText()
        {
            jugglingInfo_parent.gameObject.SetActive(showText);

            if (!showText)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();

            HandStatus handStatus = handController.GetHandStatus();
            handController.GetDistance(out float distanceLeft, out float distanceRight);
            handFrameRate = handController.GetHandFrameRate();

#if UNITY_IOS
            TrackingState faceStatus = faceController.GetTrackingState();
            faceController.GetDistance(out float distanceFace);
            faceFrameRate = faceController.GetFaceFrameRate();
#endif

            //frame rate
            sb.AppendFormat("Hand:{0, -4:0.00}fps", handFrameRate);
            sb.AppendLine();

#if UNITY_IOS
            sb.AppendFormat("Face:{0, -4:0.00}fps", faceFrameRate);
            sb.AppendLine();
#endif

            //juggling mode
            sb.AppendFormat("Juggling Mode:{0}", appController.GetCurrentMode());
            sb.AppendLine();

            //hand status
            sb.AppendFormat("Hand Status:{0}", handStatus);
            sb.AppendLine();

#if UNITY_IOS
            //face status
            sb.AppendFormat("Face Status:{0}", faceStatus);
            sb.AppendLine();
#endif

            //distance
            sb.AppendFormat("Distance(L):{0, -4:0.00}m", distanceLeft);
            sb.AppendLine();
            sb.AppendFormat("Distance(R):{0, -4:0.00}m", distanceRight);
            sb.AppendLine();

#if UNITY_IOS
            sb.AppendFormat("Distance(Face):{0, -4:0.00}m", distanceFace);
            sb.AppendLine();
#endif
            //balls
            sb.AppendFormat("Left Balls:{0}", leftHand.GetBallsCount());
            sb.AppendLine();
            sb.AppendFormat("Right Balls:{0}", rightHand.GetBallsCount());
            sb.AppendLine();

            //throw power
            sb.AppendFormat("Throw Power:{0, -4:0.00}", throwPower);

            text.text = sb.ToString();
        }

        /// <summary>
        /// displays or hides text.
        /// </summary>
        /// <param name="showText"></param>
        public void ShowText(bool showText)
        {
            this.showText = showText;
        }

        /// <summary>
        /// returns whether the text is visible or not.
        /// </summary>
        /// <returns></returns>
        public bool IsTextShowing()
        {
            return showText;
        }
    }
}
