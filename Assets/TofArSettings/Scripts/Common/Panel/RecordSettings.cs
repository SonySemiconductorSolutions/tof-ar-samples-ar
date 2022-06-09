/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofArSettings.Body;
using TofArSettings.Color;
using TofArSettings.Face;
using TofArSettings.Tof;
using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.UI
{
    public class RecordSettings : SettingsBase
    {
        [Header("Use Component")]

        /// <summary>
        /// Use/do not use Color component
        /// </summary>
        [SerializeField]
        bool color = true;

        /// <summary>
        /// Use/do not use Tof component
        /// </summary>
        [SerializeField]
        bool tof = true;

        /// <summary>
        /// Use/do not use Body component
        /// </summary>
        [SerializeField]
        bool body = false;

        /// <summary>
        /// Use/do not use Face component
        /// </summary>
        [SerializeField]
        bool face = false;

        const string startText = "Record";
        const string stopText = "Stop";

        const string singleStartText = "Snapshot";
        const string singleStopText = "Stop";


        ItemDropdown itemTimer;
        ItemButton itemSingleBtn, itemMultiBtn;

        UI.Panel panelStatus;

        UnityEvent addControllersEvent = new UnityEvent();

        void Awake()
        {
            // Get panel for displaying information
            foreach (var panel in GetComponentsInChildren<UI.Panel>())
            {
                if (panel.PanelObj.name.Contains("Status"))
                {
                    panelStatus = panel;
                    break;
                }
            }
        }

        void OnDisable()
        {
            // When disabling this panel, also disable associated panels
            panelStatus.PanelObj.SetActive(false);
        }

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUITimer,
                MakeUIExecSingle,
                MakeUIExecMulti
            };

            if (color)
            {
                var colorRecCtrl = FindObjectOfType<ColorRecordController>();
                controllers.Add(colorRecCtrl);
            }

            if (tof)
            {
                var tofRecCtrl = FindObjectOfType<TofRecordController>();
                controllers.Add(tofRecCtrl);
            }

            if (body)
            {
                var bodyRecCtrl = FindObjectOfType<BodyRecordController>();
                controllers.Add(bodyRecCtrl);
            }

            if (face)
            {
                var faceRecCtrl = FindObjectOfType<FaceRecordController>();
                controllers.Add(faceRecCtrl);
            }

            addControllersEvent.Invoke();

            for (int i = 0; i < controllers.Count; i++)
            {
                var recCtrl = controllers[i] as RecordController;
                if (recCtrl != null)
                {
                    if (recCtrl.IsMultiple)
                    {
                        recCtrl.OnChangeStatus += OnChangeMultiRecStatus;
                    }
                    else
                    {
                        recCtrl.OnChangeStatus += OnChangeSingleRecStatus;
                    }
                }
            }

            base.Start();
        }

        /// <summary>
        /// Make recording timer selection UI
        /// </summary>
        void MakeUITimer()
        {
            var recCtrl = controllers[0] as RecordController;
            var timerOptions = new string[RecordController.TimerCounts.Length];
            for (int i = 0; i < timerOptions.Length; i++)
            {
                timerOptions[i] = RecordController.TimerCounts[i].ToString();
            }

            if (recCtrl != null)
            {
                itemTimer = settings.AddItem("Timer", timerOptions, recCtrl.TimerIndex, ChangeTimer);

                recCtrl.OnChangeTimerIndex += (index) =>
                {
                    itemTimer.Index = index;
                };
            }
        }

        /// <summary>
        /// Select recording timer
        /// </summary>
        /// <param name="index">Recording timer index</param>
        void ChangeTimer(int index)
        {
            for (int i = 0; i < controllers.Count; i++)
            {
                var recCtrl = controllers[i] as RecordController;
                if (recCtrl != null)
                {
                    recCtrl.TimerIndex = index;
                }
            }
        }

        /// <summary>
        /// Make start button
        /// </summary>
        void MakeUIExecSingle()
        {
            for (int i = 0; i < controllers.Count; i++)
            {
                var recCtrl = controllers[i] as RecordController;
                if (recCtrl != null && !recCtrl.IsMultiple)
                {
                    itemSingleBtn = settings.AddItem(singleStartText, ExecSingleFrame);

                    break;
                }
            }
        }

        /// <summary>
        /// Make start button
        /// </summary>
        void MakeUIExecMulti()
        {
            for (int i = 0; i < controllers.Count; i++)
            {
                var recCtrl = controllers[i] as RecordController;
                if (recCtrl != null && recCtrl.IsMultiple)
                {
                    itemMultiBtn = settings.AddItem(startText, ExecMultiFrame);
                    break;
                }
            }
        }

        /// <summary>
        /// Start recording
        /// </summary>
        void ExecSingleFrame()
        {
            for (int i = 0; i < controllers.Count; i++)
            {
                var recCtrl = controllers[i] as RecordController;
                if (recCtrl != null && !recCtrl.IsMultiple)
                {
                    recCtrl.Exec();
                }
            }
        }


        /// <summary>
        /// Start recording
        /// </summary>
        void ExecMultiFrame()
        {
            for (int i = 0; i < controllers.Count; i++)
            {
                var recCtrl = controllers[i] as RecordController;
                if (recCtrl != null && recCtrl.IsMultiple)
                {
                    recCtrl.Exec();
                }
            }
        }

        /// <summary>
        /// Event that is called when recording status is changed
        /// </summary>
        /// <param name="status">Recording status</param>
        void OnChangeSingleRecStatus(RecordController.RecStatus status)
        {
            switch (status)
            {
                case RecordController.RecStatus.Start:
                    itemSingleBtn.Title = singleStopText;
                    if (itemMultiBtn != null)
                    {
                        itemMultiBtn.Interactable = false;
                    }
                    ToolbarNotSelect(true);
                    break;

                case RecordController.RecStatus.Stop:
                    // Reset
                    itemSingleBtn.Title = singleStartText;
                    if (itemMultiBtn != null)
                    {
                        itemMultiBtn.Interactable = true;
                    }
                    ToolbarNotSelect(false);
                    break;
            }
        }

        /// <summary>
        /// Event that is called when recording status is changed
        /// </summary>
        /// <param name="status">Recording status</param>
        void OnChangeMultiRecStatus(RecordController.RecStatus status)
        {
            switch (status)
            {
                case RecordController.RecStatus.Start:
                    // Change appearance of button and make other buttons non-interactable
                    itemTimer.Interactable = false;
                    if (itemSingleBtn != null)
                    {
                        itemSingleBtn.Interactable = false;
                    }
                    itemMultiBtn.Title = stopText;
                    ToolbarNotSelect(true);
                    break;

                case RecordController.RecStatus.Stop:
                    // Reset
                    itemTimer.Interactable = true;
                    if (itemSingleBtn != null)
                    {
                        itemSingleBtn.Interactable = true;
                    }
                    itemMultiBtn.Title = startText;
                    ToolbarNotSelect(false);
                    break;
            }
        }

        /// <summary>
        /// Toggle toolbar button
        /// </summary>
        /// <param name="state">On/Off</param>
        private void ToolbarNotSelect(bool state)
        {
            Toolbar toolbar = FindObjectOfType<Toolbar>();
            if (toolbar)
            {
                toolbar.SetNotSelect(state);
            }
        }

        /// <summary>
        /// Controller registration event
        /// </summary>
        /// <param name="unityAction">Argumentless delegate</param>
        public void AddListenerAddControllersEvent(UnityAction unityAction)
        {
            addControllersEvent.AddListener(unityAction);
        }

        /// <summary>
        /// Register controller
        /// </summary>
        /// <param name="controller">RecordController</param>
        public void SetController(ControllerBase controller)
        {
            controllers.Add(controller);
        }
    }
}
