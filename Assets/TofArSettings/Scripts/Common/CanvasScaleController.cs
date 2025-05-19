/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
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
        /// (Settings for mobile)
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

        protected Rect area = new Rect();
        protected Vector2 baseReso;
        protected Toolbar toolbar;
        protected Vector2 latestSafeAreaSize;

        private string modelName;

        protected virtual void Awake()
        {
            // Get UI
            if (!canvasScaler)
            {
                canvasScaler = FindAnyObjectByType<CanvasScaler>();
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

            if (TofArManager.Instance != null)
            {
                var deviceCapability = TofArManager.Instance.GetProperty<DeviceCapabilityProperty>();
                modelName = deviceCapability.modelName;
            }
        }

        protected virtual void Start()
        {
            toolbar = FindAnyObjectByType<Toolbar>();
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
            if (area.size == newArea.size && area.position == newArea.position &&
                latestSafeAreaSize == SafeAreaSize)
            {
                return;
            }

            area.Set(newArea.x, newArea.y, newArea.width, newArea.height);
            area.position = newArea.position;

#if UNITY_EDITOR
            // If the Editor's game view is "Simulator", calculate as if running on mobile
            if (UnityEngine.Device.Application.isEditor)
            {
                canvasScaler.referenceResolution = baseReso;
            }
            else
#endif
            {
#if UNITY_ANDROID || UNITY_IOS
                CalcResolutionByRealSize();
#else
                canvasScaler.referenceResolution = baseReso;
#endif
            }

            float scWidth = Screen.width;
            float scHeight = Screen.height;

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

        /// <summary>
        /// Calculate CanvasScaler's ReferenceResolution using the actual size
        /// </summary>
        protected virtual void CalcResolutionByRealSize()
        {
            float scWidth = Screen.width;
            float scHeight = Screen.height;

            // Calculate the actual size per pixel from the screen width and CanvasScaler's ReferenceResolution
            float realScWidth = (scWidth < scHeight) ? scWidth : scHeight;

            realScWidth *= 25.4f / GetDPI(); ;
            float pixelSize = realScWidth / baseReso.x;

            // Scale the UI so that the toolbar width matches the actual size
            float barWidth = toolbar.BarWidth * pixelSize;
            float ratio = realBarWidth / barWidth;
            canvasScaler.referenceResolution = baseReso / ratio;
        }

        private float GetDPI()
        {
            if (!string.IsNullOrEmpty(modelName))
            {
                if (modelName.Equals("iPhone15,4")) //iPhone 15 6.1inch 1179x2556
                {
                    return 461;
                }
                else if (modelName.Equals("iPhone15,5")) //iPhone 15 Plus 6.7inch 1290x2796
                {
                    return 460;
                }
                else if (modelName.Equals("iPhone16,1")) //iPhone 15 Pro 6.1inch 1179x2556
                {
                    return 461;
                }
                else if (modelName.Equals("iPhone16,2")) //iPhone 15 Pro Max 6.7inch 1290x2796
                {
                    return 460;
                }
                else
                {
                    return Screen.dpi;
                }
            }
            else
            {
                return Screen.dpi;
            }
        }
    }
}
