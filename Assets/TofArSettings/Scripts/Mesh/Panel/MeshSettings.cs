/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
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
            reductionLevelController = FindObjectOfType<ReductionLevelController>();
            controllers.Add(reductionLevelController);
            managerController = FindObjectOfType<MeshManagerController>();
            controllers.Add(managerController);

            base.Start();
        }
        /// <summary>
        /// Make ReductionLevel UI
        /// </summary>
        void MakeUIReductionLevel()
        {
            itemReductionLevel = settings.AddItem("Reduction Level",
                ReductionLevelController.Min, ReductionLevelController.Max,
                ReductionLevelController.Step, reductionLevelController.ReductionLevel,
                ChangeReductionLevel, -4);

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
    }
}
