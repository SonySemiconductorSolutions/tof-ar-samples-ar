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
using UnityEngine.XR.ARFoundation;
using TofAr.V0.Segmentation;
using TofAr.V0.Tof;


namespace TofArARSamples.StepOn
{
    public class AppController : MonoBehaviour
    {
        public ARCameraManager cameraManager;
        public ARPlaneManager planeManager;
        public RawImage rawImage;
        public Text fpsText;
        private float fpsTime = 0;
        private int fpsCount = 0;

        private Texture2D humanTexture;    
        
        private SynchronizationContext context;
        
        public InputField humanColliderIntervalInputField;

        [SerializeField]
        private HumanColliderManager humanColliderManager;
        

        
        // Start is called before the first frame update
        void Start()
        {
            context = SynchronizationContext.Current;

            TofArSegmentationManager.OnSegmentationEstimated += OnSegmentationEstimated;
            TofArTofManager.Instance.CalibrationSettingsLoaded.AddListener(OnCalibrationSettingsLoaded);

            stayColliderFrameNumsInputField.text = stayColliderFrameNum.ToString();
            stayColliderRatioThresholdInputField.text = stayColliderRatioThreshold.ToString(); 
                
            humanColliderIntervalInputField.text = humanColliderManager.humanColliderInterval.ToString(); 
            areaIntervalInputField.text = areaInterval.ToString();
            
            slopeInputField.text = slope.ToString();
            interceptInputField.text = intercept.ToString();
            minColliderInputField.text = minColliderThreshold.ToString();
            
            framesBeforeShowEffectInputField.text = framesBeforeShowEffect.ToString();
            
            planeManager.planesChanged += OnPlanesChanged;

            floorEffects = new EffectObject[maxFloorEffects];
            wallEffects = new EffectObject[maxWallEffects];
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


        private List<ARPlane> planes = new List<ARPlane>();
        private List<PlaneTouchManager> managers = new List<PlaneTouchManager>();

        private void OnPlanesChanged(ARPlanesChangedEventArgs args)
        {
            foreach (var plane in args.added)
            {
                if (!plane.TryGetComponent<PlaneTouchManager>(out var m)) { continue; }

                m.slope = slope;
                m.intercept = intercept;
                m.minColliderThreshold = minColliderThreshold;
                m.stayColliderFrameNum = stayColliderFrameNum;
                m.stayColliderRatioThreshold = stayColliderRatioThreshold;
                m.framesBeforeShowEffect = framesBeforeShowEffect;
                m.ShowColliderCountsText(showColliderCounts);
                m.SetAreaInterval(areaInterval);
                
                if (planes.Contains(plane)) { continue; }
                planes.Add(plane);
                managers.Add(m);
                
                if (!plane.TryGetComponent<PlaneMeshVisuallizer>(out var v)) { continue; }
                v.visible = showPlane;
            }
            
            foreach (var plane in args.removed)
            {
                if (!plane.TryGetComponent<PlaneTouchManager>(out var m)) { continue; }

                if (!planes.Contains(plane)) { continue; }
                planes.Remove(plane);
                managers.Remove(m);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Update fps
            if (fpsCount >= 100)
            {
                fpsText.text = string.Format("{0:F2} fps", 1f / (fpsTime / 100f));
                fpsTime = 0;
                fpsCount = 0;
            }
            fpsTime += Time.deltaTime;
            fpsCount++;
            
            time += Time.deltaTime;
        }
        
        [SerializeField]
        private EffectObject[] prefabEffectFloor;
        [SerializeField]
        private EffectObject[] prefabEffectWall;
        private EffectObject[] floorEffects;
        private EffectObject[] wallEffects;

        [SerializeField]
        private int maxFloorEffects = 15;
        [SerializeField]
        private int maxWallEffects = 5;
        private int floorEffectsIndex = 0;
        private int wallEffectsIndex = 0;

        // Called before OnTriggerStay
        private void FixedUpdate() 
        {
            //Update plane area
            foreach (var m in managers)
            {
                m.UpdateArea(out var startTouching, out var onTouching, out var finishTouching);

                foreach (var areaPoint in onTouching)
                {
                    // Create effect
                    var prefabs = areaPoint.planeType == PlaneType.XZ ? prefabEffectFloor : prefabEffectWall;
                    var effects = areaPoint.planeType == PlaneType.XZ ? floorEffects : wallEffects;

                    // Check for existing effect
                    EffectObject effect = null;
                    foreach (var e in effects)
                    {
                        if (e == null) { continue; }
                        if (e.areaPoint != areaPoint) { continue; }
                        effect = e;
                    }

                    // For not created effect
                    if (effect == null)
                    {
                        var createdEffects = effects.Where((e) => e != null);
                        
                        // For afford to create effect
                        if (createdEffects.Count() - 1 < effects.Length - 1)
                        {
                            effect = Instantiate<EffectObject>(prefabs[UnityEngine.Random.Range(0, prefabs.Length - 1)]);
                            effect.gameObject.SetActive(false);
                            effects[areaPoint.planeType == PlaneType.XZ ? floorEffectsIndex : wallEffectsIndex] = effect;
                            if (areaPoint.planeType == PlaneType.XZ)
                            {
                                floorEffectsIndex++;
                            }
                            else
                            {
                                wallEffectsIndex++;
                            }
                        }
                        else
                        {
                            // For floor
                            if (areaPoint.planeType == PlaneType.XZ)
                            {
                                if (floorEffectsIndex >= maxFloorEffects - 1)
                                {
                                    floorEffectsIndex = 0;
                                }
                                else
                                {
                                    floorEffectsIndex++;
                                }
                                effect = effects[floorEffectsIndex];
                            }
                            else
                            {
                                if (wallEffectsIndex >= maxWallEffects - 1)
                                {
                                    wallEffectsIndex = 0;
                                }
                                 else
                                {
                                    wallEffectsIndex++;
                                }
                                effect = effects[wallEffectsIndex];
                            }
                            effect.gameObject.SetActive(false);
                        }
                    }
                    
                    // Update effect
                    effect.areaPoint = areaPoint;
                    effect.transform.position = areaPoint.worldPosition;
                    var trs = effect.gameObject.transform;
                    if (areaPoint.planeType == PlaneType.XZ)
                    {
                        trs.rotation = Quaternion.FromToRotation(Vector3.up, m.plane.normal);
                    }
                    else
                    {
                        // 壁の場合
                        var lookAtRotation = Quaternion.LookRotation(m.plane.normal, Vector3.up);
                        var offsetRotation = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
                        trs.rotation = lookAtRotation * offsetRotation;
                    }

                    // Show effect
                    effect.gameObject.SetActive(true);
                }
            } 
            
            // Add collision counts
            if (stayColliderCounts.Count == stayColliderFrameNum)
            {
                stayColliderCounts.Clear();
            }        
            stayColliderCounts.Add(stayColliders.Count());
            
            //stayColliderCountsText.text = string.Join("\n", stayColliderCounts);        
            stayColliders.Clear(); 
        }
        
        public Text stayColliderCountsText;
        public int stayColliderFrameNum = 5;
        public float stayColliderRatioThreshold = 0.6f;
        public int slope = -500;
        public int intercept = 450;
        public int minColliderThreshold = 15;
        public int framesBeforeShowEffect = 5;

        public InputField stayColliderFrameNumsInputField;
        public InputField stayColliderRatioThresholdInputField;
        public InputField slopeInputField;
        public InputField interceptInputField;
        public InputField minColliderInputField;
        public InputField framesBeforeShowEffectInputField;

        private List<int> stayColliderCounts = new List<int>();
        private List<HumanCollider> stayColliders = new List<HumanCollider>();

        
        public void OnValueChangedSlope(string text)
        {
            if (int.TryParse(text, out int s))
            {
                slope = s;
                
                foreach (var m in managers)
                {
                    m.slope = slope;
                }
            }
        }
        
        public void OnValueChangedIntercept(string text)
        {
            if (int.TryParse(text, out int i))
            {
                intercept = i;
                
                foreach (var m in managers)
                {
                    m.intercept = intercept;
                }
            }
        }
        
        public void OnValueChangedMinColliderThreshold(string text)
        {
            if (int.TryParse(text, out int threshold))
            {
                minColliderThreshold = threshold;
                
                foreach (var m in managers)
                {
                    m.minColliderThreshold = threshold;
                }
            }
        }
        
        public void OnValueChangedFramesBeforeShowEffect(string text)
        {
            if (int.TryParse(text, out int frames))
            {
                framesBeforeShowEffect = frames;
                
                foreach (var m in managers)
                {
                    m.framesBeforeShowEffect = framesBeforeShowEffect;
                }
            }
        }
        
        private void OnTriggerStayHumanCollider(HumanCollider humanCollider)
        {
            stayColliders.Add(humanCollider);
        }

        
        private float CalcHumanColliderSize(float fov, float z, int length)
        {
            var rad = fov / 2 * Mathf.Deg2Rad;
            var l = 2 * z * Mathf.Tan(rad);
            return l / length;
        }      
        
        public void OnValueChangedShowColliderToggle(bool visible)
        {
            humanColliderManager.ShowCollider(visible);
        }  
        
        
        public void OnValueChangedStayColliderFrameNums(string text)
        {
            if (int.TryParse(text, out int frameNum))
            {
                stayColliderCounts.Clear();
                stayColliderFrameNum = frameNum;
                
                foreach (var m in managers)
                {
                    m.stayColliderFrameNum = stayColliderFrameNum;
                }
            }
        }
        
        public void OnValueChangedStayColliderRatioThreshold(string text)
        {
            if (float.TryParse(text, out float rationThreshold))
            {
                stayColliderRatioThreshold = rationThreshold;
                foreach (var m in managers)
                {
                    m.stayColliderRatioThreshold = stayColliderRatioThreshold;
                }
            }
        }
        
        private bool showPlane = false;
        public void OnValueChangedVisibleToggle(bool value)
        {          
            showPlane = value;
            foreach (var p in planeManager.trackables)
            {
                if (!p.TryGetComponent<PlaneMeshVisuallizer>(out var v)) { continue; }
                v.visible = showPlane;
            }  
        }
        
        
        public void OnValueChangedColliderInterval(string text)
        {
            if (int.TryParse(text, out int interval))
            {
                humanColliderManager.humanColliderInterval = interval;
            }
        }
        
        private bool showColliderCounts = false;
        public void OnValueChangedShowColliderCountsToggle(bool visible)
        {
            showColliderCounts = visible;
            humanColliderManager.ShowCollider(visible);

            foreach (var m in managers)
            {
                m.ShowColliderCountsText(showColliderCounts);
                m.showEffect = !showColliderCounts;
            }
        }
        
        public InputField areaIntervalInputField;
        public float areaInterval = 0.15f;
        public void OnValueChangedAreaInterval(string text)
        {
            if (float.TryParse(text, out float interval))
            {
                areaInterval = interval;
                
                foreach (var p in planeManager.trackables)
                {
                    var m = p.GetComponent<PlaneTouchManager>();
                    m?.SetAreaInterval(areaInterval);
                }  
            }
        }
        
        public Text stencilFpsText;
        private int count = 0;
        private float time = 0;
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
                            stencilFpsText.text = string.Format("Stencil: {0:F2} fps", 1f / (time / 10f));
                            count = 0;
                            time = 0;
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
