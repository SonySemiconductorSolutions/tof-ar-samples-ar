/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using TofAr.V0.Slam;
using UnityEngine;
using UnityEngine.Events;


namespace TofArSettings.Slam
{
    public class SlamSettings : UI.SettingsBase
    {
        SlamManagerController managerController;
        General.CameraApiController cameraApiController;

        UI.ItemToggle itemStartStream;
        UI.ItemDropdown itemPoseSource;
        
        /// <summary>
        /// アプリ起動後(Awake後)の動作(Unity標準関数)
        /// </summary>
        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIStartStream,
                MakeUIPoseSource
            };
            managerController = FindObjectOfType<SlamManagerController>();
            controllers.Add(managerController);

            cameraApiController = FindObjectOfType<General.CameraApiController>();
            cameraApiController.OnChangeApi += (idx) =>
            {
                UpdateInteractability();
            };

            base.Start();
        }

        void MakeUIStartStream()
        {
            itemStartStream = settings.AddItem("Start Stream", managerController.IsStreamActive(), ChangeStartStream);
            managerController.OnStreamStartStatusChanged += (val) =>
            {
                itemStartStream.OnOff = val;
            };
        }

        void MakeUIPoseSource()
        {
            itemPoseSource = settings.AddItem("Camera Pose Source", managerController.PoseSourceNames, managerController.Index, ChangePoseSource);
            managerController.OnChangeIndex += (index) =>
            {
                itemPoseSource.Index = index;
            };

            if (TofAr.V0.TofArManager.Instance.UsingIos)
            {
                UpdateInteractability();
            }
        }

        private void UpdateInteractability()
        {
            bool interactable = cameraApiController.CameraApi == TofAr.V0.IosCameraApi.ArKit;
            if (itemPoseSource)
            {
                itemPoseSource.Interactable = interactable;
            }
        }


        /// <summary>
        /// If Slam stream occured or not
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

        void ChangePoseSource(int index)
        {
            if (managerController.Index != index)
            {
                if (managerController.IsStreamActive())
                {
                    managerController.StopStream();
                    managerController.Index = index;
                    managerController.StartStream();
                }
                else
                {
                    managerController.Index = index;
                }
            }

        }

    }
}
