/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Face
{
    public class FaceSettings : UI.SettingsBase
    {
        FaceRuntimeController runtimeController;
        FaceEstimatorController faceEstimatorController;

        UI.ItemDropdown itemMode, itemRuntimeMode;
        UI.ItemSlider itemThread;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIDetectorType,
                MakeUIRuntime
            };

            runtimeController = FindObjectOfType<FaceRuntimeController>();
            controllers.Add(runtimeController);
            faceEstimatorController = FindObjectOfType<FaceEstimatorController>();
            controllers.Add(faceEstimatorController);

            base.Start();
        }

        /// <summary>
        /// Make Body dictionary UI
        /// </summary>
        void MakeUIDetectorType()
        {
            itemMode = settings.AddItem("Detector Type", runtimeController.DetectorTypeNames,
                runtimeController.DetectorTypeIndex, ChangeMode);

            runtimeController.OnChangeDetectorType += (index) =>
            {
                itemMode.Index = index;
                SetExternalInteractability();
            };
        }

        /// <summary>
        /// Change Body dictionary
        /// </summary>
        /// <param name="index">Hand dictionary index</param>
        void ChangeMode(int index)
        {
            runtimeController.DetectorTypeIndex = index;
        }

        /// <summary>
        /// Make Runtime UI
        /// </summary>
        void MakeUIRuntime()
        {
            itemRuntimeMode = settings.AddItem("Face Runtime", faceEstimatorController.ExecModeNames,
                faceEstimatorController.ExecModeIndex, ChangeRuntimeMode);
            itemThread = settings.AddItem("Face Threads",
                FaceEstimatorController.ThreadMin, FaceEstimatorController.ThreadMax,
                FaceEstimatorController.ThreadStep, faceEstimatorController.ModeThreads,
                ChangeThreads);

            faceEstimatorController.OnChangeRuntimeMode += (index) =>
            {
                itemRuntimeMode.Index = index;
                itemThread.Interactable = faceEstimatorController.IsInteractableModeThreads;
            };

            faceEstimatorController.OnUpdateRuntimeModeList += (list, runtimeModeIndex) =>
            {
                itemRuntimeMode.Options = list;
                itemRuntimeMode.Index = runtimeModeIndex;
            };

            faceEstimatorController.OnChangeModeThreads += (val) =>
            {
                itemThread.Value = val;
            };

            SetExternalInteractability();
        }

        /// <summary>
        /// Enable or disable the interactability of external UI based on the runtime controller
        /// </summary>
        void SetExternalInteractability()
        {
            var interactible = runtimeController.DetectorTypeIndex > 0 &&
                runtimeController.DetectorType == TofAr.V0.Face.FaceDetectorType.External;
            itemRuntimeMode.Interactable = interactible;
            itemThread.Interactable = interactible && faceEstimatorController.IsInteractableModeThreads;
        }

        /// <summary>
        /// Change RuntimeMode1
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeRuntimeMode(int index)
        {
            faceEstimatorController.ExecModeIndex = index;
        }

        /// <summary>
        /// Change value of Threads of RuntimeMode1
        /// </summary>
        /// <param name="val">Thread count</param>
        void ChangeThreads(float val)
        {
            faceEstimatorController.ModeThreads = Mathf.RoundToInt(val);
        }

    }
}
