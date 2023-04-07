/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;
using TofAr.V0.Hand;
using TofArSettings.UI;

namespace TofArSettings.Hand
{
    public class HandPanel : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup imageCanvas;
        [SerializeField]
        private Text poseText;
        [SerializeField]
        private Image poseImage;

        private float initPosX, initPosY;
        ScreenRotateController scRotCtrl;
        Toolbar toolbar;

        private void Awake()
        {
            initPosX = this.GetComponent<RectTransform>().anchoredPosition.x;
            initPosY = this.GetComponent<RectTransform>().anchoredPosition.y;
            toolbar = FindObjectOfType<Toolbar>();
            scRotCtrl = FindObjectOfType<ScreenRotateController>();
            scRotCtrl.OnRotateScreen += OnRotateScreen;
        }

        // Start is called before the first frame update
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

            if (imageCanvas == null) { 
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
            RectTransform rectTransform = this.GetComponent<RectTransform>();

            if (scRotCtrl.IsPortrait)
            {
                rectTransform.anchoredPosition = new Vector2(initPosX, initPosY + toolbar.BarWidth);
            }
            else
            {
                float offset = rectTransform.anchorMin.x == 1f ? toolbar.BarWidth : 0f;
                rectTransform.anchoredPosition = new Vector2(initPosX - offset, initPosY);
            }

        }
    }
}
