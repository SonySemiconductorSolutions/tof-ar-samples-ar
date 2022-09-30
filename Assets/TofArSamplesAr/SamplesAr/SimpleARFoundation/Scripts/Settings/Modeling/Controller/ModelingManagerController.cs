/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Modeling;
using TofArSettings;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace TofArARSamples.SimpleARFoundation
{
    public class ModelingManagerController : ControllerBase
    {
        private ARSession arSession;
        private ARMeshManager arMeshManager;

        /// <summary>
        /// Event that is called when Modeling stream is started and status is changed
        /// </summary>
        public event ChangeToggleEvent OnStreamStartStatusChanged;

        protected override void Start()
        {
            arSession = FindObjectOfType<ARSession>();
            arMeshManager = FindObjectOfType<ARMeshManager>(true);

            base.Start();
        }

        public void StartStream()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                arMeshManager.enabled = true;
            } 
            else
            {
                TofArModelingManager.Instance.StartStream();
            }
            OnStreamStartStatusChanged?.Invoke(true);
        }

        public void StopStream()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                arMeshManager.DestroyAllMeshes();
                arSession.Reset();
                arMeshManager.enabled = false;
            }
            else
            {
                TofArModelingManager.Instance.StopStream();
            }
            OnStreamStartStatusChanged?.Invoke(false);
        }
    }
}
