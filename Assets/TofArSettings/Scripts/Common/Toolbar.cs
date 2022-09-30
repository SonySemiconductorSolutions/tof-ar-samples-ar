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
        public float BarWidth { get; protected set; }

        protected RectTransform rtBar;
        protected RectTransform rtLeft;
        protected RectTransform rtRight;
        protected RectTransform rtBottom;
        protected float iconWidthLeft;
        protected float iconWidthRight;
        protected RectOffset padLeft;
        protected RectOffset padRight;
        protected float spaceLeft;
        protected float spaceRight;

        protected ScreenRotateController scRotCtrl;
        protected CanvasScaleController canvasScCtrl;

        [SerializeField]
        protected GameObject notSelect;

        protected bool notSelectState = false;

        protected RectTransform rtBand;

        protected virtual void Awake()
        {
            // Get UI
            rtBar = GetComponent<RectTransform>();
            BarWidth = GetWidth(rtBar);
            foreach (var layout in GetComponentsInChildren<HorizontalOrVerticalLayoutGroup>())
            {
                if (layout.name.Contains("Left"))
                {
                    rtLeft = layout.GetComponent<RectTransform>();
                    iconWidthLeft = GetWidth(rtLeft);
                    padLeft = layout.padding;
                    spaceLeft = layout.spacing;
                }
                else if (layout.name.Contains("Right"))
                {
                    rtRight = layout.GetComponent<RectTransform>();
                    iconWidthRight = GetWidth(rtRight);
                    padRight = layout.padding;
                    spaceRight = layout.spacing;
                }
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                var tr = transform.GetChild(i);
                if (tr.name.Contains("Bottom"))
                {
                    rtBottom = tr.GetComponent<RectTransform>();
                    break;
                }
            }

            foreach (var img in GetComponentsInChildren<Image>())
            {
                if (img.name.Contains("Band"))
                {
                    rtBand = img.GetComponent<RectTransform>();
                    break;
                }
            }

            scRotCtrl = FindObjectOfType<ScreenRotateController>();
            canvasScCtrl = FindObjectOfType<CanvasScaleController>();
        }

        protected virtual void OnEnable()
        {
            scRotCtrl.OnRotateScreen += Move;
        }

        protected virtual void OnDisable()
        {
            if (scRotCtrl)
            {
                scRotCtrl.OnRotateScreen -= Move;
            }
        }

        protected virtual void Start()
        {
            Move(Screen.orientation);
        }

        /// <summary>
        /// Move toolbar according to screen orientation
        /// </summary>
        /// <param name="ori">Screen orientation</param>
        protected virtual void Move(ScreenOrientation ori)
        {
            float max = Mathf.Max(Screen.width, Screen.height);
            var bandSize = Vector2.zero;

            if (scRotCtrl.IsPortrait)
            {
                AdjustBar(true);
                AdjustIcons(true, rtLeft, iconWidthLeft, padLeft, spaceLeft,
                    TextAnchor.MiddleLeft);
                AdjustIcons(true, rtRight, iconWidthRight, padRight, spaceRight,
                    TextAnchor.MiddleRight);

                bandSize.x = max;
            }
            else
            {
                AdjustBar(false);
                AdjustIcons(false, rtLeft, iconWidthLeft, padLeft, spaceLeft,
                    TextAnchor.UpperCenter);
                AdjustIcons(false, rtRight, iconWidthRight, padRight, spaceRight,
                    TextAnchor.LowerCenter);

                bandSize.y = max;
            }

            // Extend the toolbar out of the Safe Area
            rtBand.sizeDelta = bandSize;

            SetOutArea();
        }

        /// <summary>
        /// Get fixed width of UI
        /// </summary>
        /// <param name="rt">UI</param>
        /// <returns>Fixed width</returns>
        protected virtual float GetWidth(RectTransform rt)
        {
            // Shorter side has fixed width
            return (rt.rect.width < rt.rect.height) ?
                rt.rect.width : rt.rect.height;
        }

        /// <summary>
        /// Adjust toolbar
        /// </summary>
        /// <param name="isPortrait">Screen is portrait or landscape</param>
        protected virtual void AdjustBar(bool isPortrait)
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
        protected virtual void AdjustIcons(bool isPortrait, RectTransform rt,
            float iconWidth, RectOffset padding, float spacing, TextAnchor childAlignment)
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
        protected virtual void SetOutArea()
        {
            if (scRotCtrl.IsPortrait)
            {
                // Move to bottom and make horizontal
                rtBottom.anchorMin = new Vector2(0.5f, 0);
                rtBottom.anchorMax = new Vector2(0.5f, 0);
                rtBottom.pivot = new Vector2(0.5f, 1);
            }
            else
            {
                // Move to right and make vertical
                rtBottom.anchorMin = new Vector2(1, 0.5f);
                rtBottom.anchorMax = new Vector2(1, 0.5f);
                rtBottom.pivot = new Vector2(0, 0.5f);
            }

            float max = Mathf.Max(rtBand.sizeDelta.x, rtBand.sizeDelta.y) * 2;
            float w = 300;
            float h = 300;
            if (scRotCtrl.IsPortrait)
            {
                w = max;
            }
            else
            {
                h = max;
            }

            rtBottom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            rtBottom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        }

        /// <summary>
        /// Set not selected
        /// </summary>
        /// <param name="state">Show/Hide</param>
        public virtual void SetNotSelect(bool state)
        {
            if (notSelectState != state)
            {
                notSelect.SetActive(state);
                notSelectState = state;
            }
        }
    }
}
