/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using UnityEngine;
using TofAr.V0.Face;
using System;

namespace TofArSettings.Face
{
    public class FaceEstimatorController : ControllerBase
    {
        FaceEstimator faceEstimator;
        FaceManagerController managerController;

        protected void Awake()
        {
            faceEstimator = FindObjectOfType<FaceEstimator>();
            managerController = FindObjectOfType<FaceManagerController>();
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
                return faceEstimator.ExecMode;
            }

            set
            {
                if (value != faceEstimator.ExecMode)
                {
                    modeIndex = Utils.Find(value, ExecModeList);
                    faceEstimator.ExecMode = value;
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

        protected override void Start()
        {
            base.Start();
            UpdateRuntimeModeList();
        }

        public int ModeThreads
        {
            get
            {
                return faceEstimator.ThreadsNum;
            }

            set
            {
                if (value != faceEstimator.ThreadsNum && IsInteractableModeThreads)
                {
                    faceEstimator.ThreadsNum = value;
                    managerController.RestartStream();
                    OnChangeModeThreads?.Invoke(faceEstimator.ThreadsNum);
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
        /// <param name="list">RuntimeMode name list</param>
        /// <param name="mode1Index">RuntimeMode1 index</param>
        public delegate void UpdateRuntimeModeListEvent(string[] list,
            int mode1Index);

        public event ChangeIndexEvent OnChangeRuntimeMode;

        public event UpdateRuntimeModeListEvent OnUpdateRuntimeModeList;

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

            // Get intial values of RuntimeMode1
            modeIndex = Utils.Find(faceEstimator.ExecMode, ExecModeList);
            if (modeIndex < 0)
            {
                modeIndex = 0;
            }

            OnUpdateRuntimeModeList?.Invoke(ExecModeNames, ExecModeIndex);
        }

    }
}
