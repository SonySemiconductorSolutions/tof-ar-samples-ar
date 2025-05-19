/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class FingerTouchTargetController : ControllerBase
    {
        /// <summary>
        /// Event that is called when Target Finger is changed
        /// </summary>
        /// <param name="targetFinger">Target Finger</param>
        public delegate void ChangeTargetFingersEvent(
            FingerTouchDetector.TargetFinger targetFinger);

        public event ChangeTargetFingersEvent OnChangeTargetFingers;

        FingerTouchDetector detector;

        protected override void Start()
        {
            detector = FindAnyObjectByType<FingerTouchDetector>();
            base.Start();
        }

        /// <summary>
        /// Get Target Finger status
        /// </summary>
        /// <param name="side">Hand direction</param>
        /// <param name="finger">Finger</param>
        /// <returns>If Target Finger is set or not</returns>
        public bool GetTargetFinger(FingerTouchDetector.HandSide side,
            HandPointIndex finger)
        {
            var target = GetTargetIndex(side, finger);
            return GetTargetFinger(target);
        }

        /// <summary>
        /// Get Target Finger status
        /// </summary>
        /// <param name="target">Target Finger identifier</param>
        /// <returns>If Target Finger is set or not</returns>
        public bool GetTargetFinger(FingerTouchDetector.TargetFinger target)
        {
            return detector.targetFingers.HasFlag(target);
        }

        /// <summary>
        /// Set Target Finger
        /// </summary>
        /// <param name="side">Hand direction</param>
        /// <param name="finger">Finger</param>
        /// <param name="onOff">Designate/undesignate</param>
        public void SetTargetFinger(FingerTouchDetector.HandSide side,
            HandPointIndex finger, bool onOff)
        {
            var target = GetTargetIndex(side, finger);
            SetTargetFinger(target, onOff);
        }

        /// <summary>
        /// Set Target Finger
        /// </summary>
        /// <param name="target">Target Finger identifier</param>
        /// <param name="onOff">Designate/undesignate</param>
        public void SetTargetFinger(FingerTouchDetector.TargetFinger target, bool onOff)
        {
            bool state = GetTargetFinger(target);
            if (state == onOff)
            {
                return;
            }

            if (onOff)
            {
                detector.targetFingers |= target;
            }
            else
            {
                detector.targetFingers &= ~target;
            }

            OnChangeTargetFingers?.Invoke(detector.targetFingers);
        }

        /// <summary>
        /// Get Target Finger identifier
        /// </summary>
        /// <param name="side">Hand direction</param>
        /// <param name="finger">Finger</param>
        /// <returns>Target Finger identifier</returns>
        FingerTouchDetector.TargetFinger GetTargetIndex(
            FingerTouchDetector.HandSide side, HandPointIndex finger)
        {
            var result = FingerTouchDetector.TargetFinger.None;
            if (side == FingerTouchDetector.HandSide.LeftHand)
            {
                switch (finger)
                {
                    case HandPointIndex.ThumbTip:
                        result = FingerTouchDetector.TargetFinger.LeftThumb;
                        break;
                    case HandPointIndex.IndexTip:
                        result = FingerTouchDetector.TargetFinger.LeftIndex;
                        break;
                    case HandPointIndex.MidTip:
                        result = FingerTouchDetector.TargetFinger.LeftMiddle;
                        break;
                    case HandPointIndex.RingTip:
                        result = FingerTouchDetector.TargetFinger.LeftRing;
                        break;
                    case HandPointIndex.PinkyTip:
                        result = FingerTouchDetector.TargetFinger.LeftPinky;
                        break;
                }
            }
            else
            {
                switch (finger)
                {
                    case HandPointIndex.ThumbTip:
                        result = FingerTouchDetector.TargetFinger.RightThumb;
                        break;
                    case HandPointIndex.IndexTip:
                        result = FingerTouchDetector.TargetFinger.RightIndex;
                        break;
                    case HandPointIndex.MidTip:
                        result = FingerTouchDetector.TargetFinger.RightMiddle;
                        break;
                    case HandPointIndex.RingTip:
                        result = FingerTouchDetector.TargetFinger.RightRing;
                        break;
                    case HandPointIndex.PinkyTip:
                        result = FingerTouchDetector.TargetFinger.RightPinky;
                        break;
                }
            }

            return result;
        }
    }
}
