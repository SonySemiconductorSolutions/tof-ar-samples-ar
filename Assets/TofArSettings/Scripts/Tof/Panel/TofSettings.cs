/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine.Events;

namespace TofArSettings.Tof
{
    public class TofSettings : ImageSettings
    {
        ResortConfidenceController resortConfiCtrl;

        UI.ItemToggle itemResortConfi;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIMode,
                MakeUIDelay,
                MakeUIFpsRequest,
                MakeUIExposure,
                MakeUIResort
            };

            mgrCtrl = FindAnyObjectByType<TofManagerController>();
            resortConfiCtrl = mgrCtrl.GetComponent<ResortConfidenceController>();
            controllers.Add(resortConfiCtrl);

            base.Start();
        }

        protected override void MakeUI()
        {
            base.MakeUI();

            AddDropdownToController(ComponentType.Tof);
        }

        /// <summary>
        /// Make Resort Confidence UI
        /// </summary>
        void MakeUIResort()
        {
            if (!TofAr.V0.TofArManager.Instance.UsingIos)
            {
                itemResortConfi = settings.AddItem("Resort Confidence",
                  resortConfiCtrl.Resort, ChangeResortConfi);

                resortConfiCtrl.OnChange += (onOff) =>
                {
                    itemResortConfi.OnOff = onOff;
                };
            }
        }

        /// <summary>
        /// Change Resort Confidence value
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeResortConfi(bool onOff)
        {
            resortConfiCtrl.Resort = onOff;
        }
    }
}
