/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections;
using TofAr.V0;
using TofAr.V0.Hand;
using UnityEngine;

namespace TofArSettings.Hand
{
    public class RecogModeController : ControllerBase
    {
        int index = -1;
        public int Index
        {
            get { return index; }

            set
            {
                if (value != index && 0 <= value && value < ModeList.Length)
                {
                    index = value;
                    Mode = ModeList[value];
                }
            }
        }

        public RecogMode Mode
        {
            get
            {
                return TofArHandManager.Instance.RecogMode;
            }

            set
            {
                StartCoroutine(ApplyMode(value));
            }
        }

        RecogMode[] modeList = new RecogMode[0];
        public RecogMode[] ModeList
        {
            get { return modeList; }
        }

        string[] modeNames = new string[0];
        public string[] RecogModeNames
        {
            get { return modeNames; }
        }

        bool isRunning = false;

        const float timeOut = 5;

        /// <summary>
        /// Event that is called when Hand dictionary is changed
        /// </summary>
        /// <param name="index">Hand dictionary index</param>
        /// <param name="conf">Hand recognition settings</param>
        public delegate void ChangeRecogEvent(int index, RecognizeConfigProperty conf);

        public event ChangeRecogEvent OnChangeRecog;

        public event UpdateArrayEvent OnUpdateList;

        [SerializeField]
        private bool initializeByRecommendedValue = true;

        protected override void Start()
        {
            UpdateRecogModeList();

            base.Start();
        }

        /// <summary>
        /// Update Hand dictionary list
        /// </summary>
        void UpdateRecogModeList()
        {
            var mgr = TofArHandManager.Instance;

            // Get dictionary list
            var array = TofArHandManager.Instance.SupportedRecogModes;
            if (ModeList.Length != array.Length)
            {
                Array.Resize(ref modeList, array.Length);
                Array.Resize(ref modeNames, ModeList.Length);
            }

            // Make top one the first
            ModeList[0] = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                var mode = array[i];
                ModeList[i] = mode;
                RecogModeNames[i] = mode.ToString();
            }

            // Set intial values
            int idx = Utils.Find(Mode, ModeList, 0);
            if (initializeByRecommendedValue)
            {
                var runtimeSettings = TofArManager.Instance.RuntimeSettings;
                var platformConfig = TofArManager.Instance.GetProperty<PlatformConfigurationProperty>();
                if ((runtimeSettings.distribution == Distribution.Pro)
                    && (TofArManager.Instance.UsingIos
                    || (platformConfig?.platformConfigurationPC?.customData?.Contains("k4a") == true)
                    || (platformConfig?.platformConfigurationPC?.customData?.Contains("int") == true)))
                {
                    idx = Array.IndexOf(this.ModeList, RecogMode.HeadMount + 2);
                }
            }
            if (idx < 0)
            {
                idx = 0;
            }
            Index = idx;

            OnUpdateList?.Invoke(RecogModeNames, Index);
        }

        /// <summary>
        /// Change dictionary
        /// </summary>
        /// <param name="mode">Hand dictionary</param>
        IEnumerator ApplyMode(RecogMode mode)
        {
            // Wait until the previous process is completed
            while (isRunning)
            {
                yield return null;
            }

            isRunning = true;

            index = Utils.Find(mode, ModeList, 0);

            // Change
            TofArHandManager.Instance.RecogMode = mode;
            HandModel.RecogMode = mode;

            // Wait until changes have been reflected
            var mgr = TofArHandManager.Instance;
            RecognizeConfigProperty conf;
            float time = 0;
            while (true)
            {
                conf = mgr.GetProperty<RecognizeConfigProperty>();
                if ((conf != null && mgr.RecogMode == conf.recogMode) || time >= timeOut ||
                    HandModel.RecogMode != mode)
                {
                    break;
                }

                time += Time.deltaTime;

                yield return null;
            }

            yield return null;

            isRunning = false;

            OnChangeRecog?.Invoke(Index, conf);
        }
    }
}
