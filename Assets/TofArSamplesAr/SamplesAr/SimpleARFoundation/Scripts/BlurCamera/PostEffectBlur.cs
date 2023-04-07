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
    public class PostEffectBlur : MonoBehaviour
    {
        private Camera cam;
        [SerializeField]
        private Camera parentCamera;
        private RenderTexture targetRtDirect;
        private RenderTexture targetRtBlur;

        private CommandBuffer blurBuffer;

        public Material combineMaterial;
        public Material matTestY, matTestX, matMask;
        private float[] weights = new float[5];

        private bool enableBlur = true;
        public bool EnableBlur
        {
            get => enableBlur;
            set
            {
                enableBlur = value;
                SetupBlur();
            }
        }

        [SerializeField]
        private float blur = 80;
        public float Blur
        {
            get => blur;
            set {
                if (blur * value == 0f)
                {
                    blur = value;
                    SetupBlur();
                } else
                {
                    blur = value;
                }
                
            }
        }
        private float lastBlur = 0f;

        public float Normalize { get => normalize; set => normalize = value; }
        public float normalize = 1.4f;
        private float lastNormalize = 1.4f;

        private int width, height;
        [SerializeField]
        private int blurScale = 8;

        [SerializeField]
        Material sceneMeshMaterial;

        private void Start()
        {
            cam = GetComponent<Camera>();
            cam.projectionMatrix = parentCamera.projectionMatrix;

            width = parentCamera.pixelWidth;
            height = parentCamera.pixelHeight;
            targetRtDirect = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
            cam.targetTexture = targetRtDirect;

            //targetRtNonblur = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
            targetRtBlur = new RenderTexture(width / blurScale, height / blurScale, 0, RenderTextureFormat.ARGB32);
            blurBuffer = new CommandBuffer();
            blurBuffer.name = "MaskBlur";
            cam.AddCommandBuffer(CameraEvent.AfterEverything, blurBuffer);
            UpdateWeights();
            SetupBlur();
        }

        void Update()
        {
            if (Blur != BlurSettings.BlurStrength)
            {
                Blur = BlurSettings.BlurStrength;
            }

            if (EnableBlur && (lastBlur != blur || lastNormalize != normalize))
            {
                lastBlur = blur;
                lastNormalize = normalize;
                if (blur > 0)
                {
                    UpdateWeights();
                }
            }
            cam.projectionMatrix = parentCamera.projectionMatrix;

            if (width != parentCamera.pixelWidth || height != parentCamera.pixelHeight)
            {
                width = parentCamera.pixelWidth;
                height = parentCamera.pixelHeight;

                targetRtDirect = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
                cam.targetTexture = targetRtDirect;
                SetupBlur();
            }
        }

        private void UpdateWeights()
        {
            float total = 0;
            float d = blur * blur * 0.001f;

            for (int i = 0; i < weights.Length; i++)
            {
                // Offset position per x.
                float x = i * 2f;
                float w = Mathf.Exp(-0.5f * (x * x) / d);
                weights[i] = w;

                if (i > 0)
                {
                    w *= 2.0f;
                }

                total += w;
            }

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] /= total / normalize;
            }
            matTestX.SetFloatArray("_Weights", weights);
            matTestY.SetFloatArray("_Weights", weights);
        }

        private bool cullingMode;
        private void OnPreRender()
        {
            cullingMode = GL.invertCulling;
            GL.invertCulling = false;
            sceneMeshMaterial.SetInt("_Filled", 1);
            
        }

        private void OnPostRender()
        {
            GL.invertCulling = cullingMode;
            sceneMeshMaterial.SetInt("_Filled", 0);
        }

        private void SetupBlur()
        {
            blurBuffer.Clear();
            if (EnableBlur && blur > 0)
            {
                targetRtBlur = new RenderTexture(width / blurScale, height / blurScale, 0, RenderTextureFormat.ARGB32);

                combineMaterial.SetTexture("_Mask", targetRtBlur);
                blurBuffer.Blit(BuiltinRenderTextureType.CameraTarget, targetRtBlur, matMask);

                int bgID = Shader.PropertyToID("_Temp1");
                blurBuffer.GetTemporaryRT(bgID, targetRtBlur.width, targetRtBlur.height, 0, FilterMode.Bilinear);
                float x = 1.0f / targetRtBlur.width;
                matTestX.SetVector("_Offsets", new Vector4(x, 0, 0, 0));
                blurBuffer.Blit(targetRtBlur, bgID, matTestX);

                float y = 1.0f / targetRtBlur.height;
                matTestY.SetVector("_Offsets", new Vector4(0, y, 0, 0));
                blurBuffer.Blit(bgID, targetRtBlur, matTestY);
            } else
            {
                targetRtBlur = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);

                combineMaterial.SetTexture("_Mask", targetRtBlur);
                blurBuffer.Blit(BuiltinRenderTextureType.CameraTarget, targetRtBlur, matMask);
            }
        }
    }
}
