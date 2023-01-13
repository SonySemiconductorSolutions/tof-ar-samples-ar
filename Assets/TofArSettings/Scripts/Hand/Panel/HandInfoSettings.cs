/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine;

namespace TofArSettings.Hand
{
    public class HandInfoSettings : UI.SettingsBase
    {
        [Header("Default value")]

        [SerializeField]
        bool fps = false;

        [SerializeField]
        bool handStatus = false;

        [SerializeField]
        bool pose = false;

        [SerializeField]
        bool gesture = false;

        [SerializeField]
        bool distance = false;

        [SerializeField]
        bool cameraIntrinsics = false;

        [SerializeField]
        bool spanDistance = false;

        [SerializeField]
        bool displayHandpoints = false;

        enum InfoType : int
        {
            HandStatus,
            PoseStatus,
            Gesture,
            Distance,
            CameraIntrinsics,
            SpanDistance,
            Handpoints
        }

        UI.Panel panelFps, panelInfo, panelHandPoints;
        Dictionary<InfoType, HandInfo> informations = new Dictionary<InfoType, HandInfo>();

        protected override void MakeUI()
        {
            AddItems();

            AssignPanels();

            AddInformations();

            ShowFps(fps);

            if (handStatus || pose || gesture || distance || cameraIntrinsics ||
                spanDistance || displayHandpoints)
            {
                ShowHandStatus(handStatus);
                ShowPose(pose);
                ShowGesture(gesture);
                ShowDist(distance);
                ShowCameraIntrinsics(cameraIntrinsics);
                ShowSpan(spanDistance);
                ShowHandPoints(displayHandpoints);
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
            settings.AddItem("Hand Status", handStatus, ShowHandStatus);
            settings.AddItem("Hand Pose", pose, ShowPose);
            settings.AddItem("Gesture", gesture, ShowGesture);
            settings.AddItem("Distance", distance, ShowDist);
            settings.AddItem("Span Distance", spanDistance, ShowSpan);
            settings.AddItem("Camera Intrinsics", cameraIntrinsics, ShowCameraIntrinsics);
            settings.AddItem("Display Hand points", displayHandpoints, ShowHandPoints);
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

            // Get panel for displaying information
            foreach (var panel in GetComponentsInChildren<UI.Panel>())
            {
                if (panel.PanelObj.name.Contains("HandPoints"))
                {
                    panelHandPoints = panel;
                    break;
                }
            }
        }

        private void AddInformations()
        {
            var infoUis = panelInfo.PanelObj.GetComponentsInChildren<HandInfo>();
            for (int i = 0; i < infoUis.Length; i++)
            {
                var info = infoUis[i];
                if (info is InfoHandStatus)
                {
                    informations.Add(InfoType.HandStatus, info);
                }
                else if (info is InfoPoseStatus)
                {
                    informations.Add(InfoType.PoseStatus, info);
                }
                else if (info is InfoGesture)
                {
                    informations.Add(InfoType.Gesture, info);
                }
                else if (info is InfoDistance)
                {
                    informations.Add(InfoType.Distance, info);
                }
                else if (info is InfoCameraIntrinsics)
                {
                    informations.Add(InfoType.CameraIntrinsics, info);
                }
                else if (info is InfoSpanDistance)
                {
                    informations.Add(InfoType.SpanDistance, info);
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
        /// Toggle Hand Status display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowHandStatus(bool onOff)
        {
            ShowInfo(InfoType.HandStatus, onOff, true);
        }

        /// <summary>
        /// Toggle Pose Status display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowPose(bool onOff)
        {
            ShowInfo(InfoType.PoseStatus, onOff, true);
        }

        /// <summary>
        /// Toggle Gesture display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowGesture(bool onOff)
        {
            ShowInfo(InfoType.Gesture, onOff, true);
        }

        /// <summary>
        /// Toggle Distance display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowDist(bool onOff)
        {
            ShowInfo(InfoType.Distance, onOff, true);
        }

        /// <summary>
        /// Toggle Camera Intrinsics display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowCameraIntrinsics(bool onOff)
        {
            ShowInfo(InfoType.CameraIntrinsics, onOff, true);
        }

        /// <summary>
        /// Toggle Span Distance display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowSpan(bool onOff)
        {
            ShowInfo(InfoType.SpanDistance, onOff, true);
        }

        /// <summary>
        /// Toggle Hand Points display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowHandPoints(bool onOff)
        {
            if (onOff)
            {
                panelHandPoints.OpenPanel(false);
            }
            else
            {
                panelHandPoints.ClosePanel();
            }
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
    }
}
