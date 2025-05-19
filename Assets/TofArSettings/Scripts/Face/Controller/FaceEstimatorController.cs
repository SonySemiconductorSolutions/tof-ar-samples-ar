/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

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
            faceEstimator = FindAnyObjectByType<FaceEstimator>();
            managerController = FindAnyObjectByType<FaceManagerController>();
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

        public ProcessMode[] ProcessModeList { get; private set; }

        string[] processModeNames = new string[0];
        public string[] ProcessModeNames
        {
            get { return processModeNames; }
        }

        int processModeLandmarkIndex;
        public int ProcessModeLandmarkIndex
        {
            get { return processModeLandmarkIndex; }
            set
            {
                if (value != processModeLandmarkIndex && 0 <= value &&
                    value < ProcessModeList.Length)
                {
                    ProcessModeLandmark = ProcessModeList[value];
                }
            }
        }

        public ProcessMode ProcessModeLandmark
        {
            get
            {
                return faceEstimator.LandmarkProcessMode;
            }

            set
            {
                if (value != faceEstimator.LandmarkProcessMode)
                {
                    processModeLandmarkIndex = Utils.Find(value, ProcessModeList);
                    faceEstimator.LandmarkProcessMode = value;
                    managerController.RestartStream();
                    OnChangeModelType?.Invoke(processModeLandmarkIndex);
                }
            }
        }

        public BlendshapeEstimator[] BlendshapeEstimatorList { get; private set; }

        string[] blendshapeEstimatorNames = new string[0];
        public string[] BlendshapeEstimatorNames
        {
            get { return blendshapeEstimatorNames; }
        }

        int blendshapeEstimatorIndex;
        public int BlendshapeEstimatorIndex
        {
            get { return blendshapeEstimatorIndex; }
            set
            {
                if (value != blendshapeEstimatorIndex && 0 <= value &&
                    value < BlendshapeEstimatorList.Length)
                {
                    BlendshapeEstimator = BlendshapeEstimatorList[value];
                }
            }
        }

        public BlendshapeEstimator BlendshapeEstimator
        {
            get
            {
                return faceEstimator.BlendShapeEstimatorType;
            }

            set
            {
                if (value != faceEstimator.BlendShapeEstimatorType)
                {
                    blendshapeEstimatorIndex = Utils.Find(value, BlendshapeEstimatorList);
                    faceEstimator.BlendShapeEstimatorType = value;
                    managerController.RestartStream();
                    OnChangeBlendshapeEstimator?.Invoke(blendshapeEstimatorIndex);
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            UpdateRuntimeModeList();
            UpdateProcessModeList();
            UpdateBlendshapeEstimatorList();
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

        public event ChangeIndexEvent OnChangeRuntimeMode, OnChangeModelType, OnChangeBlendshapeEstimator;

        public event UpdateRuntimeModeListEvent OnUpdateRuntimeModeList, OnUpdateProcessModeLandmarkList, OnUpdateBlendshapeEstimatorList;

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
            if (faceEstimator != null)
            {
                modeIndex = Utils.Find(faceEstimator.ExecMode, ExecModeList);
                if (modeIndex < 0)
                {
                    modeIndex = 0;
                }
            }

            OnUpdateRuntimeModeList?.Invoke(ExecModeNames, ExecModeIndex);
        }

        /// <summary>
        /// Update ProcessMode list
        /// </summary>
        void UpdateProcessModeList()
        {
            // Get RuntimeMode list
            ProcessModeList = (ProcessMode[])Enum.GetValues(typeof(ProcessMode));
            if (ProcessModeList.Length != processModeNames.Length)
            {
                Array.Resize(ref processModeNames, ProcessModeList.Length);
            }

            for (int i = 0; i < ProcessModeList.Length; i++)
            {
                processModeNames[i] = ProcessModeList[i].ToString();
            }

            // Get intial values of RuntimeMode1
            if (faceEstimator != null)
            {
                processModeLandmarkIndex = Utils.Find(faceEstimator.LandmarkProcessMode, ProcessModeList);
                if (processModeLandmarkIndex < 0)
                {
                    processModeLandmarkIndex = 0;
                }
            }

            OnUpdateProcessModeLandmarkList?.Invoke(ProcessModeNames, ProcessModeLandmarkIndex);
        }

        /// <summary>
        /// Update BlendshapeEstimator list
        /// </summary>
        void UpdateBlendshapeEstimatorList()
        {
            // Get RuntimeMode list
            BlendshapeEstimatorList = (BlendshapeEstimator[])Enum.GetValues(typeof(BlendshapeEstimator));
            if (BlendshapeEstimatorList.Length != blendshapeEstimatorNames.Length)
            {
                Array.Resize(ref blendshapeEstimatorNames, BlendshapeEstimatorList.Length);
            }

            for (int i = 0; i < BlendshapeEstimatorList.Length; i++)
            {
                blendshapeEstimatorNames[i] = BlendshapeEstimatorList[i].ToString();
            }

            // Get intial values of RuntimeMode1
            if (faceEstimator != null)
            {
                blendshapeEstimatorIndex = Utils.Find(faceEstimator.BlendShapeEstimatorType, BlendshapeEstimatorList);
                if (blendshapeEstimatorIndex < 0)
                {
                    blendshapeEstimatorIndex = 0;
                }
            }

            OnUpdateBlendshapeEstimatorList?.Invoke(BlendshapeEstimatorNames, BlendshapeEstimatorIndex);
        }

    }
}
