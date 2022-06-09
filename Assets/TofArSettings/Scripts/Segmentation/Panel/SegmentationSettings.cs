/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Segmentation;
using UnityEngine;
using UnityEngine.Events;


namespace TofArSettings.Segmentation
{
    public class SegmentationSettings : UI.SettingsBase
    {
        UI.ItemToggle itemSkySegmentation, itemHumanSegmentation;
        UI.ItemToggle itemNotSkySegmentation, itemNotHumanSegmentation;

        private SkySegmentationController skySegCtrl;
        private HumanSegmentationController humanSegCtrl;
        private SegmentationManagerController managerController;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIHuman,
                MakeUISky
            };

            skySegCtrl = FindObjectOfType<SkySegmentationController>();
            controllers.Add(skySegCtrl);
            humanSegCtrl = FindObjectOfType<HumanSegmentationController>();
            controllers.Add(humanSegCtrl);
            managerController = FindObjectOfType<SegmentationManagerController>();
            controllers.Add(managerController);

            base.Start();
        }


        /// <summary>
        /// Make SkySegmentationState UI
        /// </summary>
        void MakeUISky()
        {
            itemSkySegmentation = settings.AddItem("Sky Segmentation", skySegCtrl.SkySegmentationEnabled, ChangeSkySegmentation
                );

            skySegCtrl.OnSkyChange += (onOff) =>
            {
                itemSkySegmentation.OnOff = onOff;
            };

            itemNotSkySegmentation = settings.AddItem("Not Sky Segmentation", skySegCtrl.NotSkySegmentationEnabled, ChangeNotSkySegmentation
                );

            skySegCtrl.OnNotSkyChange += (onOff) =>
            {
                itemNotSkySegmentation.OnOff = onOff;
            };
        }

        /// <summary>
        /// Change SkySegmentationState
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeSkySegmentation(bool onOff)
        {
            skySegCtrl.SkySegmentationEnabled = onOff;
            StartOrStopSegmentation();
        }

        /// <summary>
        /// Change NotSkySegmentationState
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeNotSkySegmentation(bool onOff)
        {
            skySegCtrl.NotSkySegmentationEnabled = onOff;
            StartOrStopSegmentation();
        }

        /// <summary>
        /// Make HumanSegmentation UI
        /// </summary>
        void MakeUIHuman()
        {
            itemHumanSegmentation = settings.AddItem("Human Segmentation", humanSegCtrl.HumanSegmentationEnabled, ChangeHumanSegmentation
                );

            humanSegCtrl.OnHumanChange += (onOff) =>
            {
                itemHumanSegmentation.OnOff = onOff;
            };

            itemNotHumanSegmentation = settings.AddItem("Not Human Segmentation", humanSegCtrl.NotHumanSegmentationEnabled, ChangeNotHumanSegmentation
                );

            humanSegCtrl.OnNotHumanChange += (onOff) =>
            {
                itemNotHumanSegmentation.OnOff = onOff;
            };
        }

        /// <summary>
        /// Change HumanSegmentationState
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeHumanSegmentation(bool onOff)
        {
            humanSegCtrl.HumanSegmentationEnabled = onOff;
            StartOrStopSegmentation();
        }

        /// <summary>
        /// Change NotHumanSegmentationState
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeNotHumanSegmentation(bool onOff)
        {
            humanSegCtrl.NotHumanSegmentationEnabled = onOff;
            StartOrStopSegmentation();
        }

        private void StartOrStopSegmentation()
        {
            if (skySegCtrl.IsSkySegRunning || humanSegCtrl.IsHumanSegRunning)
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
