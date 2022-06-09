/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TofAr.V0.Hand;
using System.Threading;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class manages values of hand data in juggling scene.
    /// </summary>
    public class JugglingHandController : MonoBehaviour
    {
        private HandData handData;
        private RecognizeResultProperty recResult;

        [SerializeField]
        private JugglingHand rightHand, leftHand;

        [SerializeField]
        private HandModel handModelRight, handModelLeft;

        private int frameCount;
        private float handFrameRate = 15.0f;
        private float prevTime;

        private SynchronizationContext context;

        private void Awake()
        {
            context = SynchronizationContext.Current;
        }

        private void OnEnable()
        {
            //Add Listener
            TofArHandManager.OnFrameArrived += OnFrameArrived;
        }

        private void OnDisable()
        {
            //Remove Listener
            TofArHandManager.OnFrameArrived -= OnFrameArrived;
        }

        private void Update()
        {
            float time = Time.realtimeSinceStartup - prevTime;

            //measure framerate per 0.5 seconds
            if (time >= 0.5f)
            {
                handFrameRate = frameCount / time;
                frameCount = 0;
                prevTime = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// called when a TofArHandManager frame arrived that includes hand data.
        /// </summary>
        /// <param name="sender">TofArHandManager</param>
        private void OnFrameArrived(object sender)
        {
            frameCount++;

            var handManager = sender as TofArHandManager;

            if (handManager == null)
            {
                return;
            }

            handData = handManager.HandData;
            recResult = handManager.HandData.Data;

            GetCenterPosition(out Vector3 positionLeft, out Vector3 positionRight);

            context.Post((d) =>
            {
                leftHand.SetHandPos(positionLeft);
                rightHand.SetHandPos(positionRight);
            }, null);
            
        }

        /// <summary>
        /// returns current hand status.
        /// </summary>
        /// <returns></returns>
        public HandStatus GetHandStatus()
        {
            if (recResult == null)
            {
                return HandStatus.NoHand;
            }

            return recResult.handStatus;
        }

        /// <summary>
        /// gets coordinates of the center of hands.
        /// </summary>
        /// <param name="positionLeft"></param>
        /// <param name="positionRight"></param>
        private void GetCenterPosition(out Vector3 positionLeft, out Vector3 positionRight)
        {
            //initialize parameters
            positionLeft = Vector3.zero;
            positionRight = Vector3.zero;

            if (recResult == null)
            {
                return;
            }

            //Left Hand
            if (recResult.handStatus == HandStatus.BothHands
                || recResult.handStatus == HandStatus.LeftHand)
            {
                positionLeft = recResult.featurePointsLeft[(int)HandPointIndex.HandCenter];
            }

            //Right Hand
            if (recResult.handStatus == HandStatus.BothHands
                || recResult.handStatus == HandStatus.RightHand)
            {
                positionRight = recResult.featurePointsRight[(int)HandPointIndex.HandCenter];
            }
        }

        /// <summary>
        /// get distance between each hands and the camera.
        /// </summary>
        /// <param name="distanceLeft"></param>
        /// <param name="distanceRight"></param>
        public void GetDistance(out float distanceLeft, out float distanceRight)
        {
            //initialize parameters
            distanceLeft = 0f;
            distanceRight = 0f;

            if (recResult == null)
            {
                return;
            }

            //Left Hand
            if (recResult.handStatus == HandStatus.BothHands
                || recResult.handStatus == HandStatus.LeftHand)
            {
                distanceLeft = recResult.featurePointsLeft[(int)HandPointIndex.HandCenter].magnitude;
            }

            //Right Hand
            if (recResult.handStatus == HandStatus.BothHands
                || recResult.handStatus == HandStatus.RightHand)
            {
                distanceRight = recResult.featurePointsRight[(int)HandPointIndex.HandCenter].magnitude;
            }
        }

        /// <summary>
        /// get gestures of each hands.
        /// </summary>
        /// <param name="leftPose"></param>
        /// <param name="rightPose"></param>
        private void GetPoses(out PoseIndex leftPose, out PoseIndex rightPose)
        {
            leftPose = PoseIndex.None;
            rightPose = PoseIndex.None;

            if (handData == null)
            {
                return;
            }

            handData.GetPoseIndex(out leftPose, out rightPose);
        }

        /// <summary>
        /// checks if both hands is open.
        /// </summary>
        /// <returns></returns>
        public bool PalmHands()
        {
            GetPoses(out PoseIndex left, out PoseIndex right);
            return (left == PoseIndex.OpenPalm && right == PoseIndex.OpenPalm);
        }

        /// <summary>
        /// sets a threshold for the distance at which the ball can be caught.
        /// </summary>
        /// <param name="catchBallRange"></param>
        public void SetCatchBallRange(float catchBallRange)
        {
            rightHand.SetCatchBallRange(catchBallRange);
            leftHand.SetCatchBallRange(catchBallRange);
        }

        /// <summary>
        /// sets the threshold at which the ball can be thrown.
        /// </summary>
        /// <param name="throwRange"></param>
        public void SetThrowRange(float throwRange)
        {
            rightHand.SetThrowRange(throwRange);
            leftHand.SetThrowRange(throwRange);
        }

        /// <summary>
        /// returns how much hand frames arrived in a second.
        /// </summary>
        /// <returns></returns>
        public float GetHandFrameRate()
        {
            return handFrameRate;
        }

        /// <summary>
        /// sets the HandModel's visibility.
        /// </summary>
        /// <param name="enable"></param>
        public void SetVisible(bool enable)
        {
            handModelRight.gameObject.SetActive(enable);
            handModelLeft.gameObject.SetActive(enable);
        }
    }
}
