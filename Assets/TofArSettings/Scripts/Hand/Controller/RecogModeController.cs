/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections;
using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class RecogModeController : ControllerBase
    {
        int index = 0;
        public int Index
        {
            get { return index; }

            set
            {
                if (value != index && 0 <= value && value < ModeList.Length)
                {
                    index = value;
                    if (value > 0)
                    {
                        Mode = ModeList[value];
                    }
                    else
                    {
                        OnChangeRecog?.Invoke(Index, null);
                    }
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

        /// <summary>
        /// Event that is called when Hand dictionary is changed
        /// </summary>
        /// <param name="index">Hand dictionary index</param>
        /// <param name="conf">Hand recognition settings</param>
        public delegate void ChangeRecogEvent(int index, RecognizeConfigProperty conf);

        public event ChangeRecogEvent OnChangeRecog;

        public event UpdateArrayEvent OnUpdateList;

        protected override void Start()
        {
            UpdateRecogModeList();

            base.Start();

            // default to 1
            if (Index <= 0)
            {
                Index = 1;
            }
        }

        /// <summary>
        /// Update Hand dictionary list
        /// </summary>
        void UpdateRecogModeList()
        {
            var mgr = TofArHandManager.Instance;

            // Get dictionary list
            var array = TofArHandManager.Instance.SupportedRecogModes;
            if (ModeList.Length != array.Length + 1)
            {
                Array.Resize(ref modeList, array.Length + 1);
                Array.Resize(ref modeNames, ModeList.Length);
            }

            // Make top one the first
            ModeList[0] = array[0];
            RecogModeNames[0] = "-";
            for (int i = 0; i < array.Length; i++)
            {
                var mode = array[i];
                ModeList[i + 1] = mode;
                RecogModeNames[i + 1] = mode.ToString();
            }

            // Set intial values
            index = Utils.Find(Mode, ModeList, 1);
            if (index < 0)
            {
                index = 0;
            }

            OnUpdateList?.Invoke(RecogModeNames, Index);
        }

        /// <summary>
        /// Change dictionary
        /// </summary>
        /// <param name="mode">Hand dictionary</param>
        IEnumerator ApplyMode(RecogMode mode)
        {
            // Prevent multiple executions
            if (isRunning)
            {
                yield break;
            }

            isRunning = true;

            index = Utils.Find(mode, ModeList, 1);

            // Change
            TofArHandManager.Instance.RecogMode = mode;
            HandModel.RecogMode = mode;

            // Wait until changes have been reflected
            var mgr = TofArHandManager.Instance;
            RecognizeConfigProperty conf;
            while (true)
            {
                conf = mgr.GetProperty<RecognizeConfigProperty>();
                if (mgr.RecogMode == conf.recogMode)
                {
                    break;
                }

                yield return null;
            }

            yield return null;

            isRunning = false;

            OnChangeRecog?.Invoke(Index, conf);
        }
    }
}
