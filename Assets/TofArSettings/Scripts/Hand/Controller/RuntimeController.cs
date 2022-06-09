/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using TofAr.V0;
using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class RuntimeController : ControllerBase
    {
        public bool IsInteractableNnl
        {
            get
            {
                return (TofArManager.Instance.RuntimeSettings.runMode == RunMode.Default);
            }
        }

        public string[] NnLibNames { get; private set; }

        int mode1Index;
        public int RuntimeMode1Index
        {
            get { return mode1Index; }
            set
            {
                if (value != mode1Index && 0 <= value &&
                    value < RuntimeModeList.Length)
                {
                    RuntimeMode1 = RuntimeModeList[value];
                }
            }
        }

        int mode2Index;
        public int RuntimeMode2Index
        {
            get { return mode2Index; }
            set
            {
                if (value != mode2Index && 0 <= value &&
                    value < RuntimeModeList.Length)
                {
                    RuntimeMode2 = RuntimeModeList[value];
                }
            }
        }

        public RuntimeMode RuntimeMode1
        {
            get
            {
                return TofArHandManager.Instance.RuntimeMode;
            }

            set
            {
                if (value != RuntimeMode1)
                {
                    mode1Index = Utils.Find(value, RuntimeModeList);
                    TofArHandManager.Instance.RuntimeMode = value;

                    OnChangeRuntimeMode1?.Invoke(RuntimeMode1Index);
                }
            }
        }

        public RuntimeMode RuntimeMode2
        {
            get
            {
                return TofArHandManager.Instance.RuntimeModeAfter;
            }

            set
            {
                if (value != RuntimeMode2)
                {
                    mode2Index = Utils.Find(value, RuntimeModeList);
                    TofArHandManager.Instance.RuntimeModeAfter = value;

                    OnChangeRuntimeMode2?.Invoke(RuntimeMode2Index);
                }
            }
        }

        public RuntimeMode[] RuntimeModeList { get; private set; }

        string[] modeNames = new string[0];
        public string[] RuntimeModeNames
        {
            get { return modeNames; }
        }

        public int Mode1Threads
        {
            get
            {
                return TofArHandManager.Instance.NPointThreads;
            }

            set
            {
                if (value != Mode1Threads && IsInteractableMode1Threads)
                {
                    TofArHandManager.Instance.NPointThreads = value;
                    OnChangeMode1Threads?.Invoke(Mode1Threads);
                }
            }
        }

        public bool IsInteractableMode1Threads
        {
            get
            {
                return (RuntimeMode1 == RuntimeMode.Cpu);
            }
        }

        public int Mode2Threads
        {
            get
            {
                return TofArHandManager.Instance.NRegionThreads;
            }

            set
            {
                if (value != Mode2Threads && IsInteractableMode2Threads)
                {
                    TofArHandManager.Instance.NRegionThreads = value;
                    OnChangeMode2Threads?.Invoke(Mode2Threads);
                }
            }
        }

        public bool IsInteractableMode2Threads
        {
            get
            {
                return (RuntimeMode2 == RuntimeMode.Cpu);
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
        /// <param name="mode2Index">RuntimeMode2 index</param>
        public delegate void UpdateRuntimeModeListEvent(string[] list,
            int mode1Index, int mode2Index);

        public event ChangeIndexEvent OnChangeRuntimeMode1,
            OnChangeRuntimeMode2;

        public event UpdateRuntimeModeListEvent OnUpdateRuntimeModeList;

        public event ChangeIndexEvent OnChangeMode1Threads, OnChangeMode2Threads;

        RecogModeController recogModeCtrl;

        protected override void Start()
        {
            recogModeCtrl = GetComponent<RecogModeController>();

            // Change RuntimeMode settings accordingly when Hand dictionary is changed
            recogModeCtrl.OnChangeRecog += (index, conf) =>
            {
                if (conf == null)
                {
                    return;
                }

                if (RuntimeMode1 != conf.runtimeMode)
                {
                    RuntimeMode1 = conf.runtimeMode;
                }

                if (RuntimeMode2 != conf.runtimeModeAfter)
                {
                    RuntimeMode2 = conf.runtimeModeAfter;
                }
            };

            UpdateRuntimeModeList();

            base.Start();
        }

        /// <summary>
        /// Update RuntimeMode list
        /// </summary>
        void UpdateRuntimeModeList()
        {
            var mgr = TofArHandManager.Instance;

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

            // Get RuntimeMode1 intial values
            mode1Index = Utils.Find(mgr.RuntimeMode, RuntimeModeList);
            if (mode1Index < 0)
            {
                mode1Index = 0;
            }

            // Get RuntimeMode2 intial values
            mode2Index = Utils.Find(mgr.RuntimeModeAfter, RuntimeModeList);
            if (mode2Index < 0)
            {
                mode2Index = 0;
            }

            OnUpdateRuntimeModeList?.Invoke(RuntimeModeNames, RuntimeMode1Index,
                RuntimeMode2Index);
        }
    }
}
