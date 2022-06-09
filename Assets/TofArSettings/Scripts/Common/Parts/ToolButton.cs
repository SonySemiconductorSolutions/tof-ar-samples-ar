/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class ToolButton : MonoBehaviour
    {
        [SerializeField]
        bool onOff = false;
        public bool OnOff
        {
            get { return onOff; }
            set
            {
                if (onOff != value)
                {
                    onOff = value;
                    ChangeAppearance();
                }
            }
        }

        /// <summary>
        /// Event that is called when button is pressed
        /// </summary>
        /// <param name="onOff">ON/OFF</param>
        public delegate void ClickEvent(bool onOff);

        public event ClickEvent OnClick;

        ImageButtonTrigger imgBtnTrigger;
        RawImage imgPushed;
        Shadow shadow;

        Vector2 shadowDist;

        void Start()
        {
            // Get UI
            imgBtnTrigger = GetComponentInChildren<ImageButtonTrigger>();
            foreach (var img in GetComponentsInChildren<RawImage>())
            {
                if (img.name.Contains("Push"))
                {
                    imgPushed = img;
                    break;
                }
            }

            shadow = imgBtnTrigger.GetComponent<Shadow>();
            shadowDist = shadow.effectDistance;

            // Register button event
            imgBtnTrigger.OnClick += () =>
            {
                OnOff = !OnOff;
                OnClick?.Invoke(OnOff);
            };

            ChangeAppearance();
        }

        /// <summary>
        /// Change appearance
        /// </summary>
        void ChangeAppearance()
        {
            imgPushed.enabled = OnOff;
            shadow.effectDistance = (OnOff) ? Vector2.zero : shadowDist;
        }
    }
}
