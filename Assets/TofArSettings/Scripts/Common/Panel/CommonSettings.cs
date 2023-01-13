/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings
{
    public class CommonSettings : UI.SettingsBase
    {
        MaxDepthDistanceController maxDepthDistanceController;
        UI.ItemDropdown maxDepthDistance;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIMaxDepthDistance
            };

            maxDepthDistanceController = FindObjectOfType<MaxDepthDistanceController>();
            controllers.Add(maxDepthDistanceController);

            base.Start();
        }


        /// <summary>
        /// Make maximum depth distance UI
        /// </summary>
        void MakeUIMaxDepthDistance()
        {
            maxDepthDistance = settings.AddItem("Maximum Depth Distance", maxDepthDistanceController.MaxDepthDistanceList,
                maxDepthDistanceController.MaxDepthDistanceIndex, ChangeMode);

            settings.Btn.OnClick += (onOff) =>
            {
                if (onOff)
                {
                    foreach (var panel in FindObjectsOfType<UI.Panel>())
                    {
                        if (panel == settings)
                        {
                            continue;
                        }

                        if (panel.IsOpen)
                        {
                            panel.ClosePanel();
                        }
                    }
                }
            };

            maxDepthDistanceController.OnChangeDistance += (index) =>
            {
                maxDepthDistance.Index = index;
            };
        }

        /// <summary>
        /// Change maximum depth distance
        /// </summary>
        /// <param name="index">Maximum depth distance index</param>
        void ChangeMode(int index)
        {
            maxDepthDistanceController.MaxDepthDistanceIndex = index;
        }


    }

}
