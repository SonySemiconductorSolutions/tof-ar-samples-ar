/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.General
{
    public class GeneralSettingsChild : UI.SettingsBase
    {
        TofArFramerateController tofArFramerateCtrl;

        CameraApiController cameraApiCtrl;
        DepthFilteringController depthFilterCtrl;

        UI.ItemSlider itemTofArFramerate;
        UI.ItemDropdown itemCameraApi, itemFiltering;

        private DependendStreamCameraApiHandler componentCtrl;

        MirrorSettingsController mirrorSettingsCtrl;

        UI.ItemToggle itemMirror;


        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUICameraApi,
                MakeUIFramerate,
                MakeUIDepthFilteringMode,
                MakeUIMirrorSettings
            };

            componentCtrl = FindObjectOfType<DependendStreamCameraApiHandler>();

            tofArFramerateCtrl = GetComponent<TofArFramerateController>();
            controllers.Add(tofArFramerateCtrl);
            cameraApiCtrl = GetComponent<CameraApiController>();
            controllers.Add(cameraApiCtrl);
            depthFilterCtrl = GetComponent<DepthFilteringController>();
            controllers.Add(depthFilterCtrl);

            mirrorSettingsCtrl = GetComponent<MirrorSettingsController>();
            controllers.Add(mirrorSettingsCtrl);

            base.Start();
        }

        /// <summary>
        /// Make Framerate UI
        /// </summary>
        void MakeUIFramerate()
        {
            if (TofArManager.Instance.UsingIos)
            {
                // ToFAR Session Framerate
                itemTofArFramerate = settings.AddItem("ToFAR Framerate",
                    TofArFramerateController.FramerateMin,
                    TofArFramerateController.FramerateMax,
                    TofArFramerateController.FramerateStep,
                    tofArFramerateCtrl.Framerate, ChangeTofArFramerate, -4);

                tofArFramerateCtrl.OnChangeFramerate += (val) =>
                {
                    itemTofArFramerate.Value = val;
                };

                itemTofArFramerate.Interactable = cameraApiCtrl.CameraApi != IosCameraApi.AvFoundation;
            }
        }

        void MakeUICameraApi()
        {
            if (TofArManager.Instance.UsingIos)
            {
                // ToFAR Session Framerate
                itemCameraApi = settings.AddItem("Camera API", cameraApiCtrl.ApiNames, cameraApiCtrl.ApiIndex, OnChangeCameraApi);

                cameraApiCtrl.OnChangeApi += (idx) =>
                {
                    itemCameraApi.Index = idx;
                };

                componentCtrl.AddDropdown(itemCameraApi, ComponentType.Color);
                componentCtrl.AddDropdown(itemCameraApi, ComponentType.Tof);
            }
        }

        void MakeUIDepthFilteringMode()
        {
            if (TofArManager.Instance.UsingIos)
            {
                // ToFAR Session Framerate
                itemFiltering = settings.AddItem("Depth Filtering", depthFilterCtrl.DepthFilteringModeNames, depthFilterCtrl.ModeIndex, OnChangeDepthFilteringMode);

                depthFilterCtrl.OnChangeDepthFilteringMode += (idx) =>
                {
                    itemFiltering.Index = idx;
                };

                itemFiltering.Interactable = cameraApiCtrl.CameraApi == IosCameraApi.AvFoundation;
            }
        }

        void UpdateInteractibility()
        {
            if (TofArManager.Instance.UsingIos)
            {
                bool enableUIAvFoundation = cameraApiCtrl.CameraApi == IosCameraApi.AvFoundation;

                itemFiltering.Interactable = enableUIAvFoundation;
                itemTofArFramerate.Interactable = !enableUIAvFoundation;
            }
        }

        /// <summary>
        /// Make Mirror UI
        /// </summary>
        void MakeUIMirrorSettings()
        {
            // Mirroring settings
            itemMirror = settings.AddItem("Enable Mirroring", mirrorSettingsCtrl.OnOff, ChangeMirrorHorizontal, -4);

            mirrorSettingsCtrl.OnChangeMirrorSettings += (isMirroring) =>
            {
                itemMirror.OnOff = isMirroring;
            };
        }

        /// <summary>
        /// Change ToFAR Session Framerate
        /// </summary>
        /// <param name="val">ToFAR Session Framerate</param>
        void ChangeTofArFramerate(float val)
        {
            tofArFramerateCtrl.Framerate = Mathf.RoundToInt(val);
        }

        void OnChangeCameraApi(int index)
        {
            cameraApiCtrl.ApiIndex = index;
            UpdateInteractibility();
        }

        void OnChangeDepthFilteringMode(int index)
        {
            depthFilterCtrl.ModeIndex = index;
        }

        /// <summary>
        /// Change Mirroring settings
        /// </summary>
        /// <param name="val">Enable/Disable mirroring</param>
        void ChangeMirrorHorizontal(bool val)
        {
            mirrorSettingsCtrl.OnOff = val;
        }

    }
}
