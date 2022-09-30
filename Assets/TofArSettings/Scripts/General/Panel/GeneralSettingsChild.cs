/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.General
{
    public class GeneralSettingsChild : UI.SettingsBase
    {
        TofArFramerateController tofArFramerateCtrl;

        UI.ItemSlider itemTofArFramerate;

        protected override void Start()
        {
             // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIFramerate
            };

            tofArFramerateCtrl = GetComponent<TofArFramerateController>();
            controllers.Add(tofArFramerateCtrl);

            base.Start();
        }

        /// <summary>
        /// Make Framerate UI
        /// </summary>
        void MakeUIFramerate()
        {
            // ToFAR Session Framerate
            itemTofArFramerate = settings.AddItem("ToFAR Framerate",
                TofArFramerateController.FramerateMin,
                TofArFramerateController.FramerateMax,
                TofArFramerateController.FramerateStep,
                tofArFramerateCtrl.Framerate, ChangeTofArFramerate, -4);

            tofArFramerateCtrl.OnChangeFramerate += (val) =>
            {
                itemTofArFramerate.Value = val;
            };
        }

        /// <summary>
        /// Change ToFAR Session Framerate
        /// </summary>
        /// <param name="val">ToFAR Session Framerate</param>
        void ChangeTofArFramerate(float val)
        {
            tofArFramerateCtrl.Framerate = Mathf.RoundToInt(val);
        }
    }
}
