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
    public class FacialExpressionController : ControllerBase
    {
        TofArFacialExpressionEstimator facialExpressionEstimator;
        FaceManagerController managerController;

        bool onOff;
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
                        facialExpressionEstimator.StartGestureEstimation();
                    }
                    else
                    {
                        facialExpressionEstimator.StopGestureEstimation();
                    }

                    OnChangeEnable?.Invoke(OnOff);
                }
            }
        }

        public event ChangeToggleEvent OnChangeEnable;

        protected void Awake()
        {
            facialExpressionEstimator = FindObjectOfType<TofArFacialExpressionEstimator>();
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
                return facialExpressionEstimator.ExecMode;
            }

            set
            {
                if (value != facialExpressionEstimator.ExecMode)
                {
                    modeIndex = Utils.Find(value, ExecModeList);
                    facialExpressionEstimator.ExecMode = value;
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
            onOff = facialExpressionEstimator.autoStart;

            base.Start();
            UpdateRuntimeModeList();
        }

        public int ModeThreads
        {
            get
            {
                return facialExpressionEstimator.ThreadsNum;
            }

            set
            {
                if (value != facialExpressionEstimator.ThreadsNum && IsInteractableModeThreads)
                {
                    facialExpressionEstimator.ThreadsNum = value;
                    managerController.RestartStream();
                    OnChangeModeThreads?.Invoke(facialExpressionEstimator.ThreadsNum);
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

            // Get RuntimeMode1 intial values
            modeIndex = Utils.Find(facialExpressionEstimator.ExecMode, ExecModeList);
            if (modeIndex < 0)
            {
                modeIndex = 0;
            }

            OnUpdateRuntimeModeList?.Invoke(ExecModeNames, ExecModeIndex);
        }

    }
}
