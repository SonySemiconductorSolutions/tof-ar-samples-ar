/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using TofAr.V0.Color;
using TofArSettings;
using UnityEngine;
using UnityEngine.UI;

namespace TofArARSamples.BGChange
{
    /// <summary>
    ///A class that resizes the RenderTexture to match the color image of TofAr.
    /// </summary>
    public class SyncRTSize : MonoBehaviour
    {

        [SerializeField]
        private RenderTexture defaultTexture = null;

        [SerializeField]
        private Camera targetCamera = null;

        [SerializeField]
        private Material skySegmentMaterial;

        private Vector2 lastSize = Vector2.zero;

        private Vector2 colorImageSize = Vector2.zero;

        private RenderTexture lastRT = null;

        private ScreenRotateController scRotCtrl;

        private ScreenOrientation lastScreenOrientation = ScreenOrientation.Portrait;

        void Awake()
        {
            scRotCtrl = FindObjectOfType<ScreenRotateController>();
            scRotCtrl.OnRotateScreen += OnRotateScreen;
        }

        protected void OnEnable()
        {
            TofArColorManager.OnStreamStarted += OnColorStreamStart;
        }

        protected  void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnColorStreamStart;
        }

        private void OnColorStreamStart(object sender, Texture2D colorTex)
        {
            var config = TofArColorManager.Instance.GetProperty<ResolutionProperty>();
            colorImageSize = new Vector2(config.width, config.height);
        }

        /// <summary>
        /// An event called when the screen rotates
        /// </summary>
        /// <param name="ori">Screen Orientatio</param>
        void OnRotateScreen(ScreenOrientation ori)
        {
            lastScreenOrientation = ori;
            lastSize = Vector2.zero;
        }

        void Update()
        {
         
            
            if(colorImageSize!= lastSize){

                //Release the previous Render texture
                if(lastRT!=null){
                    lastRT.Release();
                }

                //Resize Render Texture to match TofAr's color picture
                //Since the color image object rotates, it is swapped vertically and horizontally.
                Vector2 renderTextureSize = Vector2.zero;
                if(lastScreenOrientation== ScreenOrientation.Portrait
                || lastScreenOrientation == ScreenOrientation.PortraitUpsideDown){
                    //When holding vertically
                    renderTextureSize = new Vector2(colorImageSize.y, colorImageSize.x);

                }else{
                    //When holding horizontally
                    renderTextureSize = new Vector2(colorImageSize.x, colorImageSize.y);
                }

                var nextRT = new RenderTexture((int)renderTextureSize.x,(int)renderTextureSize.y, defaultTexture.depth, defaultTexture.format);

                //The RenderTexture has been recreated, so reset it.
                targetCamera.targetTexture = nextRT;
                skySegmentMaterial.mainTexture = nextRT;

                //Keep as latest setting
                lastRT = nextRT;
                lastSize = colorImageSize;
            }

        }
    }
}
