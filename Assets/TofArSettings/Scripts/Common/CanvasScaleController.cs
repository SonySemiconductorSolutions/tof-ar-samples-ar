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
    public class CanvasScaleController : MonoBehaviour
    {
        /// <summary>
        /// Target Canvas to adjust
        /// </summary>
        [SerializeField]
        protected CanvasScaler canvasScaler = null;

        /// <summary>
        /// Physical width of toolbar (Unit: mm)
        /// </summary>
        [SerializeField]
        protected float realBarWidth = 8;

        /// <summary>
        /// RectTransform of SafeArea
        /// </summary>
        public RectTransform SafeAreaRt { get; protected set; }

        /// <summary>
        /// SafeArea size
        /// </summary>
        public Vector2 SafeAreaSize
        {
            get
            {
                return (SafeAreaRt) ? new Vector2(SafeAreaRt.rect.width,
                    SafeAreaRt.rect.height) : Vector2.zero;
            }
        }

        /// <summary>
        /// Event that is called when SafeArea size is changed
        /// </summary>
        /// <param name="safeAreaSize">SafeArea size</param>
        public delegate void ChangeEvent(Vector2 safeAreaSize);

        public ChangeEvent OnChangeSafeArea;

        protected Rect area;
        protected Vector2 baseReso;
        protected Toolbar toolbar;
        protected Vector2 latestSafeAreaSize;

        protected virtual void Awake()
        {
            // Get UI
            if (!canvasScaler)
            {
                canvasScaler = FindObjectOfType<CanvasScaler>();
            }

            foreach (var rt in canvasScaler.GetComponentsInChildren<RectTransform>())
            {
                if (rt.name.Contains("SafeArea"))
                {
                    SafeAreaRt = rt;
                    break;
                }
            }

            baseReso = canvasScaler.referenceResolution;
        }

        protected virtual void Start()
        {
            toolbar = FindObjectOfType<Toolbar>();
            AdjustSafeArea(Screen.safeArea);
        }

        protected virtual void Update()
        {
            AdjustSafeArea(Screen.safeArea);
        }

        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        /// Adjust UI according to SafeArea
        /// </summary>
        /// <param name="newArea">SafeArea</param>
        protected virtual void AdjustSafeArea(Rect newArea)
        {
            // Do not do anything if SafeArea has not changed
            if (area == newArea && latestSafeAreaSize == SafeAreaSize)
            {
                return;
            }

            //don't run this for debug server
            /*if(TofAr.V0.TofArManager.Instance?.RuntimeSettings.runMode == TofAr.V0.RunMode.MultiNode)
            {
                return;
            }*/

            if (Application.isEditor)
            {
                return;
            }

            area = newArea;

            float scWidth = Screen.width;
            float scHeight = Screen.height;

            // Calculate the actual saize per pixel from the screen width and CanvasScaler's ReferenceResolution
            float realScWidth = (scWidth < scHeight) ? scWidth : scHeight;
            realScWidth *= 25.4f / Screen.dpi;
            float pixelSize = realScWidth / baseReso.x;

            // Scale the UI so that the toolbar width matches the actual size
            float barWidth = toolbar.BarWidth * pixelSize;
            float ratio = realBarWidth / barWidth;
            canvasScaler.referenceResolution = baseReso / ratio;

            // Adjust the UI area to fit within the SafeArea
            var anchorMin = area.position;
            var anchorMax = area.position + area.size;
            anchorMin.x /= scWidth;
            anchorMin.y /= scHeight;
            anchorMax.x /= scWidth;
            anchorMax.y /= scHeight;
            SafeAreaRt.anchoredPosition = Vector2.zero;
            SafeAreaRt.anchorMin = anchorMin;
            SafeAreaRt.anchorMax = anchorMax;

            latestSafeAreaSize = SafeAreaSize;

            OnChangeSafeArea?.Invoke(latestSafeAreaSize);
        }
    }
}
