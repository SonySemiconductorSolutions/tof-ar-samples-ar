/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class ProcessLevelController : ControllerBase
    {
        int index;
        public int Index
        {
            get { return index; }
            set
            {
                if (value != index && 0 <= value &&
                    value < ProcessLevelList.Length)
                {
                    index = value;
                    TofArHandManager.Instance.ProcessLevel =
                        ProcessLevelList[value];

                    OnChange?.Invoke(Index);
                }
            }
        }

        public TofAr.V0.Hand.ProcessLevel Level
        {
            get
            {
                return TofArHandManager.Instance.ProcessLevel;
            }

            set
            {
                if (value != Level)
                {
                    Index = Utils.Find(value, ProcessLevelList);
                }
            }
        }

        public TofAr.V0.Hand.ProcessLevel[] ProcessLevelList { get; private set; }

        string[] levelNames = new string[0];
        public string[] ProcessLevelNames
        {
            get { return levelNames; }
        }

        public event ChangeIndexEvent OnChange;

        public event UpdateArrayEvent OnUpdateList;

        RecogModeController recogModeCtrl;

        protected override void Start()
        {
            recogModeCtrl = GetComponent<RecogModeController>();

            // Change ProcessLevel settings accordingly when Hand dictionary is changed
            recogModeCtrl.OnChangeRecog += (index, conf) =>
            {
                if (conf == null)
                {
                    return;
                }

                if (Level != conf.processLevel)
                {
                    Level = conf.processLevel;
                }
            };

            UpdateList();

            base.Start();
        }

        /// <summary>
        /// Update list
        /// </summary>
        void UpdateList()
        {
            var mgr = TofArHandManager.Instance;

            // Get Process Level list
            ProcessLevelList = mgr.SupportedProcessLevels;
            if (ProcessLevelList.Length != ProcessLevelNames.Length)
            {
                Array.Resize(ref levelNames, ProcessLevelList.Length);
            }

            // Set display name
            for (int i = 0; i < ProcessLevelList.Length; i++)
            {
                string levelName = string.Empty;
                switch (ProcessLevelList[i])
                {
                    case TofAr.V0.Hand.ProcessLevel.HandCenterOnly:
                        levelName = "Hand center";
                        break;

                    case TofAr.V0.Hand.ProcessLevel.HandPoints:
                        levelName = "25 points";
                        break;
                }

                levelNames[i] = levelName;
            }

            // Get initial values
            index = Utils.Find(mgr.ProcessLevel, ProcessLevelList);
            if (index < 0)
            {
                index = 0;
            }

            OnUpdateList?.Invoke(ProcessLevelNames, Index);
        }
    }
}
