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
    public enum EditFlags
    {
        None = 0,
        Deletable = 1,
        Renamable = 2
    }

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

        protected ImageButtonTrigger imgBtnTrigger;
        protected RawImage imgOn, imgOff;
        protected Text txtLabel;
        protected bool finishedSetup = false;

        /// <summary>
        /// Event for button click
        /// </summary>
        public UnityAction OnClick;

        /// <summary>
        /// Event for delete
        /// </summary>
        public UnityAction OnDelete;

        /// <summary>
        /// Event for rename
        /// </summary>
        public UnityAction OnRename;

        void Awake()
        {
            InitOnAwake();
        }

        protected void InitOnAwake()
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
        /// <param name="onDelete">Event that is called when Delete Button is selected</param>
        /// <param name="onRename">Event that is called when Rename Button is selected</param>
        public void Init(string label, bool onOff, UnityAction onClick, UnityAction onDelete, UnityAction onRename)
        {
            if (!finishedSetup)
            {
                InitOnAwake();
            }

            txtLabel.text = label;
            OnOff = onOff;
            OnClick = onClick;
            OnDelete = onDelete;
            OnRename = onRename;

            float buttonsWidth = 0;
            int offset = 10;

            // show/hide delete/rename button
            // UI取得
            foreach (var btn in GetComponentsInChildren<Button>())
            {
                if (btn.name.Contains("RenameButton"))
                {
                    bool active = OnRename != null;
                    btn.gameObject.SetActive(active);
                    if (active)
                    {
                        buttonsWidth += btn.GetComponent<RectTransform>().rect.width;
                        buttonsWidth += offset;
                    }
                }
                else if (btn.name.Contains("DeleteButton"))
                {
                    bool active = OnDelete != null;
                    btn.gameObject.SetActive(active);
                    if (active)
                    {
                        buttonsWidth += btn.GetComponent<RectTransform>().rect.width;
                        buttonsWidth += offset;
                    }
                }
            }

            var sizeDelta = txtLabel.rectTransform.sizeDelta;
            sizeDelta.x -= buttonsWidth;
            txtLabel.rectTransform.sizeDelta = sizeDelta;
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

        /// <summary>
        /// Delete event
        /// </summary>
        public void ButtonOnDeleteEvent()
        {
            OnDelete?.Invoke();
        }

        /// <summary>
        /// Rename event
        /// </summary>
        public void ButtonOnRenameEvent()
        {
            OnRename?.Invoke();
        }
    }
}
