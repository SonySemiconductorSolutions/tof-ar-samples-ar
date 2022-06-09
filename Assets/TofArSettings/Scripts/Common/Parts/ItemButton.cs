/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine.Events;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class ItemButton : Item
    {
        public override bool Interactable
        {
            set
            {
                base.Interactable = value;
                btn.interactable = value;
            }
        }

        public UnityAction OnClick;

        Button btn;

        protected override void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            base.Awake();

            // Do not add colon to button contents
            AddColon = false;

            // Get UI
            btn = GetComponentInChildren<Button>();

            // Register button event
            btn.onClick.AddListener(() =>
            {
                OnClick?.Invoke();
            });
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="onClick">Event that is called when button is pressed</param>
        public void Init(string title, int relativeFontSize, float fixedTitleWidth,
            UnityAction onClick)
        {
            base.Init(title, relativeFontSize, fixedTitleWidth);
            OnClick = onClick;
        }
    }
}
