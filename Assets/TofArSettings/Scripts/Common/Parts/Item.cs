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
    public class Item : MonoBehaviour
    {
        public string Title
        {
            get { return txtTitle.text; }
            set
            {
                name = value;
                txtTitle.text = (AddColon) ? $"{value} :" : value;

                // Adjust text width
                float width = (fixedTitleWidth > 0) ?
                    fixedTitleWidth : txtTitle.preferredWidth;
                titleRt.sizeDelta = new Vector2(
                    width, titleRt.sizeDelta.y);
            }
        }

        public float WidthDiff
        {
            get
            {
                return titleRt.sizeDelta.x - defaultTitleWidth;
            }
        }

        /// <summary>
        /// Add colon to title or not
        /// </summary>
        public bool AddColon = true;

        protected bool interactable = true;
        public virtual bool Interactable
        {
            get { return interactable; }
            set
            {
                interactable = value;

                // Set title to half transparent if non-interactable
                UnityEngine.Color titleColor = txtTitle.color;
                titleColor.a = (Interactable) ? titleAlpha : titleAlpha * 0.5f;
                txtTitle.color = titleColor;
            }
        }

        protected Text txtTitle;
        float titleAlpha;
        protected RectTransform titleRt;
        float defaultTitleWidth;
        float fixedTitleWidth = 0;
        protected bool finishedSetup = false;

        protected SettingsPrefabManager prefabMgr;

        protected virtual void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            foreach (var txt in GetComponentsInChildren<Text>())
            {
                if (txt.name.Contains("Title"))
                {
                    txtTitle = txt;
                    break;
                }
            }

            titleAlpha = txtTitle.color.a;

            titleRt = txtTitle.GetComponent<RectTransform>();
            defaultTitleWidth = titleRt.sizeDelta.x;

            prefabMgr = SettingsPrefabManager.Instance;

            finishedSetup = true;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        public virtual void Init(string title, int relativeFontSize,
            float fixedTitleWidth)
        {
            if (!finishedSetup)
            {
                Awake();
            }

            txtTitle.fontSize += relativeFontSize;
            this.fixedTitleWidth = fixedTitleWidth;
            Title = title;
        }
    }
}
