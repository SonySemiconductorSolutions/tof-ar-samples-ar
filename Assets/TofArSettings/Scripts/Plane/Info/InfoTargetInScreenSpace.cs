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
    public class InfoTargetInScreenSpace : PlaneInfo
    {
        UI.InfoLR uiInfoLR;
        Vector2 screenSpacePosition = new Vector2();

        private void Start()
        {
            uiInfoLR = GetComponent<UI.InfoLR>();
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
            uiInfoLR.LeftText = screenSpacePosition.x.ToString("F2");
            uiInfoLR.RightText = screenSpacePosition.y.ToString("F2");            
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
                screenSpacePosition = selectedPlane.CenterPosition;
            }
        }
    }
}
