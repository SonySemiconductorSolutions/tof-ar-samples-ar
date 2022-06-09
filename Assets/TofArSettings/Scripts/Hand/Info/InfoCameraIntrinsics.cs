/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Threading;
using TofAr.V0.Tof;

namespace TofArSettings.Hand
{
    public class InfoCameraIntrinsics : HandInfo
    {
        UI.InfoLR uiSizeCInfo;
        UI.InfoLR uiSizeDInfo;
        UI.InfoLR uiColorIPFInfo;
        UI.InfoLR uiColorIPCInfo;
        UI.InfoLR uiDepthIPFInfo;
        UI.InfoLR uiDepthIPCInfo;

        UI.InfoLCR uiComerasR1Info;
        UI.InfoLCR uiComerasR2Info;
        UI.InfoLCR uiComerasR3Info;
        UI.InfoLCR uiComerasPInfo;

        SynchronizationContext context;

        protected override void Awake()
        {
            base.Awake();

            // Due to the possibility of event occuring before Start, get in Awake
            context = SynchronizationContext.Current;

            foreach (var ui in GetComponentsInChildren<UI.InfoLR>())
            {
                if (ui.name.Contains("SizeC"))
                {
                    uiSizeCInfo = ui;
                }
                else if (ui.name.Contains("SizeD"))
                {
                    uiSizeDInfo = ui;
                }
                else if (ui.name.Contains("ColorIPF"))
                {
                    uiColorIPFInfo = ui;
                }
                else if (ui.name.Contains("ColorIPC"))
                {
                    uiColorIPCInfo = ui;
                }
                else if (ui.name.Contains("DepthIPF"))
                {
                    uiDepthIPFInfo = ui;
                }
                else if (ui.name.Contains("DepthIPC"))
                {
                    uiDepthIPCInfo = ui;
                }
            }

            foreach (var ui in GetComponentsInChildren<UI.InfoLCR>())
            {
                if (ui.name.Contains("CamerasR1"))
                {
                    uiComerasR1Info = ui;
                }
                else if (ui.name.Contains("CamerasR2"))
                {
                    uiComerasR2Info = ui;
                }
                else if (ui.name.Contains("CamerasR3"))
                {
                    uiComerasR3Info = ui;
                }
                else if (ui.name.Contains("CamerasP"))
                {
                    uiComerasPInfo = ui;
                }
            }

            TofArTofManager.Instance?.CalibrationSettingsLoaded.
                AddListener(OnCalibrationLoaded);
        }

        /// <summary>
        /// Event that is called when Calibration is loaded
        /// </summary>
        /// <param name="settings">Calibration settings</param>
        void OnCalibrationLoaded(CalibrationSettingsProperty settings)
        {
            // Show
            context.Post((s) =>
            {
                uiSizeCInfo.SetText(settings.colorWidth.ToString(), settings.colorHeight.ToString());
                uiSizeDInfo.SetText(settings.depthWidth.ToString(), settings.depthHeight.ToString());

                uiColorIPFInfo.SetText($"{settings.c.fx:F4}", $"{settings.c.fy:F4}");
                uiColorIPCInfo.SetText($"{settings.c.cx:F4}", $"{settings.c.cy:F4}");
                uiDepthIPFInfo.SetText($"{settings.d.fx:F4}", $"{settings.d.fy:F4}");
                uiDepthIPCInfo.SetText($"{settings.d.cx:F4}", $"{settings.d.cy:F4}");

                uiComerasR1Info.SetText($"{settings.R.a:F4}", $"{settings.R.b:F4}", $"{settings.R.c:F4}");
                uiComerasR2Info.SetText($"{settings.R.d:F4}", $"{settings.R.e:F4}", $"{settings.R.f:F4}");
                uiComerasR3Info.SetText($"{settings.R.g:F4}", $"{settings.R.h:F4}", $"{settings.R.i:F4}");
                uiComerasPInfo.SetText($"{settings.T.x:F4}", $"{settings.T.y:F4}", $"{settings.T.z:F4}");

            }, null);
        }
    }
}
