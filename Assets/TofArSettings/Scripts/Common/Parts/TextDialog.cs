/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class TextDialog : MonoBehaviour
    {
        RectTransform dialogRt, contentRt;
        ImageButtonTrigger imgTrigger;
        bool finishedSetup = false;
        Vector2 defaultContentSize;
        Text txt;

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

            if (!dialogTr)
            {
                return;
            }

            dialogRt = dialogTr.GetComponent<RectTransform>();
            if (!dialogRt)
            {
                return;
            }

            defaultContentSize = dialogRt.sizeDelta;
            defaultContentSize.x = 0;

            var scrollView = GetComponentInChildren<ScrollRect>();
            if (!scrollView)
            {
                return;
            }

            contentRt = scrollView.content;
            if (!contentRt)
            {
                return;
            }

            contentRt.sizeDelta = defaultContentSize;

            // Close dialog after tapping outside of screen
            if (bgTr)
            {
                imgTrigger = bgTr.GetComponent<ImageButtonTrigger>();
            }

            if (imgTrigger)
            {
                imgTrigger.OnClick += () =>
                {
                    if (gameObject.activeSelf)
                    {
                        gameObject.SetActive(false);
                    }
                };
            }

            txt = dialogTr.GetComponentInChildren<Text>();

            finishedSetup = true;
        }

        /// <summary>
        /// Change Text
        /// </summary>
        /// <param name="str">Text</param>
        public void ChangeAppearance(string str)
        {
            if (txt.text.Equals(str))
            {
                return;
            }

            txt.text = str;

            // Need to call it twice because get preferredHeight correctly
            txt.rectTransform.sizeDelta = new Vector2(txt.preferredWidth, txt.preferredHeight);
            txt.rectTransform.sizeDelta = new Vector2(txt.preferredWidth, txt.preferredHeight);

            // Adjust dialog size
            var size = new Vector2(dialogRt.sizeDelta.x, defaultContentSize.y);
            size.y += txt.preferredHeight;
            dialogRt.sizeDelta = size;
        }
    }
}
