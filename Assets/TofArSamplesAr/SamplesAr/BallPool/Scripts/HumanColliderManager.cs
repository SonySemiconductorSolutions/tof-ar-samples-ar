/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using TofAr.V0.Tof;
using TofAr.V0.Segmentation;
using TofAr.V0.Coordinate;

namespace TofArARSamples.BallPool
{
    public class HumanColliderManager : MonoBehaviour
    {
        [SerializeField]
        private HumanCollider humanColliderPrefab;
        private CalibrationSettingsProperty cs;
        private HumanCollider[] humanColliders;
        public int humanColliderInterval = 2;
        private HumanCollider.HumanColliderDelegate humanColliderDelegate;

        private byte[] humanMask;
        private float scaleFromMaskToColor;
        private DepthToColorProperty colorFromDepth;
        private Quaternion screenRotation;
        private float fovH;
        private float fovV;

        public void Configure(CalibrationSettingsProperty cs, HumanCollider.HumanColliderDelegate humanColliderDelegate)
        {
            this.cs = cs;
            fovH = 2 * Mathf.Atan(cs.depthWidth / (2 * cs.d.fx)) * 180 / Mathf.PI;
            fovV = 2 * Mathf.Atan(cs.depthHeight / (2 * cs.d.fy)) * 180 / Mathf.PI;
            this.humanColliderDelegate = humanColliderDelegate;        
        }

        private void UpdateHumanColliderFrom(byte[] humanMask, DepthToColorProperty depthToColor, SegmentationResult result, float scale, Quaternion rotation)
        {
            var recipirocalColorFx = 1 / cs.c.fx;
            var recipirocalColorFy = 1 / cs.c.fy;

            for (int y = 0; y < cs.depthHeight; y++)
            {
                for (int x = 0; x < cs.depthWidth; x++)
                {
                    // Calc collider index
                    if ((x + cs.depthWidth * y + y) % humanColliderInterval != 0) { continue; } 
                    var index = x + cs.depthWidth * y;
                    var colliderIndex = humanColliderInterval == 1 ? index : (x + cs.depthWidth * y + y) / humanColliderInterval;
                    if (colliderIndex >= humanColliders.Length) { continue; }
                    
                    // Calc mask point
                    var colorPoint = depthToColor.colorPoints[index];
                    var maskPointX = (int)((colorPoint.x - cs.colorWidth / 2) / scale + result.maskBufferWidth / 2);
                    var maskPointY = (int)((colorPoint.y - cs.colorHeight / 2) / scale + result.maskBufferHeight / 2);
                    
                    if (maskPointX < result.maskBufferWidth && maskPointY < result.maskBufferHeight)
                    {
                        var maskIndex = maskPointX + result.maskBufferWidth * maskPointY;
                        if (maskIndex < humanMask.Length && maskIndex >= 0)
                        {
                            humanColliders[colliderIndex].gameObject.SetActive(humanMask[maskIndex] != 0);
                            if (humanMask[maskIndex] != 0)
                            {
                                // Calc camera position
                                var depth = depthToColor.depthFrame[x + cs.depthWidth * y];
                                var cameraX = (colorPoint.x - cs.c.cx) * depth * recipirocalColorFx;
                                var cameraY = (colorPoint.y - cs.c.cy) * depth * recipirocalColorFy;
                                
                                humanColliders[colliderIndex].transform.localPosition = rotation * new Vector3(cameraX, -cameraY, depth) / 1000f;
                                var scaleX = Mathf.Min(CalcHumanColliderSize(fovH, depth / 1000f, cs.depthWidth) * humanColliderInterval / 2, 0.01f);
                                var scaleY = Mathf.Min(CalcHumanColliderSize(fovV, depth / 1000f, cs.depthHeight) * humanColliderInterval / 2, 0.01f);
                                humanColliders[colliderIndex].transform.localScale = new Vector3(scaleY, scaleX, 0.01f);
                                humanColliders[colliderIndex].transform.rotation = Quaternion.FromToRotation(Vector3.back, Camera.main.transform.position - humanColliders[colliderIndex].transform.position);
                            }
                        }
                    }
                    else
                    {
                        humanColliders[colliderIndex].gameObject.SetActive(false);
                        continue;
                    }
                }
            }
        }

        public void UpdateColliders(SegmentationResult result)
        {
            if (humanColliders == null || humanColliders.Length == 0)
            {
                CreateHumanColliders();
            }
            else if (humanColliders.Length != cs.depthWidth * cs.depthHeight / humanColliderInterval)    
            {
                ResizeHumanColliders();
                print("Resize human Colliders: size is " + cs.depthWidth * cs.depthHeight / humanColliderInterval);
            } 

            // Update human mask
            if (humanMask == null)
            {
                humanMask = new byte[result.maskBufferWidth * result.maskBufferHeight];
            }
            if (result.dataStructureType == DataStructureType.RawPointer)
            {
                Marshal.Copy((IntPtr)result.rawPointer, humanMask, 0, result.maskBufferWidth * result.maskBufferHeight);
            }
            else if (result.dataStructureType == DataStructureType.MaskBufferByte)
            {
                Array.Copy(result.maskBufferByte, humanMask, result.maskBufferWidth * result.maskBufferHeight);
            }
            scaleFromMaskToColor = Mathf.Min((float)cs.colorWidth / (float)result.maskBufferWidth, (float)cs.colorHeight / (float)result.maskBufferHeight);
            int rot = TofAr.V0.TofArManager.Instance.GetScreenOrientation();
            screenRotation = Quaternion.Euler(0, 0, rot);
            
            colorFromDepth = TofArCoordinateManager.Instance.GetProperty<DepthToColorProperty>(new DepthToColorProperty {depthFrame = TofArTofManager.Instance.DepthData.Data});
            if (colorFromDepth?.colorPoints == null)
            {
                print("no color points in cordinate");
                return;
            }

            UpdateHumanColliderFrom(humanMask, colorFromDepth, result, scaleFromMaskToColor, screenRotation);
        }

        public void ShowCollider(bool visible)
        {
            foreach (var c in humanColliders)
            {
                c.Show(visible);
            }
        }

        private void ResizeHumanColliders()
        {
            foreach (var c in humanColliders)
            {
                if (c == null) { continue; }
                DestroyImmediate(c.gameObject);
            }
            CreateHumanColliders();
        }
        
        public void CreateHumanColliders()
        {
            var length = cs.depthWidth * cs.depthHeight / humanColliderInterval;
            humanColliders = new HumanCollider[length];
            for (int i = 0; i < humanColliders.Length; i++)
            {
                var c = Instantiate<HumanCollider>(humanColliderPrefab);
                c.transform.parent = Camera.main.transform;
                c.transform.localPosition = Vector3.zero;
                if (humanColliderDelegate != null)
                {
                    c.humanColliderDelegate += humanColliderDelegate;
                }
                c.gameObject.SetActive(false);
                humanColliders[i] = c;
            }
        }

        private float CalcHumanColliderSize(float fov, float z, int length)
        {
            var rad = fov / 2 * Mathf.Deg2Rad;
            var l = 2 * z * Mathf.Tan(rad);
            return l / length;
        }  
    }
}
