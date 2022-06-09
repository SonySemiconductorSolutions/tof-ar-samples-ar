/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using TofAr.V0.Face;

namespace TofArSettings.Face
{
    public class FaceRuntimeController : ControllerBase
    {
        FaceEstimator faceEstimator;

        protected void Awake()
        {
            faceEstimator = FindObjectOfType<FaceEstimator>();
        }

        int detectorTypeIndex;
        public int DetectorTypeIndex
        {
            get { return detectorTypeIndex; }
            set
            {
                if (value != detectorTypeIndex && 0 <= value && value < DetectorTypeList.Length)
                {
                    detectorTypeIndex = value;
                    if (value > 0)
                    {
                        DetectorType = DetectorTypeList[value];
                    }
                    else
                    {
                        OnChangeDetectorType.Invoke(detectorTypeIndex);
                    }
                }
            }
        }

        public FaceDetectorType DetectorType
        {
            get
            {
                return TofArFaceManager.Instance.DetectorType;
            }

            private set
            {
                detectorTypeIndex = Utils.Find(value, DetectorTypeList, 1);
                faceEstimator.enabled = value == FaceDetectorType.External;
                TofArFaceManager.Instance.DetectorType = value;

                if (TofArFaceManager.Instance.DetectorType == value)
                {
                    OnChangeDetectorType?.Invoke(DetectorTypeIndex);
                }
            }
        }

        public FaceDetectorType[] DetectorTypeList { get; private set; }

        public string[] DetectorTypeNames { get; private set; }

        void OnEnable()
        {
            TofArFaceManager.OnStreamStarted += OnStreamStarted;
        }

        void OnDisable()
        {
            TofArFaceManager.OnStreamStarted -= OnStreamStarted;
        }

        protected override void Start()
        {
            // Get NNL list
            var types = ((FaceDetectorType[])Enum.GetValues(typeof(FaceDetectorType))).ToList();


            var defaultFaceDetector = TofAr.V0.TofArManager.Instance.UsingIos ? FaceDetectorType.Internal_ARKit : FaceDetectorType.External;

            var propTexts = new List<string>();
            int defaultIndex = 0;
            for (int i = 0; i < types.Count; i++)
            {
                var prop = types[i];
                string text = types[i] == FaceDetectorType.External ? "TFLite Model" :
                    types[i] == FaceDetectorType.Internal_ARKit ? "ARKit" :
                    types[i].ToString();

                // Use recommended values for initial values
                if (prop == defaultFaceDetector)
                {
                    defaultIndex = i;
                }

                propTexts.Add(text);

            }

            // Highlight recommended values and move to the top of the list
            string defaultText = $"<color=red>{propTexts[defaultIndex]}</color>";
            types.RemoveAt(defaultIndex);
            propTexts.RemoveAt(defaultIndex);
            types.Insert(0, defaultFaceDetector);
            propTexts.Insert(0, defaultText);

            // Add empty option at the top
            types.Insert(0, (FaceDetectorType)((int)FaceDetectorType.External + 1));
            propTexts.Insert(0, "-");

            DetectorTypeList = types.ToArray();
            DetectorTypeNames = propTexts.ToArray();

            if (DetectorTypeIndex <= 0)
            {
                DetectorTypeIndex = 1;
            }

            base.Start();
        }

        /// <summary>
        /// Event that is called when Body stream is started
        /// </summary>
        /// <param name="sender">TofArBodyManager</param>
        void OnStreamStarted(object sender)
        {
            DetectorTypeIndex = Utils.Find(DetectorType, DetectorTypeList, 1);
        }

        /// <summary>
        /// Event that is called when detector type is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeDetectorType;
    }
}
