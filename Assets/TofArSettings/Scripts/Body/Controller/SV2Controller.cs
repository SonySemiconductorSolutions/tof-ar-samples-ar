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
                if (!TofArManager.Instance.UsingIos)
                {
                    var currentProperty = TofArBodyManager.Instance.GetProperty<RecognizeConfigProperty>();
                    return currentProperty != null ? currentProperty.runtimeMode : RuntimeMode.Cpu;
                }
                return RuntimeMode.Cpu;
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

        protected override void Start()
        {
            base.Start();
            UpdateRuntimeModeList();
        }

        public int ModeThreads
        {
            get
            {
                if (!TofArManager.Instance.UsingIos)
                {
                    var currentProperty = TofArBodyManager.Instance.GetProperty<RecognizeConfigProperty>();
                    return currentProperty != null ? currentProperty.nThreads : 1;
                }
                return 1;
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
        public delegate void UpdateRuntimeModeListEvent(string[] list,
            int mode1Index);

        /// <summary>
        /// Event that is called when NNL, RuntimeModel or RuntimeMode2 is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeRuntimeMode;

        /// <summary>
        /// Event that is called when RuntimeMode list is updated
        /// </summary>
        public event UpdateRuntimeModeListEvent OnUpdateRuntimeModeList;

        /// <summary>
        /// Event that is called when thread count is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeModeThreads;

        /// <summary>
        /// Update RuntimeMode list
        /// </summary>
        void UpdateRuntimeModeList()
        {
            var mgr = TofArBodyManager.Instance;

            // Get RuntimeMode list
            RuntimeModeList = (RuntimeMode[])Enum.GetValues(typeof(RuntimeMode));
            if (RuntimeModeList.Length != modeNames.Length)
            {
                Array.Resize(ref modeNames, RuntimeModeList.Length);
            }

            for (int i = 0; i < RuntimeModeList.Length; i++)
            {
                modeNames[i] = RuntimeModeList[i].ToString();
            }

            if (!TofArManager.Instance.UsingIos)
            {
                var currentProperty = mgr.GetProperty<RecognizeConfigProperty>();
                // Get initial values for RuntimeMode1
                modeIndex = Utils.Find(currentProperty.runtimeMode, RuntimeModeList);
                if (modeIndex < 0)
                {
                    modeIndex = 0;
                }
            }

            OnUpdateRuntimeModeList?.Invoke(RuntimeModeNames, RuntimeModeIndex);
        }
    }
}
