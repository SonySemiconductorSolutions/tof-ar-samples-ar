/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Plane;
using UnityEngine;

namespace TofArSettings.Plane
{
    public abstract class PlaneInfo : MonoBehaviour
    {
        protected PlaneArrangement selectedPlane;

        public Vector2 Size
        {
            get { return rt.sizeDelta; }
            set
            {
                rt.sizeDelta = value;
            }
        }

        RectTransform rt;

        protected virtual void OnEnable()
        {
            PlaneInfoSettings.OnPlaneArrangementChanged += PlaneArrangementChanged;

            // We need to pull the object when the object activates
            var planeInfo = FindAnyObjectByType<PlaneInfoSettings>();
            if(planeInfo != null)
            {
                if(planeInfo.selectedPlane != null)
                {
                    selectedPlane = planeInfo.selectedPlane;
                }
            }
        }

        protected virtual void OnDisable()
        {
            PlaneInfoSettings.OnPlaneArrangementChanged -= PlaneArrangementChanged;
        }

        protected virtual void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        protected void PlaneArrangementChanged(PlaneArrangement planeObject)
        {
            selectedPlane = planeObject;
        }
    }
}
