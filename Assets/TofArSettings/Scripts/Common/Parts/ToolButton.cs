/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
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
                    ChangeAppearance(OnOff);
                }
            }
        }

        public bool Interactable
        {
            set
            {
                if (imgBtnTrigger != null)
                {
                    imgBtnTrigger.Interactable = value;
                }
                if (imgIcon != null)
                {
                    imgIcon.color = value ? initColor : disabledColor;
                }
                if (shadow != null)
                {
                    shadow.effectDistance = value ? shadowDist : Vector2.zero;
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
        RawImage imgIcon;
        Shadow shadow;

        Vector2 shadowDist;

        UnityEngine.Color initColor;
        UnityEngine.Color disabledColor = UnityEngine.Color.gray;

        void Start()
        {
            // Get UI
            imgBtnTrigger = GetComponentInChildren<ImageButtonTrigger>();
            foreach (var img in GetComponentsInChildren<RawImage>())
            {
                if (imgPushed == null && img.name.Contains("Push"))
                {
                    imgPushed = img;
                }
                if (imgIcon == null && img.name.Contains("Icon"))
                {
                    imgIcon = img;
                    disabledColor = initColor = imgIcon.color;
                    disabledColor.a = 0.4f;
                }
            }

            shadow = imgBtnTrigger.GetComponent<Shadow>();
            shadowDist = shadow.effectDistance;

            // Register button event
            imgBtnTrigger.OnClick += () =>
            {
                // Only change the appearance when the actual ON/OFF and appearance are different
                // (e.g. when a child panel is open)
                if (OnOff != imgPushed.enabled)
                {
                    ChangeAppearance(OnOff);
                }
                else
                {
                    OnOff = !OnOff;
                }

                OnClick?.Invoke(OnOff);
            };

            ChangeAppearance(OnOff);
        }

        /// <summary>
        /// Change appearance
        /// </summary>
        /// <param name="onOff">ON/OFF</param>
        public void ChangeAppearance(bool onOff)
        {
            imgPushed.enabled = onOff;
            shadow.effectDistance = (onOff) ? Vector2.zero : shadowDist;
        }
    }
}
