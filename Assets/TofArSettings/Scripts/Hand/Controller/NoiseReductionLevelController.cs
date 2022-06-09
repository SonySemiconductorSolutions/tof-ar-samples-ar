/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class NoiseReductionLevelController : ControllerBase
    {
        int index;
        public int Index
        {
            get { return index; }
            set
            {
                if (value != index && 0 <= value &&
                    value < NoiseReductionLevelList.Length)
                {
                    index = value;
                    TofArHandManager.Instance.NoiseReductionLevel =
                        NoiseReductionLevelList[value];

                    OnChange?.Invoke(Index);
                }
            }
        }

        public NoiseReductionLevel Level
        {
            get
            {
                return TofArHandManager.Instance.NoiseReductionLevel;
            }

            set
            {
                if (value != Level)
                {
                    Index = Utils.Find(value, NoiseReductionLevelList);
                }
            }
        }

        public NoiseReductionLevel[] NoiseReductionLevelList { get; private set; }

        string[] levelNames = new string[0];
        public string[] NoiseReductionLevelNames
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

                if (Level != conf.noiseReductionLevel)
                {
                    Level = conf.noiseReductionLevel;
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
            NoiseReductionLevelList = (NoiseReductionLevel[])Enum.GetValues(typeof(NoiseReductionLevel));
            levelNames = Enum.GetNames(typeof(NoiseReductionLevel));

            // Get initial values
            index = Utils.Find(mgr.NoiseReductionLevel, NoiseReductionLevelList);
            if (index < 0)
            {
                index = 0;
            }

            OnUpdateList?.Invoke(NoiseReductionLevelNames, Index);
        }
    }
}
