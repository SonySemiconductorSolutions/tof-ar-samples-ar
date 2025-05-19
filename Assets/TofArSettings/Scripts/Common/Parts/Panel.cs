/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class Panel : MonoBehaviour
    {
        /// <summary>
        /// Button to open/close Panel
        /// </summary>
        public ToolButton Btn = null;

        /// <summary>
        /// Panel GameObject
        /// </summary>
        public GameObject PanelObj = null;

        /// <summary>
        /// Fade in/out time (Unit: s)
        /// </summary>
        public float FadeTime = 0.3f;

        /// <summary>
        /// Hide Panel after Start has been executed or hide immediately
        /// </summary>
        public bool WaitSetup = false;

        /// <summary>
        /// Height offset (Portrait)
        /// </summary>
        public float HeightOffsetPortrait = 200;

        /// <summary>
        /// Height offset (Landscape)
        /// </summary>
        public float HeightOffsetLandscape = 40;

        /// <summary>
        /// Panel open/close status
        /// </summary>
        public bool IsOpen
        {
            get
            {
                if (PanelObj)
                {
                    return PanelObj.activeSelf;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Panel size
        /// </summary>
        public Vector2 Size
        {
            get { return rt.sizeDelta; }
            set
            {
                rt.sizeDelta = value;
            }
        }

        /// <summary>
        /// Panel padding
        /// </summary>
        public RectOffset Padding
        {
            get { return layout.padding; }
        }

        /// <summary>
        /// Event that is called when Panel status is changed
        /// </summary>
        /// <param name="openOrClose">Opened/Closed</param>
        public delegate void ChangeEvent(bool openOrClose);

        public event ChangeEvent OnChangeStart;
        public event ChangeEvent OnChange;

        protected RectTransform rt;
        CanvasGroup canvasGroup;
        HorizontalOrVerticalLayoutGroup layout;

        protected Coroutine coOpen;
        protected Coroutine coClose;
        protected bool isRunngingOpen = false;
        protected bool isRunngingClose = false;

        protected List<Panel> childPanels = new List<Panel>();

        protected virtual void Awake()
        {
            rt = PanelObj.GetComponent<RectTransform>();
            layout = PanelObj.GetComponent<HorizontalOrVerticalLayoutGroup>();
        }

        protected virtual void Start()
        {
            if (Btn)
            {
                Btn.OnClick += OnClick;
            }

            canvasGroup = PanelObj.GetComponent<CanvasGroup>();
            if (!canvasGroup)
            {
                Debug.LogError("Panel required Canvas Group Component. Please add.");
                return;
            }

            // Hide
            StartCoroutine(WaitAndClose());
        }

        /// <summary>
        /// Open Panel
        /// </summary>
        /// <param name="closeOther">Close other open panels/Do nothing</param>
        public virtual void OpenPanel(bool closeOther = true)
        {
            if (isRunngingClose && coClose != null)
            {
                StopCoroutine(coClose);
                isRunngingClose = false;
                coClose = null;
            }

            coOpen = StartCoroutine(Show(closeOther));
        }

        /// <summary>
        /// Close Panel
        /// </summary>
        public void ClosePanel()
        {
            if (isRunngingOpen && coOpen != null)
            {
                StopCoroutine(coOpen);
                isRunngingOpen = false;
                coOpen = null;
            }

            coClose = StartCoroutine(Hide());
        }

        public void AutoClosePanel(float sec)
        {
            StartCoroutine(AutoHide(sec));
        }

        /// <summary>
        /// Register child panel
        /// </summary>
        /// <param name="child">child panel</param>
        public void RegisterChildPanel(Panel child)
        {
            child.OnChangeStart += OnChangeChild;
            childPanels.Add(child);
        }

        /// <summary>
        /// Unregister child panel
        /// </summary>
        /// <param name="child">child panel</param>
        public void UnregisterChildPanel(Panel child)
        {
            int index = childPanels.IndexOf(child);
            if (index >= 0)
            {
                child.OnChangeStart -= OnChangeChild;
                childPanels.RemoveAt(index);
            }
        }

        /// <summary>
        /// Action when open
        /// </summary>
        protected virtual void OnOpenStart()
        {
            OnChangeStart?.Invoke(true);
        }

        /// <summary>
        /// Action when opened
        /// </summary>
        protected virtual void OnOpen()
        {
            OnChange?.Invoke(true);
        }

        /// <summary>
        /// Action when open
        /// </summary>
        protected virtual void OnCloseStart()
        {
            OnChangeStart?.Invoke(false);
        }

        /// <summary>
        /// Action when closed
        /// </summary>
        protected virtual void OnClose()
        {
            OnChange?.Invoke(false);
        }

        /// <summary>
        /// Automatically hide after a specified number of seconds
        /// </summary>
        /// <param name="sec">Seconds</param>
        protected virtual IEnumerator AutoHide(float sec)
        {
            // Do not do anything if closed
            if (!PanelObj || !PanelObj.activeSelf)
            {
                yield break;
            }

            float time = 0.0f;
            while (true)
            {
                time += Time.deltaTime;
                if (time >= sec)
                {
                    break;
                }

                yield return null;
            }

            ClosePanel();
        }

        /// <summary>
        /// Close other panels
        /// </summary>
        protected virtual void CloseOther()
        {
            foreach (var panel in FindObjectsByType<UI.Panel>(FindObjectsSortMode.None))
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
        /// Event that is called when button is pressed
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void OnClick(bool onOff)
        {
            if (onOff)
            {
                OpenPanel();
            }
            else
            {
                ClosePanel();

                // Close child panels
                if (childPanels != null)
                {
                    for (int i = 0; i < childPanels.Count; i++)
                    {
                        var child = childPanels[i];
                        if (child.IsOpen)
                        {
                            child.ClosePanel();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event that is called when child panel is opened
        /// </summary>
        /// <param name="openOrClose">Open/Close</param>
        void OnChangeChild(bool openOrClose)
        {
            if (!Btn)
            {
                return;
            }

            // When the child panel is opened, the button color should remain the same
            if (openOrClose)
            {
                Btn.ChangeAppearance(true);
            }

            // When a child panel is closed, if this panel is also closed, turn off the appearance of the button
            if (!openOrClose && !IsOpen)
            {
                Btn.ChangeAppearance(false);
            }
        }

        /// <summary>
        /// Close Panel after waiting approximately 1 frame
        /// </summary>
        IEnumerator WaitAndClose()
        {
            if (WaitSetup)
            {
                yield return null;
            }

            if (PanelObj.activeSelf)
            {
                PanelObj.SetActive(false);
            }
        }

        /// <summary>
        /// Fade Panel in
        /// </summary>
        /// <param name="closeOther">Close other open panels/Do nothing</param>
        protected IEnumerator Show(bool closeOther = true)
        {
            if (Btn)
            {
                Btn.OnOff = true;
            }

            if (!PanelObj || !canvasGroup)
            {
                yield break;
            }

            isRunngingOpen = true;

            if (closeOther)
            {
                CloseOther();
            }

            OnOpenStart();

            float alpha = 0.0f;
            canvasGroup.alpha = alpha;
            PanelObj.SetActive(true);

            // If child panels exists, set the appearance of the buttons so that they do not look OFF
            if (Btn && childPanels != null && childPanels.Count > 0)
            {
                Btn.ChangeAppearance(true);
            }

            yield return null;

            while (alpha < 1.0f)
            {
                alpha += (Time.deltaTime / FadeTime);
                if (alpha >= 1.0f)
                {
                    alpha = 1.0f;
                }

                canvasGroup.alpha = alpha;
                yield return null;
            }

            isRunngingOpen = false;

            OnOpen();
        }

        /// <summary>
        /// Fade Panel out
        /// </summary>
        IEnumerator Hide()
        {
            if (Btn)
            {
                Btn.OnOff = false;
            }

            if (!PanelObj || !canvasGroup)
            {
                yield break;
            }

            isRunngingClose = true;

            OnCloseStart();

            float alpha = canvasGroup.alpha;
            while (alpha > 0.0f)
            {
                alpha -= (Time.deltaTime / FadeTime);
                if (alpha <= 0.0f)
                {
                    alpha = 0.0f;
                }

                canvasGroup.alpha = alpha;
                yield return null;
            }

            isRunngingClose = false;

            OnClose();

            PanelObj.SetActive(false);
        }
    }
}
