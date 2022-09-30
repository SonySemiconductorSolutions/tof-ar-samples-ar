/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using TofAr.V0;
using TofAr.V0.Color;
using System.Collections;
using UnityEngine.Rendering;

namespace TofArARSamples.BallPool
{
    [RequireComponent(typeof(Camera))]
    public class DebugServerColorBackground : MonoBehaviour
    {
        [SerializeField]
        private Material yuvViewMaterial;
        [SerializeField]
        private Texture whiteTex;
        [SerializeField]
        private Material addMat;
        [SerializeField]
        private Material rotateMat;

        private Material instanceYuvMaterial;

        private CommandBuffer backgroundBuffer, yuvBuffer;

        private RenderTexture yuvBackground;
        private RenderTexture rgbBackground;

        private Camera backgroundCam;

        private int currentRotation = 1;

        private LensFacing currentFacingDirection;
        private Matrix4x4 initProjectionMatrix;
        private bool isCullingInverted;

        private void Start()
        {
            if (TofArManager.Instance.RuntimeSettings.runMode == RunMode.MultiNode)
            {
                backgroundBuffer = new CommandBuffer();
                backgroundBuffer.name = "Background rendering";
                int bgID = Shader.PropertyToID("_Temp1");
                backgroundBuffer.GetTemporaryRT(bgID, -1, -1, 24, FilterMode.Bilinear);

                backgroundBuffer.Blit(BuiltinRenderTextureType.CameraTarget, bgID, addMat);
                backgroundBuffer.Blit(bgID, BuiltinRenderTextureType.CameraTarget, rotateMat);
                backgroundCam.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, backgroundBuffer);

                yuvBuffer = new CommandBuffer();
                yuvBuffer.name = "YUV loading";
                backgroundCam.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, yuvBuffer);

                Debug.Log("Buffers added");
            }

            instanceYuvMaterial = new Material(yuvViewMaterial);

        }

        private void OnEnable()
        {
            if (TofArManager.Instance.RuntimeSettings.runMode == RunMode.MultiNode)
            {
                backgroundCam = GetComponent<Camera>();
                initProjectionMatrix = backgroundCam.projectionMatrix;

                //StartCoroutine(StartColorCoroutine());
                TofArColorManager.OnStreamStarted += StreamStarted;
                TofArManager.OnScreenOrientationUpdated += OnScreenRotation;

                var orientationProperty = TofArManager.Instance.GetProperty<DeviceOrientationsProperty>();
                OnScreenRotation(ScreenOrientation.AutoRotation, orientationProperty.screenOrientation);
            }
        }

        private void OnDisable()
        {
            this.StopAllCoroutines();
            TofArColorManager.OnStreamStarted -= StreamStarted;
            TofArManager.OnScreenOrientationUpdated -= OnScreenRotation;
            TofArColorManager.Instance.StopStream();
        }

        private void OnPreRender()
        {
            isCullingInverted = GL.invertCulling;
            if (!TofArManager.Instance.UsingIos && this.currentFacingDirection == LensFacing.Front)
            {
                GL.invertCulling = true;
            }
        }

        private void OnPostRender()
        {
            GL.invertCulling = isCullingInverted;
        }

        private IEnumerator StartColorCoroutine()
        {
            yield return new WaitForEndOfFrame();
            TofArColorManager.Instance.StartStream(true);
        }


        private void OnScreenRotation(ScreenOrientation pre, ScreenOrientation cur)
        {
            currentRotation = (int)cur;
            rotateMat.SetInt("_ScreenRotation", currentRotation);
            SetScreenUVs();
            AdjustMatrix();
        }

        private void SetScreenUVs()
        {
            //work out the right UV values to exactly fit around the screen maintaining aspect
            var colorRes = TofArColorManager.Instance.GetProperty<ResolutionProperty>();
            float colorWidth = currentRotation < 3 ? colorRes.height : colorRes.width;
            float colorHeight = currentRotation < 3 ? colorRes.width : colorRes.height;
            var screenRatio = (float)Screen.width / (float)Screen.height;
            var colorRatio = colorWidth / colorHeight;

            if (colorRatio < screenRatio)
            {
                //color is taller
                float scale = colorRatio / screenRatio;
                float offset = (colorHeight - (colorWidth / screenRatio)) / (2 * colorHeight);
                rotateMat.SetVector("_ScaleOffset", new Vector4(1, scale, 0, offset));
            }
            else
            {
                //color is wider
                float scale = screenRatio / colorRatio;
                float offset = (colorWidth - (colorHeight * screenRatio)) / (2 * colorWidth);
                rotateMat.SetVector("_ScaleOffset", new Vector4(scale, 1, offset, 0));
            }
        }

        private void AdjustMatrix()
        {
            var projectionMatrix = initProjectionMatrix;

            var colorRes = TofArColorManager.Instance.GetProperty<ResolutionProperty>();
            float colorWidth = colorRes.width;
            float colorHeight = colorRes.height;
            var screenRatio = (float)Screen.width / (float)Screen.height;
            var colorRatio = colorWidth / colorHeight;

            float fScale;

            if (currentRotation == (int)ScreenOrientation.Portrait || currentRotation == (int)ScreenOrientation.PortraitUpsideDown)
            {
                colorRatio = colorHeight / colorWidth;

                if (screenRatio < colorRatio)
                {
                    fScale = 1f;
                }
                else
                {
                    fScale = screenRatio / colorRatio;
                }
            }
            else
            {
                if (screenRatio < colorRatio)
                {
                    fScale = colorRatio;
                }
                else
                {
                    fScale = screenRatio;
                }

            }

            if (!TofArManager.Instance.UsingIos && currentFacingDirection == LensFacing.Front)
            {
                projectionMatrix.m00 = initProjectionMatrix.m00 * -1;
            }

            projectionMatrix.m00 *= fScale;
            projectionMatrix.m11 *= fScale;

            backgroundCam.projectionMatrix = projectionMatrix;
        }


        private void StreamStarted(object sender, Texture2D colorTexture)
        {
            var resProp = TofArColorManager.Instance.GetProperty<ResolutionProperty>();
            currentFacingDirection = (LensFacing)resProp.lensFacing;
            var projectionMatrix = initProjectionMatrix;
            if (!TofArManager.Instance.UsingIos && currentFacingDirection == LensFacing.Front)
            {
                projectionMatrix.m00 = initProjectionMatrix.m00 * -1;
            }
            backgroundCam.projectionMatrix = projectionMatrix;

            StartCoroutine(SetColorTextureAndMaterial());
        }

        private IEnumerator SetColorTextureAndMaterial()
        {
            var instance = TofArColorManager.Instance;
            while (backgroundCam == null || instance.ColorData == null)
            {
                yield return null;
            }

            Debug.Log("Set color and material");

            switch (TofArManager.Instance.GetScreenOrientation())
            {
                case 270:
                    currentRotation = (int)ScreenOrientation.Portrait;
                    break;
                case 90:
                    currentRotation = (int)ScreenOrientation.PortraitUpsideDown;
                    break;
                case 0:
                    currentRotation = (int)ScreenOrientation.LandscapeLeft;
                    break;
                case 180:
                    currentRotation = (int)ScreenOrientation.LandscapeRight;
                    break;
            }

            rotateMat.SetInt("_ScreenRotation", currentRotation);
            SetScreenUVs();
            AdjustMatrix();

            if ((instance.ColorData.Type == ColorRawDataType.BGRA) || (instance.ColorData.Type == ColorRawDataType.RGB))
            {
                if (rgbBackground == null)
                {
                    var colorRes = TofArColorManager.Instance.GetProperty<ResolutionProperty>();
                    rgbBackground = new RenderTexture(colorRes.width, colorRes.height, 16);
                }

                if (!TofArManager.Instance.UsingIos && currentFacingDirection == LensFacing.Front)
                {
                    yuvBuffer.Blit(instance.ColorTexture, rgbBackground, new Vector2(-1, -1), new Vector2(1, 1));
                }
                else
                {
                    yuvBuffer.Blit(instance.ColorTexture, rgbBackground, new Vector2(1, -1), new Vector2(1, 1));
                }

                addMat.SetTexture("_BlendTex", rgbBackground);
            }
            else if (instance.ColorData.Type == ColorRawDataType.NV21)
            {
                if (yuvBackground == null)
                {
                    yuvBackground = new RenderTexture(instance.CurrentYWidth, instance.YHeight, 16);
                }
                instanceYuvMaterial.SetTexture("_YTex", instance.YTexture);
                instanceYuvMaterial.SetTexture("_UVTex", instance.UVTexture);
                if (!TofArManager.Instance.UsingIos && currentFacingDirection == LensFacing.Front)
                {
                    instanceYuvMaterial.SetTextureScale("_YTex", new Vector2(-1, 1));
                }
                else
                {
                    instanceYuvMaterial.SetTextureScale("_YTex", new Vector2(1, 1));
                }
                yuvBuffer.Blit(whiteTex, yuvBackground, instanceYuvMaterial);

                addMat.SetTexture("_BlendTex", yuvBackground);
            }
        }
    }
}