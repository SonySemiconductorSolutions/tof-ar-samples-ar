/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class SettingsPanel : Panel
    {
        public UnityAction OnClickBack;

        RawImage imgTitle, imgBackBtn;
        Text txtTitle;
        ImageButtonTrigger backTrigger;

        RectTransform uiArea, contentArea;
        HorizontalOrVerticalLayoutGroup panelLayout;
        Vector2 headerSize;
        float offset;

        ScrollRect scrollRect;
        float scrollBarSpace;

        List<Item> items = new List<Item>();

        CanvasScaleController canvasScCtrl;
        ScreenRotateController scRotCtrl;
        Toolbar toolbar;

        SettingsPrefabManager prefabMgr;

        protected override void Awake()
        {
            // Get UI
            PanelObj = transform.GetChild(0).gameObject;

            foreach (var img in GetComponentsInChildren<RawImage>())
            {
                if (img.name.Contains("Icon"))
                {
                    imgTitle = img;
                }
                else if (img.name.Contains("Btn"))
                {
                    imgBackBtn = img;
                }
            }

            txtTitle = GetComponentInChildren<Text>();

            scrollRect = GetComponentInChildren<ScrollRect>();
            uiArea = scrollRect.GetComponent<RectTransform>();

            // Get Scrollbar width
            var scrollbar = scrollRect.verticalScrollbar;
            var scrollbarRt = scrollbar.GetComponent<RectTransform>();
            scrollBarSpace = scrollbarRt.sizeDelta.x + scrollRect.verticalScrollbarSpacing;

            backTrigger = GetComponentInChildren<ImageButtonTrigger>();

            prefabMgr = SettingsPrefabManager.Instance;

            base.Awake();

            // Set this transform as the target of size adjustment
            rt = GetComponent<RectTransform>();

            // Determine the height of the header section from the font size and the underline thickness
            panelLayout = PanelObj.GetComponent<VerticalLayoutGroup>();
            float width = Size.x - panelLayout.padding.left - panelLayout.padding.right;
            float height = txtTitle.fontSize * 3 + panelLayout.spacing;
            headerSize = new Vector2(width, height);

            // Get actual location of where UI is placed
            var vlg = uiArea.GetComponentInChildren<VerticalLayoutGroup>();
            contentArea = vlg.GetComponent<RectTransform>();

            canvasScCtrl = FindObjectOfType<CanvasScaleController>();
            canvasScCtrl.OnChangeSafeArea += (safeAreaSize) =>
            {
                AdjustUISize();
            };
            toolbar = FindObjectOfType<Toolbar>();

            scRotCtrl = canvasScCtrl.GetComponent<ScreenRotateController>();
            scRotCtrl.OnRotateScreen += OnRotateScreen;
            OnRotateScreen(Screen.orientation);
        }

        void OnDestroy()
        {
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Destroy(items[i].gameObject);
                }

                items = null;
            }
        }

        /// <summary>
        /// Action when open
        /// </summary>
        protected override void OnOpenStart()
        {
            StartCoroutine(AdjustUIWidth());
            base.OnOpenStart();
        }

        protected override void CloseOther()
        {
            foreach (var panel in FindObjectsOfType<SettingsPanel>())
            {
                if (panel == this)
                {
                    continue;
                }

                if (panel.IsOpen)
                {
                    panel.ClosePanel();
                }
            }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="titleIcon">Title icon image</param>
        /// <param name="iconColor">Title icon color</param>
        /// <param name="title">Title</param>
        /// <param name="btn">Open/close button</param>
        /// <param name="enableBack">Use/do not use back button</param>
        /// <param name="offset">Distance between toolbar and Panel</param>
        /// <param name="onClickBack">Event that is called when back button is pressed</param>
        public void Init(Texture titleIcon, UnityEngine.Color iconColor,
            string title, ToolButton btn, bool enableBack, float offset,
            UnityAction onClickBack)
        {
            imgTitle.texture = titleIcon;
            imgTitle.color = iconColor;
            txtTitle.text = title;
            Btn = btn;
            if (enableBack)
            {
                OnClickBack = onClickBack;
                backTrigger.OnClick += () =>
                {
                    OnClickBack?.Invoke();
                };
            }
            else
            {
                // Hide back button
                imgBackBtn.gameObject.SetActive(false);

                // Remove trigger for back
                Destroy(backTrigger);
                backTrigger = null;
            }

            this.offset = offset;
        }

        /// <summary>
        /// Add item (Nest Menu)
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="icon">Icon image</param>
        /// <param name="iconColor">Icon color</param>
        /// <param name="onClick">Event that is called when button is pressed</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="lineAlpha">Alpha of line color</param>
        /// <returns>Item class</returns>
        public ItemNestMenu AddItem(string title, Texture icon, UnityEngine.Color iconColor,
            UnityAction onClick, int relativeFontSize = 0, float fixedTitleWidth = 0, byte lineAlpha = 255)
        {
            var item = AddItem(prefabMgr.ItemNestMenuPrefab, lineAlpha) as ItemNestMenu;
            if (item != null)
            {
                item.Init(title, relativeFontSize, fixedTitleWidth, icon, iconColor,
                onClick);
            }
            return item;
        }

        /// <summary>
        /// Add item (Button)
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="onClick">Event that is called when button is pressed</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="lineAlpha">Alpha of line color</param>
        /// <returns>Item class</returns>
        public ItemButton AddItem(string title, UnityAction onClick,
            int relativeFontSize = 0, float fixedTitleWidth = 0, byte lineAlpha = 255)
        {
            var item = AddItem(prefabMgr.ItemButtonPrefab, lineAlpha) as ItemButton;
            if (item != null)
            {
                item.Init(title, relativeFontSize, fixedTitleWidth, onClick);
            }
            return item;
        }

        /// <summary>
        /// Add item (Dropdown)
        /// </summary>
        /// <param name="options">List of options</param>
        /// <param name="index">Intial value</param>
        /// <param name="onChange">Event that is called when Dropdown value is changed</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="width">Dropdown width</param>
        /// <param name="lineAlpha">Alpha of line color</param>
        /// <returns>Item class</returns>
        public ItemDropdown AddItem(string title, string[] options, int index,
            ItemDropdown.ChangeEvent onChange, int relativeFontSize = 0,
            float fixedTitleWidth = 0, float width = 0, byte lineAlpha = 255)
        {
            var item = AddItem(prefabMgr.ItemDropdownPrefab, lineAlpha) as ItemDropdown;
            if (item != null)
            {
                item.Init(title, relativeFontSize, fixedTitleWidth, options, index,
                  onChange, width);
            }
            return item;
        }

        /// <summary>
        /// Add item (Dropdown)
        /// </summary>
        /// <param name="options">List of options</param>
        /// <param name="index">Intial value</param>
        /// <param name="onChange">Event that is called when Dropdown value is changed</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="width">Dropdown width</param>
        /// <param name="lineAlpha">Alpha of line color</param>
        /// <returns>Item class</returns>
        public ItemDropdown AddItem(string title, KeyValuePair<string, EditFlags>[] options, int index,
            ItemDropdown.ChangeEvent onChange, ItemDropdown.DeleteEvent onDelete, ItemDropdown.RenameEvent onRename,
            int relativeFontSize = 0, float fixedTitleWidth = 0, float width = 0, byte lineAlpha = 255)
        {
            var item = AddItem(prefabMgr.ItemDropdownPrefab, lineAlpha) as ItemDropdown;
            if (item != null)
            {
                item.Init(title, relativeFontSize, fixedTitleWidth, options, index,
                  onChange, onDelete, onRename, width);
            }
            return item;
        }

        /// <summary>
        /// Add item (InputField, string)
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="val">Initial value</param>
        /// <param name="onChange">Event that is called when InputField value is changed</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="width">InputField width</param>
        /// <param name="lineAlpha">Alpha of line color</param>
        /// <returns>Item class</returns>
        public ItemInputField AddItem(string title, string val,
            ItemInputField.ChangeTextEvent onChange, int relativeFontSize = 0,
            float fixedTitleWidth = 0, float width = 0, byte lineAlpha = 255)
        {
            var item = AddItem(prefabMgr.ItemInputFieldPrefab, lineAlpha) as ItemInputField;
            if (item != null)
            {
                item.Init(title, relativeFontSize, fixedTitleWidth, val, onChange, width);
            }

            return item;
        }

        /// <summary>
        /// Add item (InputField, integer)
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <param name="val">Initial value</param>
        /// <param name="onChange">Event that is called when InputField value is changed</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="width">InputField width</param>
        /// <param name="lineAlpha">Alpha of line color</param>
        /// <returns>Item class</returns>
        public ItemInputField AddItem(string title, int min, int max, int val,
            ItemInputField.ChangeIntEvent onChange, int relativeFontSize = 0,
            float fixedTitleWidth = 0, float width = 0, byte lineAlpha = 255)
        {
            var item = AddItem(prefabMgr.ItemInputFieldPrefab, lineAlpha) as ItemInputField;
            if (item != null)
            {
                item.Init(title, relativeFontSize, fixedTitleWidth, min, max, val,
                onChange, width);
            }
            return item;
        }

        /// <summary>
        /// Add item (InputField, float)
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <param name="val">Initial value</param>
        /// <param name="onChange">Event that is called when InputField value is changed</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="width">InputField width</param>
        /// <param name="lineAlpha">Alpha of line color</param>
        /// <returns>Item class</returns>
        public ItemInputField AddItem(string title, float min, float max,
            float val, ItemInputField.ChangeFloatEvent onChange,
            int relativeFontSize = 0, float fixedTitleWidth = 0, float width = 0, byte lineAlpha = 255)
        {
            var item = AddItem(prefabMgr.ItemInputFieldPrefab, lineAlpha) as ItemInputField;
            if (item != null)
            {
                item.Init(title, relativeFontSize, fixedTitleWidth, min, max, val,
                onChange, width);
            }
            return item;
        }

        /// <summary>
        /// Add item (Slider)
        /// </summary>
        /// <param name="title">Title/param>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <param name="step">Step value</param>
        /// <param name="val">Initial value</param>
        /// <param name="onChange">Event that is called when value is changed</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="lineAlpha">Alpha of line color</param>
        /// <returns>Item class</returns>
        public ItemSlider AddItem(string title, float min, float max,
            float step, float val, ItemSlider.ChangeEvent onChange,
            int relativeFontSize = 0, float fixedTitleWidth = 0, byte lineAlpha = 255)
        {
            var item = AddItem(prefabMgr.ItemSliderPrefab, lineAlpha) as ItemSlider;
            if (item != null)
            {
                item.Init(title, relativeFontSize, fixedTitleWidth, min, max, step,
                val, onChange);
            }
            return item;
        }

        /// <summary>
        /// Add item (Text)
        /// </summary>
        /// <param name="text">Title</param>
        /// <param name="fontStyle">Font Style</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="lineAlpha">Alpha of line color</param>
        /// <returns>Item class</returns>
        public ItemText AddItem(string text, FontStyle fontStyle = FontStyle.Normal,
            bool useDialog = false, int relativeFontSize = 0,
            float fixedTitleWidth = 0, byte lineAlpha = 255)
        {
            var item = AddItem(prefabMgr.ItemTextPrefab, lineAlpha) as ItemText;
            if (item != null)
            {
                item.Init(text, fontStyle, useDialog, relativeFontSize, fixedTitleWidth);
            }

            return item;
        }

        /// <summary>
        /// Add item (Toggle)
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="val">Initial value</param>
        /// <param name="onChange">Event that is called when Toggle status is changed</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="lineAlpha">Alpha of line color</param>
        /// <returns>Item class</returns>
        public ItemToggle AddItem(string title, bool val,
            ItemToggle.ChangeEvent onChange, int relativeFontSize = 0,
            float fixedTitleWidth = 0, byte lineAlpha = 255)
        {
            var item = AddItem(prefabMgr.ItemTogglePrefab, lineAlpha) as ItemToggle;
            if (item != null)
            {
                item.Init(title, relativeFontSize, fixedTitleWidth, val, onChange);
            }
            return item;
        }

        /// <summary>
        /// Add item (Multiple Toggle)
        /// </summary>
        /// <param name="title">Title of each Toggle</param>
        /// <param name="val">Initial value of each Toggle</param>
        /// <param name="onChange">Event that is called when state of each Toggle is changed</param>
        /// <param name="relativeFontSize">Title font size (relative) of each Toggle</param>
        /// <param name="fixedTitleWidth">Title fixed width of each Toggle</param>
        /// <param name="lineAlpha">Alpha of line color</param>
        /// <returns>Item class</returns>
        public ItemToggleMulti AddItem(string[] title, bool[] val,
            ItemToggleMulti.ChangeEvent onChange, int[] relativeFontSize = null,
            float[] fixedTitleWidth = null, byte lineAlpha = 255)
        {
            var item = AddItem(prefabMgr.ItemToggleMultiPrefab, lineAlpha) as ItemToggleMulti;
            if (item != null)
            {
                item.Init(title, relativeFontSize, fixedTitleWidth, val, onChange);
            }
            return item;
        }

        /// <summary>
        /// Adjust UI width with scroll bar
        /// </summary>
        IEnumerator AdjustUIWidth()
        {
            yield return null;

            float w = (scrollRect.verticalScrollbar.gameObject.activeSelf) ? scrollBarSpace : 0;
            for (int i = 0; i < items.Count; i++)
            {
                items[i].AdjustUIWidth(w);
            }
        }

        /// <summary>
        /// Adjust overall height so the UI does not exceed the placement area
        /// </summary>
        public void AdjustUISize()
        {
            float contentHeight = contentArea.sizeDelta.y + headerSize.y + (offset * 4);
            Vector2 safeArea = canvasScCtrl.SafeAreaSize;
            float heightOffset = (scRotCtrl.IsPortraitScreen) ?
                HeightOffsetPortrait : HeightOffsetLandscape;
            float height = (contentHeight > safeArea.y - heightOffset) ?
                safeArea.y - heightOffset - headerSize.y : contentArea.sizeDelta.y;

            uiArea.sizeDelta = new Vector2(uiArea.sizeDelta.x, height);
            Size = new Vector2(Size.x, height + headerSize.y);

            StartCoroutine(AdjustUIWidth());
        }

        /// <summary>
        /// Event that is called when screen is rotated
        /// </summary>
        /// <param name="ori">Screen orientation</param>
        void OnRotateScreen(ScreenOrientation ori)
        {
            float pos = toolbar.BarWidth + offset;

            if (scRotCtrl.IsPortraitScreen)
            {
                // Move panel based on lower center
                rt.anchorMin = new Vector2(0.5f, 0);
                rt.anchorMax = new Vector2(0.5f, 0);
                rt.pivot = new Vector2(0.5f, 0);
                rt.anchoredPosition = new Vector2(0, pos);
            }
            else
            {
                // Move panel based on center right
                rt.anchorMin = new Vector2(1, 0.5f);
                rt.anchorMax = new Vector2(1, 0.5f);
                rt.pivot = new Vector2(1, 0.5f);
                rt.anchoredPosition = new Vector2(-pos, 0);
            }

            AdjustUISize();
        }

        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="itemPrefab">Item prefab</param>
        /// <param name="lineAlpha">Alpha of line color</param>
        /// <returns>Item</returns>
        Item AddItem(GameObject itemPrefab, byte lineAlpha)
        {
            if (!itemPrefab)
            {
                Debug.LogError("AddItem is failed : Prefab is Empty.");
                return null;
            }

            // Create margin line for second and after
            float itemHeight = 0;
            if (items.Count > 0)
            {
                var lineObj = Instantiate(prefabMgr.LinePrefab, contentArea);
                lineObj.name = "-";
                var lineRt = lineObj.GetComponent<RectTransform>();
                itemHeight = lineRt.sizeDelta.y;
                var lineImg = lineObj.GetComponent<Image>();
                Color32 lineColor = lineImg.color;
                lineColor.a = lineAlpha;
                lineImg.color = lineColor;
            }

            // Create parts
            var itemObj = Instantiate(itemPrefab, contentArea);
            var item = itemObj.GetComponent<Item>();

            // Get height of added parts
            var itemRt = item.GetComponent<RectTransform>();
            itemHeight += itemRt.sizeDelta.y;

            // Adjust height of UI placement area
            if (contentArea.childCount <= 1)
            {
                contentArea.sizeDelta = new Vector2(contentArea.sizeDelta.x, itemHeight);
            }
            else
            {
                contentArea.sizeDelta += new Vector2(0, itemHeight);
            }

            items.Add(item);

            return item;
        }
    }
}
