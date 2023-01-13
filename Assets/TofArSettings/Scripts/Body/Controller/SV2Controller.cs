﻿/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using TofAr.V0.Body;
using TofAr.V0.Tof;
using UnityEngine;
using TofAr.V0.Body.SV2;
using System.Linq;
using System;
using TofAr.V0;

namespace TofArSettings.Body
{
    public class SV2Controller : ControllerBase
    {
        int modeIndex;
        public int RuntimeModeIndex
        {
            get { return modeIndex; }
            set
            {
                if (value != modeIndex && 0 <= value &&
                    value < RuntimeModeList.Length)
                {
                    RuntimeMode = RuntimeModeList[value];
                }
            }
        }


        public RuntimeMode RuntimeMode
        {
            get
            {
                var currentProperty = TofArBodyManager.Instance.GetProperty<RecognizeConfigProperty>();
                return currentProperty != null ? currentProperty.runtimeMode : RuntimeMode.Cpu;   
            }

            set
            {
                if (value != RuntimeMode)
                {
                    modeIndex = Utils.Find(value, RuntimeModeList);
                    var currentProperty = TofArBodyManager.Instance.GetProperty<RecognizeConfigProperty>();
                    currentProperty.runtimeMode = value;
                    TofArBodyManager.Instance.SetProperty(currentProperty);
                    OnChangeRuntimeMode?.Invoke(RuntimeModeIndex);
                }
            }
        }

        public RuntimeMode[] RuntimeModeList { get; private set; }

        string[] modeNames = new string[0];
        public string[] RuntimeModeNames
        {
            get { return modeNames; }
        }

        int recogModeIndex;
        public int RecogModeIndex
        {
            get { return recogModeIndex; }
            set
            {
                if (value != recogModeIndex && 0 <= value &&
                    value < RecogModeList.Length)
                {
                    RecogMode = RecogModeList[value];
                }
            }
        }

        public RecogMode RecogMode
        {
            get
            {
                var currentProperty = TofArBodyManager.Instance.GetProperty<RecognizeConfigProperty>();
                return currentProperty != null ? currentProperty.recogMode : RecogMode.Face2Face;
            }

            set
            {
                if (value != RecogMode)
                {
                    StartCoroutine(ApplyMode(value));
                }
            }
        }

        bool isRunning = false;

        IEnumerator ApplyMode(RecogMode mode)
        {
            if (isRunning)
            {
                yield break;
            }

            isRunning = true;

            var mgr = TofArBodyManager.Instance;

            recogModeIndex = Utils.Find(mode, RecogModeList);
            var currentProperty = mgr.GetProperty<RecognizeConfigProperty>();
            currentProperty.recogMode = mode;
            TofArBodyManager.Instance.SetProperty(currentProperty);

            yield return null;

            RecognizeConfigProperty conf;

            while (true)
            {
                conf = mgr.GetProperty<RecognizeConfigProperty>();
                if (mode == conf.recogMode)
                {
                    break;
                }

                yield return null;
            }

            yield return null;

            isRunning = false;

            OnChangeRecogMode?.Invoke(RecogModeIndex, conf);
        }

        public RecogMode[] RecogModeList { get; private set; }

        string[] recogModeNames = new string[0];
        public string[] RecogModeNames
        {
            get { return recogModeNames; }
        }

        protected override void Start()
        {
            base.Start();

            OnChangeRecogMode += (index, conf) =>
            {
                if (conf == null)
                {
                    return;
                }

                Debug.Log("nThread : " + conf.nThreads);

                int runtimeModeIndex = Utils.Find(conf.runtimeMode, RuntimeModeList);
                OnChangeRuntimeMode?.Invoke(runtimeModeIndex);
                OnChangeModeThreads?.Invoke(conf.nThreads);
                int noiseReductionLevelIndex = Utils.Find(conf.noiseReductionLevel, NoiseReductionLevelList);
                OnChangeNoiseReductionLevel?.Invoke(noiseReductionLevelIndex);
            };

            UpdateRuntimeModeList();
            UpdateRecogModeList();
            UpdateNoiseReductionList();
        }

        public int ModeThreads
        {
            get
            {
                var currentProperty = TofArBodyManager.Instance.GetProperty<RecognizeConfigProperty>();
                return currentProperty != null ? currentProperty.nThreads : 1;
            }

            set
            {
                if (value != ModeThreads && IsInteractableModeThreads)
                {
                    var currentProperty = TofArBodyManager.Instance.GetProperty<RecognizeConfigProperty>();
                    currentProperty.nThreads = value;
                    TofArBodyManager.Instance.SetProperty(currentProperty);
                    OnChangeModeThreads?.Invoke(value);
                }
            }
        }

        public bool IsInteractableModeThreads
        {
            get
            {
                return (RuntimeMode == RuntimeMode.Cpu);
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
        public delegate void UpdateListEvent(string[] list,
            int mode1Index);

        /// <summary>
        /// Event that is called when NNL, RuntimeModel or RuntimeMode2 is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeRuntimeMode, OnChangeNoiseReductionLevel;

        /// <summary>
        /// Event that is called when RuntimeMode list is updated
        /// </summary>
        public event UpdateListEvent OnUpdateRuntimeModeList, OnUpdateRecogModeList, OnUpdateNoiseReductionList;

        /// <summary>
        /// Event that is called when thread count is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeModeThreads;

        /// <summary>
        /// Event that is called when body dictionary is changed
        /// </summary>
        /// <param name="index">body dictionary index</param>
        /// <param name="conf">body recognition settings</param>
        public delegate void ChangeRecogEvent(int index, RecognizeConfigProperty conf);

        public event ChangeRecogEvent OnChangeRecogMode;

        /// <summary>
        /// Update RuntimeMode list
        /// </summary>
        void UpdateRuntimeModeList()
        {
            var mgr = TofArBodyManager.Instance;

            // Get RuntimeMode list
            RuntimeModeList = mgr.SupportedRuntimeModes;
            if (RuntimeModeList.Length != modeNames.Length)
            {
                Array.Resize(ref modeNames, RuntimeModeList.Length);
            }

            for (int i = 0; i < RuntimeModeList.Length; i++)
            {
                modeNames[i] = RuntimeModeList[i].ToString();
            }

            var currentProperty = mgr.GetProperty<RecognizeConfigProperty>();
            // Get initial values for RuntimeMode1
            modeIndex = Utils.Find(currentProperty.runtimeMode, RuntimeModeList);
            if (modeIndex < 0)
            {
                modeIndex = 0;
            }

            OnUpdateRuntimeModeList?.Invoke(RuntimeModeNames, RuntimeModeIndex);
        }

        /// <summary>
        /// Update RecogMode list
        /// </summary>
        void UpdateRecogModeList()
        {
            var mgr = TofArBodyManager.Instance;

            // Get RuntimeMode list
            RecogModeList = (RecogMode[])Enum.GetValues(typeof(RecogMode));
            if (RecogModeList.Length != recogModeNames.Length)
            {
                Array.Resize(ref recogModeNames, RecogModeList.Length);
            }

            for (int i = 0; i < RecogModeList.Length; i++)
            {
                recogModeNames[i] = RecogModeList[i].ToString();
            }

            var currentProperty = mgr.GetProperty<RecognizeConfigProperty>();
            // Get initial values for RuntimeMode1
            recogModeIndex = Utils.Find(currentProperty.recogMode, RecogModeList);
            if (recogModeIndex < 0)
            {
                recogModeIndex = 0;
            }

            OnUpdateRecogModeList?.Invoke(RecogModeNames, RecogModeIndex);
        }

        int noiseReductionIndex;
        public int NoiseReductionIndex
        {
            get { return noiseReductionIndex; }
            set
            {
                if (value != noiseReductionIndex && 0 <= value &&
                    value < NoiseReductionLevelList.Length)
                {
                    noiseReductionIndex = value;
                    var recognizeConfig = TofArBodyManager.Instance.GetProperty<RecognizeConfigProperty>();
                    recognizeConfig.noiseReductionLevel = NoiseReductionLevelList[value];
                    TofArBodyManager.Instance.SetProperty<RecognizeConfigProperty>(recognizeConfig);

                    OnChangeNoiseReductionLevel?.Invoke(noiseReductionIndex);
                }
            }
        }

        public NoiseReductionLevel Level
        {
            get
            {
                var recognizeConfig = TofArBodyManager.Instance.GetProperty<RecognizeConfigProperty>();
                return recognizeConfig.noiseReductionLevel;
            }

            set
            {
                if (value != Level)
                {
                    NoiseReductionIndex = Utils.Find(value, NoiseReductionLevelList);
                }
            }
        }

        string[] levelNames = new string[0];
        public string[] NoiseReductionLevelNames
        {
            get { return levelNames; }
        }

        public NoiseReductionLevel[] NoiseReductionLevelList { get; private set; }

        /// <summary>
        /// Update list
        /// </summary>
        void UpdateNoiseReductionList()
        {
            var mgr = TofArBodyManager.Instance;

            // Get Process Level list
            NoiseReductionLevelList = (NoiseReductionLevel[])Enum.GetValues(typeof(NoiseReductionLevel));
            levelNames = Enum.GetNames(typeof(NoiseReductionLevel));

            // Get initial values
            var recognizeConfig = mgr.GetProperty<RecognizeConfigProperty>();
            noiseReductionIndex = Utils.Find(recognizeConfig.noiseReductionLevel, NoiseReductionLevelList);
            if (noiseReductionIndex < 0)
            {
                noiseReductionIndex = 0;
            }

            OnUpdateNoiseReductionList?.Invoke(NoiseReductionLevelNames, NoiseReductionIndex);
        }
    }
}
