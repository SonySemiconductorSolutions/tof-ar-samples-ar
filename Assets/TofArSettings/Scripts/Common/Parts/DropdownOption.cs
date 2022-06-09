/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class DropdownOption : MonoBehaviour
    {
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

        public UnityAction OnClick;

        ImageButtonTrigger imgBtnTrigger;
        RawImage imgOn, imgOff;
        Text txtLabel;
        bool finishedSetup = false;

        void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            // Get UI
            foreach (var img in GetComponentsInChildren<RawImage>())
            {
                if (img.name.Contains("On"))
                {
                    imgOn = img;
                }
                else if (img.name.Contains("Off"))
                {
                    imgOff = img;
                }
            }

            txtLabel = GetComponentInChildren<Text>();

            // Register button event
            imgBtnTrigger = GetComponentInChildren<ImageButtonTrigger>();
            if (imgBtnTrigger != null)
            {
                imgBtnTrigger.OnClick += () =>
                {
                    OnOff = true;
                    OnClick?.Invoke();
                };
            }

            finishedSetup = true;
        }

        void Start()
        {
            ChangeAppearance();
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="onOff">RadioButton status</param>
        /// <param name="onClick">Event that is called when RadioButton is selected</param>
        public void Init(string label, bool onOff, UnityAction onClick)
        {
            if (!finishedSetup)
            {
                Awake();
            }

            txtLabel.text = label;
            OnOff = onOff;
            OnClick = onClick;
        }

        /// <summary>
        /// Change appearance
        /// </summary>
        void ChangeAppearance()
        {
            imgOn.enabled = OnOff;
            imgOff.enabled = !OnOff;
        }

        /// <summary>
        /// Button click event
        /// </summary>
        public void ButtonOnClickEvent()
        {
            OnOff = true;
            OnClick?.Invoke();
        }
    }
}
