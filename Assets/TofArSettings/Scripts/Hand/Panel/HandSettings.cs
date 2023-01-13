/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Hand
{
    public class HandSettings : UI.SettingsBase
    {
        RecogModeController recogModeCtrl;
        RuntimeController runtimeCtrl;
        ProcessLevelController processLevelCtrl;
        TrackingModeController trackingModeCtrl;
        NoiseReductionLevelController noiseReductionCtrl;
        HandManagerController managerController;

        UI.ItemDropdown itemMode, itemRuntimeMode1, itemRuntimeMode2,
            itemProcessLevel, itemNoiseReduction;
        UI.ItemToggle itemTrack;
        UI.ItemSlider itemMode1Thread, itemMode2Thread;
        UI.ItemToggle itemStartStream;

        StatusHistory statusHistory;
        UI.Panel panelHistory;

        void Awake()
        {
            // Get panel for displaying information
            foreach (var panel in GetComponentsInChildren<UI.Panel>())
            {
                if (panel.PanelObj.name.Contains("History"))
                {
                    panelHistory = panel;
                    break;
                }
            }

            statusHistory = panelHistory.PanelObj.GetComponentInChildren<StatusHistory>();
        }

        void OnDisable()
        {
            // When this panel is disabled, also disable other associated panels
            panelHistory.PanelObj.SetActive(false);
        }

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIStartStream,
                MakeUIMode,
                MakeUIRuntime,
                MakeUIProcessLevel,
                MakeUITrackingMode,
                MakeUIStatusHistory,
                MakeUINoiseReductionLevel
            };

            recogModeCtrl = FindObjectOfType<RecogModeController>();
            controllers.Add(recogModeCtrl);
            runtimeCtrl = recogModeCtrl.GetComponent<RuntimeController>();
            controllers.Add(runtimeCtrl);
            processLevelCtrl = recogModeCtrl.GetComponent<ProcessLevelController>();
            controllers.Add(processLevelCtrl);
            trackingModeCtrl = recogModeCtrl.GetComponent<TrackingModeController>();
            controllers.Add(trackingModeCtrl);
            noiseReductionCtrl = recogModeCtrl.GetComponent<NoiseReductionLevelController>();
            managerController = FindObjectOfType<HandManagerController>();
            controllers.Add(managerController);

            base.Start();
        }

        /// <summary>
        /// Make Hand dictionary UI
        /// </summary>
        void MakeUIMode()
        {
            itemMode = settings.AddItem("Mode", recogModeCtrl.RecogModeNames,
                recogModeCtrl.Index, ChangeMode);

            recogModeCtrl.OnChangeRecog += (index, conf) =>
            {
                itemMode.Index = index;
            };

            recogModeCtrl.OnUpdateList += (list, index) =>
            {
                itemMode.Options = list;
                itemMode.Index = index;
            };
        }

        /// <summary>
        /// Change Hand dictionary
        /// </summary>
        /// <param name="index">Hand dictionary index</param>
        void ChangeMode(int index)
        {
            recogModeCtrl.Index = index;
        }

        /// <summary>
        /// Make Runtime UI
        /// </summary>
        void MakeUIRuntime()
        {
            itemRuntimeMode1 = settings.AddItem("Runtime Mode1",
                runtimeCtrl.RuntimeModeNames, runtimeCtrl.RuntimeMode1Index,
                ChangeRuntimeMode1);
            itemMode1Thread = settings.AddItem("  Threads",
                RuntimeController.ThreadMin, RuntimeController.ThreadMax,
                RuntimeController.ThreadStep, runtimeCtrl.Mode1Threads,
                ChangeMode1Threads);
            itemRuntimeMode2 = settings.AddItem("Runtime Mode2",
                runtimeCtrl.RuntimeModeNames, runtimeCtrl.RuntimeMode2Index,
                ChangeRuntimeMode2);
            itemMode2Thread = settings.AddItem("  Threads",
                RuntimeController.ThreadMin, RuntimeController.ThreadMax,
                RuntimeController.ThreadStep, runtimeCtrl.Mode2Threads,
                ChangeMode2Threads);

            runtimeCtrl.OnChangeRuntimeMode1 += (index) =>
            {
                itemRuntimeMode1.Index = index;
                itemMode1Thread.Interactable = runtimeCtrl.IsInteractableMode1Threads;
            };

            runtimeCtrl.OnChangeRuntimeMode2 += (index) =>
            {
                itemRuntimeMode2.Index = index;
                itemMode2Thread.Interactable = runtimeCtrl.IsInteractableMode2Threads;
            };

            runtimeCtrl.OnUpdateRuntimeModeList += (list, runtimeMode1Index,
                runtimeMode2Index) =>
            {
                itemRuntimeMode1.Options = list;
                itemRuntimeMode1.Index = runtimeMode1Index;
                itemRuntimeMode2.Options = list;
                itemRuntimeMode2.Index = runtimeMode2Index;
            };

            runtimeCtrl.OnChangeMode1Threads += (val) =>
            {
                itemMode1Thread.Value = val;
            };

            runtimeCtrl.OnChangeMode2Threads += (val) =>
            {
                itemMode2Thread.Value = val;
            };

            itemMode1Thread.Interactable = runtimeCtrl.IsInteractableMode1Threads;
            itemMode2Thread.Interactable = runtimeCtrl.IsInteractableMode2Threads;
        }

        /// <summary>
        /// Change RuntimeMode1
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeRuntimeMode1(int index)
        {
            runtimeCtrl.RuntimeMode1Index = index;
        }

        /// <summary>
        /// Change RuntimeMode2
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeRuntimeMode2(int index)
        {
            runtimeCtrl.RuntimeMode2Index = index;
        }

        /// <summary>
        /// Change value of Threads of RuntimeMode1
        /// </summary>
        /// <param name="val">Thread count</param>
        void ChangeMode1Threads(float val)
        {
            runtimeCtrl.Mode1Threads = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Change value of Threads of RuntimeMode2
        /// </summary>
        /// <param name="val">Thread count</param>
        void ChangeMode2Threads(float val)
        {
            runtimeCtrl.Mode2Threads = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Make Process Level UI
        /// </summary>
        void MakeUIProcessLevel()
        {
            itemProcessLevel = settings.AddItem("Process Level",
                processLevelCtrl.ProcessLevelNames, processLevelCtrl.Index,
                ChangeProcessLevel);

            processLevelCtrl.OnChange += (index) =>
            {
                itemProcessLevel.Index = index;
            };

            processLevelCtrl.OnUpdateList += (list, index) =>
            {
                itemProcessLevel.Options = list;
                itemProcessLevel.Index = index;
            };
        }

        /// <summary>
        /// Change Process Level
        /// </summary>
        /// <param name="index">Process Level index</param>
        void ChangeProcessLevel(int index)
        {
            processLevelCtrl.Index = index;
        }


        /// <summary>
        /// Make Noise Reduction Level UI
        /// </summary>
        void MakeUINoiseReductionLevel()
        {
            itemNoiseReduction = settings.AddItem("Noise Reduction Level",
                noiseReductionCtrl.NoiseReductionLevelNames, noiseReductionCtrl.Index,
                ChangeNoiseReductionLevel, -7);

            noiseReductionCtrl.OnChange += (index) =>
            {
                itemNoiseReduction.Index = index;
            };

            noiseReductionCtrl.OnUpdateList += (list, index) =>
            {
                itemNoiseReduction.Options = list;
                itemNoiseReduction.Index = index;
            };
        }

        /// <summary>
        /// Change Noise Reduction Level
        /// </summary>
        /// <param name="index">Noise Reduction Level index</param>
        void ChangeNoiseReductionLevel(int index)
        {
            noiseReductionCtrl.Index = index;
        }

        /// <summary>
        /// Make Tracking Mode UI
        /// </summary>
        void MakeUITrackingMode()
        {
            itemTrack = settings.AddItem("Tracking Mode", trackingModeCtrl.OnOff,
    ChangeTrackingMode);

            trackingModeCtrl.OnChange += (onOff) =>
            {
                itemTrack.OnOff = onOff;
            };
        }

        /// <summary>
        /// Toggle Tracking Mode
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeTrackingMode(bool onOff)
        {
            trackingModeCtrl.OnOff = onOff;
        }

        /// <summary>
        /// Make Hand Status History UI
        /// </summary>
        void MakeUIStatusHistory()
        {
            settings.AddItem("Status History", false, ShowStatusHistory);
        }

        /// <summary>
        /// Toggle Hand Status History display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ShowStatusHistory(bool onOff)
        {
            if (onOff)
            {
                panelHistory.OpenPanel(false);
            }
            else
            {
                panelHistory.ClosePanel();
            }
        }

        /// <summary>
        /// Make StartStream UI
        /// </summary>
        void MakeUIStartStream()
        {
            itemStartStream = settings.AddItem("Start Stream", TofArHandManager.Instance.autoStart, ChangeStartStream);
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
