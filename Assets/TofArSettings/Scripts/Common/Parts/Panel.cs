/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
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
        /// Height offset
        /// </summary>
        public float HeightOffset = 300;

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

        public event ChangeEvent OnChange;

        protected RectTransform rt;
        CanvasGroup canvasGroup;
        HorizontalOrVerticalLayoutGroup layout;

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
            if (IsOpen)
            {
                return;
            }

            StartCoroutine(Show(closeOther));
        }

        /// <summary>
        /// Close Panel
        /// </summary>
        public void ClosePanel()
        {
            if (!IsOpen)
            {
                return;
            }

            StartCoroutine(Hide());
        }

        /// <summary>
        /// Action when opened
        /// </summary>
        protected virtual void OnOpen()
        {
            OnChange?.Invoke(true);
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
            foreach (var panel in FindObjectsOfType<UI.Panel>())
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
            if (!PanelObj || !canvasGroup ||
                PanelObj.activeSelf)
            {
                yield break;
            }

            if (Btn)
            {
                Btn.OnOff = true;
            }

            if (closeOther)
            {
                CloseOther();
            }

            float alpha = 0.0f;
            canvasGroup.alpha = alpha;
            PanelObj.SetActive(true);
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

            OnOpen();
        }

        /// <summary>
        /// Fade Panel out
        /// </summary>
        IEnumerator Hide()
        {
            if (!PanelObj || !canvasGroup ||
                !PanelObj.activeSelf)
            {
                yield break;
            }

            if (Btn)
            {
                Btn.OnOff = false;
            }

            float alpha = 1.0f;
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

            OnClose();

            PanelObj.SetActive(false);
        }
    }
}
