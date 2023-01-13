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
        
        int detectorTypeIndex;
        public int DetectorTypeIndex
        {
            get { return detectorTypeIndex; }
            set
            {
                if (value != detectorTypeIndex && 0 <= value && value < DetectorTypeList.Length)
                {
                    detectorTypeIndex = value;
                    DetectorType = DetectorTypeList[value];
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
                detectorTypeIndex = Utils.Find(value, DetectorTypeList, 0);
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
            BodyPoseDetectorType[] types = ((BodyPoseDetectorType[])Enum.GetValues(typeof(BodyPoseDetectorType))).Where(x => x == BodyPoseDetectorType.Internal_SV2).ToArray();
            DetectorTypeList = new BodyPoseDetectorType[types.Length];
            DetectorTypeNames = new string[DetectorTypeList.Length];
            for (int i = 0; i < types.Length; i++)
            {
                DetectorTypeNames[i] =  types[i] == BodyPoseDetectorType.Internal_SV2 ? "SV2" :
                    types[i].ToString();
                DetectorTypeList[i] = types[i];
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
            if (DetectorTypeList != null && DetectorTypeList.Length > 0)
            {
                DetectorTypeIndex = Utils.Find(DetectorType, DetectorTypeList, 0);
            }
        }

        /// <summary>
        /// Event that is called when Detector type is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeDetectorType;
    }
}
