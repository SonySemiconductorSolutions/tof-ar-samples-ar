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
    public class Toolbar : MonoBehaviour
    {
        /// <summary>
        /// Toolbar width
        /// </summary>
        public float BarWidth { get; private set; }

        RectTransform rtBar, rtLeft, rtRight, rtBottom;
        float iconWidthLeft, iconWidthRight;
        RectOffset padLeft, padRight;
        float spaceLeft, spaceRight;

        ScreenRotateController scRotCtrl;

        [SerializeField]
        GameObject notSelect;

        bool notSelectState = false;

        void Awake()
        {
            // Get UI
            rtBar = GetComponent<RectTransform>();
            BarWidth = GetWidth(rtBar);
            foreach (var hori in GetComponentsInChildren<HorizontalLayoutGroup>())
            {
                if (hori.name.Contains("Left"))
                {
                    rtLeft = hori.GetComponent<RectTransform>();
                    iconWidthLeft = GetWidth(rtLeft);
                    padLeft = hori.padding;
                    spaceLeft = hori.spacing;
                }
                else if (hori.name.Contains("Right"))
                {
                    rtRight = hori.GetComponent<RectTransform>();
                    iconWidthRight = GetWidth(rtRight);
                    padRight = hori.padding;
                    spaceRight = hori.spacing;
                }
            }

            rtBottom = transform.Find("Bottom").gameObject.GetComponent<RectTransform>();

            scRotCtrl = FindObjectOfType<ScreenRotateController>();

            SetOutArea();
        }

        void OnEnable()
        {
            scRotCtrl.OnRotateScreen += Move;
        }

        void OnDisable()
        {
            if (scRotCtrl)
            {
                scRotCtrl.OnRotateScreen -= Move;
            }
        }

        /// <summary>
        /// Move toolbar according to screen orientation
        /// </summary>
        /// <param name="ori">Screen orientation</param>
        void Move(ScreenOrientation ori)
        {
            if (scRotCtrl.IsPortrait)
            {
                AdjustBar(true);
                AdjustIcons(true, rtLeft, iconWidthLeft, padLeft, spaceLeft,
                    TextAnchor.MiddleLeft);
                AdjustIcons(true, rtRight, iconWidthRight, padRight, spaceRight,
                    TextAnchor.MiddleRight);
            }
            else
            {
                AdjustBar(false);
                AdjustIcons(false, rtLeft, iconWidthLeft, padLeft, spaceLeft,
                    TextAnchor.UpperCenter);
                AdjustIcons(false, rtRight, iconWidthRight, padRight, spaceRight,
                    TextAnchor.LowerCenter);
            }

            SetOutArea();
        }

        /// <summary>
        /// Get fixed width of UI
        /// </summary>
        /// <param name="rt">UI</param>
        /// <returns>Fixed width</returns>
        float GetWidth(RectTransform rt)
        {
            // Shorter side has fixed width
            return (rt.rect.width < rt.rect.height) ?
                rt.rect.width : rt.rect.height;
        }

        /// <summary>
        /// Adjust toolbar
        /// </summary>
        /// <param name="isPortrait">Screen is portrait or landscape</param>
        void AdjustBar(bool isPortrait)
        {
            if (isPortrait)
            {
                // Move to bottom and make horizontal
                rtBar.anchorMin = new Vector2(0, 0);
                rtBar.anchorMax = new Vector2(1, 0);
                rtBar.pivot = new Vector2(0.5f, 0);
                rtBar.sizeDelta = new Vector2(0, BarWidth);
            }
            else
            {
                // Move to right and make vertical
                rtBar.anchorMin = new Vector2(1, 0);
                rtBar.anchorMax = new Vector2(1, 1);
                rtBar.pivot = new Vector2(1, 0.5f);
                rtBar.sizeDelta = new Vector2(BarWidth, 0);
            }
        }

        /// <summary>
        /// Adjust toolbar icons
        /// </summary>
        /// <param name="isPortrait">Screen is portrait or landscape</param>
        /// <param name="rt">UI where icons are placed</param>
        /// <param name="iconWidth">Width of UI where icons are placed</param>
        /// <param name="padding">LayoutGroup padding</param>
        /// <param name="spacing">LayoutGroup spacing</param>
        /// <param name="childAlignment">Icon arrangement</param>
        void AdjustIcons(bool isPortrait, RectTransform rt, float iconWidth,
            RectOffset padding, float spacing, TextAnchor childAlignment)
        {
            if (!rt)
            {
                return;
            }

            if (isPortrait)
            {
                // Line up icons horizontally if screen orientation is vertical
                rt.anchorMin = new Vector2(0, 0.5f);
                rt.anchorMax = new Vector2(1, 0.5f);
                rt.sizeDelta = new Vector2(0, iconWidth);

                var vlg = rt.GetComponent<VerticalLayoutGroup>();
                if (vlg)
                {
                    DestroyImmediate(vlg);
                }

                var hlg = rt.GetComponent<HorizontalLayoutGroup>();
                if (!hlg)
                {
                    hlg = rt.gameObject.AddComponent<HorizontalLayoutGroup>();
                }

                hlg.padding = padding;
                hlg.spacing = spacing;
                hlg.childAlignment = childAlignment;
                hlg.childControlWidth = false;
                hlg.childControlHeight = false;
                hlg.childForceExpandWidth = false;
                hlg.childForceExpandHeight = true;
            }
            else
            {
                // Line up icons vertically if screen orientation is horizontal
                rt.anchorMin = new Vector2(0.5f, 0);
                rt.anchorMax = new Vector2(0.5f, 1);
                rt.sizeDelta = new Vector2(iconWidth, 0);

                var hlg = rt.GetComponent<HorizontalLayoutGroup>();
                if (hlg)
                {
                    DestroyImmediate(hlg);
                }

                var vlg = rt.GetComponent<VerticalLayoutGroup>();
                if (!vlg)
                {
                    vlg = rt.gameObject.AddComponent<VerticalLayoutGroup>();
                }

                var pad = new RectOffset(padding.top, padding.bottom, padding.left, padding.right);
                vlg.padding = pad;
                vlg.spacing = spacing;
                vlg.childAlignment = childAlignment;
                vlg.childControlWidth = false;
                vlg.childControlHeight = false;
                vlg.childForceExpandWidth = true;
                vlg.childForceExpandHeight = false;

            }
        }

        /// <summary>
        /// Display outside the lower SafeArea
        /// </summary>
        private void SetOutArea()
        {
            if (scRotCtrl.IsPortrait)
            {
                Rect safeArea = Screen.safeArea;
                rtBottom.sizeDelta = new Vector2(0, safeArea.y);
            }
            else
            {
                Debug.Log("Screen L");

                rtBottom.sizeDelta = Vector2.zero;
            }
        }

        /// <summary>
        /// Set not selected
        /// </summary>
        /// <param name="state">Show/Hide</param>
        public void SetNotSelect(bool state)
        {
            if (notSelectState != state)
            {
                notSelect.SetActive(state);
                notSelectState = state;
            }
        }
    }
}
