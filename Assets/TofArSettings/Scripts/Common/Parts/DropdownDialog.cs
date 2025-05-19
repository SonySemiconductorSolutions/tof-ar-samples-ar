/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
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

        /// <summary>
        /// 選択肢が選ばれた時に呼ばれるイベント
        /// </summary>
        /// <param name="index">選択肢のインデックス</param>
        public delegate void DeleteEvent(int index);

        /// <summary>
        /// 選択肢が選ばれた時に呼ばれるイベント
        /// </summary>
        public event DeleteEvent OnDelete;

        /// <summary>
        /// 選択肢が選ばれた時に呼ばれるイベント
        /// </summary>
        /// <param name="index">選択肢のインデックス</param>
        public delegate void RenameEvent(int index, string newName);

        /// <summary>
        /// 選択肢が選ばれた時に呼ばれるイベント
        /// </summary>
        public event RenameEvent OnRename;

        private DeleteConfirmPanel deletePanel;
        private RenamePanel renamePanel;
        private MultiCamSupportToggles multiCamSupportToggles;

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
                else if (tr.name.Contains("Toggles"))
                {
                    //supportToggles = tr;
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
            defaultContentSize.x = 0;
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

            deletePanel = this.transform.GetComponentInChildren<DeleteConfirmPanel>();
            renamePanel = this.transform.GetComponentInChildren<RenamePanel>();
            multiCamSupportToggles = this.transform.GetComponentInChildren<MultiCamSupportToggles>();
            SetActiveMultiCamSupportToggles(false);

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

            if (options == null)
            {
                return;
            }

            for (int i = 0; i < options.Length; i++)
            {
                bool onOff = (i == index);
                AddOption(options[i], onOff);
            }
        }

        /// <summary>
        /// Register options
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="index">Initial value</param>
        public void AddOptions(KeyValuePair<string,EditFlags>[] options, int index)
        {
            if (!finishedSetup)
            {
                Awake();
            }

            for (int i = 0; i < options.Length; i++)
            {
                bool onOff = (i == index);
                AddOptionEditable(options[i].Key, options[i].Value, onOff);
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
        /// Select Active DropdownOption
        /// </summary>
        /// <param name="indices">index list</param>
        public void SelectActiveOptions(int[] indices)
        {
            var optRt = options[0].GetComponent<RectTransform>();
            float optHeight = optRt.sizeDelta.y;

            for (int i = 1; i < options.Count; i++)
            {
                if (0 <= Array.IndexOf(indices, i))
                {
                    options[i].gameObject.SetActive(true);

                    optRt = options[i].GetComponent<RectTransform>();
                    optHeight += optRt.sizeDelta.y;
                    optHeight += contentLayout.spacing;
                }
                else
                {
                    options[i].gameObject.SetActive(false);
                }
            }

            contentRt.sizeDelta = defaultContentSize;
            contentRt.sizeDelta += new Vector2(0, optHeight);
        }

        /// <summary>
        /// Select Reset Active DropdownOption 
        /// </summary>
        public void ClearSelectActiveOptions()
        {
            var optRt = options[0].GetComponent<RectTransform>();
            float optHeight = optRt.sizeDelta.y;

            for (int i = 1; i < options.Count; i++)
            {
                options[i].gameObject.SetActive(true);

                optRt = options[i].GetComponent<RectTransform>();
                optHeight += optRt.sizeDelta.y;
                optHeight += contentLayout.spacing;
            }

            contentRt.sizeDelta = defaultContentSize;
            contentRt.sizeDelta += new Vector2(0, optHeight);
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
            }, null, null);

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

        void AddOptionEditable(string label, EditFlags editFlags, bool onOff)
        {
            // 選択肢生成
            int index = options.Count;
            var obj = Instantiate(prefabMgr.DropdownOptionPrefab, contentRt);
            var option = obj.GetComponent<DropdownOption>();

            UnityEngine.Events.UnityAction onDeleteAction = null;
            UnityEngine.Events.UnityAction onRenameAction = null;

            if (editFlags.HasFlag(EditFlags.Deletable))
            {
                onDeleteAction = () => OnClickDelete(index);
            }

            if (editFlags.HasFlag(EditFlags.Renamable))
            {
                onRenameAction = () => OnClickRename(index);
            }

            option.Init(label, onOff, () => { OnClick(index); }, onDeleteAction, onRenameAction);

            options.Add(option);

            // 選択肢一覧のサイズ調整
            var optRt = obj.GetComponent<RectTransform>();
            float optHeight = optRt.sizeDelta.y;
            if (options.Count > 1)
            {
                optHeight += contentLayout.spacing;
            }

            contentRt.sizeDelta += new Vector2(0, optHeight);

            // ダイアログは指定した高さ以下に調整
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

        void OnClickDelete(int index)
        {
            deletePanel.OnConfirmDelete = (isDelete) => {
                if (isDelete)
                {
                    if (index >= 0)
                    {
                        OnDelete?.Invoke(index);
                    }
                }
                deletePanel.PanelObj.SetActive(false);

                deletePanel.OnConfirmDelete = null;
            };

            // open confirmation dialog
            deletePanel.OpenPanel(false);
        }

        void OnClickRename(int index)
        {
            renamePanel.OnConfirmRename = (newName) => {
                if (newName != null && newName.Length > 0)
                {
                    OnRename?.Invoke(index, newName);
                }

                renamePanel.PanelObj.SetActive(false);

                renamePanel.OnConfirmRename = null;
            };

            // open confirmation dialog
            renamePanel.OpenPanel(false);

        }

        /// <summary>
        /// SetActive MultiCamSupportToggles GameObject
        /// </summary>
        /// <param name="state"></param>
        public void SetActiveMultiCamSupportToggles(bool state)
        {
            if (!state)
            {
                multiCamSupportToggles.ResetIsOn();
            }

            multiCamSupportToggles.gameObject.SetActive(state);
        }

    }
}
