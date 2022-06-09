/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Slam;
using UnityEngine;
using UnityEngine.Events;


namespace TofArSettings.Slam
{
    public class SlamSettings : UI.SettingsBase
    {
        SlamManagerController managerController;
        UI.ItemToggle itemStartStream;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIStartStream,
            };
            managerController = FindObjectOfType<SlamManagerController>();
            controllers.Add(managerController);

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
    }
}
