/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using TofArSettings.UI;
using TofAr.V0.Hand;
using System;

namespace TofArSettings.Hand
{
    public class HandGestureInfoSettings : SettingsBase
    {
        [SerializeField]
        private GestureIndex[] ignoreGestures;

        [Header("Default value")]

        [SerializeField]
        private bool show = true;

        private HandGestureInfoController gestureInfoCtrl;

        private Dictionary<GestureIndex, ItemToggle> itemToggles = new Dictionary<GestureIndex, ItemToggle>();

        protected virtual void Awake()
        {
            gestureInfoCtrl = GetComponent<HandGestureInfoController>();
            gestureInfoCtrl.enabled = true;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
            MakeUIShow,
            MakeUIGestureNotify
            };
            controllers.Add(gestureInfoCtrl);

            ChangeShow(show);

            base.Start();
        }

        void MakeUIShow()
        {
            settings.AddItem("Show", show, ChangeShow);
        }

        void MakeUIGestureNotify()
        {
            var gestureNotifyList = gestureInfoCtrl.GestureNotify;
            if (gestureNotifyList != null)
            {
                var gestureNames = Enum.GetNames(typeof(GestureIndex));
                for (int i = 0; i < gestureNotifyList.Length; i++)
                {
                    if (ignoreGestures.Contains((GestureIndex)i)) { continue; }
                    var index = i;
                    var gestureName = gestureNames[index];
                    var gestureNotify = settings.AddItem("Show: " + gestureName, gestureNotifyList[i], (onOff) =>
                    {
                        gestureInfoCtrl.SetGestureNotify(index, onOff);
                    });
                    itemToggles[(GestureIndex)i] = gestureNotify;
                }
            }
            
        }


        void ChangeShow(bool val)
        {
            this.show = val;
            gestureInfoCtrl.Show = this.show;
        }
    }
}
