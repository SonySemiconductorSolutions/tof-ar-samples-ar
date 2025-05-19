/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022, 2023, 2024 Sony Semiconductor Solutions Corporation.
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
        bool distance = false;

        [SerializeField]
        bool cameraIntrinsics = false;

        [SerializeField]
        bool spanDistance = false;

        [SerializeField]
        bool displayHandpoints = false;

        [SerializeField]
        bool confidence = false;

        enum InfoType : int
        {
            HandStatus,
            PoseStatus,
            Gesture,
            Distance,
            CameraIntrinsics,
            SpanDistance,
            Handpoints,
            Confidence
        }

        UI.Panel panelFps, panelInfo, panelHandPoints, panelHandPose;
        Dictionary<InfoType, HandInfo> informations = new Dictionary<InfoType, HandInfo>();

        HandPoseInfoSettings poseInfoSettings;
        HandGestureInfoSettings gestureInfoSettings;

        private void Awake()
        {
            poseInfoSettings = GetComponentInChildren<HandPoseInfoSettings>();
            gestureInfoSettings = GetComponentInChildren<HandGestureInfoSettings>();
        }

        protected override void MakeUI()
        {
            AddItems();

            AssignPanels();

            AddInformations();

            ShowFps(fps);

            if (handStatus || distance || cameraIntrinsics ||
                spanDistance || displayHandpoints || confidence)
            {
                ShowHandStatus(handStatus);
                ShowDist(distance);
                ShowCameraIntrinsics(cameraIntrinsics);
                ShowSpan(spanDistance);
                ShowHandPoints(displayHandpoints);
                ShowConfidence(confidence);
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

            if (poseInfoSettings)
            {
                settings.AddItem(poseInfoSettings.Title, poseInfoSettings.TitleIcon, poseInfoSettings.IconColor, () =>
                {
                    poseInfoSettings.OpenPanel();
                });

                poseInfoSettings.OnBack += () =>
                {
                    settings.OpenPanel();
                };

                // Link a child panel to parent panel
                poseInfoSettings.LinkParent(settings.RegisterChildPanel);
            }

            if (gestureInfoSettings)
            {
                settings.AddItem(gestureInfoSettings.Title, gestureInfoSettings.TitleIcon, gestureInfoSettings.IconColor, () =>
                {
                    gestureInfoSettings.OpenPanel();
                });

                gestureInfoSettings.OnBack += () =>
                {
                    settings.OpenPanel();
                };

                // Link a child panel to parent panel
                gestureInfoSettings.LinkParent(settings.RegisterChildPanel);
            }

            settings.AddItem("Distance", distance, ShowDist);
            settings.AddItem("Span Distance", spanDistance, ShowSpan);
            settings.AddItem("Confidence", confidence, ShowConfidence);
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
                }
                else if (panel.PanelObj.name.Contains("HandPose"))
                {
                    panelHandPose = panel;
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
                else if (info is InfoConfidence)
                {
                    informations.Add(InfoType.Confidence, info);
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
        /// Toggle Confidence Distance display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowConfidence(bool onOff)
        {
            ShowInfo(InfoType.Confidence, onOff, true);
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
