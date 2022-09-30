/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

namespace TofArARSamples.StepOn
{
    public enum PlaneType
    {
        XY, XZ, YZ
    }
    
    public class AreaPoint: MonoBehaviour
    {
        public float X;
        public float Y;
        public float Z;
        public Vector3 worldPosition = Vector3.zero;
        public int[] stayCounts;
        public int currentStayCounts = 0;
        public bool maxIsNearArea = false;
        public int level = 0;
        public bool isStepOn = false;
        public TextMeshPro textMesh;
        #region arranged by Jetman
        public PlaneType planeType;
        #endregion
        
        // Init
        public AreaPoint(float X, float Y, float Z, int stayColliderFrameNums, TextMeshPro prefabTextMesh)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.stayCounts = new int[stayColliderFrameNums];
            this.textMesh = Instantiate(prefabTextMesh);
            this.textMesh.gameObject.SetActive(false);
        }
    }

    public class PlaneTouchManager : MonoBehaviour
    {   
        // Property
        public TextMeshPro prefabTextMesh;

        private PlaneType planeType;
        
        public ARPlane plane;

        private List<AreaPoint> areaPoints = new List<AreaPoint>();
        
        private readonly Color stepOnColor = Color.red;
        private readonly Color notStepOnColor = new Color(0, 0, 0, 0.2f);
        private readonly Color nearStepOnColor = Color.yellow;

        private readonly Vector3[] directionsForXY = new Vector3[2] { Vector3.forward, Vector3.back};
        private readonly Vector3[] directionsForXZ = new Vector3[2] { Vector3.up, Vector3.down};
        private readonly Vector3[] directionsForYZ = new Vector3[2] { Vector3.right, Vector3.left};


        private void OnEnable() 
        {
            plane = GetComponent<ARPlane>(); 
            UpdatePlaneType(plane.normal);
        }

        private void OnDisable()
        {
            foreach (var areaPoint in areaPoints)
            {
                areaPoint.textMesh.gameObject.SetActive(false);
            }
        }

        const int humanColliderLayer = 6;
        private void OnTriggerStay(Collider other) 
        {
            if (other.gameObject.layer != humanColliderLayer) { return; }
            
            var X = CalcAreaPoint(other.transform.position.x);
            var Y = CalcAreaPoint(other.transform.position.y);
            var Z = CalcAreaPoint(other.transform.position.z);
            
            if (!TryToFindAreaPoint(X, Y, Z, out var point))
            {
                point = new AreaPoint(X, Y, Z, stayColliderFrameNum, prefabTextMesh);
                areaPoints.Add(point);
            }
            
            point.currentStayCounts++;
        }
        
        private float CalcAreaPoint(float value)
        {
            return value > 0 ? (int)(value / areaInterval) + 0.5f : (int)(value / areaInterval) - 0.5f;
        }
        
        public float DistanceToCameraFrom(AreaPoint areaPoint)
        {
            return Vector3.Distance(areaPoint.worldPosition, Camera.main.transform.position);
        }
        
        public int CalcStayColliderThreshold(AreaPoint areaPoint)
        {
            return (int)Mathf.Max(slope * DistanceToCameraFrom(areaPoint) + intercept, minColliderThreshold);
        }

        private bool EvaluateStepOn(AreaPoint areaPoint)
        {
            var stayColliderThreshold = CalcStayColliderThreshold(areaPoint);        
            var counts = areaPoint.stayCounts.Where((c) => c > stayColliderThreshold).Count();
            return counts >= stayColliderFrameNum * stayColliderRatioThreshold;
        }
        
        private List<AreaPoint> startTouchingArea = new List<AreaPoint>();
        private List<AreaPoint> onTouchingArea = new List<AreaPoint>();
        private List<AreaPoint> finishTouchingArea = new List<AreaPoint>();

        public void UpdateArea(out List<AreaPoint> startTouching, out List<AreaPoint> onTouching, out List<AreaPoint> finishTouching)
        {
            startTouchingArea.Clear();
            onTouchingArea.Clear();
            finishTouchingArea.Clear();

            foreach (var areaPoint in areaPoints)
            {
                // Resize stay counts array
                if (areaPoint.stayCounts.Length != stayColliderFrameNum)
                {
                    areaPoint.stayCounts = new int[stayColliderFrameNum];
                }
                
                // Update stay counts array
                for (int i = 0; i < stayColliderFrameNum - 1; i++)
                {
                    areaPoint.stayCounts[i] = areaPoint.stayCounts[i + 1];
                }
                areaPoint.stayCounts[stayColliderFrameNum - 1] = areaPoint.currentStayCounts;
                
                // Clear current stay counts;
                areaPoint.currentStayCounts = 0;
                
                // Calc position
                var position = new Vector3(areaPoint.X, areaPoint.Y, areaPoint.Z);
                position *= areaInterval;
                
                var directions = directionsForXY;
                switch (planeType)
                {
                    case PlaneType.XY:
                        directions = directionsForXY;
                        break;
                    case PlaneType.XZ:
                        directions = directionsForXZ;
                        break;
                    case PlaneType.YZ:
                        directions = directionsForYZ;
                        break;
                }
                
                foreach (var direction in directions)
                {
                    var ray = new Ray(position, direction);
                    if (!plane.infinitePlane.Raycast(ray, out var enter)) { continue; }
                    position = ray.GetPoint(enter);
                }
                
                // Set world position
                areaPoint.worldPosition = position;
            }

            foreach (var areaPoint in areaPoints)
            {   
                areaPoint.planeType = planeType;

                // Detect step on
                var isStepOn = EvaluateStepOn(areaPoint);
                
                // Update text mesh
                areaPoint.textMesh.transform.position = areaPoint.worldPosition;
                areaPoint.textMesh.text = string.Format("{0:F2}, {1}\n", 
                    DistanceToCameraFrom(areaPoint), CalcStayColliderThreshold(areaPoint)) + string.Join("\n", areaPoint.stayCounts);
                areaPoint.textMesh.color = isStepOn ? stepOnColor : notStepOnColor;
                areaPoint.textMesh.gameObject.SetActive(showColliderCountsText);
                areaPoint.textMesh.transform.rotation = Quaternion.FromToRotation(Vector3.back, plane.normal);
                
                // For step on
                if (isStepOn)
                {   
                    // Check near area                                                        
                    var nearAreaPoints = FindNearAreaPoints(areaPoint);
                    var nearStepPoints = nearAreaPoints.Where((p) => EvaluateStepOn(p)).ToArray();
                    if (nearStepPoints.Length > 0)
                    {
                        foreach (var p in nearStepPoints)
                        {
                            if (p.level > areaPoint.level)
                            {
                                areaPoint.maxIsNearArea = true;
                                areaPoint.textMesh.color = nearStepOnColor;
                                break;
                            }
                        }
                    }
                        
                    areaPoint.level = !areaPoint.maxIsNearArea ? Mathf.Min(255, areaPoint.level + 2) : Mathf.Max(0, areaPoint.level - 5);
                        
                    // For start to step on
                    if (!areaPoint.isStepOn) 
                    { 
                        areaPoint.isStepOn = true;   
                        startTouchingArea.Add(areaPoint);
                    }
                    else
                    {
                        if (showEffect && areaPoint.level >= 2 * framesBeforeShowEffect)
                        {
                            if (!areaPoint.maxIsNearArea)
                            {
                                onTouchingArea.Add(areaPoint);
                            }
                        }
                    }
                }
                else
                {
                    areaPoint.level = Mathf.Max(0, areaPoint.level - 5);

                    if (areaPoint.isStepOn)
                    {
                        finishTouchingArea.Add(areaPoint);
                    }
                    areaPoint.isStepOn = false;
                }
            }  
            
            // Update plane type
            UpdatePlaneType(plane.normal);

            startTouching = startTouchingArea;
            onTouching = onTouchingArea;
            finishTouching = finishTouchingArea;
        }
        
        private void UpdatePlaneType(Vector3 normal)
        {
            if (Mathf.Abs(normal.x) >= Mathf.Abs(normal.y) && Mathf.Abs(normal.x) >= Mathf.Abs(normal.z))
            {
                planeType = PlaneType.YZ;
            }    
            else if (Mathf.Abs(normal.y) >= Mathf.Abs(normal.x) && Mathf.Abs(normal.y) >= Mathf.Abs(normal.z))
            {
                planeType = PlaneType.XZ;
            }
            else if (Mathf.Abs(normal.z) >= Mathf.Abs(normal.x) && Mathf.Abs(normal.z) >= Mathf.Abs(normal.y))
            {
                planeType = PlaneType.XY;
            }  
        }
        
        private void ClearAreaPoints()
        {
            foreach (var p in areaPoints)
            {
                if (p.textMesh != null) 
                {
                    DestroyImmediate(p.textMesh.gameObject);
                }
            }
            areaPoints.Clear();
        }
        
        private List<AreaPoint> FindNearAreaPoints(AreaPoint point)
        {
            var nearAreaPoints = new List<AreaPoint>();
            var margins = new int[3] { -1, 0, 1};
            foreach (var marginU in margins)
            {
                foreach (var marginV in margins)
                {
                    if (marginU == 0 && marginV == 0) { continue; }
                    IEnumerable<AreaPoint> points = null;
                    switch (planeType)
                    {
                        case PlaneType.XY:
                            points = areaPoints.Where((p) => p.X == point.X + marginU && p.Y == point.Y + marginV);
                            break;
                        case PlaneType.XZ:
                            points = areaPoints.Where((p) => p.X == point.X + marginU && p.Z == point.Z + marginV);
                            break;
                        case PlaneType.YZ:
                            points = areaPoints.Where((p) => p.Z == point.Z + marginU && p.Y == point.Y + marginV);                        
                            break;
                    }
                    if (points?.Count() == 0) { continue; }
                    nearAreaPoints.Add(points.ElementAt(0));
                }
            }
            return nearAreaPoints;
        }
        
        private bool TryToFindAreaPoint(float X, float Y, float Z, out AreaPoint areaPoint)
        {
            IEnumerable<AreaPoint> points = null;
            switch (planeType)
            {
                case PlaneType.XY:
                    points = areaPoints.Where((p) => p.X == X && p.Y == Y);
                    break;
                case PlaneType.XZ:
                    points = areaPoints.Where((p) => p.X == X && p.Z == Z);
                    break;
                case PlaneType.YZ:
                    points = areaPoints.Where((p) => p.Y == Y && p.Z == Z);
                    break;
            }
            areaPoint = points?.Count() == 0 ? null : points.ElementAt(0);
            return points?.Count() != 0;
        }
        
        private bool showColliderCountsText = false;
        public void ShowColliderCountsText(bool show)
        {
            showColliderCountsText = show;
        }
        
        public bool showEffect = true;
        
        public int slope = -500;
        public int intercept = 450;
        public int minColliderThreshold = 15;
        public int framesBeforeShowEffect = 5;
        
        public int stayColliderFrameNum = 5;
        public float stayColliderRatioThreshold = 0.6f;
        
        private float areaInterval = 0.15f;    
        public void SetAreaInterval(float value)
        {
            areaInterval = value;
            ClearAreaPoints();
        }
        
    }
}
