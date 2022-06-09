/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TofArSettings.UI
{
    public class ItemNestMenu : Item
    {
        public UnityAction OnClick;

        RawImage imgIcon;
        ImageButtonTrigger imgBtnTrigger;

        protected override void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            base.Awake();

            // Get UI
            foreach (var img in GetComponentsInChildren<RawImage>())
            {
                if (img.name.Contains("Icon"))
                {
                    imgIcon = img;
                    break;
                }
            }

            imgBtnTrigger = GetComponent<ImageButtonTrigger>();

            // Register button event
            imgBtnTrigger.OnClick += () =>
            {
                OnClick?.Invoke();
            };
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="icon">Icon image</param>
        /// <param name="iconColor">Icon color</param>
        /// <param name="onClick">Event that is called when button is pressed</param>
        public void Init(string title, int relativeFontSize, float fixedTitleWidth,
            Texture icon, UnityEngine.Color iconColor, UnityAction onClick)
        {
            base.Init(title, relativeFontSize, fixedTitleWidth);

            imgIcon.texture = icon;
            imgIcon.color = iconColor;
            OnClick = onClick;
        }
    }
}
