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
    /// This class is used to control the effects of the stamp (when it is displayed). 
    /// It is attached to the stamp object prefab.
    /// </summary>
    public class StampEffectController : MonoBehaviour
    {
        private Material mat;
        private MaterialPropertyBlock materialPropertyBlock;
        private MeshRenderer meshRenderer;

        /// <summary>
        /// Functions for animating stamp effects.
        /// </summary>
        IEnumerator StartAppearanceEffect()
        {
            float value = 0;
            yield return new WaitForSeconds(3f);


            while (value < 1f)
            {
                materialPropertyBlock.SetFloat("_fadeValue", value);
                meshRenderer.SetPropertyBlock(materialPropertyBlock);
                value += Time.deltaTime * 0.2f;

                yield return null;
            }

            yield return null;
        }

        /// <summary>
        /// Changing the "stampEffectNumber" argument changes the effect when the image is displayed.
        /// Three effects are implemented.
        /// 0 : fade in
        /// 1 : slide in
        /// 2 : block in
        /// </summary>
        public void InitStamp(Texture tex, int stampEffectNumber)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.GetPropertyBlock(materialPropertyBlock);


            materialPropertyBlock.SetTexture("_MainTexture", tex);
            materialPropertyBlock.SetInt("_effectIdx", stampEffectNumber);
            meshRenderer.SetPropertyBlock(materialPropertyBlock);

            StartCoroutine(StartAppearanceEffect());
        }
    }
}
