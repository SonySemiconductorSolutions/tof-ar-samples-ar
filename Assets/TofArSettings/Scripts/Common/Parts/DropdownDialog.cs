/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class DropdownDialog : MonoBehaviour
    {
        /// <summary>
        /// Dialog max height
        /// </summary>
        public float MaxHeight = 600;

        /// <summary>
        /// Event that is called when option is selected
        /// </summary>
        /// <param name="index">Option index</param>
        public delegate void SelectEvent(int index);

        public event SelectEvent OnSelect;

        RectTransform dialogRt, contentRt;
        VerticalLayoutGroup contentLayout;
        List<DropdownOption> options = new List<DropdownOption>();
        ImageButtonTrigger imgTrigger;
        bool finishedSetup = false;
        Vector2 defaultContentSize;

        SettingsPrefabManager prefabMgr;

        void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            Transform bgTr = null;
            Transform dialogTr = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                var tr = transform.GetChild(i);
                if (tr.name.Contains("Bg"))
                {
                    bgTr = tr;
                }
                else if (tr.name.Contains("Dialog"))
                {
                    dialogTr = tr;
                }
            }
            if (dialogTr == null)
            {
                return;
            }
            dialogRt = dialogTr.GetComponent<RectTransform>();
            if (dialogRt == null)
            {
                return;
            }
            defaultContentSize = dialogRt.sizeDelta;
            var scrollView = GetComponentInChildren<ScrollRect>();
            if (scrollView == null)
            {
                return;
            }
            contentRt = scrollView.content;
            if (contentRt == null)
            {
                return;
            }
            contentRt.sizeDelta = defaultContentSize;
            contentLayout = contentRt.GetComponent<VerticalLayoutGroup>();

            // Close dialog after tapping outside of screen
            if (bgTr != null)
            {
                imgTrigger = bgTr.GetComponent<ImageButtonTrigger>();
            }
            if (imgTrigger != null)
            {
                imgTrigger.OnClick += () =>
                {
                    if (gameObject.activeSelf)
                    {
                        gameObject.SetActive(false);
                    }
                };
            }


            prefabMgr = SettingsPrefabManager.Instance;

            finishedSetup = true;
        }

        /// <summary>
        /// Register options
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="index">Initial value</param>
        public void AddOptions(string[] options, int index)
        {
            if (!finishedSetup)
            {
                Awake();
            }

            for (int i = 0; i < options.Length; i++)
            {
                bool onOff = (i == index);
                AddOption(options[i], onOff);
            }
        }

        /// <summary>
        /// Reset options
        /// </summary>
        public void ClearOptions()
        {
            for (int i = 0; i < options.Count; i++)
            {
                Destroy(options[i].gameObject);
            }

            options.Clear();

            // Reset size of option list
            contentRt.sizeDelta = defaultContentSize;
        }

        /// <summary>
        /// Change RadioButton display
        /// </summary>
        /// <param name="index">Index of selected value</param>
        public void ChangeAppearance(int index)
        {
            for (int i = 0; i < options.Count; i++)
            {
                options[i].OnOff = (i == index);
            }
        }

        /// <summary>
        /// Add option
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="onOff">Selected or not</param>
        void AddOption(string label, bool onOff)
        {
            // Make option
            int index = options.Count;
            var obj = Instantiate(prefabMgr.DropdownOptionPrefab, contentRt);
            var option = obj.GetComponent<DropdownOption>();
            option.Init(label, onOff, () =>
            {
                OnClick(index);
            });

            options.Add(option);

            // Adjust size of option list
            var optRt = obj.GetComponent<RectTransform>();
            float optHeight = optRt.sizeDelta.y;
            if (options.Count > 1)
            {
                optHeight += contentLayout.spacing;
            }

            contentRt.sizeDelta += new Vector2(0, optHeight);

            // Adjust dialog below specified height
            float height = (contentRt.sizeDelta.y < MaxHeight) ?
                contentRt.sizeDelta.y : MaxHeight;
            dialogRt.sizeDelta = new Vector2(dialogRt.sizeDelta.x, height);
        }

        /// <summary>
        /// Event that is called when option is selected
        /// </summary>
        /// <param name="index">Option index</param>
        void OnClick(int index)
        {
            ChangeAppearance(index);
            OnSelect?.Invoke(index);
        }
    }
}
