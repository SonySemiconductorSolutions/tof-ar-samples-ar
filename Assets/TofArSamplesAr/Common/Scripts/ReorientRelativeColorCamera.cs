/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofAr.V0.Tof
{
    public class ReorientRelativeColorCamera : MonoBehaviour
    {       
        public bool enableRotateInWorld = true;

        private Matrix4x4 u2uLL;
        private Matrix4x4 u2uLR;
        private Matrix4x4 u2uP;
        private Matrix4x4 u2uPUD;

        private void Start()
        {
            TofArTofManager.Instance?.CalibrationSettingsLoaded.AddListener(Apply);
        }

        private void OnDestroy()
        {
            TofArTofManager.Instance?.CalibrationSettingsLoaded.RemoveListener(Apply);
        }

        public void Apply(CalibrationSettingsProperty settings)
        {
            var R = settings.R;
            var T = settings.T;
            var cRT = new Matrix4x4();
            cRT[0, 0] = R.a; cRT[0, 1] = R.d; cRT[0, 2] = R.g; cRT[0, 3] = -T.x * 0.001f;
            cRT[1, 0] = R.b; cRT[1, 1] = R.e; cRT[1, 2] = R.h; cRT[1, 3] = -T.y * 0.001f;
            cRT[2, 0] = R.c; cRT[2, 1] = R.f; cRT[2, 2] = R.i; cRT[2, 3] = -T.z * 0.001f;
            cRT[3, 0] = 0; cRT[3, 1] = 0; cRT[3, 2] = 0; cRT[3, 3] = 1;

            //you have to swap the coordinates when the phone rotates, else the rotation is wrong
            Matrix4x4 c2uLL = new Matrix4x4();
            c2uLL[0, 0] = 1; c2uLL[0, 1] = 0; c2uLL[0, 2] = 0; c2uLL[0, 3] = 0;
            c2uLL[1, 0] = 0; c2uLL[1, 1] = -1; c2uLL[1, 2] = 0; c2uLL[1, 3] = 0;
            c2uLL[2, 0] = 0; c2uLL[2, 1] = 0; c2uLL[2, 2] = 1; c2uLL[2, 3] = 0;
            c2uLL[3, 0] = 0; c2uLL[3, 1] = 0; c2uLL[3, 2] = 0; c2uLL[3, 3] = 1;
            Matrix4x4 u2cLL = c2uLL.transpose;

            Matrix4x4 c2uLR = new Matrix4x4();
            c2uLR[0, 0] = -1; c2uLR[0, 1] = 0; c2uLR[0, 2] = 0; c2uLR[0, 3] = 0;
            c2uLR[1, 0] = 0; c2uLR[1, 1] = 1; c2uLR[1, 2] = 0; c2uLR[1, 3] = 0;
            c2uLR[2, 0] = 0; c2uLR[2, 1] = 0; c2uLR[2, 2] = 1; c2uLR[2, 3] = 0;
            c2uLR[3, 0] = 0; c2uLR[3, 1] = 0; c2uLR[3, 2] = 0; c2uLR[3, 3] = 1;
            Matrix4x4 u2cLR = c2uLR.transpose;

            Matrix4x4 c2uP = new Matrix4x4();
            c2uP[0, 0] = 0; c2uP[0, 1] = -1; c2uP[0, 2] = 0; c2uP[0, 3] = 0;
            c2uP[1, 0] = -1; c2uP[1, 1] = 0; c2uP[1, 2] = 0; c2uP[1, 3] = 0;
            c2uP[2, 0] = 0; c2uP[2, 1] = 0; c2uP[2, 2] = 1; c2uP[2, 3] = 0;
            c2uP[3, 0] = 0; c2uP[3, 1] = 0; c2uP[3, 2] = 0; c2uP[3, 3] = 1;
            Matrix4x4 u2cP = c2uP.transpose;

            Matrix4x4 c2uPUD = new Matrix4x4();
            c2uPUD[0, 0] = 0; c2uPUD[0, 1] = 1; c2uPUD[0, 2] = 0; c2uPUD[0, 3] = 0;
            c2uPUD[1, 0] = 1; c2uPUD[1, 1] = 0; c2uPUD[1, 2] = 0; c2uPUD[1, 3] = 0;
            c2uPUD[2, 0] = 0; c2uPUD[2, 1] = 0; c2uPUD[2, 2] = 1; c2uPUD[2, 3] = 0;
            c2uPUD[3, 0] = 0; c2uPUD[3, 1] = 0; c2uPUD[3, 2] = 0; c2uPUD[3, 3] = 1;
            Matrix4x4 u2cPUD = c2uPUD.transpose;

            u2uLL = c2uLL * cRT * u2cLL;
            u2uLR = c2uLR * cRT * u2cLR;
            u2uP = c2uP * cRT * u2cP;
            u2uPUD = c2uPUD * cRT * u2cPUD;

            RotateAccordingToOrientation();
        }

        private void OnEnable()
        {
            TofArManager.OnScreenOrientationUpdated += OnScreenOrientationChanged;

            RotateAccordingToOrientation();
        }

        private void OnDisable()
        {
            TofArManager.OnScreenOrientationUpdated -= OnScreenOrientationChanged;
        }

        private void OnScreenOrientationChanged(ScreenOrientation previousOrientation, ScreenOrientation newOrientation)
        {
            RotateAccordingToOrientation();
        }

        void RotateAccordingToOrientation()
        {
            int imageRotation = 0;
            Matrix4x4 rotmat = u2uLL;
            if (enableRotateInWorld)
            {
                if (!UnityEngine.XR.XRSettings.enabled)
                {
                    imageRotation = TofArManager.Instance.GetScreenOrientation();

                    {
                        switch (imageRotation)
                        {
                            case 270:
                                rotmat = u2uP;
                                break;
                            case 90:
                                rotmat = u2uPUD;
                                break;
                            case 0:
                                rotmat = u2uLL;
                                break;
                            case 180:
                                rotmat = u2uLR;
                                break;
                            default:
                                break;
                        }
                    }


                }
                else
                {

                    if ((TofArManager.Instance.EnabledOrientations & (TofArManager.Instance.EnabledOrientations - 1)) != 0)
                    {
                        switch (Input.deviceOrientation)
                        {
                            case DeviceOrientation.LandscapeLeft:
                                rotmat = u2uLL;
                                imageRotation = 0; break;
                            case DeviceOrientation.LandscapeRight:
                                rotmat = u2uLR;
                                imageRotation = 180; break;
                            default:
                                break;
                        }

                    }
                    else
                    {
                        switch (TofArManager.Instance.EnabledOrientations)
                        {
                            case EnabledOrientation.LandscapeLeft:
                                rotmat = u2uLL;
                                imageRotation = 0; break;
                            case EnabledOrientation.LandscapeRight:
                                rotmat = u2uLR;
                                imageRotation = 180; break;
                            default:
                                break;
                        }

                    }
                }
            }
            var rotation = Quaternion.LookRotation(rotmat.MultiplyVector(Vector3.forward), rotmat.MultiplyVector(Vector3.up));
            transform.localRotation = rotation;
            gameObject.transform.localPosition = rotmat.MultiplyPoint3x4(Vector3.zero);
            if (this.enableRotateInWorld)
            {
                this.gameObject.transform.Rotate(new Vector3(0, 0, imageRotation));
            }
        }
    }

}
