/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace TofArARSamples.TextureRoom
{
    /// <summary>
    /// Class for changing the background material depending on the environment
    /// </summary>
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField]
        private Material arkitBackgroundMaterial;

        [SerializeField]
        private Material arcoreBackgroundMaterial;

        [SerializeField]
        private ARCameraBackground arCameraBackground;

        void Start()
        {
#if UNITY_EDITOR
            Debug.Log("Change to BackgroundMaterial in the UNITY_EDITOR environment.");
            arCameraBackground.customMaterial = arkitBackgroundMaterial;

#elif UNITY_IOS
            Debug.Log("Change to the BackgroundMaterial of the UNITY_IOS environment.");
            arCameraBackground.customMaterial = arkitBackgroundMaterial;

#elif UNITY_ANDROID
            Debug.Log("Change to the BackgroundMaterial of the UNITY_ANDROID environment.");
            arCameraBackground.customMaterial = arcoreBackgroundMaterial;
#endif

        }
    }
}
