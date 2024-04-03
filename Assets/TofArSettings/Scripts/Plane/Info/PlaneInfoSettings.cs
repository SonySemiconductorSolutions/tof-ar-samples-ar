/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Plane;
using System.Collections.Generic;
using UnityEngine;

namespace TofArSettings.Plane
{
    public class PlaneInfoSettings : UI.SettingsBase
    {
        [Header("Default value")]

        [SerializeField]
        bool fps = false;

        [SerializeField]
        bool planeSize = false;

        [SerializeField]
        bool planeNormal = false;

        [SerializeField]
        bool targetScreenSpace = false;

        [SerializeField]
        bool targetWorldSpace = false;

        public delegate void PlaneArrangementChangedHandler(PlaneArrangement planeObject);

        public static event PlaneArrangementChangedHandler OnPlaneArrangementChanged;

        public PlaneArrangement selectedPlane = null;

        enum InfoType : int
        {
            PlaneSize,
            PlaneNormal,
            TargetScreenSpace,
            TargetWorldSpace
        }

        UI.Panel panelFps, panelInfo;
        Dictionary<InfoType, PlaneInfo> informations = new Dictionary<InfoType, PlaneInfo>();

        protected override void MakeUI()
        {
            AddItems();

            AssignPanels();

            AddInformations();

            ShowFps(fps);

            if(planeSize || planeNormal || targetScreenSpace)
            {
                ShowPlaneSize(planeSize);
                ShowPlaneNormal(planeNormal);
                ShowTargetScreenSpace(targetScreenSpace);
                ShowTargetWorldSpace(targetWorldSpace);
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

        private void AddItems()
        {
            // Create UI contents
            settings.AddItem("FPS", fps, ShowFps);

            settings.AddItem("Plane Size", planeSize, ShowPlaneSize);
            settings.AddItem("Plane Normal", planeNormal, ShowPlaneNormal);
            settings.AddItem("Target Screen Space", targetScreenSpace, ShowTargetScreenSpace);
            settings.AddItem("Target World Space", targetWorldSpace, ShowTargetWorldSpace);
        }

        private void AssignPanels()
        {
            // Get UI for DebugInfo display area
            foreach (var panel in GetComponentsInChildren<UI.Panel>())
            {
                if (panel.PanelObj.name.Contains("Fps"))
                {
                    panelFps = panel;
                }
                else if (panel.PanelObj.name.Contains("Info"))
                {
                    panelInfo = panel;
                }
            }
        }

        private void AddInformations()
        {
            var infoUis = panelInfo.PanelObj.GetComponentsInChildren<PlaneInfo>();
            for (int i = 0; i < infoUis.Length; i++)
            {
                var info = infoUis[i];
                if (info is InfoPlaneSize)
                {
                    informations.Add(InfoType.PlaneSize, info);
                }
                else if (info is InfoPlaneNormal)
                {
                    Debug.Log("INFORMATIONS");
                    informations.Add(InfoType.PlaneNormal, info);
                }
                else if (info is InfoTargetInScreenSpace)
                {
                    informations.Add(InfoType.TargetScreenSpace, info);
                }
                else if (info is InfoTargetInWorldSpace)
                {
                    informations.Add(InfoType.TargetWorldSpace, info);
                }
            }
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
        /// Toggle plane size display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowPlaneSize(bool onOff)
        {
            ShowInfo(InfoType.PlaneSize, onOff, true);
        }

        /// <summary>
        /// Toggle plane normal display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowPlaneNormal(bool onOff)
        {
            ShowInfo(InfoType.PlaneNormal, onOff, true);
        }

        /// <summary>
        /// Toggle target display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowTargetScreenSpace(bool onOff)
        {
            ShowInfo(InfoType.TargetScreenSpace, onOff, true);
        }

        /// <summary>
        /// Toggle target display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowTargetWorldSpace(bool onOff)
        {
            ShowInfo(InfoType.TargetWorldSpace, onOff, true);
        }

        /// <summary>
        /// Toggle Debug Info display
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
            panelInfo.Size = new Vector2(
                panelInfo.Size.x, panelInfo.Size.y + adjustHeight);

            bool show = false;
            if (autoOpen)
            {
                // Display panel if even one of contents is displayed
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
                panelInfo.OpenPanel(false);
            }
            else
            {
                panelInfo.ClosePanel();
            }
        }

        /// <summary>
        /// Changes the plane that has been selected to display its information
        /// </summary>
        /// <param name="planeObject">Selected plane</param>
        public void ChangeSelectedPlane(PlaneArrangement planeObject)
        {
            selectedPlane = planeObject;

            if(OnPlaneArrangementChanged != null)
            {
                OnPlaneArrangementChanged(planeObject);
            }
        }
    }
}