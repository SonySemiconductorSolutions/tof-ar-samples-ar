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
    public class InfoPlaneSize : PlaneInfo
    {
        UI.Info uiInfo;
        float planeSize = -1f;

        private void Start()
        {
            uiInfo = GetComponent<UI.Info>();
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
            uiInfo.InfoText = planeSize.ToString("F1");
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
                planeSize = selectedPlane.Data.radius * 1000f;
            }
        }
    }
}
