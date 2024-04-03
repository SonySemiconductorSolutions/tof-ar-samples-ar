/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TofAr.V0.Hand;
using TofAr.V0;
using UnityEditor;
using System.Collections.Generic;

namespace TofArSettings.Hand
{
    public class HandPoseInfo : MonoBehaviour
    {

        private AudioSource source;
        [SerializeField]
        private AudioClip gesture;
        [SerializeField]
        private HandPanel leftHand;
        [SerializeField]
        private HandPanel rightHand;
        [SerializeField]
        private GesturePanel leftGesture;
        [SerializeField]
        private GesturePanel rightGesture;
        [SerializeField]
        private GesturePanel bothGesture;
        [SerializeField]
        private Canvas canvas;

        public int PoseFrame = 3;
        [CustomType(typeof(GestureIndex))]
        public bool[] GestureNotify;

        private Dictionary<int,int> gestureNotifyMap;

        public bool ShowGesture;
        public bool ShowPose
        {
            set
            {
                leftHand.gameObject.SetActive(value);
                rightHand.gameObject.SetActive(value);
            }
        }

        [SerializeField]
        private Sprite[] poseImages;

        private PoseIndex leftPose;
        private PoseIndex rightPose;

        private PoseIndex[] leftPoses;
        private PoseIndex[] rightPoses;
        private bool showLeft = false;
        private bool showRight = false;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            leftPoses = new PoseIndex[PoseFrame];
            rightPoses = new PoseIndex[PoseFrame];

            if (canvas == null)
            {
                var transformTmp = this.transform;
                while (transformTmp != null)
                {
                    // Find parent canvas
                    var canvasTmp = transformTmp.GetComponentInParent<Canvas>();
                    if (canvasTmp != null)
                    {
                        canvas = canvasTmp;
                    }
                    transformTmp = transformTmp.parent;
                }
                
            }

            gestureNotifyMap = new Dictionary<int, int>();
            int key = 0;
            foreach (var gestureIndex in Enum.GetValues(typeof(GestureIndex)))
            {
                int idx = (int)gestureIndex;
                gestureNotifyMap.Add(idx, key++);
            }
        }

        private void OnEnable()
        {
            TofArHandManager.OnGestureEstimated += OnGestureEstimated;
            TofArHandManager.OnFrameArrived += OnFrameArrived;
        }

        private void OnDisable()
        {
            TofArHandManager.OnGestureEstimated -= OnGestureEstimated;
            TofArHandManager.OnFrameArrived -= OnFrameArrived;
        }


        void Update()
        {
            if (showLeft)
            {
                showLeft = false;
                leftHand.SetPose(leftPose, FindPoseImage(leftPose));
            }

            if (showRight)
            {
                showRight = false;
                rightHand.SetPose(rightPose, FindPoseImage(rightPose));
            }
        }

        // Update is called once per frame
        void OnFrameArrived(object sender)
        {
            if (TofArHandManager.Instance.HandData != null)
            {
                TofArHandManager.Instance.HandData.GetPoseIndex(out leftPose, out rightPose);
            }
            else
            {
                leftPose = PoseIndex.None;
                rightPose = PoseIndex.None;
            }

            if (leftPoses.Length != PoseFrame)
            {
                Array.Resize(ref leftPoses, PoseFrame);
            }
            if (rightPoses.Length != PoseFrame)
            {
                Array.Resize(ref rightPoses, PoseFrame);
            }

            for (int i = 0; i < PoseFrame - 1; i++)
            {
                leftPoses[i] = leftPoses[i + 1];
                rightPoses[i] = rightPoses[i + 1];
            }
            leftPoses[PoseFrame - 1] = leftPose;
            rightPoses[PoseFrame - 1] = rightPose;

            if (leftPoses.Where((p) => p == leftPose).Count() == PoseFrame)
            {
                showLeft = true;
            }

            if (rightPoses.Where((p) => p == rightPose).Count() == PoseFrame)
            {
                showRight = true;
            }
        }

        private Sprite FindPoseImage(PoseIndex pose)
        {
            if (pose == PoseIndex.None) 
            { 
                return null; 
            }
            if ((int)pose >= poseImages.Length) 
            { 
                return null; 
            }
            return poseImages[(int)pose];
        }


        private void OnGestureEstimated(object sender, GestureResultProperty result)
        {
            if (!ShowGesture) 
            { 
                return; 
            }

            int gestureIndex = (int)result.gestureIndex;
            int mappedIndex = gestureNotifyMap[gestureIndex];
            if (GestureNotify.Length < mappedIndex || !GestureNotify[mappedIndex])
            {
                return;
            }

            if (result.gestureHand == GestureHand.RightHand)
            {
                rightGesture.SetGesture(
                    result.gestureIndex,
                    AnchordPositionFor(TofArHandManager.Instance.HandData?.Data.featurePointsRight[(int)HandPointIndex.HandCenter])
                );
                source.PlayOneShot(gesture);
            }
            else if (result.gestureHand == GestureHand.LeftHand)
            {
                leftGesture.SetGesture(
                    result.gestureIndex,
                    AnchordPositionFor(TofArHandManager.Instance.HandData?.Data.featurePointsLeft[(int)HandPointIndex.HandCenter])
                );
                source.PlayOneShot(gesture);
            }
            else if (result.gestureHand == GestureHand.BothHands)
            {
                var pointRight = TofArHandManager.Instance.HandData?.Data.featurePointsRight[(int)HandPointIndex.HandCenter];
                var pointLeft = TofArHandManager.Instance.HandData?.Data.featurePointsLeft[(int)HandPointIndex.HandCenter];
                var anchordPosition = AnchordPositionFor(pointLeft, pointRight);

                bothGesture.SetGesture(result.gestureIndex, anchordPosition);

                source.PlayOneShot(gesture);
            }
        }

        private Vector2 AnchordPositionFor(Vector3? position)
        {
            if (canvas != null)
            {
                var viewport = position != null ? Camera.main.WorldToViewportPoint(position.Value) : Vector3.zero;
                var canvasRectTransform = canvas.GetComponent<RectTransform>();
                var x = viewport.x * canvasRectTransform.sizeDelta.x - canvasRectTransform.sizeDelta.x / 2;
                var y = viewport.y * canvasRectTransform.sizeDelta.y - canvasRectTransform.sizeDelta.y / 2;

                if (TofArManager.Instance.GetScreenOrientation() == 0)
                {
                    return new Vector2(x, y);
                }
                else if (TofArManager.Instance.GetScreenOrientation() == 90)
                {
                    return new Vector2(-y, x);
                }
                else if (TofArManager.Instance.GetScreenOrientation() == 180)
                {
                    return new Vector2(-x, -y);
                }
                else if (TofArManager.Instance.GetScreenOrientation() == 270)
                {
                    return new Vector2(y, -x);
                }
            }
            
            return Vector2.zero;
            
        }

        private Vector2 AnchordPositionFor(Vector3? positionLeft, Vector3? positionRight)
        {
            Vector2 left = AnchordPositionFor(positionLeft);
            Vector2 right = AnchordPositionFor(positionRight);

            return (left + right) / 2;
        }

        public class CustomTypeAttribute : PropertyAttribute 
        {
            public Type customType;

            public CustomTypeAttribute(Type type)
            {
                this.customType = type;
            }
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(CustomTypeAttribute))]
        public class CustomTypePropertyDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
            }
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var attr = attribute as CustomTypeAttribute;
                if (attr != null)
                {
                    var enumNames = Enum.GetNames(attr.customType);
                    if (int.TryParse(property.propertyPath.Split('[', ']')[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out int idx))
                    {
                        label = new GUIContent(enumNames[idx]);
                    }
                }
                
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
            }
        }
#endif


    }

}

