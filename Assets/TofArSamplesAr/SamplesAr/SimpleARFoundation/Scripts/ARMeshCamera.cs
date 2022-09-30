/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace TofArARSamples.SimpleARFoundation
{
    public class ARMeshCamera : MonoBehaviour
    {
        public ARCameraManager arCameraManager;

        // Match projection matrix
        void Update()
        {
            this.GetComponent<Camera>().projectionMatrix = arCameraManager.GetComponent<Camera>().projectionMatrix;
        }
    }
}
