/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using TofAr.V0.Body;
using TofAr.V0.Tof;
using UnityEngine;
using TofAr.V0.Body.SV1;
using System;

namespace TofArSettings.Body
{
    public class SV1Controller : ControllerBase
    {
        SV1BodyPoseEstimator poseEstimator;
        BodyManagerController managerController;

        protected void Awake()
        {
            poseEstimator = FindObjectOfType<SV1BodyPoseEstimator>();
            managerController = FindObjectOfType<BodyManagerController>();
        }

        int modeIndex;
        public int ExecModeIndex
        {
            get { return modeIndex; }
            set
            {
                if (value != modeIndex && 0 <= value &&
                    value < ExecModeList.Length)
                {
                    ExecMode = ExecModeList[value];
                }
            }
        }

        public TensorFlowLite.Runtime.TFLiteRuntime.ExecMode ExecMode
        {
            get
            {
                return poseEstimator.execMode;
            }

            set
            {
                if (value != poseEstimator.execMode)
                {
                    modeIndex = Utils.Find(value, ExecModeList);
                    poseEstimator.execMode = value;
                    managerController.RestartStream();
                    OnChangeRuntimeMode?.Invoke(ExecModeIndex);
                }
            }
        }

        public TensorFlowLite.Runtime.TFLiteRuntime.ExecMode[] ExecModeList { get; private set; }

        string[] modeNames = new string[0];
        public string[] ExecModeNames
        {
            get { return modeNames; }
        }

        int bodyShotIndex;
        public int BodyShotIndex
        {
            get { return bodyShotIndex; }
            set
            {
                if (value != bodyShotIndex && 0 <= value &&
                    value < BodyShotList.Length)
                {
                    BodyShot = BodyShotList[value];
                }
            }
        }

        public BodyShot BodyShot
        {
            get
            {
                return poseEstimator.bodyShot;
            }

            set
            {
                if (value != poseEstimator.bodyShot)
                {
                    bodyShotIndex = Utils.Find(value, BodyShotList);
                    poseEstimator.bodyShot = value;
                    managerController.RestartStream();
                    OnChangeBodyShot?.Invoke(BodyShotIndex);
                }
            }
        }

        public BodyShot[] BodyShotList { get; private set; }

        string[] bodyShotNames = new string[0];
        public string[] BodyShotNames
        {
            get { return bodyShotNames; }
        }

        protected override void Start()
        {
            base.Start();
            UpdateRuntimeModeList();
            UpdateBodyShotList();
        }

        public int ModeThreads
        {
            get
            {
                return poseEstimator.threadsNum;
            }

            set
            {
                if (value != poseEstimator.threadsNum && IsInteractableModeThreads)
                {
                    poseEstimator.threadsNum = value;
                    managerController.RestartStream();
                    OnChangeModeThreads?.Invoke(poseEstimator.threadsNum);
                }
            }
        }

        public bool IsInteractableModeThreads
        {
            get
            {
                return (ExecMode == TensorFlowLite.Runtime.TFLiteRuntime.ExecMode.EXEC_MODE_CPU);
            }
        }


        public const int ThreadMin = 1;
        public const int ThreadMax = 4;
        public const int ThreadStep = 1;

        /// <summary>
        /// Event that is called when RuntimeMode list is updated
        /// </summary>
        /// <param name="list">List of RuntimeMode names</param>
        /// <param name="mode1Index">RuntimeMode1 index</param>
        public delegate void UpdateRuntimeModeListEvent(string[] list,
            int mode1Index);

        /// <summary>
        /// Event that is called when RuntimeMode1 or RuntimeMode2 is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeRuntimeMode;

        /// <summary>
        /// Event that is called when RuntimeMode list is updated
        /// </summary>
        public event UpdateRuntimeModeListEvent OnUpdateRuntimeModeList;

        /// <summary>
        /// Event that is called when BodyShot list is updated
        /// </summary>
        /// <param name="list">List of BodyShot names</param>
        /// <param name="mode1Index">RuntimeMode1 index</param>
        public delegate void UpdateBodyShotListEvent(string[] list,
            int mode1Index);

        /// <summary>
        /// Event that is called when BodyShot is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeBodyShot;

        //public event UpdateBodyShotListEvent OnUpdateBodyShotList;

        /// <summary>
        /// Event that is called when thread count is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeModeThreads;

        /// <summary>
        /// Update RuntimeMode list
        /// </summary>
        void UpdateRuntimeModeList()
        {
            // Get RuntimeMode list
            ExecModeList = (TensorFlowLite.Runtime.TFLiteRuntime.ExecMode[])Enum.GetValues(typeof(TensorFlowLite.Runtime.TFLiteRuntime.ExecMode));
            if (ExecModeList.Length != modeNames.Length)
            {
                Array.Resize(ref modeNames, ExecModeList.Length);
            }

            for (int i = 0; i < ExecModeList.Length; i++)
            {
                modeNames[i] = ExecModeList[i].ToString();
            }

            // Get initial values for RuntimeMode1
            modeIndex = Utils.Find(poseEstimator.execMode, ExecModeList);
            if (modeIndex < 0)
            {
                modeIndex = 0;
            }

            OnUpdateRuntimeModeList?.Invoke(ExecModeNames, ExecModeIndex);
        }

        /// <summary>
        /// Update BodyShot list
        /// </summary>
        void UpdateBodyShotList()
        {
            // Get list
            BodyShotList = (BodyShot[])Enum.GetValues(typeof(BodyShot));
            if (BodyShotList.Length != bodyShotNames.Length)
            {
                Array.Resize(ref bodyShotNames, BodyShotList.Length);
            }

            for (int i = 0; i < BodyShotList.Length; i++)
            {
                bodyShotNames[i] = BodyShotList[i].ToString();
            }

            // Get initial values
            bodyShotIndex = Utils.Find(poseEstimator.bodyShot, BodyShotList);
            if (bodyShotIndex < 0)
            {
                bodyShotIndex = 0;
            }

            OnUpdateRuntimeModeList?.Invoke(BodyShotNames, BodyShotIndex);
        }
    }
}
