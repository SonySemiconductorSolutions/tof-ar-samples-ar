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
    public class InfoTargetInWorldSpace : PlaneInfo
    {
        UI.InfoLCR uiInfoLCR;
        Vector3 worldSpacePosition = new Vector3();

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
            uiInfoLCR.LeftText = worldSpacePosition.x.ToString("F1");
            uiInfoLCR.CenterText = worldSpacePosition.y.ToString("F1");
            uiInfoLCR.RightText = worldSpacePosition.z.ToString("F1");            
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
                worldSpacePosition = selectedPlane.Data.center * 1000f;
            }
        }
    }
}
