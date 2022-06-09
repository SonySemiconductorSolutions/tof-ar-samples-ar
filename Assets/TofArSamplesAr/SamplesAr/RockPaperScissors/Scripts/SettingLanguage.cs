/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using TofArSettings.UI;
using UnityEngine.Events;

namespace TofArARSamples.RockPaperScissors
{
    /// <summary>
    /// Classes used in the language setting UI
    /// </summary>
    public class SettingLanguage : SettingsBase
    {
        private RpsAppProgressManager rpsAppProgressManager = null;

        public int boxIndex = 0;

        void Awake()
        {
            rpsAppProgressManager = FindObjectOfType<RpsAppProgressManager>();
        }

        protected override void Start()
        {
            PrepareUI();
            base.Start();
        }

        /// <summary>
        /// Get ready to create a UI
        /// </summary>
        protected virtual void PrepareUI()
        {
            var list = new List<UnityAction>();

            list.Add(() =>
            {
                string[] boxList = new string[] { "English", "中文（簡体字）", "日本語" };
                settings.AddItem("Language", boxList, boxIndex, ChangeDropdown);
            });

            uiOrder = list.ToArray();
        }

        /// <summary>
        /// When changing the dropdown
        /// </summary>
        /// <param name="index"></param>
        public void ChangeDropdown(int index)
        {
            boxIndex = index;
            rpsAppProgressManager.SettingLanguage(boxIndex);
        }

        /// <summary>
        /// Create the contents of the UI
        /// </summary>
        protected override void MakeUI()
        {
            base.MakeUI();
        }
    }
}
