/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine;

namespace TofArSettings.MarkRecog
{
    public class MarkRecogInfoSetting : Hand.HandInfoSettings
    {
        [SerializeField]
        bool recogResult = true;

        [SerializeField]
        bool recogStatus = true;

        enum InfoType : int
        {
            RecogResult,
            RecogStatus
        }

        UI.Panel panelMark;
        Dictionary<InfoType, MarkRecogInfo> informations = new Dictionary<InfoType, MarkRecogInfo>();

        protected override void MakeUI()
        {
            // Create UI contents
            settings.AddItem("Recog Status", recogStatus, ShowRecogStatus);
            settings.AddItem("Recog Result", recogResult, ShowRecogResult);
            // Get UI for DebugInfo display area
            foreach (var panel in GetComponentsInChildren<UI.Panel>())
            {
                if (panel.PanelObj.name.Contains("Mark"))
                {
                    panelMark = panel;
                }
            }

            var infoUis = panelMark.PanelObj.GetComponentsInChildren<MarkRecogInfo>();
            for (int i = 0; i < infoUis.Length; i++)
            {
                var info = infoUis[i];
                if (info is InfoMarkRecogResult)
                {
                    informations.Add(InfoType.RecogResult, info);
                }
                else if (info is InfoMarkRecogStatus)
                {
                    informations.Add(InfoType.RecogStatus, info);
                }
            }

            if (recogResult || recogStatus)
            {
                ShowRecogStatus(recogStatus);
                ShowRecogResult(recogResult);
            }
            else
            {
                // Prevent empty panels from being shown when all hidden
                foreach (var key in informations.Keys)
                {
                    ShowInfo(key, false, false);
                }
            }

            base.MakeUI();
        }


        /// <summary>
        /// Toggle Recog Status display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowRecogStatus(bool onOff)
        {
            ShowInfo(InfoType.RecogStatus, onOff, true);
        }

        /// <summary>
        /// Toggle Result Status display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowRecogResult(bool onOff)
        {
            ShowInfo(InfoType.RecogResult, onOff, true);
        }

        /// <summary>
        /// Display Debug Info display
        /// </summary>
        /// <param name="infoType">Debug Info type</param>
        /// <param name="onOff">Show/Hide</param>
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

            AdjustInfoArea(adjustHeight, autoOpen);
        }

        /// <summary>
        /// Show DebugInfo display area
        /// </summary>
        /// <param name="adjustHeight">Height to be adjusted</param>
        /// <param name="autoOpen">Automatically open panel or not</param>
        void AdjustInfoArea(float adjustHeight, bool autoOpen)
        {
            // Adjust height
            panelMark.Size = new Vector2(
                panelMark.Size.x, panelMark.Size.y + adjustHeight);

            bool show = false;
            if (autoOpen)
            {
                // Show panel if even one of the contents is shown
                foreach (var key in informations.Keys)
                {
                    if (informations[key].gameObject.activeSelf)
                    {
                        show = true;
                        break;
                    }
                }
            }

            if (show)
            {
                panelMark.OpenPanel(false);
            }
            else
            {
                panelMark.ClosePanel();
            }
        }
    }
}
