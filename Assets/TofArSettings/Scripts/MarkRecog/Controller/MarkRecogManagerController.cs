/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.MarkRecog;
using UnityEngine;

namespace TofArSettings.MarkRecog
{
    public class MarkRecogManagerController : ControllerBase
    {
        private HandBrush brush;
        private RenderTexture markImage;
        private Texture2D tex;

        private bool isDrawing;
        public bool IsDrawing
        {
            get => isDrawing;
            private set
            {
                if (value != isDrawing)
                {
                    isDrawing = value;
                    OnDrawingStateChanged?.Invoke(isDrawing);
                }
            }
        }

        private ChannelIds currentClassId = ChannelIds.None;
        public ChannelIds CurrentClassId
        {
            get
            {
                return currentClassId;
            }
            private set
            {
                if (value != currentClassId)
                {
                    currentClassId = value;
                    OnClassIdChanged?.Invoke(currentClassId);
                }
            }
        }

        /// <summary>
        /// Event that is called when ChannelId is changed
        /// </summary>
        /// <param name="id"></param>
        public delegate void ChangeClassEvent(ChannelIds id);

        public event ChangeClassEvent OnClassIdChanged;

        public event ChangeToggleEvent OnDrawingStateChanged;

        protected void Awake()
        {
            brush = FindObjectOfType<HandBrush>();
            var markCamera = GameObject.Find("MarkCamera");
            var markCameraCam = markCamera.GetComponent<Camera>();
            markImage = markCameraCam.targetTexture;
        }

        protected void OnEnable()
        {
            brush.DrawStarted += BrushOnStartDraw;
            brush.DrawStopped += BrushOnStopDraw;
        }

        protected void OnDisable()
        {
            brush.DrawStarted -= BrushOnStartDraw;
            brush.DrawStopped -= BrushOnStopDraw;
        }

        private void BrushOnStopDraw()
        {
            IsDrawing = false;

            RecognizeMark();
        }

        private void BrushOnStartDraw()
        {
            IsDrawing = true;
        }

        private void RecognizeMark()
        {
            RenderTexture.active = markImage;

            if (tex == null)
            {
                tex = new Texture2D(markImage.width, markImage.height, TextureFormat.ARGB32, false);
            }
            tex.ReadPixels(new Rect(0, 0, markImage.width, markImage.height), 0, 0);
            tex.Apply();

            RenderTexture.active = null;

            ResultProperty prop = new ResultProperty()
            {
                image = tex
            };

            prop = TofArMarkRecogManager.Instance?.GetProperty<ResultProperty>(prop);
            if (prop == null)
            {
                return;
            }

            var levels = prop.levels;
            if (levels != null)
            {
                float max_level = 0f;
                int maxIdx = 0;


                for (int i = 0; i < levels.Length; i++)
                {
                    float l = levels[i];
                    if (l > max_level)
                    {
                        max_level = l;
                        maxIdx = i;
                    }
                }

                if (maxIdx == levels.Length - 1)
                {
                    maxIdx = (int)ChannelIds.None;
                }

                CurrentClassId = (ChannelIds)maxIdx;
            }
        }
    }
}
