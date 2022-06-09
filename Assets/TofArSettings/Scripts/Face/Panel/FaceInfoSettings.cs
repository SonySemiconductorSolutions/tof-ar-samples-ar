/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine;

namespace TofArSettings.Face
{
    public class FaceInfoSettings : UI.SettingsBase
    {
        [SerializeField]
        bool faceStatus = false;

        [SerializeField]
        bool facialExpression = false;

        enum InfoType : int
        {
            FaceStatus,
            ExpressionStatus
        }

        UI.Panel panelFace;
        Dictionary<InfoType, FaceInfo> informations = new Dictionary<InfoType, FaceInfo>();

        protected override void MakeUI()
        {
            // Create UI contents
            settings.AddItem("Face Status", faceStatus, ShowFaceStatus);
            settings.AddItem("Facial Expressions", facialExpression, ShowFacialExpression);

            // Get UI of DebugInfo display area
            foreach (var panel in GetComponentsInChildren<UI.Panel>())
            {
                if (panel.PanelObj.name.Contains("Face"))
                {
                    panelFace = panel;
                }
            }

            var infoUis = panelFace.PanelObj.GetComponentsInChildren<FaceInfo>();
            for (int i = 0; i < infoUis.Length; i++)
            {
                var info = infoUis[i];
                if (info is InfoFaceStatus)
                {
                    informations.Add(InfoType.FaceStatus, info);
                }
                else if (info is FacialExpressionsInfo)
                {
                    informations.Add(InfoType.ExpressionStatus, info);
                }
            }

            if (faceStatus || facialExpression)
            {
                ShowFaceStatus(faceStatus);
                ShowFacialExpression(facialExpression);
            }
            else
            {
                // Prevent empty panels from being displayed when all hidden
                foreach (var key in informations.Keys)
                {
                    ShowInfo(key, false, false);
                }
            }

            base.MakeUI();
        }

        /// <summary>
        /// Toggle Face Status display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowFaceStatus(bool onOff)
        {
            ShowInfo(InfoType.FaceStatus, onOff, true);
        }

        /// <summary>
        /// Toggle Facial Expression display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowFacialExpression(bool onOff)
        {
            ShowInfo(InfoType.ExpressionStatus, onOff, true);
        }

        /// <summary>
        /// Toggle Debug Info display
        /// </summary>
        /// <param name="infoType">Debug Info type</param>
        /// <param name="onOff">Show/Hide</param>
        /// <param name="autoOpen">Automatically open Panel or not</param>
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
        /// <param name="autoOpen">Automatically open Panel or not</param>
        void AdjustInfoArea(float adjustHeight, bool autoOpen)
        {
            // Adjust height
            panelFace.Size = new Vector2(
                panelFace.Size.x, panelFace.Size.y + adjustHeight);

            bool show = false;
            if (autoOpen)
            {
                // Display panel if even one of the contents is shown
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
                panelFace.OpenPanel(false);
            }
            else
            {
                panelFace.ClosePanel();
            }
        }
    }
}
