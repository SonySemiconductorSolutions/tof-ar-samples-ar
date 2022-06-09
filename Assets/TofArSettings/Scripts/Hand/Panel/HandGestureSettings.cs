/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Hand
{
    public class HandGestureSettings : UI.SettingsBase
    {
        GestureController gestureCtrl;
        GestureIntervalFrameNotRecogController intervalNotRecogCtrl;
        GestureFramesForDetectNoHandsController noHandsCtrl;

        UI.ItemToggle itemEnable, itemAcc;
        UI.ItemSlider itemRecogTh, itemNotRecogInterval,
            itemFrameNoHand, itemEstFrame;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIEnable,
                MakeUIIntervalFrameNotRecog,
                MakeUIFramesForDetectNoHands
            };

            gestureCtrl = FindObjectOfType<GestureController>();
            controllers.Add(gestureCtrl);
            intervalNotRecogCtrl = gestureCtrl.GetComponent<GestureIntervalFrameNotRecogController>();
            controllers.Add(intervalNotRecogCtrl);
            noHandsCtrl = gestureCtrl.GetComponent<GestureFramesForDetectNoHandsController>();
            controllers.Add(noHandsCtrl);

            base.Start();
        }

        /// <summary>
        /// Make UI
        /// </summary>
        void MakeUIEnable()
        {
            itemEnable = settings.AddItem("Enabled", gestureCtrl.OnOff, ChangeEnable);
            gestureCtrl.OnChangeEnable += (onOff) =>
            {
                itemEnable.OnOff = onOff;
            };
        }

        /// <summary>
        /// Toggle gesture recognition
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeEnable(bool onOff)
        {
            gestureCtrl.OnOff = onOff;
        }

        /// <summary>
        /// Make IntervalFrameNotRecognized UI
        /// </summary>
        void MakeUIIntervalFrameNotRecog()
        {
            itemNotRecogInterval = settings.AddItem("IntervalFrame\nNotRecognized",
                GestureIntervalFrameNotRecogController.Min,
                GestureIntervalFrameNotRecogController.Max,
                GestureIntervalFrameNotRecogController.Step,
                intervalNotRecogCtrl.IntervalFrameNotRecognized,
                ChangeIntervalFrameNotRecog, -4);

            intervalNotRecogCtrl.OnChange += (val) =>
            {
                itemNotRecogInterval.Value = val;
            };
        }

        /// <summary>
        /// Change IntervalFrameNotRecognized
        /// </summary>
        /// <param name="val">IntervalFrameNotRecognized</param>
        void ChangeIntervalFrameNotRecog(float val)
        {
            intervalNotRecogCtrl.IntervalFrameNotRecognized = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Make FramesForDetectNoHands UI
        /// </summary>
        void MakeUIFramesForDetectNoHands()
        {
            itemFrameNoHand = settings.AddItem("FramesFor\nDetectNoHands",
                GestureFramesForDetectNoHandsController.Min,
                GestureFramesForDetectNoHandsController.Max,
                GestureFramesForDetectNoHandsController.Step,
                noHandsCtrl.FramesForDetectNoHands,
                ChangeFramesForDetectNoHands, -4);

            noHandsCtrl.OnChange += (val) =>
            {
                itemFrameNoHand.Value = val;
            };
        }

        /// <summary>
        /// Change FramesForDetectNoHands
        /// </summary>
        /// <param name="val">FramesForDetectNoHands</param>
        void ChangeFramesForDetectNoHands(float val)
        {
            noHandsCtrl.FramesForDetectNoHands = Mathf.RoundToInt(val);
        }
    }
}
