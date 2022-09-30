/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */
using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Body
{
    public class BodySettings : UI.SettingsBase
    {
        BodyRuntimeController runtimeController;
        SV1Controller sv1Controller;
        SV2Controller sv2Controller;

        UI.ItemDropdown itemMode, itemRuntimeModeSV1, itemRuntimeModeSV2, itemBodyShotSV1, itemRecogModeSV2, itemNoiseReduction;
        UI.ItemSlider itemSV1Thread, itemSV2Thread;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIDetectorType,
                MakeUIRuntime
            };

            runtimeController = FindObjectOfType<BodyRuntimeController>();
            controllers.Add(runtimeController);
            sv1Controller = FindObjectOfType<SV1Controller>();
            controllers.Add(sv1Controller);
            sv2Controller = FindObjectOfType<SV2Controller>();
            controllers.Add(sv2Controller);

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
                SetSV1Interactability();
                SetSV2Interactability();
            };
        }

        /// <summary>
        /// Change Body dictionary
        /// </summary>
        /// <param name="index">Body dictionary index</param>
        void ChangeMode(int index)
        {
            runtimeController.DetectorTypeIndex = index;
        }

        /// <summary>
        /// Make Runtime UI
        /// </summary>
        void MakeUIRuntime()
        {
            itemRuntimeModeSV1 = settings.AddItem("SV1 Runtime", sv1Controller.ExecModeNames,
                sv1Controller.ExecModeIndex, ChangeRuntimeModeSV1);
            itemBodyShotSV1 = settings.AddItem("SV1 Body Shot", sv1Controller.BodyShotNames,
                sv1Controller.BodyShotIndex, ChangeBodyShotSV1);
            itemSV1Thread = settings.AddItem("SV1 Threads",
                SV1Controller.ThreadMin, SV1Controller.ThreadMax,
                SV1Controller.ThreadStep, sv1Controller.ModeThreads,
                ChangeSV1Threads);
            itemRuntimeModeSV2 = settings.AddItem("SV2 Runtime", sv2Controller.RuntimeModeNames,
                sv2Controller.RuntimeModeIndex, ChangeRuntimeModeSV2);
            itemRecogModeSV2 = settings.AddItem("SV2 RecogMode", sv2Controller.RecogModeNames,
                sv2Controller.RecogModeIndex, ChangeRecogModeSV2);
            itemSV2Thread = settings.AddItem("SV2 Threads",
                SV2Controller.ThreadMin, SV2Controller.ThreadMax,
                SV2Controller.ThreadStep, sv2Controller.ModeThreads,
                ChangeSV2Threads);
            itemNoiseReduction = settings.AddItem("SV2 Noise Reduction Level", sv2Controller.NoiseReductionLevelNames,
                sv2Controller.NoiseReductionIndex, ChangeNoiseReductionSV2);

            sv1Controller.OnChangeBodyShot += (index) =>
            {
                itemBodyShotSV1.Index = index;
            };

            sv1Controller.OnChangeRuntimeMode += (index) =>
            {
                itemRuntimeModeSV1.Index = index;
                itemSV1Thread.Interactable = sv1Controller.IsInteractableModeThreads;
            };

            sv2Controller.OnChangeRuntimeMode += (index) =>
            {
                itemRuntimeModeSV2.Index = index;
                itemSV2Thread.Interactable = sv2Controller.IsInteractableModeThreads;
            };

            sv1Controller.OnUpdateRuntimeModeList += (list, runtimeModeIndex) =>
            {
                itemRuntimeModeSV1.Options = list;
                itemRuntimeModeSV1.Index = runtimeModeIndex;
            };

            sv2Controller.OnUpdateRuntimeModeList += (list, runtimeModeIndex) =>
            {
                itemRuntimeModeSV2.Options = list;
                itemRuntimeModeSV2.Index = runtimeModeIndex;
            };

            sv2Controller.OnChangeRecogMode += (index) =>
            {
                itemRecogModeSV2.Index = index;
            };

            sv2Controller.OnUpdateRecogModeList += (list, recogModeIndex) =>
            {
                itemRecogModeSV2.Options = list;
                itemRecogModeSV2.Index = recogModeIndex;
            };

            sv2Controller.OnChangeNoiseReductionLevel += (index) =>
            {
                itemNoiseReduction.Index = index;
            };

            sv2Controller.OnUpdateNoiseReductionList += (list, index) =>
            {
                itemNoiseReduction.Options = list;
                itemNoiseReduction.Index = index;
            };

            sv1Controller.OnChangeModeThreads += (val) =>
            {
                itemSV1Thread.Value = val;
            };

            sv2Controller.OnChangeModeThreads += (val) =>
            {
                itemSV2Thread.Value = val;
            };

            SetSV1Interactability();
            SetSV2Interactability();
        }

        /// <summary>
        /// Enabled or disable the interactibility of sv1 UI based on the runtime controller
        /// </summary>
        void SetSV1Interactability()
        {
            var interactible = runtimeController.DetectorTypeIndex > 0 &&
                runtimeController.DetectorType == TofAr.V0.Body.BodyPoseDetectorType.External;
            itemRuntimeModeSV1.Interactable = interactible;
            itemSV1Thread.Interactable = interactible && sv1Controller.IsInteractableModeThreads;
            itemBodyShotSV1.Interactable = interactible;
        }

        /// <summary>
        /// Enabled or disable the interactibility of sv2 UI based on the runtime controller
        /// </summary>
        void SetSV2Interactability()
        {
            var interactible = runtimeController.DetectorTypeIndex > 0 &&
                runtimeController.DetectorType == TofAr.V0.Body.BodyPoseDetectorType.Internal_SV2;
            itemRuntimeModeSV2.Interactable = interactible;
            itemRecogModeSV2.Interactable = interactible;
            itemSV2Thread.Interactable = interactible && sv2Controller.IsInteractableModeThreads;
            itemNoiseReduction.Interactable = interactible;
        }

        /// <summary>
        /// Change BodyShotSV1
        /// </summary>
        /// <param name="index">BodyShot index</param>
        void ChangeBodyShotSV1(int index)
        {
            sv1Controller.BodyShotIndex = index;
        }

        /// <summary>
        /// Change RuntimeMode1
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeRuntimeModeSV1(int index)
        {
            sv1Controller.ExecModeIndex = index;
        }

        /// <summary>
        /// Change thread count of RuntimeMode1
        /// </summary>
        /// <param name="val">Thread count</param>
        void ChangeSV1Threads(float val)
        {
            sv1Controller.ModeThreads = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Change RuntimeMode2
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeRuntimeModeSV2(int index)
        {
            sv2Controller.RuntimeModeIndex = index;
        }

        /// <summary>
        /// Change RecogMode
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeRecogModeSV2(int index)
        {
            sv2Controller.RecogModeIndex = index;
        }

        /// <summary>
        /// Change NoiseReductionLevel
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeNoiseReductionSV2(int index)
        {
            sv2Controller.NoiseReductionIndex = index;
        }

        /// <summary>
        /// Change thread count of RuntimeMode2
        /// </summary>
        /// <param name="val">Thread count</param>
        void ChangeSV2Threads(float val)
        {
            sv2Controller.ModeThreads = Mathf.RoundToInt(val);
        }
    }
}
