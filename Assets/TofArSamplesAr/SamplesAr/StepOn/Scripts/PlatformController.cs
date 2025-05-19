/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.ThirdParty.ARFoundationConnector;
using TofAr.V0.Segmentation.Human;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace TofArARSamples.StepOn
{
    public class PlatformController : MonoBehaviour
    {
        [SerializeField]
        private ARFoundationSegmentationConnector segmentationConnector;
        [SerializeField]
        private HumanSegmentationDetector humanSegmentationDetector;
        [SerializeField]
        private AROcclusionManager occlusionManager;

        void Start()
        {
#if UNITY_EDITOR
            Debug.Log("UNITY_EDITOR environment: Switch Modleing");

            segmentationConnector.enabled = true;
            occlusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Fastest;
            segmentationConnector.HumanStencilMode = ARFCHumanSegmentationStencilMode.Fastest;
            segmentationConnector.HumanDepthMode = ARFCHumanSegmentationDepthMode.Disabled;
            humanSegmentationDetector.IsActive = false;

            occlusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferHumanOcclusion;

#elif UNITY_IOS
            Debug.Log("UNITY_IOS:Switch Modeling");
            
            segmentationConnector.enabled = true;
            occlusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Fastest;
            segmentationConnector.HumanStencilMode = ARFCHumanSegmentationStencilMode.Fastest;
            segmentationConnector.HumanDepthMode = ARFCHumanSegmentationDepthMode.Disabled;
            humanSegmentationDetector.IsActive = false;
            
            occlusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferHumanOcclusion;

#elif UNITY_ANDROID
            Debug.Log("UNITY_ANDROID environment:Switch Modeling");
            
            segmentationConnector.enabled = false;
            occlusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Disabled;
			occlusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Disabled;
			segmentationConnector.HumanStencilMode = ARFCHumanSegmentationStencilMode.Disabled;
            humanSegmentationDetector.IsActive = true; 

            occlusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferEnvironmentOcclusion;
#endif
        }
    }
}
