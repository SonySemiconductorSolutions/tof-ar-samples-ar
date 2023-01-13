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

        public bool Interactable
        {
            set
            {
                imgBtnTrigger.Interactable = value;
                imgIcon.color = value ? initColor : disabledColor;
                shadow.effectDistance = value ? shadowDist : Vector2.zero;
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
