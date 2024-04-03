/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArSettings.UI
{
    public class ItemText : Item
    {
        public override bool Interactable
        {
            set
            {
                base.Interactable = value;
                imgBtnTrigger.Interactable = value;
            }
        }

        /// <summary>
        /// ダイアログが開いている/閉じている
        /// </summary>
        public bool IsOpen
        {
            get { return dialog.gameObject.activeSelf; }
            set
            {
                if (dialog && dialog.gameObject.activeSelf != value)
                {
                    dialog.ChangeAppearance(Title);
                    dialog.gameObject.SetActive(value);
                }
            }
        }

        ImageButtonTrigger imgBtnTrigger;
        TextDialog dialog;

        protected override void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            base.Awake();

            // Get UI
            imgBtnTrigger = GetComponentInChildren<ImageButtonTrigger>();

            // Register button event
            imgBtnTrigger.OnClick += () =>
            {
                IsOpen = !IsOpen;
            };
        }

        /// <summary>
        /// Intialize
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="fontStyle">Font Style</param>
        /// <param name="useDialog">Open dialog when clicked</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        public void Init(string title, FontStyle fontStyle, bool useDialog,
            int relativeFontSize, float fixedTitleWidth)
        {
            base.Init(title, fontStyle, relativeFontSize, fixedTitleWidth);

            imgBtnTrigger.enabled = useDialog;

            if (useDialog)
            {
                // Create/place dialog directly under SafeArea or Canvas
                var canvasTr = transform.parent;
                while (true)
                {
                    if (canvasTr.name.Contains("SafeArea") ||
                        canvasTr.TryGetComponent(out Canvas canvas))
                    {
                        break;
                    }

                    canvasTr = canvasTr.parent;
                }

                var obj = Instantiate(prefabMgr.TextDialogPrefab, canvasTr);
                dialog = obj.GetComponent<TextDialog>();

                dialog.name = $"TextDialog_{title}";
            }

            // Close dialog
            IsOpen = false;
        }
    }
}
