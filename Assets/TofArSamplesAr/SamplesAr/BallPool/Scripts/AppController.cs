/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TofAr.V0.Segmentation;
using TofAr.V0.Tof;
using UnityEngine.XR.ARFoundation;

namespace TofArARSamples.BallPool
{
    public class AppController : MonoBehaviour
    {
        public ARMeshManager meshManager;
        public ModelingMesh modelingMesh;
        
        public Toggle ballToggle;
        
        public RawImage rawImage;
        private Texture2D humanTexture;   

        private SynchronizationContext context;

        [SerializeField]
        private HumanColliderManager humanColliderManager;

        // Start is called before the first frame update
        void Start()
        {
            context = SynchronizationContext.Current; 
            
            TofArSegmentationManager.OnSegmentationEstimated += OnSegmentationEstimated;
            TofArTofManager.Instance.CalibrationSettingsLoaded.AddListener(OnCalibrationSettingsLoaded);
            
            ballToggle.onValueChanged.AddListener(OnValueChangedBall);
            
            maxBallNumInput.onEndEdit.AddListener(OnEndEditBallNum);
            maxBallNumInput.text = maxBallNum.ToString();
            
            ballsPerSecondsInput.onEndEdit.AddListener(OnEndEditBallsPerSeconds);
            ballsPerSecondsInput.text = ballsPerSeconds.ToString();
            
            CreateBalls(maxBallNum);
        }

        private void OnDestroy()
        {
            TofArSegmentationManager.OnSegmentationEstimated -= OnSegmentationEstimated;
            TofArTofManager.Instance.CalibrationSettingsLoaded.RemoveListener(OnCalibrationSettingsLoaded);
        }

        private void OnCalibrationSettingsLoaded(CalibrationSettingsProperty calibrationSettings)
        {
            humanColliderManager.Configure(calibrationSettings, null);
        }

        // Update is called once per frame
        void Update()
        {        
            stencilTime += Time.deltaTime;
            updateTime += Time.deltaTime;
            updateCount ++;

            if (updateCount >= 10)
            {
                fpsText.text = string.Format("App: {0:F2} fps", 1f / (updateTime / 10f));
                updateCount = 0;
                updateTime = 0;
            }
            
            time += Time.deltaTime;
            if (time > 1f / (float)ballsPerSeconds)
            {
                time = 0;
                var hideBalls = balls.Where((b) => !b.gameObject.activeSelf);
                if (ballToggle.isOn && hideBalls.Count() > 0)
                {
                    for (int i = 0; i < ballPoints.Length; i++)
                    {
                        if (i >= hideBalls.Count()) { continue; }
                        var b = hideBalls.ElementAt(i);                    
                        b.transform.position = Camera.main.transform.position + Camera.main.transform.forward.normalized * ballPoints[i].z + new Vector3(ballPoints[i].x, ballPoints[i].y, 0) + Vector3.one * Random(-randomRange, randomRange);
                        b.gameObject.SetActive(true);
                        b.rb.velocity = Vector3.zero;
                        b.rb.angularVelocity = Vector3.zero;
                    }
                }
            }
        }
        
        public MeshFilter visibleMeshPrefab;
        public MeshFilter occlusionMeshPrefab;
        
        public Material visibleMaterial;
        public Material occlusionMaterial;
        
        public void OnValueChangedVisibleMesh(bool visible)
        {
            meshManager.meshPrefab = visible ? visibleMeshPrefab : occlusionMeshPrefab;

            #if UNITY_IOS
            foreach (var m in meshManager.meshes)
            {
                if (m.TryGetComponent<MeshRenderer>(out var renderer))
                {
                    renderer.sharedMaterial = visible ? visibleMaterial : occlusionMaterial;
                }
            }
            #else
            modelingMesh.ChangeMeshVisualize(visible);
            #endif
        }
        
        public InputField maxBallNumInput;
        public InputField ballsPerSecondsInput;
        public ARBall ballPrefab;
        private ARBall[] balls;
        public int maxBallNum = 100;
        
        public Vector3[] ballPoints;
        public int ballsPerSeconds = 3;
        public float randomRange = 0.05f;
        private float time = 0;
        
        public Text fpsText;
        public Text stencilText;
        private int updateCount = 0;
        private int count = 0;
        private float stencilTime = 0;
        private float updateTime = 0;

        public void OnValueChangedBall(bool value)
        {
            // Hide balls
            if (!value)
            {
                foreach (var b in balls)
                {
                    b.gameObject.SetActive(false);
                }
            }
        }
        
        public void OnEndEditBallNum(string text)
        {
            if (int.TryParse(text, out var num))
            {
                CreateBalls(num);
            }
        }
        
        public void OnEndEditBallsPerSeconds(string text)
        {
            if (int.TryParse(text, out var balls))
            {
                ballsPerSeconds = balls;
            }
        }
        
        public void OnValueChangedShowCollider(bool visible)
        {
            humanColliderManager.ShowCollider(visible);
        }

        private void CreateBalls(int num)
        {
            if (balls != null) 
            {
                foreach (var b in balls)
                {
                    DestroyImmediate(b.gameObject);
                }
            }
            
            balls = new ARBall[num];
            for (int i = 0; i < num; i++)
            {
                var b = Instantiate<ARBall>(ballPrefab);
                b.gameObject.SetActive(false);
                balls[i] = b;
            }
        }
        
        private float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }
        
        private void OnSegmentationEstimated(SegmentationResults segmentationResults)
        {
            if (TofArTofManager.Instance.DepthData == null) { return; }

            context.Post((s) =>
            {

                foreach (var result in segmentationResults.results)
                {
                    if (
                        result.name == "HumanMask" || // Android
                        result.name == "Human" // iOS HumanStencil
                    ) 
                    {
                        if (result.dataStructureType != DataStructureType.RawPointer && result.dataStructureType != DataStructureType.MaskBufferByte)
                        {
                            continue;
                        }

                        // Count stencil fps
                        if (count >= 10)
                        {
                            stencilText.text = string.Format("Stencil: {0:F2} fps", 1f / (stencilTime / 10f));
                            count = 0;
                            stencilTime = 0;
                        }
                        count++;

                        if (humanTexture == null)
                        {
                            // Init human mask texture                          
                            humanTexture = new Texture2D(result.maskBufferWidth, result.maskBufferHeight, TextureFormat.R8, false);
                        }

                        switch (result.dataStructureType)
                        {
                            case DataStructureType.RawPointer:
                                // Show human mask
                                humanTexture.LoadRawTextureData((IntPtr)result.rawPointer, result.maskBufferHeight * result.maskBufferWidth * sizeof(byte));
                                break;
                            case DataStructureType.MaskBufferByte:
                                // Show human mask
                                humanTexture.LoadRawTextureData(result.maskBufferByte);
                                break;
                            default: break;
                        }
                        humanTexture.Apply();
                        rawImage.texture = humanTexture;

                        // Update human colliders        
                        humanColliderManager.UpdateColliders(result);
                    }
                }
            }, null);
        }
    }
}
