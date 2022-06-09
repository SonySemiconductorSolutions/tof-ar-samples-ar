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
    public class ItemToggle : Item
    {
        public override bool Interactable
        {
            set
            {
                base.Interactable = value;
                imgBtnTrigger.Interactable = value;

                UnityEngine.Color on = imgOnColor;
                UnityEngine.Color off = imgOffColor;
                if (!value)
                {
                    UnityEngine.Color dark = new UnityEngine.Color(0.2f, 0.2f, 0.2f, 0);
                    on -= dark;
                    off -= dark;
                }

                imgOn.color = on;
                imgOff.color = off;
            }
        }

        bool onOff = false;
        public bool OnOff
        {
            get { return onOff; }
            set
            {
                if (onOff != value)
                {
                    onOff = value;

                    OnChange?.Invoke(OnOff);
                    OnChangeWithIndex?.Invoke(Index, OnOff);

                    ChangeAppearance();
                }
            }
        }

        /// <summary>
        /// Identification number
        /// </summary>
        public int Index { get; private set; } = 0;

        /// <summary>
        /// Event that is called when toggle status is changed
        /// </summary>
        /// <param name="onOff">On/Off</param>
        public delegate void ChangeEvent(bool onOff);

        public event ChangeEvent OnChange;

        /// <summary>
        /// Event that is called when toggle status is changed (with identification number)
        /// </summary>
        /// <param name="index">Identification number</param>
        /// <param name="onOff">On/Off</param>
        public delegate void ChangeWithIndexEvent(int index, bool onOff);
        public event ChangeWithIndexEvent OnChangeWithIndex;

        ImageButtonTrigger imgBtnTrigger;
        RawImage imgOn, imgOff;
        UnityEngine.Color imgOnColor, imgOffColor;

        protected override void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            base.Awake();

            // Get UI
            imgBtnTrigger = GetComponentInChildren<ImageButtonTrigger>();
            foreach (var img in GetComponentsInChildren<RawImage>())
            {
                if (img.name.Contains("On"))
                {
                    imgOn = img;
                    imgOnColor = img.color;
                }
                else if (img.name.Contains("Off"))
                {
                    imgOff = img;
                    imgOffColor = img.color;
                }
            }

            // Register button event
            imgBtnTrigger.OnClick += () =>
            {
                OnOff = !OnOff;
            };
        }

        void Start()
        {
            ChangeAppearance();
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="onOff">Toggle status</param>
        /// <param name="onChange">Event that is called when toggle is pressed</param>
        public void Init(string title, int relativeFontSize, float fixedTitleWidth,
            bool onOff, ChangeEvent onChange)
        {
            base.Init(title, relativeFontSize, fixedTitleWidth);

            OnOff = onOff;
            OnChange = onChange;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="onOff">Toggle status</param>
        /// <param name="index">Identification number</param>
        /// <param name="onChange">Event that is called when toggle is pressed</param>
        public void Init(string title, int relativeFontSize, float fixedTitleWidth,
            bool onOff, int index, ChangeWithIndexEvent onChange)
        {
            base.Init(title, relativeFontSize, fixedTitleWidth);

            OnOff = onOff;
            Index = index;
            OnChangeWithIndex = onChange;
        }

        /// <summary>
        /// Change appearance
        /// </summary>
        void ChangeAppearance()
        {
            imgOn.enabled = OnOff;
            imgOff.enabled = !OnOff;
        }
    }
}
