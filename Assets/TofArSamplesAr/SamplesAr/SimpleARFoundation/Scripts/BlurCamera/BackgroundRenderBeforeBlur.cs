/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.Rendering;

namespace TofArARSamples.SimpleARFoundation
{
    [RequireComponent(typeof(Camera))]
    public class BackgroundRenderBeforeBlur : MonoBehaviour
    {
        private CommandBuffer mixedBackgroundBuffer;
        private Camera cam;

        [SerializeField]
        private Camera backgroundCamera;

        [SerializeField]
        private Material backgroundSummingMat;
        [SerializeField]
        private Material postEffectMat;

        private RenderTexture arRenderTexture;

        private int width, height;

        private void Start()
        {
            cam = this.GetComponent<Camera>();
            width = cam.pixelWidth;
            height = cam.pixelHeight;

            arRenderTexture = new RenderTexture(width, height, 16);
            backgroundCamera.targetTexture = arRenderTexture;
            
            backgroundSummingMat.SetTexture("_BlendTex", arRenderTexture);
            postEffectMat.SetTexture("_MobileRGB", arRenderTexture);

            mixedBackgroundBuffer = new CommandBuffer();
            mixedBackgroundBuffer.name = "BackroundMixingAndBlur";

            int bgID = Shader.PropertyToID("_Temp1");
            mixedBackgroundBuffer.GetTemporaryRT(bgID, -1, -1, 24, FilterMode.Bilinear);

            mixedBackgroundBuffer.Blit(BuiltinRenderTextureType.CameraTarget, bgID);
            mixedBackgroundBuffer.Blit(bgID, BuiltinRenderTextureType.CameraTarget, backgroundSummingMat);


            mixedBackgroundBuffer.Blit(BuiltinRenderTextureType.CameraTarget, bgID);
            mixedBackgroundBuffer.Blit(bgID, BuiltinRenderTextureType.CameraTarget, postEffectMat);
           
            cam.AddCommandBuffer(CameraEvent.BeforeImageEffects, mixedBackgroundBuffer);
        }

        private void OnDisable()
        {
            backgroundCamera.targetTexture = null;
            cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, mixedBackgroundBuffer);
        }

        private void Update()
        {
            if (width != cam.pixelWidth || height != cam.pixelHeight)
            {
                width = cam.pixelWidth;
                height = cam.pixelHeight;

                arRenderTexture = new RenderTexture(width, height, 16);
                backgroundCamera.targetTexture = arRenderTexture;
                
                backgroundSummingMat.SetTexture("_BlendTex", arRenderTexture);
                postEffectMat.SetTexture("_MobileRGB", arRenderTexture);

            }
        }

    }
}
