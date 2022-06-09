/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using TofAr.V0.Tof;
using TofArSettings;
using UnityEngine;

namespace TofArSamples
{
    public abstract class FovAdjuster : MonoBehaviour
    {
        /// <summary>
        /// Target to adjust FoV
        /// (optional)
        /// </summary>
        public Camera Cam;

        /// <summary>
        /// Camera FoV
        /// </summary>
        public float Fov { get; private set; } = 60;

        /// <summary>
        /// Camera Aspect
        /// </summary>
        public float Aspect
        {
            get
            {
                if (Cam != null)
                {
                    // Standardize as it could be a reciprocal
                    return (Cam.aspect < 1) ? Cam.aspect : 1 / Cam.aspect;
                }
                else
                {
                    float mainAspect = Camera.main.aspect;
                    return (mainAspect < 1) ? mainAspect : 1 / mainAspect;
                }
            }
        }

        
        public bool UniformFill = false;

        public float FillScale
        {
            get
            {
                return fillScale;
            }
        }

        /// <summary>
        /// Event that is called when the FoV is changed
        /// </summary>
        /// <param name="fov">Camera FoV</param>
        /// <param name="aspect">Camera Aspect</param>
        public delegate void ChangeEvent(float fov, float aspect, float fillScale = 1f);

        /// <summary>
        /// Event that is called when the FoV is changed
        /// </summary>
        public event ChangeEvent OnChangeFov;

        ScreenRotateController scRotCtrl;
        protected float fy;
        protected int height;
        protected int width;
        protected float fillScale = 1f;

        void Awake()
        {
            scRotCtrl = FindObjectOfType<ScreenRotateController>();
            scRotCtrl.OnRotateScreen += OnRotateScreen;
        }

        private void OnDestroy()
        {
            scRotCtrl.OnRotateScreen -= OnRotateScreen;
        }

        void OnEnable()
        {
            TofArTofManager.Instance?.CalibrationSettingsLoaded.AddListener(
                OnLoadCalib);
        }

        void OnDisable()
        {
            TofArTofManager.Instance?.CalibrationSettingsLoaded.RemoveListener(
                OnLoadCalib);
        }

        /// <summary>
        /// Event that is called when calibration settings have been loaded
        /// </summary>
        /// <param name="calibration">Calibration setting values</param>
        protected abstract void OnLoadCalib(CalibrationSettingsProperty calibration);

        /// <summary>
        /// Event that is called when the screen is rotated
        /// </summary>
        /// <param name="ori">Screen orientation</param>
        void OnRotateScreen(ScreenOrientation ori)
        {
            Adjust();
        }

        /// <summary>
        /// Adjust FoV
        /// </summary>
        protected virtual void Adjust()
        {
            if (fy <= 0 || height <= 0)
            {
                return;
            }

            float screenAspect = Screen.height > Screen.width ? (float)Screen.height / (float)Screen.width : (float)Screen.width / (float)Screen.height;

            float fillHeight = height * screenAspect;

            if (UniformFill)
            {
                fillScale = width / fillHeight;
            }
            else
            {
                fillScale = 1f;
            }
            Fov = 2 * Mathf.Atan2(0.5f * height, fy) * Mathf.Rad2Deg * fillScale;

             // When the screen is oriented vertically, the previously calculated horizontal FoV will be converted to vertical
            if (scRotCtrl.IsPortrait)
            {
                Fov = Camera.HorizontalToVerticalFieldOfView(Fov, Aspect);
            }

            if (Cam != null)
            {
                Cam.fieldOfView = Fov;
            }
                
            OnChangeFov?.Invoke(Fov, Aspect, fillScale);
        }
    }
}
