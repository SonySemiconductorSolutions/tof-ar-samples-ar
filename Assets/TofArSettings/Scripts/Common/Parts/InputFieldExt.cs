/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class InputFieldExt : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerUpHandler, IDragHandler, IScrollHandler
    {
        private InputField inputField;
        private bool isDragging = false;
        private ScrollRect scrollRect;

        // Start is called before the first frame update
        void Awake()
        {
            scrollRect = GetComponentInParent<ScrollRect>();

            inputField = GetComponent<InputField>();
            inputField.DeactivateInputField();
        }

        /// <summary>
        /// Event that is called when InputField interaction is ended
        /// </summary>
        /// <param name="eventData">Information on the location of where the event was triggered</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging)
            {
                inputField.ActivateInputField();
            }
        }

        /// <summary>
        /// Event that is called when InputField drag is started
        /// </summary>
        /// <param name="eventData">Information on the location of where the event was triggered</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (scrollRect)
            {
                scrollRect.OnBeginDrag(eventData);
            }

            isDragging = true;
            inputField.DeactivateInputField();
        }

        /// <summary>
        /// Event that is called when InputField drag is ended
        /// </summary>
        /// <param name="eventData">Information on the location of where the event was triggered</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (scrollRect)
            {
                scrollRect.OnEndDrag(eventData);
            }

            isDragging = false;
        }

        /// <summary>
        /// Event that is called when InputField is dragged
        /// </summary>
        /// <param name="eventData">Information on the location of where the event was triggered</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (scrollRect)
            {
                scrollRect.OnDrag(eventData);
            }
        }

        /// <summary>
        /// Event that is called when InputField is scrolled
        /// </summary>
        /// <param name="eventData">Information on the location of where the event was triggered</param>
        public void OnScroll(PointerEventData eventData)
        {
            if (scrollRect)
            {
                scrollRect.OnScroll(eventData);
            }
        }
    }
}
