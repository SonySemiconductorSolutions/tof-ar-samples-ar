/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */
 
using TofAr.V0.Plane;
using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Plane
{
    public class PlaneSettings : UI.SettingsBase
    {        
        PlaneManagerController managerController;
        
        DetectIntervalController detectIntervalController;
        

        UI.ItemDropdown itemMode, itemRuntimeMode, itemProcessModeLandmark;
        UI.ItemSlider itemDetectInterval;
        UI.ItemToggle itemStartStream;

        protected override void Start()
        {
            managerController = FindAnyObjectByType<PlaneManagerController>();
            controllers.Add(managerController);

            detectIntervalController = managerController.GetComponent<DetectIntervalController>();
            controllers.Add(detectIntervalController);

            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIStartStream,
                MakeUIDetectInterval,
            };

            base.Start();

            settings.OnChangeStart += OnChangePanel;
        }

        /// <summary>
        /// Make StartStream UI
        /// </summary>
        void MakeUIStartStream()
        {
            itemStartStream = settings.AddItem("Start Stream", TofArPlaneManager.Instance.autoStart, ChangeStartStream);

            managerController.OnStreamStartStatusChanged += (val) =>
            {
                itemStartStream.OnOff = val;
            };
        }

        /// <summary>
        /// If stream oocurs or not
        /// </summary>
        /// <param name="val">Stream started or not</param>
        void ChangeStartStream(bool val)
        {
            if (val)
            {
                managerController.StartStream();
            }
            else
            {
                managerController.StopStream();
            }
        }

        /// <summary>
        /// Make Detect Interval UI
        /// </summary>
        void MakeUIDetectInterval()
        {
            itemDetectInterval = settings.AddItem("Detect Interval",
                DetectIntervalController.Min,
                DetectIntervalController.Max,
                DetectIntervalController.Step,
                detectIntervalController.DetectInterval,
                ChangeDetectInterval, -4);

            itemDetectInterval.OnChange += (val) =>
            {
                itemDetectInterval.Value = val;
            };
        }

        /// <summary>
        /// Change Detect Interval
        /// </summary>
        /// <param name="val">DetectInterval</param>
        void ChangeDetectInterval(float val)
        {
            detectIntervalController.DetectInterval = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Event called when the state of the panel changes
        /// </summary>
        /// <param name="onOff">open/close</param>
        void OnChangePanel(bool onOff)
        {
            if (onOff)
            {
                itemStartStream.OnOff = TofArPlaneManager.Instance.IsStreamActive;
            }
        }
    }
}
