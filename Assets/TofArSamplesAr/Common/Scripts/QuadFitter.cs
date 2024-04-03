/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using TofArSettings;
using UnityEngine;

namespace TofArSamples
{
    public class QuadFitter : MonoBehaviour
    {
        /// <summary>
        /// FoV to be fitted
        /// </summary>
        public FovAdjuster fovAdjuster;

        /// <summary>
        /// Aspect arrangement code of TofAr Quad
        /// </summary>
        public QuadAspectFitter aspectFitter;

        /// <summary>
        /// View transform
        /// </summary>
        public Transform ViewTr { get => aspectFitter.transform; }

        /// <summary>
        /// z distance of Quad from camera
        /// </summary>
        public float PlacementDistance = 1;

        protected ScreenRotateController scRotCtrl;

        protected virtual void Awake()
        {
            scRotCtrl = FindObjectOfType<ScreenRotateController>();
        }

        protected virtual void OnEnable()
        {
            fovAdjuster.OnChangeFov += Fitting;
            scRotCtrl.OnRotateDevice += OnRotateDevice;
            Fitting();
        }

        protected virtual void OnDisable()
        {
            if (fovAdjuster)
            {
                fovAdjuster.OnChangeFov -= Fitting;
            }

            if (scRotCtrl)
            {
                scRotCtrl.OnRotateDevice -= OnRotateDevice;
            }
        }

        /// <summary>
        /// Execute fitting
        /// </summary>
        public void Fitting()
        {
            float fov = fovAdjuster.Fov;
            float aspect = fovAdjuster.Aspect;
            Fitting(fov, aspect);
        }

        /// <summary>
        /// Execute fitting
        /// </summary>
        /// <param name="fov">Camera FoV</param>
        /// <param name="aspect">Camera Aspect</param>
        protected void Fitting(float fov, float aspect)
        {
            aspectFitter.ScaleFactor = PlacementDistance;
            bool isPortrait = scRotCtrl.IsPortraitDevice;

            // Scale is set in landscape mode, so when the screen is oriented in portrait mode, get size accordingly
            float w = ViewTr.localScale.x;
            float h = ViewTr.localScale.y;
            if (isPortrait)
            {
                float tmp = w;
                w = h;
                h = tmp;
            }

            // Calculate and place at the distance from the camera in order for it to fit perfectly
            var pos = ViewTr.localPosition;
            pos.z = 0.5f / Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            if (isPortrait)
            {
                pos.z *= w / aspect;
            }
            else
            {
                pos.z *= h;
            }

            ViewTr.localPosition = pos;
        }

        /// <summary>
        /// Event that is called when device is rotated
        /// </summary>
        /// <param name="ori">Screen orientation</param>
        void OnRotateDevice(ScreenOrientation ori)
        {
            Fitting();
        }
    }
}
