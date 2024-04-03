/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings
{
    public class ScreenRotateController : MonoBehaviour
    {
        public virtual bool IsPortraitDevice
        {
            get
            {
                return IsPortraitTofAr();
            }
        }

        public virtual bool IsPortraitScreen
        {
            get
            {
                if (canvasScCtrl)
                {
                    var size = canvasScCtrl.SafeAreaSize;
                    return (size.x <= size.y);
                }

                return IsPortraitTofAr();
            }
        }

        public virtual ScreenOrientation OrientationDevice { get; protected set; }
        public virtual ScreenOrientation OrientationScreen { get; protected set; }

        public UnityAction<ScreenOrientation> OnRotateDevice;
        public UnityAction<ScreenOrientation> OnRotateScreen;

        protected bool isPortraitDevice;
        protected bool isPortraitScreen;

        protected UI.CanvasScaleController canvasScCtrl;

        protected virtual void Awake()
        {
            canvasScCtrl = GetComponent<UI.CanvasScaleController>();
            isPortraitDevice = IsPortraitDevice;
            isPortraitScreen = IsPortraitScreen;
        }

        protected virtual void OnEnable()
        {
            TofArManager.OnScreenOrientationUpdated += OnTofArScreenRotated;
        }

        protected virtual void OnDisable()
        {
            TofArManager.OnScreenOrientationUpdated -= OnTofArScreenRotated;
        }

        protected virtual void Start()
        {
            if (TofArManager.Instantiated)
            {
                int angle = TofArManager.Instance.GetDeviceOrientation();
                if (angle == 0)
                {
                    OrientationDevice = ScreenOrientation.LandscapeLeft;
                }
                else if (angle == 90)
                {
                    OrientationDevice = ScreenOrientation.PortraitUpsideDown;
                }
                else if (angle == 180)
                {
                    OrientationDevice = ScreenOrientation.LandscapeRight;
                }
                else if (angle == 270)
                {
                    OrientationDevice = ScreenOrientation.Portrait;
                }

                OnRotateDevice?.Invoke(OrientationDevice);
            }

            OnRotateScreen?.Invoke(Screen.orientation);
        }

        protected virtual void Update()
        {
            if (!TofArManager.Instantiated && isPortraitDevice != IsPortraitDevice)
            {
                isPortraitDevice = IsPortraitDevice;
                OrientationDevice = Screen.orientation;
                OnRotateDevice?.Invoke(OrientationDevice);
            }

            if (isPortraitScreen != IsPortraitScreen)
            {
                isPortraitScreen = IsPortraitScreen;
                if (IsPc())
                {
                    OrientationScreen = (isPortraitScreen) ?
                        ScreenOrientation.Portrait : ScreenOrientation.LandscapeLeft;
                }
                else
                {
                    OrientationScreen = Screen.orientation;
                }

                OnRotateScreen?.Invoke(OrientationScreen);
            }
        }

        protected virtual void OnTofArScreenRotated(ScreenOrientation prev, ScreenOrientation current)
        {
            OrientationDevice = current;
            OnRotateDevice?.Invoke(OrientationDevice);
        }

        public static bool IsPc()
        {
            return (Application.platform != RuntimePlatform.Android &&
                Application.platform != RuntimePlatform.IPhonePlayer);
        }

        public static bool IsPortraitTofAr()
        {
            if (TofArManager.Instantiated)
            {
                int angle = TofArManager.Instance.GetScreenOrientation();
                return (angle % 180 != 0);
            }
            else
            {
                var ori = Screen.orientation;
                return (ori == ScreenOrientation.Portrait ||
                    ori == ScreenOrientation.PortraitUpsideDown || ori == ScreenOrientation.AutoRotation);
            }
        }
    }
}
