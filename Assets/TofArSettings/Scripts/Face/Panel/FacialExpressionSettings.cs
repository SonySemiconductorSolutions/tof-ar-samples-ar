/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine.Events;

namespace TofArSettings.Face
{
    public class FacialExpressionSettings : UI.SettingsBase
    {
        FacialExpressionController facialExpressionController;

        UI.ItemToggle itemEnable;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIRuntime
            };

            facialExpressionController = FindAnyObjectByType<FacialExpressionController>();
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
        }

        /// <summary>
        /// Toggle gesture recognition
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeEnable(bool onOff)
        {
            facialExpressionController.OnOff = onOff;
        }
    }
}
