/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using TofAr.V0;
using TofAr.V0.Tof;
using UnityEngine.XR.ARFoundation;
using System.Collections;

namespace TofArARSamples.SimpleARFoundation
{
    public class WorldCamera : VirtualToFCamera
    {
        public ARSession arSession;
        private bool sessionEnabled;

        ScreenOrientation currentOrientation = ScreenOrientation.Portrait;

        private void OnEnable()
        {
            StartCoroutine(Init());
        }

        private void OnDisable()
        {
            TofArManager.OnScreenOrientationUpdated -= OnOrientationChange;
        }

        private System.Collections.IEnumerator Init()
        {
            while (currentOrientation == (ScreenOrientation)0)
            {
                var orientationProperty = TofArManager.Instance.GetProperty<DeviceOrientationsProperty>();
                currentOrientation = orientationProperty.screenOrientation;
                if (currentOrientation == (ScreenOrientation)0)
                {
                    yield return null;
                    continue;
                }
            }

            TofArManager.OnScreenOrientationUpdated += OnOrientationChange;
            sessionEnabled = this.arSession.enabled;
            thisCamera = GetComponent<Camera>();

            yield return UpdateProjectionCoroutine();
        }


        private void Update()
        {
            if (this.sessionEnabled != this.arSession.enabled)
            {
                this.sessionEnabled = this.arSession.enabled;
                OnSessionSetEnabled(this.sessionEnabled);
            }
        }

        private void OnSessionSetEnabled(bool enabled)
        {
            StartCoroutine(UpdateProjectionCoroutine());
        }

        private System.Collections.IEnumerator UpdateProjectionCoroutine()
        {
            yield return new WaitForEndOfFrame();
            thisCamera.projectionMatrix = CreateProjectionMatrix(TofArTofManager.Instance.CalibrationSettings);
        }


        private void OnOrientationChange(ScreenOrientation prev, ScreenOrientation current)
        {
            currentOrientation = current;
            StartCoroutine(UpdateProjectionCoroutine());
        }

        private bool cullingMode;
        private void OnPreRender()
        {
            cullingMode = GL.invertCulling;
            GL.invertCulling = false;
        }

        private void OnPostRender()
        {
            GL.invertCulling = cullingMode;
        }

        protected override Matrix4x4 CreateProjectionMatrix(CalibrationSettingsProperty settings)
        {

            float width = settings.colorWidth;
            float height = settings.colorHeight;
            var internals = settings.c;
            if (width == 0 || height == 0)
            {
                return thisCamera.projectionMatrix;
            }

            float sWidth = this.portraitHeight;
            float sHeight = this.portraitWidth;

            float sFit = sWidth / width;
            float sFill = sHeight / height;
            float scale = sFill / sFit;
            float screenRatio = sWidth / sHeight;
            float imgRatio = width / height;

            currentOrientation = TofArManager.Instance.GetProperty<DeviceOrientationsProperty>().screenOrientation;

            if (TofArManager.Instance.RuntimeSettings.runMode == RunMode.Default)
            {
                if (currentOrientation == ScreenOrientation.Portrait || currentOrientation == ScreenOrientation.PortraitUpsideDown)
                {
                    width = settings.colorHeight;
                    height = settings.colorWidth;
                    internals.fx = settings.c.fy;
                    internals.fy = settings.c.fx;

                    if (screenRatio > imgRatio)
                    {
                        width = (int)(width * scale);
                    }
                    else
                    {
                        height = (int)(height / scale);
                    }
                }
                else
                {
                    if (screenRatio > imgRatio)
                    {
                        height = (int)(height * scale);
                    }
                    else
                    {
                        width = (int)(width / scale);
                    }
                }
            }
            else
            {
                bool isPortrait = Screen.height > Screen.width;

                if (currentOrientation == ScreenOrientation.Portrait || currentOrientation == ScreenOrientation.PortraitUpsideDown)
                {
                    width = settings.colorHeight;
                    height = settings.colorWidth;
                    internals.fx = settings.c.fy;
                    internals.fy = settings.c.fx;

                    if (isPortrait)
                    {
                        width = (int)(width * scale);
                    }
                    else
                    {
                        sFit = sWidth / width;
                        sFill = sHeight / height;
                        scale = sFill / sFit;
                        height = (int)(height * scale);
                    }
                }
                else
                {
                    if (isPortrait)
                    {
                        sFit = sWidth / height;
                        sFill = sHeight / width;
                        scale = sFill / sFit;
                        width = (int)(width * scale);
                    }
                    else
                    {
                        height = (int)(height * scale);
                    }
                }
            }

            float right = width * nearClip / (2 * internals.fx);
            float top = height * nearClip / (2 * internals.fy);

            return Matrix4x4.Frustum(-right, +right, -top, +top, nearClip, farClip);

        }
    }
}
