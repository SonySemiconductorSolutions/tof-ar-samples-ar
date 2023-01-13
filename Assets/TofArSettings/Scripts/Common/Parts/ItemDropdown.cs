/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace TofArSettings.UI
{
    public class ItemDropdown : Item
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
        /// Dropdown width
        /// </summary>
        public float Width
        {
            get { return dropdownRt.sizeDelta.x; }
            set
            {
                if (value > 0)
                {
                    dropdownRt.sizeDelta = new Vector2(value, dropdownRt.sizeDelta.y);
                }
            }
        }

        int index = 0;
        public int Index
        {
            get { return index; }
            set
            {
                if (index != value && 0 <= value && value < Options.Length)
                {
                    index = value;

                    OnChange?.Invoke(Index);

                    ChangeAppearance();
                }
            }
        }

        /// <summary>
        /// Options
        /// </summary>
        public string[] Options
        {
            get {

                string [] options = optionsEditable.Select(x => x.Key).ToArray();
                return options; 
            }
            set
            {
                optionsEditable = new KeyValuePair<string, EditFlags>[value.Length];
                for (int i = 0; i < optionsEditable.Length; i++)
                {
                    optionsEditable[i] = new KeyValuePair<string, EditFlags>(value[i], EditFlags.None);
                }

                dialog.ClearOptions();
                dialog.AddOptions(value, Index);

                ChangeAppearance();
            }
        }

        KeyValuePair<string, EditFlags> [] optionsEditable;
        public KeyValuePair<string, EditFlags>[] OptionsEditable
        {
            get { return optionsEditable; } 
            set
            {
                optionsEditable = value;
                dialog.ClearOptions();
                dialog.AddOptions(optionsEditable, Index);

                ChangeAppearance();
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
                if (dialog.gameObject.activeSelf != value)
                {
                    dialog.gameObject.SetActive(value);
                }
            }
        }

        /// <summary>
        /// Event that is called when dropwdown value is changed
        /// </summary>
        /// <param name="index">Option index</param>
        public delegate void ChangeEvent(int index);
        public event ChangeEvent OnChange;

        /// <summary>
        /// Event that is called when dropdown item is deleted
        /// </summary>
        /// <param name="index">Option index</param>
        public delegate void DeleteEvent(int index);
        public event DeleteEvent OnDelete;

        /// <summary>
        /// Event that is called when dropdown item is renamed
        /// </summary>
        /// <param name="index">Option index</param>
        public delegate void RenameEvent(int index, string newName);
        public event RenameEvent OnRename;

        Text txtLabel;
        ImageButtonTrigger imgBtnTrigger;
        RectTransform dropdownRt;
        DropdownDialog dialog;

        protected override void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            base.Awake();

            // Get UI
            foreach (var txt in GetComponentsInChildren<Text>())
            {
                if (txt.name.Contains("Label"))
                {
                    txtLabel = txt;
                    break;
                }
            }

            imgBtnTrigger = GetComponentInChildren<ImageButtonTrigger>();
            dropdownRt = imgBtnTrigger.GetComponent<RectTransform>();

            // Register button event
            imgBtnTrigger.OnClick += () =>
            {
                IsOpen = !IsOpen;
            };

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

            var obj = Instantiate(prefabMgr.DropdownDialogPrefab, canvasTr);
            dialog = obj.GetComponent<DropdownDialog>();

            // Close dialog when option is selected
            dialog.OnSelect += (index) =>
            {
                IsOpen = false;
                Index = index;
            };

            dialog.OnDelete += (index) =>
            {
                OnDelete?.Invoke(index);
            };

            dialog.OnRename += (index, newName) =>
            {
                OnRename?.Invoke(index, newName);
                ChangeAppearance();
            };
        }

        void Start()
        {
            ChangeAppearance();
        }

        /// <summary>
        /// Intialize
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        public override void Init(string title, int relativeFontSize,
            float fixedTitleWidth)
        {
            base.Init(title, relativeFontSize, fixedTitleWidth);
            dialog.name = $"DropDialog_{title}";
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="options">List of options</param>
        /// <param name="index">Initial value</param>
        /// <param name="onChange">Event that is called when dropdown value is changed</param>
        /// <param name="width">Dropdown width</param>
        public void Init(string title, int relativeFontSize, float fixedTitleWidth,
            string[] options, int index, ChangeEvent onChange, float width = 0)
        {
            Init(title, relativeFontSize, fixedTitleWidth);

            Options = options;
            Index = index;
            OnChange = onChange;
            Width = width;

            // Close dialog
            IsOpen = false;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="options">List of options</param>
        /// <param name="index">Initial value</param>
        /// <param name="onChange">Event that is called when dropdown value is changed</param>
        /// <param name="onDelete">Event that is called when dropdown item is deleted</param>
        /// <param name="onRename">Event that is called when dropdown item is renamed</param>
        /// <param name="width">Dropdown width</param>
        public void Init(string title, int relativeFontSize, float fixedTitleWidth,
            KeyValuePair<string, EditFlags> [] options, int index, ChangeEvent onChange, DeleteEvent onDelete, RenameEvent onRename, float width = 0)
        {
            Init(title, relativeFontSize, fixedTitleWidth);

            OptionsEditable = options;
            Index = index;
            OnChange = onChange;
            OnDelete = onDelete;
            OnRename = onRename;
            Width = width;

            // ダイアログを閉じる
            IsOpen = false;
        }

        /// <summary>
        /// Change appearance
        /// </summary>
        void ChangeAppearance()
        {
            if (Options.Length > Index)
            {
                txtLabel.text = Options[Index];
            }
            dialog.ChangeAppearance(index);
        }
    }
}
