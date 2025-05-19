/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Segmentation;
using TofAr.V0.Segmentation.Human;
using TofAr.V0.Segmentation.Sky;
using UnityEngine.Events;


namespace TofArSettings.Segmentation
{
    public class SegmentationSettings : UI.SettingsBase
    {
        UI.ItemToggle itemSkySegmentation, itemHumanSegmentation;
        UI.ItemToggle itemNotSkySegmentation, itemNotHumanSegmentation;
        UI.ItemToggle itemStartStream;

        private SkySegmentationController skySegCtrl;
        private HumanSegmentationController humanSegCtrl;
        private SegmentationManagerController managerController;

        private SkySegmentationDetector skyDetector;
        private HumanSegmentationDetector humanDetector;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIStartStream,
                MakeUIHuman,
                MakeUISky
            };

            managerController = FindAnyObjectByType<SegmentationManagerController>();
            controllers.Add(managerController);
            skySegCtrl = managerController.GetComponent<SkySegmentationController>();
            controllers.Add(skySegCtrl);
            humanSegCtrl = managerController.GetComponent<HumanSegmentationController>();
            controllers.Add(humanSegCtrl);

            humanDetector = FindAnyObjectByType<HumanSegmentationDetector>();
            skyDetector = FindAnyObjectByType<SkySegmentationDetector>();

            base.Start();

            settings.OnChangeStart += OnChangePanel;
        }


        /// <summary>
        /// Make SkySegmentationState UI
        /// </summary>
        void MakeUISky()
        {
            itemSkySegmentation = settings.AddItem("Sky Segmentation", skyDetector.IsActive, ChangeSkySegmentation
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
        }

        /// <summary>
        /// Change NotSkySegmentationState
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeNotSkySegmentation(bool onOff)
        {
            skySegCtrl.NotSkySegmentationEnabled = onOff;
        }

        /// <summary>
        /// Make HumanSegmentation UI
        /// </summary>
        void MakeUIHuman()
        {
            itemHumanSegmentation = settings.AddItem("Human Segmentation", humanDetector.IsActive, ChangeHumanSegmentation
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
        }

        /// <summary>
        /// Change NotHumanSegmentationState
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeNotHumanSegmentation(bool onOff)
        {
            humanSegCtrl.NotHumanSegmentationEnabled = onOff;
        }

        /// <summary>
        /// Make StartStream UI
        /// </summary>
        void MakeUIStartStream()
        {
            itemStartStream = settings.AddItem("Start Stream", TofArSegmentationManager.Instance.autoStart, ChangeStartStream);
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
        /// Event called when the state of the panel changes
        /// </summary>
        /// <param name="onOff">open/close</param>
        void OnChangePanel(bool onOff)
        {
            if (onOff)
            {
                itemStartStream.OnOff = TofArSegmentationManager.Instance.IsStreamActive;
            }
        }
    }
}
