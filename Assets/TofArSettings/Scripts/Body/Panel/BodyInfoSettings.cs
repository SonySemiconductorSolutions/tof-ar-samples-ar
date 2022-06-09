/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine;

namespace TofArSettings.Body
{
    public class BodyInfoSettings : UI.SettingsBase
    {
        [Header("Default value")]

        [SerializeField]
        bool fps = false;

        enum InfoType : int
        {
            BodyStatus
        }

        UI.Panel panelFps;
        Dictionary<InfoType, BodyInfo> informations = new Dictionary<InfoType, BodyInfo>();

        protected override void MakeUI()
        {
            // Make UI contents
            settings.AddItem("FPS", fps, ShowFps);

            // Get UI for DebugInfo display area
            foreach (var panel in GetComponentsInChildren<UI.Panel>())
            {
                if (panel.PanelObj.name.Contains("Fps"))
                {
                    panelFps = panel;
                }
            }

            ShowFps(fps);

            base.MakeUI();
        }

        /// <summary>
        /// Toggle FPS display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowFps(bool onOff)
        {
            if (onOff)
            {
                panelFps.OpenPanel(false);
            }
            else
            {
                panelFps.ClosePanel();
            }
        }

        /// <summary>
        /// Toggle Debug Info display
        /// </summary>
        /// <param name="infoType">Debug Info type</param>
        /// <param name="onOff">On/Off</param>
        /// <param name="autoOpen">Automatically open panel or not</param>
        void ShowInfo(InfoType infoType, bool onOff, bool autoOpen)
        {
            var info = informations[infoType];
            float adjustHeight = 0;
            if (info.gameObject.activeSelf != onOff)
            {
                info.gameObject.SetActive(onOff);
                adjustHeight = (onOff) ? info.Size.y : -info.Size.y;
            }

            // AdjustInfoArea(adjustHeight, autoOpen);
        }
    }
}
