/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TofArSettings.UI;
using System;

namespace TofArARSamples.TextureRoom
{
    /// <summary>
    /// Class to add to the common UI
    /// </summary>
    public class TextureRoomSettings : SettingsBase
    {
        ItemButton addStampButton;
        ItemButton clearStampButton;
        ItemDropdown modeDropdown;

        public int modeIndex = 0;

        [SerializeField]
        StampController stampController;

        [SerializeField]
        MappingController mappingController;

        protected override void Start()
        {
            PrepareUI();
            base.Start();
        }

        protected void PrepareUI()
        {
            var list = new List<UnityAction>();

            list.Add(() =>
            {
                string[] modeList = new string[] { "TextureAnimation", "TextInput", "Stamp" };
                settings.AddItem("Mode Dropdown", modeList, modeIndex, ChangeDropdown);
            });

            list.Add(() =>
            {
                addStampButton = settings.AddItem("Add Stamp Button", AddStampButtonTap);
            });

            list.Add(() =>
            {
                clearStampButton = settings.AddItem("Clear Stamp Button", ClearStampButtonTap);
            });

            uiOrder = list.ToArray();

        }

        /// <summary>
        /// Enables or disables the "add stamp" and "clear stamp" buttons
        /// </summary>
        void ChangeButtonInteractable()
        {
            if (mappingController.getCurrentMode() == Mode.Stamp)
            {
                addStampButton.Interactable = true;
                clearStampButton.Interactable = true;
            }
            else
            {
                addStampButton.Interactable = false;
                clearStampButton.Interactable = false;
            }
        }

        public void ChangeDropdown(int index)
        {
            modeIndex = index;
            mappingController.changeCurrentMode((Mode)Enum.ToObject(typeof(Mode), index));
            ChangeButtonInteractable();
        }

        public void AddStampButtonTap()
        {
            stampController.PickImage(1024);
        }

        public void ClearStampButtonTap()
        {
            stampController.Init();
        }

        protected override void MakeUI()
        {
            base.MakeUI();
            ChangeButtonInteractable();
        }
    }
}
