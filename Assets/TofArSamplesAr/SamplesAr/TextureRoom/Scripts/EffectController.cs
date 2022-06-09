/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using UnityEngine;

namespace TofArARSamples.TextureRoom
{
    /// <summary>
    /// This class controls the effects of all
    /// </summary>
    public class EffectController : MonoBehaviour
    {
        [SerializeField]
        private Material mat;

        void Start()
        {
            StartCoroutine(WireFrameEffect());
        }

        /// <summary>
        /// Animate wireframe effects by changing the value of the "_Radius" property
        /// </summary>
        IEnumerator WireFrameEffect()
        {
            float radius = 0;

            while (true)
            {
                mat.SetFloat("_Radius", radius);
                radius += Time.deltaTime * 0.75f;

                if (radius > 10f)
                {
                    radius = 0f;
                    new WaitForSeconds(3f);
                }
                yield return null;
            }
        }
    }
}
