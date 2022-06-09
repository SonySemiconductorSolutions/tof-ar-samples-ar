/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Linq;
using TofAr.V0.Color;
using TofAr.V0.Tof;

namespace TofArSettings
{
    public class FrontRearCameraController : ControllerBase
    {
        private void Awake()
        {
            // Force both tof and color to rear if one of them doesn't support front
            TofArTofManager tofManager = TofArTofManager.Instance;
            TofArColorManager colorManager = TofArColorManager.Instance;

            var tofConfigs = tofManager.GetProperty<Camera2ConfigurationsProperty>();
            var colorConfigs = colorManager.GetProperty<AvailableResolutionsProperty>();

            var frontConfigsTof = tofConfigs.configurations.Where(x => x.lensFacing == (int)(TofAr.V0.Tof.LensFacing.Front));
            var frontConfigsColor = colorConfigs.resolutions.Where(x => x.lensFacing == (int)(TofAr.V0.Tof.LensFacing.Front));

            if (frontConfigsColor.Count() == 0 || frontConfigsTof.Count() == 0)
            {
                // set both cameras to rear 

                CameraConfigurationProperty rearConfigTof = null;
                ResolutionProperty rearConfigColor = null;

                // Get rear config for tof
                {
                    var tofDefaultConfig = tofManager.GetProperty<Camera2DefaultConfigurationProperty>();
                    int width = tofDefaultConfig.width;
                    int height = tofDefaultConfig.height;

                    if (tofDefaultConfig.lensFacing == (int)(TofAr.V0.Tof.LensFacing.Back))
                    {
                        rearConfigTof = tofDefaultConfig;
                    }
                    else
                    {
                        rearConfigTof = tofConfigs.configurations.Where(x => x.lensFacing == (int)(TofAr.V0.Tof.LensFacing.Back) && x.width == width && x.height == height).FirstOrDefault();
                    }
                }

                // Get rear config for color
                {
                    var colorDefaultConfig = colorManager.GetProperty<DefaultResolutionProperty>();
                    int width = colorDefaultConfig.width;
                    int height = colorDefaultConfig.height;

                    if (colorDefaultConfig.lensFacing == (int)(TofAr.V0.Tof.LensFacing.Back))
                    {
                        rearConfigColor = colorDefaultConfig;
                    }
                    else
                    {
                        rearConfigColor = colorConfigs.resolutions.Where(x => x.lensFacing == (int)(TofAr.V0.Tof.LensFacing.Back) && x.width == width && x.height == height).FirstOrDefault();
                    }
                }

                // Set new rear config for tof
                if (rearConfigTof != null)
                {
                    tofManager.SetProperty<Camera2SetConfigurationIdProperty>(
                        new Camera2SetConfigurationIdProperty()
                        {
                            uid = rearConfigTof.uid  
                        }
                    );
                }

                // Set new rear config for color
                if (rearConfigColor != null)
                {
                    colorManager.SetProperty<ResolutionProperty>(rearConfigColor);
                }
            }
        }
    }
}


