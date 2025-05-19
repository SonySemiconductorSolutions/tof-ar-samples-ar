/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using TofArSettings.Color;
using TofArSettings.Tof;
using UnityEngine.Events;
using TofAr.V0.Color;
using TofAr.V0;
using TofArSettings.UI;

namespace TofArSettings.ColorDepth
{
    public class CameraSettings : UI.SettingsBase
    {
        [Header("Use Component")]

        /// <summary>
        /// Use/do not use Color component
        /// </summary>
        public bool color = true;

        /// <summary>
        /// Use/do not use Tof component
        /// </summary>
        public bool tof = true;

        ColorManagerController colorMgrCtrl;
        TofManagerController tofMgrCtrl;
        UI.ItemDropdown itemColor, itemDepth;
        UI.Panel colorPanel,tofPanel;

        private const float panelShowTime = 5.0f;

        void Awake()
        {
            // Get panel for displaying information
            foreach (var panel in GetComponentsInChildren<UI.Panel>())
            {
                if (panel.name.Contains("Color"))
                {
                    colorPanel = panel;
                }

                if (panel.name.Contains("Tof"))
                {
                    tofPanel = panel;
                }
            }
        }

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIColor,
                MakeUIDepth
            };

            if (color)
            {
                colorMgrCtrl = FindAnyObjectByType<ColorManagerController>();
                controllers.Add(colorMgrCtrl);

                colorMgrCtrl.OnStreamError += (msg) =>
                {
                    if (colorPanel != null)
                    {
                        colorPanel.PanelObj.GetComponentInChildren<UnityEngine.UI.Text>().text = msg;
                        colorPanel.OpenPanel(false);
                        colorPanel.AutoClosePanel(panelShowTime);
                    }
                };
            }

            if (tof)
            {
                tofMgrCtrl = FindAnyObjectByType<TofManagerController>();
                controllers.Add(tofMgrCtrl);

                tofMgrCtrl.OnStreamError += (msg) =>
                {
                    if (tofPanel != null)
                    {
                        tofPanel.PanelObj.GetComponentInChildren<UnityEngine.UI.Text>().text = msg;
                        tofPanel.OpenPanel(false);
                        tofPanel.AutoClosePanel(panelShowTime);
                    }
                };
            }

            base.Start();

            settings.OnChangeStart += OnChangePanel;
        }

        /// <summary>
        /// Make Color UI
        /// </summary>
        void MakeUIColor()
        {
            if (!color)
            {
                return;
            }

            itemColor = settings.AddItem("Color", colorMgrCtrl.Options,
                colorMgrCtrl.Index, ChangeColor, 0, 90, 400);

            itemColor.Options = colorMgrCtrl.Options;
            var dialog = itemColor.GetDropdownDialog();

            colorMgrCtrl.OnChangeAfter += (index) =>
            {
                itemColor.Index = index;
            };

            colorMgrCtrl.OnMadeOptions += () =>
            {
                itemColor.Options = colorMgrCtrl.Options;
                SetActiveMultiCamSupportToggles(dialog);
            };

            SetActiveMultiCamSupportToggles(dialog);
        }

        private void SetActiveMultiCamSupportToggles(DropdownDialog dialog)
        {
            if (dialog != null)
            {
                dialog.SetActiveMultiCamSupportToggles(false);

                if (TofArManager.Instance != null)
                {
                    var platformConfigProperty = TofArManager.Instance.GetProperty<PlatformConfigurationProperty>();
                    if (platformConfigProperty.platformConfigurationIos.cameraApi == IosCameraApi.AvFoundation)
                    {
                        dialog.SetActiveMultiCamSupportToggles(true);
                    }
                }
            }
        }

        /// <summary>
        /// Event that is called when option is selected from Color list
        /// </summary>
        /// <param name="index">Option index</param>
        void ChangeColor(int index)
        {
            colorMgrCtrl.Index = index;
            ClosePanel();
        }

        /// <summary>
        /// Make Depth UI
        /// </summary>
        void MakeUIDepth()
        {
            if (!tof)
            {
                return;
            }

            itemDepth = settings.AddItem("Tof", tofMgrCtrl.Options,
                tofMgrCtrl.Index, ChangeDepth, 0, 90, 400);

            tofMgrCtrl.OnChangeAfter += (index) =>
            {
                itemDepth.Index = index;
            };

            tofMgrCtrl.OnMadeOptions += () =>
            {
                itemDepth.Options = tofMgrCtrl.Options;
            };
        }

        /// <summary>
        /// Event that is called when option is selected from Depth list
        /// </summary>
        /// <param name="index">Option index</param>
        void ChangeDepth(int index)
        {
            tofMgrCtrl.Index = index;
            ClosePanel();
        }

        /// <summary>
        /// Event called when the state of the panel changes
        /// </summary>
        /// <param name="onOff">open/close</param>
        void OnChangePanel(bool onOff)
        {
            if (onOff)
            {
                if (itemColor != null && colorMgrCtrl != null)
                {
                    Debug.Log($"Set color index to {colorMgrCtrl.Index}");
                    itemColor.Index = colorMgrCtrl.Index;
                }

                if (itemDepth != null && tofMgrCtrl != null)
                {
                    Debug.Log($"Set tof index to {tofMgrCtrl.Index}");
                    itemDepth.Index = tofMgrCtrl.Index;
                }
            }
        }

        private void ShowColorPanle()
        {
            colorPanel.OpenPanel();
            colorPanel.AutoClosePanel(panelShowTime);
        }
    }
}
