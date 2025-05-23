﻿/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024,2025 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using TofAr.V0.Color;

using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Color
{
    public class ColorSettings : ImageSettings
    {
        ColorExposureController colorExposureCtrl;
        ColorFormatController formatCtrl;
        FocusController focusCtrl;
        StabilizationController stabCtrl;
        ColorWhiteBalanceController whiteBalanceCtrl;
        ColorCapturePhotoController capturePhotoController;
        ColorDirectFrameCaptureController directFrameCaptureEnabledController;

        UI.ItemSlider itemFrDu, itemSens, itemFocusDist;
        UI.ItemDropdown itemFlash, itemFormat, itemWhiteBalance, itemPhotoFormat;
        UI.ItemToggle itemAutoFocus, itemStab, itemAutoWhiteBalance, itemDirectFrameCaptureEnabled;
        UI.ItemButton itemCapturePhoto;

        UI.Panel panelMsg;

        void Awake()
        {
            // Get panel for displaying information
            foreach (var panel in GetComponentsInChildren<UI.Panel>())
            {
                if (panel.PanelObj.name.Contains("Message"))
                {
                    panelMsg = panel;
                    break;
                }
            }
        }

        void OnEnable()
        {
            TofArColorManager.OnStreamStarted += OnColorStreamStarted;
            TofArColorManager.OnStreamStopped += OnColorStreamStopped;

        }

        void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
            TofArColorManager.OnStreamStopped -= OnColorStreamStopped;

            // When not using this panel, also hide associated panels
            panelMsg.PanelObj.SetActive(false);
        }

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIMode,
                MakeUIDelay,
                MakeUIFormat,
                MakeUIFpsRequest,
                MakeUILensStabilization,
                MakeUIFocus,
                MakeUIExposure,
                MakeUIWhiteBalance,
                MakeUICapturePhoto,
                MakeUIDirectFrameCaptureEnabled,
            };

            mgrCtrl = FindAnyObjectByType<ColorManagerController>();

            formatCtrl = mgrCtrl.GetComponent<ColorFormatController>();
            controllers.Add(formatCtrl);
            focusCtrl = mgrCtrl.GetComponent<FocusController>();
            controllers.Add(focusCtrl);
            stabCtrl = mgrCtrl.GetComponent<StabilizationController>();
            controllers.Add(stabCtrl);
            whiteBalanceCtrl = mgrCtrl.GetComponent<ColorWhiteBalanceController>();
            controllers.Add(whiteBalanceCtrl);
            capturePhotoController = mgrCtrl.GetComponent<ColorCapturePhotoController>();
            controllers.Add(capturePhotoController);

            // Do not add as it is the same as exposureCtrl and is already added
            colorExposureCtrl = mgrCtrl.GetComponent<ColorExposureController>();

            directFrameCaptureEnabledController = mgrCtrl.GetComponent<ColorDirectFrameCaptureController>();
            controllers.Add(directFrameCaptureEnabledController);

            base.Start();
        }

        protected override void MakeUI()
        {
            base.MakeUI();

            AddDropdownToController(UI.SettingsBase.ComponentType.Color);

            // Setup UI
            UpdateFocusUI();
            UpdateWhiteBalanceUI();

            ShowMessage();
        }

        /// <summary>
        /// Make Color Format UI
        /// </summary>
        void MakeUIFormat()
        {
            itemFormat = settings.AddItem("Color Format", formatCtrl.FormatNames,
                formatCtrl.Index, ChangeFormat, 0, 0, 280);

            formatCtrl.OnChange += (index) =>
            {
                itemFormat.Index = index;
                ShowMessage();
            };

            mgrCtrl.OnChangeAfter += (index) =>
            {
                itemFormat.Interactable = index == 0;
            };

            if (mgrCtrl.IsStreamActive())
            {
                itemFormat.Interactable = false;
                itemDirectFrameCaptureEnabled.Interactable = false;
            }
        }

        /// <summary>
        /// Change ColorFormat
        /// </summary>
        /// <param name="index">ColorFormat index</param>
        void ChangeFormat(int index)
        {
            formatCtrl.Index = index;
        }

        /// <summary>
        /// Change MessagePanel display
        /// </summary>
        void ShowMessage()
        {
            // Display warning as BGR can not be displayed using Unity texture
            if (formatCtrl.Format == ColorFormat.BGR)
            {
                panelMsg.OpenPanel(false);
            }
            else
            {
                panelMsg.ClosePanel();
            }
        }

        /// <summary>
        /// Make Lens Stabilization UI
        /// </summary>
        void MakeUILensStabilization()
        {
            if (!TofArManager.Instance.UsingIos)
            {
                itemStab = settings.AddItem("Lens Stabilization",
                stabCtrl.LensStabilization, ChangeLensStabilization);

                stabCtrl.OnChange += (onOff) =>
                {
                    itemStab.OnOff = onOff;
                };
            }
        }

        /// <summary>
        /// Toggle LensStabilization
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeLensStabilization(bool onOff)
        {
            stabCtrl.LensStabilization = onOff;
        }

        /// <summary>
        /// Make Focus UI
        /// </summary>
        void MakeUIFocus()
        {
            // Auto Focus
            itemAutoFocus = settings.AddItem("Auto Focus", focusCtrl.AutoFocus,
                ChangeAutoFocus);

            focusCtrl.OnChangeAutoFocus += (onOff) =>
            {
                itemAutoFocus.OnOff = onOff;
                SwitchFocusUIInteractable();
            };

            // Distance
            itemFocusDist = settings.AddItem("Distance", focusCtrl.DistMin,
                focusCtrl.DistMax, FocusController.DistStep, focusCtrl.Distance,
                ChangeFocusDist, 0, 0, lineAlpha);

            focusCtrl.OnChangeDist += (val) =>
            {
                itemFocusDist.Value = val;
            };

            focusCtrl.OnChangeProperty += UpdateFocusUI;
        }

        /// <summary>
        /// Toggle Auto Focus
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeAutoFocus(bool onOff)
        {
            focusCtrl.AutoFocus = onOff;
        }

        /// <summary>
        /// Change Focus Distance
        /// </summary>
        /// <param name="val">Distance</param>
        void ChangeFocusDist(float val)
        {
            focusCtrl.Distance = val;
        }

        /// <summary>
        /// Update FocusMode-related UI
        /// </summary>
        void UpdateFocusUI()
        {
            if (itemAutoFocus)
            {
                itemAutoFocus.OnOff = focusCtrl.AutoFocus;
            }

            if (itemFocusDist)
            {
                itemFocusDist.Min = focusCtrl.DistMin;
                itemFocusDist.Max = focusCtrl.DistMax;
                itemFocusDist.Value = focusCtrl.Distance;

                SwitchFocusUIInteractable();
            }
            if (focusCtrl.DistMax <= focusCtrl.DistMin)
            {
                itemAutoFocus.Interactable = itemFocusDist.Interactable = false;
            }
        }

        /// <summary>
        /// Switch interactability of Focus-related UI
        /// </summary>
        void SwitchFocusUIInteractable()
        {
            if (focusCtrl.DistMax <= focusCtrl.DistMin)
            {
                itemFocusDist.Interactable = false;
            }
            else
            {
                itemFocusDist.Interactable = !itemAutoFocus.OnOff;
            }
        }

        protected override void MakeUIExposure()
        {
            if (!TofArManager.Instance.UsingIos)
            {
                base.MakeUIExposure();

                // Frame Duration
                itemFrDu = settings.AddItem("Frame\nDuration", colorExposureCtrl.FrameDurationMin,
                    colorExposureCtrl.FrameDurationMax, ColorExposureController.FrameDurationStep,
                    colorExposureCtrl.FrameDuration, ChangeFrameDuration, 0, 0, lineAlpha);

                colorExposureCtrl.OnChangeFrameDuration += (frameDuration) =>
                {
                    itemFrDu.Value = frameDuration;
                };

                // Sensitivity
                itemSens = settings.AddItem("Sensitivity", colorExposureCtrl.SensitivityMin,
                    colorExposureCtrl.SensitivityMax, ColorExposureController.SensitivityStep,
                    colorExposureCtrl.Sensitivity, ChangeSensitivity, 0, 0, lineAlpha);

                colorExposureCtrl.OnChangeSensitivity += (sen) =>
                {
                    itemSens.Value = sen;
                };


            }

            // Flash Mode
            itemFlash = settings.AddItem("Flash Mode", colorExposureCtrl.FlashNames,
                colorExposureCtrl.FlashIndex, ChangeFlashMode);

            colorExposureCtrl.OnChangeFlashMode += (index) =>
            {
                itemFlash.Index = index;
            };
        }

        /// <summary>
        /// Change FrameDuration
        /// </summary>
        /// <param name="val">FrameDuration</param>
        void ChangeFrameDuration(float val)
        {
            colorExposureCtrl.FrameDuration = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Change Sensitivity
        /// </summary>
        /// <param name="val">Sensitivity</param>
        void ChangeSensitivity(float val)
        {
            colorExposureCtrl.Sensitivity = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Make WhiteBalance UI
        /// </summary>
        void MakeUIWhiteBalance()
        {
            if (!TofArManager.Instance.UsingIos)
            {
                itemAutoWhiteBalance = settings.AddItem("Auto WhiteBalance", whiteBalanceCtrl.AutoWhiteBalance,
                ChangeAutoWhiteBalance);

                whiteBalanceCtrl.OnChangeAutoWhiteBalance += (onOff) =>
                {
                    itemAutoWhiteBalance.OnOff = onOff;
                    SwitchWhiteBalanceUIInteractable();
                };

                itemWhiteBalance = settings.AddItem("White\nBalanceMode", whiteBalanceCtrl.WhiteBalanceNames,
                    whiteBalanceCtrl.Index, ChangeWhiteBalanceMode, -2, 0, 0, lineAlpha);

                whiteBalanceCtrl.OnChange += (index) =>
                {
                    itemWhiteBalance.Index = index;
                };

                whiteBalanceCtrl.OnChangeProperty += UpdateWhiteBalanceUI;
            }
        }

        /// <summary>
        /// Toggle AutoWhiteBalance
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeAutoWhiteBalance(bool onOff)
        {
            whiteBalanceCtrl.AutoWhiteBalance = onOff;
        }

        /// <summary>
        /// Change WhiteBalanceMode
        /// </summary>
        /// <param name="index">WhiteBalanceMode index</param>
        void ChangeWhiteBalanceMode(int index)
        {
            whiteBalanceCtrl.Index = index;
        }

        /// <summary>
        /// Update WhiteBalance-related UI
        /// </summary>
        void UpdateWhiteBalanceUI()
        {
            if (itemAutoWhiteBalance)
            {
                itemAutoWhiteBalance.OnOff = whiteBalanceCtrl.AutoWhiteBalance;
            }
            if (itemWhiteBalance)
            {
                itemWhiteBalance.Index = whiteBalanceCtrl.Index;
            }
            SwitchWhiteBalanceUIInteractable();
        }

        /// <summary>
        /// Switch interactability of WhiteBalance-related UI
        /// </summary>
        void SwitchWhiteBalanceUIInteractable()
        {
            if (itemWhiteBalance)
            {
                itemWhiteBalance.Interactable = (!itemAutoWhiteBalance.OnOff);
            }
        }

        /// <summary>
        /// Change FlashMode
        /// </summary>
        /// <param name="index">FlashMode index</param>
        void ChangeFlashMode(int index)
        {
            colorExposureCtrl.FlashIndex = index;
        }

        /// <summary>
        /// Update Exposure-related UI
        /// </summary>
        protected override void UpdateExposureUI()
        {
            base.UpdateExposureUI();

            if (itemFrDu)
            {
                itemFrDu.Min = colorExposureCtrl.FrameDurationMin;
                itemFrDu.Max = colorExposureCtrl.FrameDurationMax;
                itemFrDu.Value = colorExposureCtrl.FrameDuration;
            }

            if (itemSens)
            {
                itemSens.Min = colorExposureCtrl.SensitivityMin;
                itemSens.Max = colorExposureCtrl.SensitivityMax;
                itemSens.Value = colorExposureCtrl.Sensitivity;
            }

            if (itemFlash)
            {
                itemFlash.Index = colorExposureCtrl.FlashIndex;
            }
        }

        protected override void SwitchExposureUIInteractable()
        {
            base.SwitchExposureUIInteractable();
            if (itemFrDu)
            {
                itemFrDu.Interactable = (!itemAutoExpo.OnOff);
            }

            if (itemSens)
            {
                itemSens.Interactable = (!itemAutoExpo.OnOff);
            }
        }

        /// <summary>
        /// Make CapturePhoto UI
        /// </summary>
        void MakeUICapturePhoto()
        {
            if (TofArManager.Instance.UsingIos)
            {
                itemCapturePhoto = settings.AddItem("Capture Photo", OnClickCapturePhoto);

                itemPhotoFormat = settings.AddItem("Photo Format", capturePhotoController.FormatNames,
                    capturePhotoController.Index, ChangePhotoFormat);

                SetInteractableUICapturePhoto(false);

                capturePhotoController.OnChange += (index) =>
                {
                    itemPhotoFormat.Index = index;
                };
            }
        }

        private void SetInteractableUICapturePhoto(bool state)
        {
            if (itemCapturePhoto != null && itemPhotoFormat != null)
            {
                itemCapturePhoto.Interactable = state;
                itemPhotoFormat.Interactable = state;
            }
        }

        private void OnClickCapturePhoto()
        {
            if (TofArColorManager.Instance.IsStreamActive)
            {
                capturePhotoController.SetProperty();
            }
        }

        private void ChangePhotoFormat(int index)
        {
            capturePhotoController.Index = index;
        }

        private void OnColorStreamStarted(object sender, Texture2D colorTex)
        {
            if (TofArManager.Instance.UsingIos && mgrCtrl != null && mgrCtrl is ColorManagerController)
            {
                SetInteractableUICapturePhoto((mgrCtrl as ColorManagerController).CurrentResolution.enablePhoto);
            }
            SetInteractableUIDirectFrameCaptureEnable(false);
        }

        private void OnColorStreamStopped(object sender)
        {
            if (TofArManager.Instance?.UsingIos == true)
            {
                SetInteractableUICapturePhoto(false);
            }
            SetInteractableUIDirectFrameCaptureEnable(true);
        }

        /// <summary>
        /// Make DirectFrameCaptureEnabled UI
        /// </summary>
        void MakeUIDirectFrameCaptureEnabled()
        {
            var platformConfig = TofArManager.Instance.GetProperty<PlatformConfigurationProperty>();
            if (platformConfig?.platformConfigurationPC?.customData?.Contains("k4a") == true)
            {
                // DirectFrameCaptureEnabled
                itemDirectFrameCaptureEnabled = settings.AddItem("Direct Frame Capture", directFrameCaptureEnabledController.DirectFrameCaptureEnabled,
                    ChangeDirectFrameCaptureEnabled);

                directFrameCaptureEnabledController.OnChangeDirectFrameCaptureEnabled += (onOff) =>
                {
                    itemDirectFrameCaptureEnabled.OnOff = onOff;
                };

                directFrameCaptureEnabledController.OnChangeProperty += UpdateDirectFrameCaptureEnabledUI;
            }
        }

        /// <summary>
        /// Toggle DirectFrameCaptureEnabled
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeDirectFrameCaptureEnabled(bool onOff)
        {
            directFrameCaptureEnabledController.DirectFrameCaptureEnabled = onOff;
        }

        /// <summary>
        /// Update DirectFrameCaptureEnabled-related UI
        /// </summary>
        void UpdateDirectFrameCaptureEnabledUI()
        {
            if (itemDirectFrameCaptureEnabled)
            {
                itemDirectFrameCaptureEnabled.OnOff = directFrameCaptureEnabledController.DirectFrameCaptureEnabled;
            }
        }

        private void SetInteractableUIDirectFrameCaptureEnable(bool state)
        {
            if (itemDirectFrameCaptureEnabled != null)
            {
                itemDirectFrameCaptureEnabled.Interactable = state;
            }
        }
    }
}
