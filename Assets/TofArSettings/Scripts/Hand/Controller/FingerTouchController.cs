/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using UnityEngine;

namespace TofArSettings.Hand
{
    public class FingerTouchController : ControllerBase
    {
        bool onOff = false;
        public bool OnOff
        {
            get { return onOff; }
            set
            {
                if (onOff != value)
                {
                    onOff = value;
                    if (value)
                    {
                        detector.StartEstimation();
                    }
                    else
                    {
                        detector.StopEstimation();
                    }

                    OnChangeEnable?.Invoke(OnOff);
                }
            }
        }

        public static HandPointIndex[] Fingers = new HandPointIndex[]
        {
            HandPointIndex.ThumbTip,
            HandPointIndex.IndexTip,
            HandPointIndex.MidTip,
            HandPointIndex.RingTip,
            HandPointIndex.PinkyTip
        };

        public bool[] TouchStates { get; private set; } = new bool[10];

        /// <summary>
        /// Event that is called when touch status of changed
        /// </summary>
        /// <param name="touchStates">Touch status</param>
        public delegate void ChangeTouchStatesEvent(bool[] touchStates);

        public event ChangeToggleEvent OnChangeEnable;

        public event ChangeTouchStatesEvent OnChangeTouchStates;

        FingerTouchDetector detector;

        protected void Awake()
        {
            detector = FindAnyObjectByType<FingerTouchDetector>(FindObjectsInactive.Include);
        }

        void OnEnable()
        {
            detector.OnFingerTouchDetected.AddListener(OnFingerTouchDetected);
        }

        void OnDisable()
        {
            detector.OnFingerTouchDetected.RemoveListener(OnFingerTouchDetected);
        }

        /// <summary>
        /// Event that is called when finger touch is detected
        /// </summary>
        /// <param name="result">Detection result</param>
        void OnFingerTouchDetected(FingerTouchDetector.DetectResult result)
        {
            bool update = false;
            foreach (FingerTouchDetector.HandSide side in result.Result.Keys)
            {
                foreach (HandPointIndex finger in result.Result[side].Keys)
                {
                    int fingerIndex = GetFingerIndex(finger);
                    if (fingerIndex < 0)
                    {
                        continue;
                    }

                    bool state = (result.Result[side][finger] ==
                        FingerTouchDetector.TouchState.Touch);

                    int sideIndex = (side == FingerTouchDetector.HandSide.LeftHand) ?
                        0 : 1;
                    int index = 5 * sideIndex + fingerIndex;
                    if (TouchStates[index] != state)
                    {
                        update = true;
                    }

                    TouchStates[index] = state;
                }
            }

            if (update)
            {
                OnChangeTouchStates?.Invoke(TouchStates);
            }
        }

        /// <summary>
        /// Get finger identification number
        /// </summary>
        /// <param name="finger">Finger</param>
        /// <returns>Finger identification number</returns>
        int GetFingerIndex(HandPointIndex finger)
        {
            int result = -1;
            for (int i = 0; i < Fingers.Length; i++)
            {
                if (Fingers[i] == finger)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }
    }
}
