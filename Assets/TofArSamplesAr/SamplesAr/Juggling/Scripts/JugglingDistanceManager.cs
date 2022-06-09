/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TofAr.V0.Face;
using TofAr.V0.Hand;
using TofArSettings.Color;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class manages the distance between hand or face and the camera in juggling scene.
    /// </summary>
    public class JugglingDistanceManager : MonoBehaviour
    {
        [SerializeField]
        private JugglingFaceController faceController;

        private JugglingHandController handController;

        private ColorManagerController colorController;

        //shortest possible distance to start juggling
        [Range(0.0f, 0.7f)]
        [SerializeField]
        private float minRange = 0.6f;

        //longest possible distance to start juggling
        [Range(0.7f, 1.2f)]
        [SerializeField]
        private float maxRange = 0.9f;

        private void Start()
        {
#if UNITY_IOS
            if (faceController == null)
            {
                faceController = FindObjectOfType<JugglingFaceController>();
            }

            if (colorController == null) 
            {
                colorController = FindObjectOfType<ColorManagerController>();
            }
#endif
            if (handController == null)
            {
                handController = FindObjectOfType<JugglingHandController>();
            }
        }

        /// <summary>
        /// checks the distance if the juggling is available.
        /// </summary>
        /// <returns></returns>
        public bool IsOk()
        {
#if UNITY_IOS

            bool faceInDistance = true;

            if (colorController.CurrentResolution.lensFacing == 0)
            {
                if (faceController.GetTrackingState() != TrackingState.Tracking)
                {
                    return false;
                }

                faceController.GetDistance(out float distanceFace);

                faceInDistance = IsInRange(distanceFace);
            }
            
#endif

            if (handController.GetHandStatus() != HandStatus.BothHands)
            {
                return false;
            }

            bool isOk = false;

            handController.GetDistance(out float distanceHandLeft, out float distanceHandRight);

#if UNITY_IOS
            if (faceInDistance && IsInRange(distanceHandLeft) && IsInRange(distanceHandRight))
            {
                isOk = true;
            }
#else
            if (IsInRange(distanceHandLeft) && IsInRange(distanceHandRight))
            {
                isOk = true;
            }
#endif

            return isOk;
        }

        /// <summary>
        /// checks that the value is within the range.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool IsInRange(float param)
        {
            bool isInRange = false;

            if (minRange <= param && param <= maxRange)
            {
                isInRange = true;
            }

            return isInRange;
        }

        /// <summary>
        /// returns how much more or less distance is needed to start.
        /// </summary>
        /// <returns></returns>
        public void GetRequiredDistance(out float requiredFace, out float requiredHandLeft, out float requiredHandRight)
        {
            requiredFace = 0f;
            requiredHandLeft = 0f;
            requiredHandRight = 0f;

#if UNITY_IOS
            faceController.GetDistance(out float distanceFace);

            if (distanceFace > 0)
            {
                requiredFace = CalcRequiredDistance(distanceFace);
            }
#endif
            handController.GetDistance(out float distanceHandLeft, out float distanceHandRight);

            if (distanceHandLeft > 0)
            {
                requiredHandLeft = CalcRequiredDistance(distanceHandLeft);
            }

            if (distanceHandRight > 0)
            {
                requiredHandRight = CalcRequiredDistance(distanceHandRight);
            }
        }

        /// <summary>
        /// calculates how far or close the value is from the threshold.
        /// </summary>
        /// <param name="val">current value</param>
        /// <returns>distance needed</returns>
        private float CalcRequiredDistance(float val)
        {
            float requied = 0f;

            //farther than the longest distance
            if (val > maxRange)
            {
                requied = maxRange - val;
            }

            //closer than the shortest distance
            if (val < minRange)
            {
                requied = minRange - val;
            }

            //returns 100 when the value is within range
            if (val != 0f && IsInRange(val))
            {
                requied = 100f;
            }

            return requied;
        }
    }
}
