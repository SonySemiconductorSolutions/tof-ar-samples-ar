/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TofArSettings.UI
{
    public class ImageButtonTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        /// <summary>
        /// Color when tapped (Default: C8C8C8)
        /// </summary>
        public UnityEngine.Color TapColor = new UnityEngine.Color(0.78125f, 0.78125f, 0.78125f);

        public UnityEngine.Color DefaultColor
        {
            get
            {
                if (!finishedSetup)
                {
                    Awake();
                }

                if (img)
                {
                    return imgColor;
                }
                else if (rawImg)
                {
                    return rawColor;
                }

                return UnityEngine.Color.white;
            }
        }

        /// <summary>
        /// UI Interactability
        /// </summary>
        bool interactable = true;
        public bool Interactable
        {
            get { return interactable; }
            set
            {
                if (interactable != value)
                {
                    interactable = value;
                    UnityEngine.Color color = (value) ? DefaultColor :
                        DefaultColor - new UnityEngine.Color(0.2f, 0.2f, 0.2f, 0);
                    SetColor(color);
                }
            }
        }

        /// <summary>
        /// Events that are called when UI is touched, released or clicked
        /// </summary>
        public UnityAction OnTouchDown, OnTouchUp, OnClick;

        Image img;
        RawImage rawImg;

        UnityEngine.Color imgColor;
        UnityEngine.Color rawColor;
        bool finishedSetup = false;

        /// <summary>
        /// Called directly after application startup (Unity standard function)
        /// </summary>
        void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            img = GetComponent<Image>();
            rawImg = GetComponent<RawImage>();



            // Get appearance of each UI
            if (img)
            {
                imgColor = img.color;
            }

            if (rawImg)
            {
                rawColor = rawImg.color;
            }

            finishedSetup = true;
        }

        /// <summary>
        /// Event that is called when UI is touched
        /// </summary>
        /// <param name="data">Information on the location of where the event was triggered</param>
        public void OnPointerDown(PointerEventData data)
        {
            if (!interactable)
            {
                return;
            }

            SetColor(TapColor);

            OnTouchDown?.Invoke();
        }

        /// <summary>
        /// Event that is called when UI is touched and released
        /// </summary>
        /// <param name="data">Information on the location of where the event was triggered</param>
        public void OnPointerUp(PointerEventData data)
        {
            if (!interactable)
            {
                return;
            }

            // Reset color
            SetColor(DefaultColor);

            OnTouchUp?.Invoke();
        }

        /// <summary>
        /// Event that is called when UI is touched and released on top of the same object
        /// </summary>
        /// <param name="data">Information on the location of where the event was triggered</param>
        public void OnPointerClick(PointerEventData data)
        {
            if (!interactable)
            {
                return;
            }

            OnClick?.Invoke();
        }

        /// <summary>
        /// Change UI color
        /// </summary>
        /// <param name="color">Color</param>
        void SetColor(UnityEngine.Color color)
        {
            if (img)
            {
                img.color = color;
            }

            if (rawImg)
            {
                rawImg.color = color;
            }
        }

    }
}
