/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using UnityEngine;

namespace TofArSettings
{
    public class ScreenRotateController : MonoBehaviour
    {
        public bool IsPortrait
        {
            get
            {
                if (TofArManager.Instantiated)
                {
                    int angle = TofArManager.Instance.GetScreenOrientation();
                    return (angle % 180 != 0);
                }
                else
                {
                    return (ori == ScreenOrientation.Portrait ||
                        ori == ScreenOrientation.PortraitUpsideDown || ori == ScreenOrientation.AutoRotation);
                }
            }
        }

        ScreenOrientation ori = ScreenOrientation.AutoRotation;

        void Update()
        {
            if (TofArManager.Instantiated)
            {
                return;
            }
            // Check if the screen orientation has changed
            var newOri = Screen.orientation;
            if (ori != newOri)
            {
                ori = newOri;
                OnRotateScreen?.Invoke(ori);
            }
        }

        private void OnEnable()
        {
            TofArManager.OnScreenOrientationUpdated += OnTofArScreenRotated;
            OnRotateScreen?.Invoke(Screen.orientation);
        }

        private void OnDisable()
        {
            TofArManager.OnScreenOrientationUpdated -= OnTofArScreenRotated;
        }

        /// <summary>
        /// Event that is called when screen is rotated
        /// </summary>
        /// <param name="ori">Screen orientation</param>
        public delegate void RotateScreenEvent(ScreenOrientation ori);

        public event RotateScreenEvent OnRotateScreen;

        private void OnTofArScreenRotated(ScreenOrientation prev, ScreenOrientation current)
        {
            OnRotateScreen?.Invoke(current);
        }

    }
}
