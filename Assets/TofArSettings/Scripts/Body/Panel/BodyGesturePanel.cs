/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2024 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Threading;

using TofAr.V0.Body;

using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Body
{
    public class BodyGesturePanel : MonoBehaviour
    {
        private AudioSource source;
        [SerializeField]
        private AudioClip gesture;
        [SerializeField]
        private Text poseText;
        [SerializeField]
        private CanvasGroup gestureCanvas;
        private Text gestureText;

        [SerializeField]
        private float showTime = 0.2f;
        [SerializeField]
        private float fadeOutTime = 0.5f;
        private float time = 0;
        private SynchronizationContext syncContext;

        void Awake()
        {
            source = GetComponent<AudioSource>();
            gestureText = gestureCanvas.GetComponent<Text>();
        }

        private void OnEnable()
        {
            TofArBodyGestureManager.OnGestureEstimatedDefault += OnGestureEstimatedDefault;
            TofArBodyGestureManager.OnGestureEstimated += OnGestureEstimated;
            syncContext = SynchronizationContext.Current;
        }

        // Update is called once per frame
        void Update()
        {
            if (gestureCanvas.alpha > 0)
            {
                time += Time.deltaTime;
                if (time > showTime)
                {
                    gestureCanvas.alpha -= Time.deltaTime / fadeOutTime;
                }
            }

            if ((TofArBodyManager.Instance.DetectorType == BodyPoseDetectorType.Internal_SV2)
            && (TofArBodyManager.Instance.RecogMode != TofAr.V0.Body.SV2.RecogMode.Face2Face))
            {
                if (poseText != null)
                {
                    poseText.text = string.Empty;
                }
            }
        }

        private void OnGestureEstimatedDefault(BodyGesture result)
        {
            syncContext.Post((s) =>
            {
                var self = (s as BodyGesturePanel);
                if (self?.poseText != null)
                {
                    self.poseText.text = result.ToString();
                }
            }, this);
        }

        private void OnGestureEstimated(BodyGesture result)
        {
            syncContext.Post((s) =>
            {
                var self = (s as BodyGesturePanel);
                if (self?.gestureText != null)
                {
                    self.source?.PlayOneShot(gesture);
                    self.gestureText.text = result.ToString();
                    if ((self?.gestureCanvas != null))
                    {
                        self.gestureCanvas.alpha = 1;
                    }
                }
                time = 0;
            }, this);
        }
    }
}
