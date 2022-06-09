/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using TofAr.V0.Hand;
using UnityEngine.Events;

namespace TofArSettings.Hand
{
    public class HandFingerTouchSettings : UI.SettingsBase
    {
        FingerTouchController fingerTouchCtrl;
        FingerTouchExecModeController execCtrl;
        FingerTouchTargetController targetCtrl;

        UI.ItemToggle itemEnable;
        UI.ItemDropdown itemExecMode;
        Dictionary<HandPointIndex, UI.ItemToggleMulti> itemFingers =
            new Dictionary<HandPointIndex, UI.ItemToggleMulti>();

        UI.Panel panelFingerTouch;

        void Awake()
        {
            // Get panel for displaying information
            panelFingerTouch = GetComponentInChildren<UI.Panel>();
        }

        void OnDisable()
        {
            // When this panel is disabled, also disable associated panels
            panelFingerTouch.PanelObj.SetActive(false);
        }

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIEnable,
                MakeUIExecMode,
                MakeUIFingers
            };

            fingerTouchCtrl = FindObjectOfType<FingerTouchController>();
            controllers.Add(fingerTouchCtrl);
            execCtrl = fingerTouchCtrl.GetComponent<FingerTouchExecModeController>();
            controllers.Add(execCtrl);
            targetCtrl = fingerTouchCtrl.GetComponent<FingerTouchTargetController>();
            controllers.Add(targetCtrl);

            base.Start();
        }

        /// <summary>
        /// Make UI for finger detection status
        /// </summary>
        void MakeUIEnable()
        {
            itemEnable = settings.AddItem("Enabled", fingerTouchCtrl.OnOff, ChangeEnable);

            fingerTouchCtrl.OnChangeEnable += (onOff) =>
            {
                itemEnable.OnOff = onOff;
            };
        }

        /// <summary>
        /// Toggle finger detection
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeEnable(bool onOff)
        {
            fingerTouchCtrl.OnOff = onOff;

            // Change panel display
            if (onOff)
            {
                panelFingerTouch.OpenPanel(false);
            }
            else
            {
                panelFingerTouch.ClosePanel();
            }
        }

        /// <summary>
        /// Make Exec Mode UI
        /// </summary>
        void MakeUIExecMode()
        {
            itemExecMode = settings.AddItem("Exec Mode", execCtrl.ExecModeNames,
                execCtrl.Index, ChangeExecMode);

            execCtrl.OnChange += (index) =>
            {
                itemExecMode.Index = index;
            };
        }

        /// <summary>
        /// Change Exec Mode
        /// </summary>
        /// <param name="index">Exec Mode index</param>
        void ChangeExecMode(int index)
        {
            execCtrl.Index = index;
        }

        /// <summary>
        /// Make Target Finger UI
        /// </summary>
        void MakeUIFingers()
        {
            for (int i = 0; i < FingerTouchController.Fingers.Length; i++)
            {
                var finger = FingerTouchController.Fingers[i];
                var itemFinger = AddItemToggleMulti(finger, ChangeTargetFinger);
                itemFingers.Add(finger, itemFinger);
            }
        }

        /// <summary>
        /// Add UI for Target Finger
        /// </summary>
        /// <param name="finger">Finger</param>
        /// <param name="onChange">Event that is called when UI is used</param>
        /// <returns>UI parts</returns>
        UI.ItemToggleMulti AddItemToggleMulti(HandPointIndex finger,
            UnityAction<HandPointIndex, int, bool> onChange)
        {
            string fingerName = string.Empty;
            switch (finger)
            {
                case HandPointIndex.ThumbTip:
                    fingerName = "Thumb";
                    break;
                case HandPointIndex.IndexTip:
                    fingerName = "Index";
                    break;
                case HandPointIndex.MidTip:
                    fingerName = "Middle";
                    break;
                case HandPointIndex.RingTip:
                    fingerName = "Ring";
                    break;
                case HandPointIndex.PinkyTip:
                    fingerName = "Pinky";
                    break;
            }

            string[] title = new string[2];
            title[0] = $"{fingerName} L";
            title[1] = "  R";

            bool[] state = new bool[2];
            state[0] = targetCtrl.GetTargetFinger(
                FingerTouchDetector.HandSide.LeftHand, finger);
            state[1] = targetCtrl.GetTargetFinger(
                FingerTouchDetector.HandSide.RightHand, finger);

            return settings.AddItem(title, state, (index, onOff) =>
            {
                onChange?.Invoke(finger, index, onOff);
            });
        }

        /// <summary>
        /// CHange Target Finger
        /// </summary>
        /// <param name="finger">Finger</param>
        /// <param name="index">0 = Left, 1 = Right</param>
        /// <param name="onOff">Enable/Disable</param>
        void ChangeTargetFinger(HandPointIndex finger, int index, bool onOff)
        {
            var side = (index == 0) ? FingerTouchDetector.HandSide.LeftHand :
                FingerTouchDetector.HandSide.RightHand;
            targetCtrl.SetTargetFinger(side, finger, onOff);
        }
    }
}
