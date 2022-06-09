/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using TofArSettings.UI;
using UnityEngine;
using UnityEngine.Events;

namespace TofArARSamples.HandDecoration
{
    public class DesignSetting : SettingsBase
    {
        private HandDecoration handDecoration = null;

        public int boxIndex = 0;

        void Awake()
        {
            handDecoration = FindObjectOfType<HandDecoration>();
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
                string[] boxList = new string[] { "Flame", "Water", "Thunder", "Leaf" };
                settings.AddItem("Design", boxList, boxIndex, ChangeDropdown);
            });

            uiOrder = list.ToArray();
        }

        /// <summary>
        /// When changing the dropdown
        /// </summary>
        /// <param name="index">index</param>
        public void ChangeDropdown(int index)
        {
            boxIndex = index;
            handDecoration.SettingDesign(boxIndex);
        }

        protected override void MakeUI()
        {
            base.MakeUI();
        }
    }
}
