/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArSettings
{
    public abstract class ImageSettings : UI.SettingsBase
    {
        protected CameraManagerController mgrCtrl;
        protected ImageDelayController delayCtrl;
        protected ImageFpsRequestController fpsReqCtrl;
        protected ImageExposureController exposureCtrl;

        private DependendStreamUIHandler componentCtrl;

        UI.ItemDropdown itemMode;
        UI.ItemSlider itemDelay, itemFpsRequest, itemExpoTime;
        protected UI.ItemToggle itemAutoExpo;

        protected override void Start()
        {
            // Child class is called first
            controllers.Add(mgrCtrl);
            delayCtrl = mgrCtrl.GetComponent<ImageDelayController>();
            controllers.Add(delayCtrl);
            fpsReqCtrl = mgrCtrl.GetComponent<ImageFpsRequestController>();
            controllers.Add(fpsReqCtrl);
            exposureCtrl = mgrCtrl.GetComponent<ImageExposureController>();
            controllers.Add(exposureCtrl);

            var ctrls = FindObjectsByType<DependendStreamUIHandler>(FindObjectsSortMode.None);
            foreach (var controller in ctrls)
            {
                if (controller.GetType() == typeof(DependendStreamUIHandler))
                {
                    componentCtrl = controller;
                    break;
                }
            }

            base.Start();

            settings.OnChangeStart += OnChangePanel;
        }

        protected override void MakeUI()
        {
            base.MakeUI();

            // Setup UI
            UpdateExposureUI();
        }

        /// <summary>
        /// Make CameraConfig/Resolution UI
        /// </summary>
        protected void MakeUIMode()
        {
            itemMode = settings.AddItem("Mode", mgrCtrl.Options,
                mgrCtrl.Index, ChangeMode, 0, 0, 360);

            mgrCtrl.OnChangeAfter += (index) =>
            {
                itemMode.Index = index;
            };

            mgrCtrl.OnMadeOptions += () =>
            {
                itemMode.Options = mgrCtrl.Options;
            };
        }

        /// <summary>
        /// Add dropdown to ComponentController to enable/disable
        /// </summary>
        /// <param name="type">Component type</param>
        protected void AddDropdownToController(ComponentType type)
        {
            if (componentCtrl != null)
            {
                componentCtrl.AddDropdown(itemMode, type);
            }
        }

        /// <summary>
        /// Event that is called when option is selected from CameraConfig/Resolution list
        /// </summary>
        /// <param name="index">Option index</param>
        void ChangeMode(int index)
        {
            mgrCtrl.Index = index;
        }

        /// <summary>
        /// Make Delay UI
        /// </summary>
        protected void MakeUIDelay()
        {
            itemDelay = settings.AddItem("Delay", ImageDelayController.Min,
                ImageDelayController.Max, ImageDelayController.Step, delayCtrl.Delay,
                ChangeDelay);

            delayCtrl.OnChange += (val) =>
            {
                itemDelay.Value = val;
            };
        }

        /// <summary>
        /// Change delay
        /// </summary>
        /// <param name="val"></param>
        void ChangeDelay(float val)
        {
            delayCtrl.Delay = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Make FPS Request UI
        /// </summary>
        protected void MakeUIFpsRequest()
        {
            itemFpsRequest = settings.AddItem("FPS\nRequest", fpsReqCtrl.Min,
                fpsReqCtrl.Max, ImageFpsRequestController.Step, fpsReqCtrl.FrameRate,
                ChangeFpsRequest, -2);

            fpsReqCtrl.OnChangeFrameRate += (val) =>
            {
                itemFpsRequest.Value = val;
            };

            fpsReqCtrl.OnChangeRange += () =>
            {
                // Update FPS Request UI
                itemFpsRequest.Min = fpsReqCtrl.Min;
                itemFpsRequest.Max = fpsReqCtrl.Max;
            };
        }

        /// <summary>
        /// Change request speed
        /// </summary>
        /// <param name="val"></param>
        void ChangeFpsRequest(float val)
        {
            fpsReqCtrl.FrameRate = val;
        }

        /// <summary>
        /// Make Exposure UI
        /// </summary>
        protected virtual void MakeUIExposure()
        {
            if (!TofAr.V0.TofArManager.Instance.UsingIos)
            {
                // AutoExposure
                itemAutoExpo = settings.AddItem("Auto Exposure", exposureCtrl.AutoExposure,
                ChangeAutoExpo);

                exposureCtrl.OnChangeAuto += (onOff) =>
                {
                    itemAutoExpo.OnOff = onOff;
                    SwitchExposureUIInteractable();
                };

                // ExposureTime
                itemExpoTime = settings.AddItem("Exposure\nTime", exposureCtrl.TimeMin,
                    exposureCtrl.TimeMax, ImageExposureController.TimeStep,
                    exposureCtrl.ExposureTime, ChangeExpoTime, -2, 0, lineAlpha);

                exposureCtrl.OnChangeTime += (time) =>
                {
                    itemExpoTime.Value = time;
                };

                exposureCtrl.OnChangeProperty += UpdateExposureUI;
            }
        }

        /// <summary>
        /// Toggle AutoExposure
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeAutoExpo(bool onOff)
        {
            exposureCtrl.AutoExposure = onOff;
        }

        /// <summary>
        /// Change ExposureTime
        /// </summary>
        /// <param name="val">ExposureTime</param>
        void ChangeExpoTime(float val)
        {
            exposureCtrl.ExposureTime = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Update Exposure-related UI
        /// </summary>
        protected virtual void UpdateExposureUI()
        {
            // When ExposureTime is variable, AutoExposure can be changed
            if (itemAutoExpo)
            {
                itemAutoExpo.Interactable = (exposureCtrl.TimeMin != exposureCtrl.TimeMax);
                itemAutoExpo.OnOff = exposureCtrl.AutoExposure;
            }

            if (itemExpoTime)
            {
                itemExpoTime.Min = exposureCtrl.TimeMin;
                itemExpoTime.Max = exposureCtrl.TimeMax;
                itemExpoTime.Value = exposureCtrl.ExposureTime;

                SwitchExposureUIInteractable();
            }
        }

        /// <summary>
        /// Toggle interactability of Exposure-related UI
        /// </summary>
        protected virtual void SwitchExposureUIInteractable()
        {
            itemExpoTime.Interactable = (!itemAutoExpo.OnOff);
        }

        /// <summary>
        /// Event called when the state of the panel changes
        /// </summary>
        /// <param name="onOff">open/close</param>
        void OnChangePanel(bool onOff)
        {
            if (onOff)
            {
                itemMode.Index = mgrCtrl.Index;
            }
        }
    }
}
