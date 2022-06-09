/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class SliderExt : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler
    {
        public UnityAction OnDown, OnUp;

        private ScrollRect scrollRect;
        private Slider slider;

        private bool isDragging = false;

        private void Awake()
        {
            scrollRect = GetComponentInParent<ScrollRect>();
            slider = GetComponent<Slider>();
        }

        /// <summary>
        /// Event that is called when Slider operation is started
        /// </summary>
        /// <param name="eventData">Information on the location where event was triggered</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            OnDown?.Invoke();
        }

        /// <summary>
        /// Event that is called when Slider operation is ended
        /// </summary>
        /// <param name="eventData">Information on the location where event was triggered</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging)
            {
                OnUp?.Invoke();
            }
        }

        /// <summary>
        /// Event that is called when Slider drag is started
        /// </summary>
        /// <param name="eventData">Information on the location where event was triggered</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (scrollRect && !slider.interactable)
            {
                scrollRect.OnBeginDrag(eventData);

                isDragging = true;
            }
        }

        /// <summary>
        /// Event that is called when Slider drag is ended
        /// </summary>
        /// <param name="eventData">Information on the location where event was triggered</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (scrollRect && !slider.interactable)
            {
                scrollRect.OnEndDrag(eventData);

                isDragging = false;
            }
        }

        /// <summary>
        /// Event that is called when Slider is dragged
        /// </summary>
        /// <param name="eventData">Information on the location where event was triggered</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (scrollRect && !slider.interactable)
            {
                scrollRect.OnDrag(eventData);
            }

            slider.OnDrag(eventData);
        }

        /// <summary>
        /// Event that is called when Slider is scrolled
        /// </summary>
        /// <param name="eventData">Information on the location where event was triggered</param>
        public void OnScroll(PointerEventData eventData)
        {
            if (scrollRect && !slider.interactable)
            {
                scrollRect.OnScroll(eventData);
            }
        }
    }
}
