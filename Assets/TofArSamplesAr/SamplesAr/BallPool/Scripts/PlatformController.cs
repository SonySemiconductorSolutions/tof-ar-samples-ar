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

namespace TofArARSamples.BallPool
{
    public class PlatformController : MonoBehaviour
    {
        [SerializeField]
        private ModelingMesh modelingMesh;

        [SerializeField]
        private ARMeshManager arMeshManager;

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
            modelingMesh.enabled = false;
            arMeshManager.enabled = true;
            TofAr.V0.Modeling.TofArModelingManager.Instance.autoStart = false;
            TofAr.V0.Modeling.TofArModelingManager.Instance.StopStream();

            segmentationConnector.enabled = true;
            occlusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Fastest;
            segmentationConnector.HumanStencilMode = ARFCHumanSegmentationStencilMode.Fastest;
            segmentationConnector.HumanDepthMode = ARFCHumanSegmentationDepthMode.Disabled;
            occlusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferHumanOcclusion;
            humanSegmentationDetector.IsActive = false;

#elif UNITY_IOS
			Debug.Log("UNITY_IOS:Switch Modeling");
			modelingMesh.enabled = false;
			arMeshManager.enabled = true;
			TofAr.V0.Modeling.TofArModelingManager.Instance.autoStart = false;
			TofAr.V0.Modeling.TofArModelingManager.Instance.StopStream();
			
			segmentationConnector.enabled = true;
			occlusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Fastest;
			segmentationConnector.HumanStencilMode = ARFCHumanSegmentationStencilMode.Fastest;
			segmentationConnector.HumanDepthMode = ARFCHumanSegmentationDepthMode.Disabled;
			humanSegmentationDetector.IsActive = false;
			occlusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferHumanOcclusion;

#elif UNITY_ANDROID
			Debug.Log("UNITY_ANDROID environment:Switch Modeling");
			modelingMesh.enabled = true;
			arMeshManager.enabled = false;
			TofAr.V0.Modeling.TofArModelingManager.Instance.autoStart = true;   
			
			segmentationConnector.enabled = false;
			occlusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Disabled;
			segmentationConnector.HumanStencilMode = ARFCHumanSegmentationStencilMode.Disabled;
			segmentationConnector.HumanDepthMode = ARFCHumanSegmentationDepthMode.Disabled;
			humanSegmentationDetector.IsActive = true; 
			occlusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferEnvironmentOcclusion;
#endif
        }
    }
}
