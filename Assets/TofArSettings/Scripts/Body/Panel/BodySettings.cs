/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */
using TofAr.V0.Body;
using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Body
{
    public class BodySettings : UI.SettingsBase
    {
        BodyRuntimeController runtimeController;
        BodyManagerController managerController;
        SV2Controller sv2Controller;

        UI.ItemDropdown itemMode, itemRuntimeModeSV2, itemRecogModeSV2, itemNoiseReduction;
        UI.ItemSlider itemSV2Thread;
        UI.ItemToggle itemStartStream;
        

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIStartStream,
                MakeUIDetectorType,
                MakeUIRuntime
            };

            runtimeController = FindObjectOfType<BodyRuntimeController>();
            controllers.Add(runtimeController);
            sv2Controller = FindObjectOfType<SV2Controller>();
            controllers.Add(sv2Controller);
            managerController = FindObjectOfType<BodyManagerController>();
            controllers.Add(managerController);
            
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

            sv2Controller.OnChangeRuntimeMode += (index) =>
            {
                itemRuntimeModeSV2.Index = index;
                itemSV2Thread.Interactable = sv2Controller.IsInteractableModeThreads;
            };

            sv2Controller.OnUpdateRuntimeModeList += (list, runtimeModeIndex) =>
            {
                itemRuntimeModeSV2.Options = list;
                itemRuntimeModeSV2.Index = runtimeModeIndex;
            };

            sv2Controller.OnChangeRecogMode += (index,conf) =>
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

            sv2Controller.OnChangeModeThreads += (val) =>
            {
                itemSV2Thread.Value = val;
            };

            SetSV2Interactability();
        }

        /// <summary>
        /// Enabled or disable the interactibility of sv2 UI based on the runtime controller
        /// </summary>
        void SetSV2Interactability()
        {
            var interactible = runtimeController.DetectorType == TofAr.V0.Body.BodyPoseDetectorType.Internal_SV2;
            itemRuntimeModeSV2.Interactable = interactible;
            itemRecogModeSV2.Interactable = interactible;
            itemSV2Thread.Interactable = interactible && sv2Controller.IsInteractableModeThreads;
            itemNoiseReduction.Interactable = interactible;
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

        /// <summary>
        /// Make StartStream UI
        /// </summary>
        void MakeUIStartStream()
        {
            itemStartStream = settings.AddItem("Start Stream", TofArBodyManager.Instance.autoStart, ChangeStartStream);
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
    }
}
