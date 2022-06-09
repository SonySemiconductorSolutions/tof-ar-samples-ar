/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Face
{
    public class FacialExpressionSettings : UI.SettingsBase
    {
        FacialExpressionController facialExpressionController;

        UI.ItemToggle itemEnable;
        UI.ItemDropdown itemRuntimeMode;
        UI.ItemSlider itemThread;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIRuntime
            };

            facialExpressionController = FindObjectOfType<FacialExpressionController>();
            controllers.Add(facialExpressionController);

            base.Start();
        }

        /// <summary>
        /// Make Runtime UI
        /// </summary>
        void MakeUIRuntime()
        {
            itemEnable = settings.AddItem("Enabled", facialExpressionController.OnOff, ChangeEnable);
            facialExpressionController.OnChangeEnable += (onOff) =>
            {
                itemEnable.OnOff = onOff;
            };

            itemRuntimeMode = settings.AddItem("Runtime", facialExpressionController.ExecModeNames,
                facialExpressionController.ExecModeIndex, ChangeRuntimeMode);
            itemThread = settings.AddItem("Threads",
                FacialExpressionController.ThreadMin, FacialExpressionController.ThreadMax,
                FacialExpressionController.ThreadStep, facialExpressionController.ModeThreads,
                ChangeThreads);

            facialExpressionController.OnChangeRuntimeMode += (index) =>
            {
                itemRuntimeMode.Index = index;
                itemThread.Interactable = facialExpressionController.IsInteractableModeThreads;
            };

            facialExpressionController.OnUpdateRuntimeModeList += (list, runtimeModeIndex) =>
            {
                itemRuntimeMode.Options = list;
                itemRuntimeMode.Index = runtimeModeIndex;
            };

            facialExpressionController.OnChangeModeThreads += (val) =>
            {
                itemThread.Value = val;
            };

        }

        /// <summary>
        /// Toggle gesture recognition
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeEnable(bool onOff)
        {
            facialExpressionController.OnOff = onOff;
        }

        /// <summary>
        /// Change RuntimeMode1
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeRuntimeMode(int index)
        {
            facialExpressionController.ExecModeIndex = index;
        }

        /// <summary>
        /// Change value of Threads of RuntimeMode1
        /// </summary>
        /// <param name="val">Thread count</param>
        void ChangeThreads(float val)
        {
            facialExpressionController.ModeThreads = Mathf.RoundToInt(val);
        }

    }
}
