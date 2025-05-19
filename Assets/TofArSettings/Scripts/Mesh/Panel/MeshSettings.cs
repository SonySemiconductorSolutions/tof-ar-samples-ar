/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Mesh;
using UnityEngine;
using UnityEngine.Events;


namespace TofArSettings.Mesh
{
    public class MeshSettings : UI.SettingsBase
    {
        ReductionLevelController reductionLevelController;
        MeshManagerController managerController;
        UI.ItemSlider itemReductionLevel;
        UI.ItemToggle itemStartStream;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIStartStream,
                MakeUIReductionLevel
            };

            managerController = FindAnyObjectByType<MeshManagerController>();
            controllers.Add(managerController);
            reductionLevelController = managerController.GetComponent<ReductionLevelController>();
            controllers.Add(reductionLevelController);

            base.Start();

            settings.OnChangeStart += OnChangePanel;
        }
        /// <summary>
        /// Make ReductionLevel UI
        /// </summary>
        void MakeUIReductionLevel()
        {
            itemReductionLevel = settings.AddItem("Reduction\nLevel",
                ReductionLevelController.Min, ReductionLevelController.Max,
                ReductionLevelController.Step, reductionLevelController.ReductionLevel,
                ChangeReductionLevel);

            reductionLevelController.OnChange += (val) =>
            {
                itemReductionLevel.Value = val;
            };
        }

        /// <summary>
        /// Change ReductionLevel
        /// </summary>
        /// <param name="val">ReductionLevel</param>
        void ChangeReductionLevel(float val)
        {
            reductionLevelController.ReductionLevel = (int)val;
        }

        /// <summary>
        /// Make StartStream UI
        /// </summary>
        void MakeUIStartStream()
        {
            itemStartStream = settings.AddItem("Start Stream", TofArMeshManager.Instance.autoStart, ChangeStartStream);
            managerController.OnStreamStartStatusChanged += (val) =>
            {
                itemStartStream.OnOff = val;
            };
        }

        /// <summary>
        /// If Mesh stream oocurs or not
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
                itemStartStream.OnOff = TofArMeshManager.Instance.IsStreamActive;
            }
        }
    }
}
