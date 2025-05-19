/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using TofArSettings.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Hand
{
    public class HandPanel : MonoBehaviour
    {
        [SerializeField]
        CanvasGroup imageCanvas;
        [SerializeField]
        Text poseText;
        [SerializeField]
        Image poseImage;

        float initPosX, initPosY;
        ScreenRotateController scRotCtrl;
        Toolbar toolbar;
        RectTransform rt;

        void Awake()
        {
            rt = GetComponent<RectTransform>();
            initPosX = rt.anchoredPosition.x;
            initPosY = rt.anchoredPosition.y;
            toolbar = FindAnyObjectByType<Toolbar>();
            scRotCtrl = FindAnyObjectByType<ScreenRotateController>();
        }

        void OnEnable()
        {
            scRotCtrl.OnRotateScreen += OnRotateScreen;
        }

        void OnDisable()
        {
            if (scRotCtrl)
            {
                scRotCtrl.OnRotateScreen -= OnRotateScreen;
            }
        }

        void Start()
        {
            poseText.text = "None";
            imageCanvas.alpha = 0;

            OnRotateScreen(Screen.orientation);
        }

        public void SetPose(PoseIndex pose, Sprite image)
        {
            poseText.text = pose.ToString();
            poseImage.sprite = image;

            if (imageCanvas == null)
            {
                return;
            }

            imageCanvas.alpha = pose == PoseIndex.None ? 0 : 1;
        }

        /// <summary>
        /// Event that is called when screen is rotated
        /// </summary>
        /// <param name="ori">Screen orientation</param>
        void OnRotateScreen(ScreenOrientation ori)
        {
            if (scRotCtrl.IsPortraitScreen)
            {
                rt.anchoredPosition = new Vector2(initPosX, initPosY + toolbar.BarWidth);
            }
            else
            {
                float offset = rt.anchorMin.x == 1f ? toolbar.BarWidth : 0f;
                rt.anchoredPosition = new Vector2(initPosX - offset, initPosY);
            }
        }
    }
}
