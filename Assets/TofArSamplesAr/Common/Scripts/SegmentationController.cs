/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Threading;
using UnityEngine;
using TofAr.V0.Segmentation.Sky;
using TofAr.V0.Segmentation.Human;
using TofArSettings.Segmentation;
using System.Collections;

namespace TofArSamples.SegmentationComponent
{
    public class SegmentationController : MonoBehaviour
    {
        private SkySegmentationDetector skyDetector;
        private HumanSegmentationDetector humanDetector;

        private HumanSegmentationController humanSegmentationController;
        private SkySegmentationController skySegmentationController;

        private void Awake()
        {
            humanDetector = FindObjectOfType<HumanSegmentationDetector>();
            skyDetector = FindObjectOfType<SkySegmentationDetector>();
            humanSegmentationController = FindObjectOfType<HumanSegmentationController>();
            skySegmentationController = FindObjectOfType<SkySegmentationController>();
        }

        private SynchronizationContext context;

        public Material segmentationMaskMaterial = null;


        private void OnEnable()
        {
            humanSegmentationController.OnHumanChange += OnHumanChange;
            skySegmentationController.OnSkyChange += OnSkyChange;
            humanSegmentationController.OnNotHumanChange += OnNotHumanChange;
            skySegmentationController.OnNotSkyChange += OnNotSkyChange;
        }

        private void OnDisable()
        {
            humanSegmentationController.OnHumanChange -= OnHumanChange;
            skySegmentationController.OnSkyChange -= OnSkyChange;
            humanSegmentationController.OnNotHumanChange -= OnNotHumanChange;
            skySegmentationController.OnNotSkyChange -= OnNotSkyChange;
            this.segmentationMaskMaterial.SetFloat("_useHuman", 0);
            this.segmentationMaskMaterial.SetFloat("_useSky", 0);
            this.segmentationMaskMaterial.SetFloat("_invertHuman", 0);
            this.segmentationMaskMaterial.SetFloat("_invertSky", 0);
        }

        private IEnumerator Start()
        {
            this.context = SynchronizationContext.Current;
            yield return new WaitForEndOfFrame();

            this.segmentationMaskMaterial.SetTexture("_MaskTexHuman", this.humanDetector.MaskTexture);
            this.segmentationMaskMaterial.SetTexture("_MaskTexSky", this.skyDetector.MaskTexture);
        }

        private void OnHumanChange(bool val)
        {
            context.Post((s) =>
            {
                if (val)
                {
                    this.segmentationMaskMaterial.SetFloat("_useHuman", 1);
                }
                else
                {
                    this.segmentationMaskMaterial.SetFloat("_useHuman", 0);
                }
            }, null);
        }

        private void OnNotHumanChange(bool val)
        {
            context.Post((s) =>
            {
                if (val)
                {
                    this.segmentationMaskMaterial.SetFloat("_invertHuman", 1);
                }
                else
                {
                    this.segmentationMaskMaterial.SetFloat("_invertHuman", 0);
                }
            }, null);
        }
        private void OnSkyChange(bool val)
        {
            context.Post((s) =>
            {
                if (val)
                {
                    this.segmentationMaskMaterial.SetFloat("_useSky", 1);
                }
                else
                {
                    this.segmentationMaskMaterial.SetFloat("_useSky", 0);
                }
            }, null);
        }

        private void OnNotSkyChange(bool val)
        {
            context.Post((s) =>
            {
                if (val)
                {
                    this.segmentationMaskMaterial.SetFloat("_invertSky", 1);
                }
                else
                {
                    this.segmentationMaskMaterial.SetFloat("_invertSky", 0);
                }
            }, null);
        }
    }
}
