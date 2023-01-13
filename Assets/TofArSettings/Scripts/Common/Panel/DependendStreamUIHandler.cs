/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using TofAr.V0;
using TofAr.V0.Body;
using TofAr.V0.Color;
using TofAr.V0.Hand;
using TofAr.V0.Mesh;
using TofAr.V0.Segmentation;
using TofAr.V0.Tof;
using TofArSettings.UI;
using UnityEngine;

namespace TofArSettings
{
    public class DependendStreamUIHandler : MonoBehaviour
    {
        private Dictionary<SettingsBase.ComponentType, List<UI.ItemDropdown>> listDropdowns = new Dictionary<SettingsBase.ComponentType, List<UI.ItemDropdown>>();

        [SerializeField]
        private UI.ToolButton[] buttons;

        private TofArColorManager colorMgr;
        private TofArTofManager tofMgr;
        private TofArHandManager handMgr;
        private TofArBodyManager bodyMgr;
        private TofArSegmentationManager segMgr;
        private TofArMeshManager meshMgr;

        private List<IDependManager> managersDependTof = new List<IDependManager>();
        private List<IDependManager> managersDependColor = new List<IDependManager>();

        private void Awake()
        {
            colorMgr = TofArColorManager.Instance;
            tofMgr = TofArTofManager.Instance;
            handMgr = TofArHandManager.Instance;
            bodyMgr = TofArBodyManager.Instance;
            segMgr = TofArSegmentationManager.Instance;
            meshMgr = TofArMeshManager.Instance;

            managersDependTof.Add(handMgr);
            managersDependTof.Add(bodyMgr);
            managersDependTof.Add(meshMgr);

            managersDependColor.Add(segMgr);
        }

        private void OnEnable()
        {
            TofArColorManager.OnStreamStarted += OnColorStreamStarted;
            TofArColorManager.OnStreamStopped += OnColorStreamStopped;

            TofArTofManager.OnStreamStarted += OnTofStreamStarted;
            TofArTofManager.OnStreamStopped += OnTofStreamStopped;

            TofArHandManager.OnStreamStarted += OnStreamStarted;
            TofArHandManager.OnStreamStopped += OnStreamStopped;

            TofArBodyManager.OnStreamStarted += OnStreamStarted;
            TofArBodyManager.OnStreamStopped += OnStreamStopped;

            TofArSegmentationManager.OnStreamStarted += OnStreamStarted;
            TofArSegmentationManager.OnStreamStopped += OnStreamStopped;

            TofArMeshManager.OnStreamStarted += OnStreamStarted;
            TofArMeshManager.OnStreamStopped += OnStreamStopped;
        }

        private void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
            TofArColorManager.OnStreamStopped -= OnColorStreamStopped;

            TofArTofManager.OnStreamStarted -= OnTofStreamStarted;
            TofArTofManager.OnStreamStopped -= OnTofStreamStopped;

            TofArHandManager.OnStreamStarted -= OnStreamStarted;
            TofArHandManager.OnStreamStopped -= OnStreamStopped;

            TofArBodyManager.OnStreamStarted -= OnStreamStarted;
            TofArBodyManager.OnStreamStopped -= OnStreamStopped;

            TofArSegmentationManager.OnStreamStarted -= OnStreamStarted;
            TofArSegmentationManager.OnStreamStopped -= OnStreamStopped;

            TofArMeshManager.OnStreamStarted -= OnStreamStarted;
            TofArMeshManager.OnStreamStopped -= OnStreamStopped;
        }

        private void OnStreamStarted(object sender)
        {
            UpdateDropdownInteractibility();
        }

        private void OnStreamStopped(object sender)
        {
            UpdateDropdownInteractibility();
        }

        private void OnColorStreamStarted(object sender, Texture2D colorTexture)
        {
            OnStreamStarted(sender);
        }

        private void OnColorStreamStopped(object sender)
        {
            OnStreamStopped(sender);
        }

        private void OnTofStreamStopped(object sender)
        {
            OnStreamStopped(sender);
        }

        private void OnTofStreamStarted(object sender, Texture2D depthTexture, Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            OnStreamStarted(sender);
        }

        private void UpdateDropdownInteractibility()
        {
            UpdateUIForManager(SettingsBase.ComponentType.Color);
            UpdateUIForManager(SettingsBase.ComponentType.Tof);

            bool tofStopped = IsDependendStreamStopped(SettingsBase.ComponentType.Tof);
            bool colorStopped = IsDependendStreamStopped(SettingsBase.ComponentType.Color);

            if (buttons != null)
            {
                foreach (var btn in buttons)
                {
                    if (btn != null && btn.isActiveAndEnabled)
                    {
                        btn.Interactable = tofStopped && colorStopped;

                    }
                }
            }
        }

        private void UpdateUIForManager(SettingsBase.ComponentType type)
        {
            List<IDependManager> mgrs = type == SettingsBase.ComponentType.Color ? managersDependColor :
                type == SettingsBase.ComponentType.Tof ? managersDependTof :
                null;

            if (mgrs == null)
            {
                return;
            }

            bool isDependedStreamInactive =
                type == SettingsBase.ComponentType.Color ? colorMgr != null && !colorMgr.IsStreamActive :
                type == SettingsBase.ComponentType.Tof ? tofMgr != null && !tofMgr.IsStreamActive :
                false;


            if (listDropdowns.ContainsKey(type))
            {
                var dropdowns = listDropdowns[type];

                foreach (var dropdown in dropdowns)
                {
                    if (dropdown != null)
                    {
                        bool setInteractable = true;
                        if (isDependedStreamInactive)
                        {
                            foreach (var mgr in mgrs)
                            {
                                if (mgr?.IsStreamActive == true)
                                {
                                    setInteractable = false;
                                    break;
                                }
                            }
                        }
                        dropdown.Interactable = setInteractable;

                    }
                }
            }
        }

        private bool IsDependendStreamStopped(SettingsBase.ComponentType type)
        {
            List<IDependManager> mgrs = type == SettingsBase.ComponentType.Color ? managersDependColor :
                type == SettingsBase.ComponentType.Tof ? managersDependTof :
                null;

            if (mgrs == null)
            {
                return true;
            }

            bool isDependedStreamInactive =
                type == SettingsBase.ComponentType.Color ? colorMgr != null && !colorMgr.IsStreamActive :
                type == SettingsBase.ComponentType.Tof ? tofMgr != null && !tofMgr.IsStreamActive :
                false;

            
            if (isDependedStreamInactive)
            {
                foreach (var mgr in mgrs)
                {
                    if (mgr?.IsStreamActive == true)
                    {
                        return false;
                    }
                }
            }


            return true;
        }

        public void AddDropdown(ItemDropdown dropdown, SettingsBase.ComponentType type)
        {
            if (!listDropdowns.ContainsKey(type))
            {
                listDropdowns.Add(type, new List<ItemDropdown>());
            }
          
            listDropdowns[type].Add(dropdown);

            UpdateDropdownInteractibility();
        }


    }
}
