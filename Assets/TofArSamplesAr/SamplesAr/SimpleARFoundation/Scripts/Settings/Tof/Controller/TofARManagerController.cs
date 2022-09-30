/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;
using TofArSettings;
using UnityEngine.XR.ARFoundation;

namespace TofArARSamples.Tof
{
    public class TofARManagerController : CameraManagerController
    {

        private ARCameraManager arSession;

        private bool isAutoStarting = false;

        protected void Awake()
        {
            arSession = FindObjectOfType<ARCameraManager>();

#if !UNITY_EDITOR && !UNITY_STANDALONE
            if (TofArTofManager.Instance.autoStart)
            {
                // delay autostart until ARSession has started properly
                isAutoStarting = true;
                TofArTofManager.Instance.autoStart = false;
            }
#endif
        }

        protected override void OnEnable()
        {
            arSession.frameReceived += ARFrameReceived;

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            arSession.frameReceived -= ARFrameReceived;

            base.OnDisable();
        }

        private void ARFrameReceived(ARCameraFrameEventArgs args)
        {
            if (isAutoStarting)
            {
                isAutoStarting = false;
                TofArTofManager.Instance.StartStream(true);
            }
        }

        public override bool IsStreamActive()
        {
            return TofArTofManager.Instance.IsStreamActive;
        }

    }
}
