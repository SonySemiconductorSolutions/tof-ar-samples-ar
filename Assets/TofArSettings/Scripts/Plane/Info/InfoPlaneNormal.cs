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
    public class InfoPlaneNormal : PlaneInfo
    {
        UI.InfoLCR uiInfoLCR;
        Vector3 normalData = new Vector3();

        private void Start()
        {
            uiInfoLCR = GetComponent<UI.InfoLCR>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TofArPlaneManager.OnFrameArrived += PlaneFrameArrived;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TofArPlaneManager.OnFrameArrived -= PlaneFrameArrived;
        }

        private void Update()
        {
            uiInfoLCR.LeftText = normalData.x.ToString("F2");
            uiInfoLCR.CenterText = normalData.y.ToString("F2");
            uiInfoLCR.RightText = normalData.z.ToString("F2");            
        }

        private void PlaneFrameArrived(object sender)
        {
            var manager = sender as TofArPlaneManager;
            if(manager == null)
            {
                return;
            }

            if(selectedPlane != null)
            {
                normalData = selectedPlane.Data.normal;
            }
        }
    }
}
