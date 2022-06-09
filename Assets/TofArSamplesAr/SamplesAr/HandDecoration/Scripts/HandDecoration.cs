/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using TofAr.V0.Hand;
using TofArSettings.Hand;
using TofAr.V0;

namespace TofArARSamples.HandDecoration
{
    /// <summary>
    /// Hand decoration management class
    /// </summary>
    public class HandDecoration : MonoBehaviour
    {
        [SerializeField]
        private Transform handCenterAnchorR = null;

        [SerializeField]
        private Transform handCenterAnchorL = null;

        [SerializeField]
        private GameObject decorationR = null;

        [SerializeField]
        private GameObject decorationL = null;

        [SerializeField]
        private Sprite[] designSprites = null;

        [SerializeField]
        private Sprite[] designBlurSprites = null;

        private SpriteRenderer designR = null;

        private SpriteRenderer designL = null;

        private SpriteRenderer designBlurR = null;

        private SpriteRenderer designBlurL = null;
        
        private float countup = -2.0f;

        private RecogModeController recogModeController;

        private GestureController gestureController;

        private RecognizeResultProperty recognizeResultProperty;

        private const float basicScale = 0.0075f;

        private ScreenOrientation currentScreenOrientation;

        void Awake()
        {
            recogModeController = FindObjectOfType<RecogModeController>();
            gestureController = FindObjectOfType<GestureController>();
            designR = decorationR.transform.Find("Design").GetComponent<SpriteRenderer>();
            designL = decorationL.transform.Find("Design").GetComponent<SpriteRenderer>();
            designBlurR = decorationR.transform.Find("Design_Blur").GetComponent<SpriteRenderer>();
            designBlurL = decorationL.transform.Find("Design_Blur").GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// This function is called when the object is enabled / activated
        /// </summary>
        void OnEnable()
        {
            TofArHandManager.OnStreamStarted += OnStreamStarted;
            TofArHandManager.OnFrameArrived += HandFrameArrived;
            TofArManager.OnScreenOrientationUpdated += OnScreenOrientationUpdated;

            var orientationProperty = TofArManager.Instance.GetProperty<DeviceOrientationsProperty>();
            if (orientationProperty != null)
            { 
                OnScreenOrientationUpdated(ScreenOrientation.AutoRotation, orientationProperty.screenOrientation);
            } 
        }

        void OnDisable()
        {
            TofArHandManager.OnStreamStarted -= OnStreamStarted;
            TofArHandManager.OnFrameArrived -= HandFrameArrived;
            TofArManager.OnScreenOrientationUpdated -= OnScreenOrientationUpdated;
        }

        private void OnScreenOrientationUpdated(ScreenOrientation previousScreenOrientation, ScreenOrientation newScreenOrientation)
        {
            currentScreenOrientation = newScreenOrientation;
        }


        /// <summary>
        /// Event called at the start of the stream
        /// </summary>
        /// <param name="sender"></param>
        void OnStreamStarted(object sender)
        {
            gestureController.OnOff = true;
        }

        /// <summary>
        /// Event called when Hand data is updated
        /// </summary>
        /// <param name="sender">TofArHandManager</param>
        private void HandFrameArrived(object sender)
        {
            var manager = sender as TofArHandManager;
            if (manager == null)
            {
                return;
            }

            if (manager.ProcessLevel == ProcessLevel.HandCenterOnly)
            {
                recognizeResultProperty = null;
                return;
            }

            recognizeResultProperty = manager.HandData.Data;
        }

        void Update()
        {
            if (recognizeResultProperty == null)
            {
                decorationR.SetActive(false);
                decorationL.SetActive(false);
            }
            else
            {
                switch (recognizeResultProperty.handStatus)
                {
                    case HandStatus.RightHand:
                        Decoration(true);
                        decorationL.SetActive(false);
                        break;
                    case HandStatus.LeftHand:
                        decorationR.SetActive(false);
                        Decoration(false);
                        break;
                    case HandStatus.BothHands:
                        Decoration(true);
                        Decoration(false);
                        break;
                    default:
                        decorationR.SetActive(false);
                        decorationL.SetActive(false);
                        break;
                }

                countup += Time.deltaTime;
                if (countup >= 2.0f)
                {
                    countup = -2.0f;
                    designBlurR.transform.localScale = new Vector3(basicScale, basicScale, 0);
                    designBlurL.transform.localScale = new Vector3(basicScale, basicScale, 0);
                }
                else
                {
                    float fluctuationScale = basicScale * 0.15f;
                    designBlurR.transform.localScale = new Vector3(basicScale + fluctuationScale * Mathf.Abs(countup) / 2, basicScale + fluctuationScale * Mathf.Abs(countup) / 2, 0);
                    designBlurL.transform.localScale = new Vector3(basicScale + fluctuationScale * Mathf.Abs(countup) / 2, basicScale + fluctuationScale * Mathf.Abs(countup) / 2, 0);
                    designBlurR.color = new Color(designBlurR.color.r, designBlurR.color.g, designBlurR.color.b, 0.5f + Mathf.Abs(countup) / 4);
                    designBlurL.color = new Color(designBlurL.color.r, designBlurL.color.g, designBlurL.color.b, 0.5f + Mathf.Abs(countup) / 4);
                }
            }
        }

        /// <summary>
        /// Display decorations
        /// </summary>
        /// <param name="rightHand">rightHand</param>
        private void Decoration(bool rightHand)
        {
            Vector3[] featurePoints;
            GameObject decoration;
            Transform anchor;

            if (rightHand)
            {
                featurePoints = recognizeResultProperty.featurePointsRight;
                decoration = decorationR;
                anchor = handCenterAnchorR;
            }
            else
            {
                featurePoints = recognizeResultProperty.featurePointsLeft;
                decoration = decorationL;
                anchor = handCenterAnchorL;
            }

            Vector3 handCenter = featurePoints[13];
            Vector3 pinkyRoot = featurePoints[15];
            Vector3 midRoot = featurePoints[17];
            Vector3 indexRoot = featurePoints[18];

            if ((rightHand == true && ((handCenter.x > pinkyRoot.x && handCenter.y < midRoot.y) || (handCenter.x < pinkyRoot.x && handCenter.y > midRoot.y)))
                ||
                (rightHand == false && ((handCenter.x < pinkyRoot.x && handCenter.y < midRoot.y) || (handCenter.x > pinkyRoot.x && handCenter.y > midRoot.y))))
            {
                decoration.SetActive(false);
            }
            else
            {
                Vector3 unitVectorA = (indexRoot - handCenter).normalized;
                Vector3 unitVectorB = (midRoot - handCenter).normalized;
                Vector3 crossProduct = Vector3.Cross(unitVectorA, unitVectorB);
                decoration.transform.localPosition = handCenter;
                anchor.position = handCenter;
                anchor.LookAt(midRoot);
                decoration.transform.localRotation = Quaternion.LookRotation(crossProduct, anchor.transform.forward);
                decoration.SetActive(true);
            }
        }


        public void SettingDesign(int index)
        {
            designR.sprite = designSprites[index];
            designL.sprite = designSprites[index];
            designBlurR.sprite = designBlurSprites[index];
            designBlurL.sprite = designBlurSprites[index];
        }
    }
}
