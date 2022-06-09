/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TofAr.V0.Face;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// setup that depends on platforms
    /// </summary>
    public class JugglingSetup : MonoBehaviour
    {
        [SerializeField]
        private JugglingFaceController faceController;

        private void Start()
        {
#if UNITY_ANDROID
            FaceEstimator fac = FindObjectOfType<FaceEstimator>(true);

            if (fac == null || !fac.IsAvailable)
            {
                TofArFaceManager.Instance.enabled = false;
                faceController.enabled = false;
            }
#endif
        }
    }
}
