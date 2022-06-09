/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections;
using TofAr.V0.Body;
using TofAr.V0;
using System.Linq;

namespace TofArSettings.Body
{
    public class BodyRuntimeController : ControllerBase
    {
        TofAr.V0.Body.SV1.SV1BodyPoseEstimator sv1Estimator;

        protected void Awake()
        {
            sv1Estimator = FindObjectOfType<TofAr.V0.Body.SV1.SV1BodyPoseEstimator>();
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

        public BodyPoseDetectorType DetectorType
        {
            get
            {
                return TofArBodyManager.Instance.DetectorType;
            }

            private set
            {
                detectorTypeIndex = Utils.Find(value, DetectorTypeList, 1);
                sv1Estimator.enabled = value == BodyPoseDetectorType.External;
                TofArBodyManager.Instance.DetectorType = value;

                OnChangeDetectorType?.Invoke(DetectorTypeIndex);
            }
        }

        public BodyPoseDetectorType[] DetectorTypeList { get; private set; }

        public string[] DetectorTypeNames { get; private set; }

        void OnEnable()
        {
            TofArBodyManager.OnStreamStarted += OnStreamStarted;
        }

        void OnDisable()
        {
            TofArBodyManager.OnStreamStarted -= OnStreamStarted;
        }

        protected override void Start()
        {
            // Get NNL list
            BodyPoseDetectorType[] types = (BodyPoseDetectorType[])Enum.GetValues(typeof(BodyPoseDetectorType));
            if (TofArManager.Instance.UsingIos)
            {
                types = types.Where(x => x != BodyPoseDetectorType.Internal_SV2).ToArray();
            }
            DetectorTypeList = new BodyPoseDetectorType[types.Length + 1];
            DetectorTypeNames = new string[DetectorTypeList.Length];
            DetectorTypeNames[0] = "-";
            DetectorTypeList[0] = types[0];
            for (int i = 0; i < types.Length; i++)
            {
                DetectorTypeNames[i + 1] = types[i] == BodyPoseDetectorType.External ? "SV1" :
                    types[i] == BodyPoseDetectorType.Internal_SV2 ? "SV2" :
                    types[i].ToString();
                DetectorTypeList[i + 1] = types[i];
            }

            base.Start();

            StartCoroutine(SetDetectorType());
        }

        private IEnumerator SetDetectorType()
        {
            yield return null;
            DetectorType = TofArBodyManager.Instance.DetectorType;
        }

        /// <summary>
        /// Event that is called when Body Stream is started
        /// </summary>
        /// <param name="sender">TofArBodyManager</param>
        void OnStreamStarted(object sender)
        {
            DetectorTypeIndex = Utils.Find(DetectorType, DetectorTypeList, 1);
        }

        /// <summary>
        /// Event that is called when Detector type is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeDetectorType;
    }
}
