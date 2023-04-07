/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;
using TofAr.V0.Hand;

namespace TofArSettings.Hand
{
    public class GesturePanel : MonoBehaviour
    {
        private CanvasGroup gestureCanvas;
        private Text gestureText;

        [SerializeField]
        private float showTime = 0.2f;
        [SerializeField]
        private float fadeOutTime = 0.5f;

        private float time = 0;

        private void Awake()
        {
            gestureCanvas = GetComponent<CanvasGroup>();
            gestureText = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if (gestureCanvas.alpha > 0)
            {
                time += Time.deltaTime;
                if (time > showTime)
                {
                    gestureCanvas.alpha -= Time.deltaTime / fadeOutTime;
                }
            }
        }

        public void SetGesture(GestureIndex gesture, Vector2 anchordPosition)
        {
            gestureText.text = gesture.ToString();
            gestureText.rectTransform.anchoredPosition = anchordPosition;
            gestureCanvas.alpha = 1;
            time = 0;
        }
    }
}
