/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
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
        InternalSessionSettingsController internalSessionSettingsCtrl;

        UI.ItemToggle itemMirror;
        UI.ItemToggle itemInternalSession;


        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIInternalSessionSettings,
                MakeUICameraApi,
                MakeUIFramerate,
                MakeUIDepthFilteringMode,
                MakeUIMirrorSettings
            };

            componentCtrl = FindAnyObjectByType<DependendStreamCameraApiHandler>();

            tofArFramerateCtrl = GetComponent<TofArFramerateController>();
            controllers.Add(tofArFramerateCtrl);
            cameraApiCtrl = GetComponent<CameraApiController>();
            controllers.Add(cameraApiCtrl);
            depthFilterCtrl = GetComponent<DepthFilteringController>();
            controllers.Add(depthFilterCtrl);

            mirrorSettingsCtrl = GetComponent<MirrorSettingsController>();
            controllers.Add(mirrorSettingsCtrl);

            internalSessionSettingsCtrl = GetComponent<InternalSessionSettingsController>();
            controllers.Add(internalSessionSettingsCtrl);

            base.Start();
        }

        private void OnEnable()
        {
            TofArManager.Instance?.postInternalSessionStart.AddListener(OnInternalSessionStarted);
            TofArManager.Instance?.postInternalSessionStop.AddListener(OnInternalSessionStopped);
        }

        private void OnDisable()
        {
            TofArManager.Instance?.postInternalSessionStart.RemoveListener(OnInternalSessionStarted);
            TofArManager.Instance?.postInternalSessionStop.RemoveListener(OnInternalSessionStopped);
        }

        /// <summary>
        /// Make Framerate UI
        /// </summary>
        void MakeUIFramerate()
        {
            if (TofArManager.Instance.UsingIos)
            {
                // ToFAR Session Framerate
                itemTofArFramerate = settings.AddItem("ToFAR\nFramerate",
                    TofArFramerateController.FramerateMin,
                    TofArFramerateController.FramerateMax,
                    TofArFramerateController.FramerateStep,
                    tofArFramerateCtrl.Framerate, ChangeTofArFramerate, -2);

                tofArFramerateCtrl.OnChangeFramerate += (val) =>
                {
                    itemTofArFramerate.Value = val;
                };

                itemTofArFramerate.Interactable = cameraApiCtrl.CameraApi != IosCameraApi.AvFoundation && TofArManager.Instance.InternalSessionStarted;
            }
        }

        void MakeUICameraApi()
        {
            if (TofArManager.Instance.UsingIos)
            {
                // ToFAR Session Framerate
                itemCameraApi = settings.AddItem("Camera API", cameraApiCtrl.ApiNames,
                    cameraApiCtrl.ApiIndex, OnChangeCameraApi);

                cameraApiCtrl.OnChangeApi += (idx) =>
                {
                    itemCameraApi.Index = idx;
                };

                if (componentCtrl)
                {
                    componentCtrl.AddDropdown(itemCameraApi, ComponentType.Color);
                    componentCtrl.AddDropdown(itemCameraApi, ComponentType.Tof);
                }

                itemCameraApi.Interactable = TofArManager.Instance.InternalSessionStarted;
            }
        }

        void MakeUIDepthFilteringMode()
        {
            if (TofArManager.Instance.UsingIos)
            {
                // ToFAR Session Framerate
                itemFiltering = settings.AddItem("Depth Filtering", depthFilterCtrl.DepthFilteringModeNames,
                    depthFilterCtrl.ModeIndex, OnChangeDepthFilteringMode);

                depthFilterCtrl.OnChangeDepthFilteringMode += (idx) =>
                {
                    itemFiltering.Index = idx;
                };

                itemFiltering.Interactable = cameraApiCtrl.CameraApi == IosCameraApi.AvFoundation && TofArManager.Instance.InternalSessionStarted;
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
            itemMirror = settings.AddItem("Enable Mirroring", mirrorSettingsCtrl.OnOff, ChangeMirrorHorizontal);

            mirrorSettingsCtrl.OnChangeMirrorSettings += (isMirroring) =>
            {
                itemMirror.OnOff = isMirroring;
            };

            itemMirror.Interactable = TofArManager.Instance.InternalSessionStarted;
        }

        /// <summary>
        /// Make Internal Session UI
        /// </summary>
        void MakeUIInternalSessionSettings()
        {
#if UNITY_EDITOR
            // Internal session settings
            itemInternalSession = settings.AddItem("Enable Internal Session", TofArManager.Instance.InternalSessionStarted, ChangeInternalSessionState);
            
            internalSessionSettingsCtrl.OnChangeInternalSessionSettings += (isInternalSessionOn) =>
            {
                itemInternalSession.OnOff = isInternalSessionOn;
            };
#endif
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

        private void OnInternalSessionStarted()
        {
            ToggleItemsInteractability(true);
        }

        private void OnInternalSessionStopped()
        {
            ToggleItemsInteractability(false);
        }

        private void ToggleItemsInteractability(bool interactable)
        {
            if(itemCameraApi != null)
            {
                itemCameraApi.Interactable = interactable;
            }
            if (itemFiltering!= null)
            {
                itemFiltering.Interactable = interactable;
            }
            if (itemMirror != null)
            {
                itemMirror.Interactable = interactable;
            }
            if (itemTofArFramerate != null)
            {
                itemTofArFramerate.Interactable = interactable;
            }
        }

        /// <summary>
        /// Change Mirroring settings
        /// </summary>
        /// <param name="val">Enable/Disable mirroring</param>
        void ChangeMirrorHorizontal(bool val)
        {
            mirrorSettingsCtrl.OnOff = val;
        }

        /// <summary>
        /// Change Internal Session on or off
        /// </summary>
        /// <param name="val">On/Off Internal Session</param>
        void ChangeInternalSessionState(bool val)
        {
            internalSessionSettingsCtrl.OnOff = val;
        }

    }
}
