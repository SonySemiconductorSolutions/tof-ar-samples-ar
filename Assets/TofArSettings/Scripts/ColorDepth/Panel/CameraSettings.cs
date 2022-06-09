/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using TofArSettings.Color;
using TofArSettings.Tof;
using UnityEngine.Events;

namespace TofArSettings.ColorDepth
{
    public class CameraSettings : UI.SettingsBase
    {
        [Header("Use Component")]

        /// <summary>
        /// Use/do not use Color component
        /// </summary>
        [SerializeField]
        bool color = true;

        /// <summary>
        /// Use/do not use Tof component
        /// </summary>
        [SerializeField]
        bool tof = true;

        ColorManagerController colorMgrCtrl;
        TofManagerController tofMgrCtrl;
        UI.ItemDropdown itemColor, itemDepth;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIColor,
                MakeUIDepth
            };

            if (color)
            {
                colorMgrCtrl = FindObjectOfType<ColorManagerController>();
                controllers.Add(colorMgrCtrl);
            }

            if (tof)
            {
                tofMgrCtrl = FindObjectOfType<TofManagerController>();
                controllers.Add(tofMgrCtrl);
            }

            base.Start();
        }

        /// <summary>
        /// Make Color UI
        /// </summary>
        void MakeUIColor()
        {
            if (!color)
            {
                return;
            }

            itemColor = settings.AddItem("Color", colorMgrCtrl.Options,
                colorMgrCtrl.Index, ChangeColor, 0, 90, 400);

            colorMgrCtrl.OnChangeAfter += (index) =>
            {
                itemColor.Index = index;
            };
        }

        /// <summary>
        /// Event that is called when option is selected from Color list
        /// </summary>
        /// <param name="index">Option index</param>
        void ChangeColor(int index)
        {
            colorMgrCtrl.Index = index;
        }

        /// <summary>
        /// Make Depth UI
        /// </summary>
        void MakeUIDepth()
        {
            if (!tof)
            {
                return;
            }

            itemDepth = settings.AddItem("Tof", tofMgrCtrl.Options,
                tofMgrCtrl.Index, ChangeDepth, 0, 90, 400);

            tofMgrCtrl.OnChangeAfter += (index) =>
            {
                itemDepth.Index = index;
            };
        }

        /// <summary>
        /// Event that is called when option is selected from Depth list
        /// </summary>
        /// <param name="index">Option index</param>
        void ChangeDepth(int index)
        {
            tofMgrCtrl.Index = index;
        }
    }
}
