/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using System.Linq;
using TofAr.V0;
using TofAr.V0.Color;
using TofAr.V0.Tof;
using TofArSettings.UI;
using UnityEngine;

namespace TofArSettings
{
    public abstract class IDependendStreamUIHandler: MonoBehaviour
    {
        public delegate void StreamStatusChangedDelegate();
        public StreamStatusChangedDelegate OnStreamStatusChanged;

        public abstract void AddTofDependendStreams(List<IDependManager> managersDepend);

        public abstract void AddColorDependendStreams(List<IDependManager> managersDepend);
    }

    public class DependendStreamUIHandler : MonoBehaviour
    {
        protected Dictionary<SettingsBase.ComponentType, List<UI.ItemDropdown>> listDropdowns = new Dictionary<SettingsBase.ComponentType, List<UI.ItemDropdown>>();

        [SerializeField]
        private UI.ToolButton[] buttons;

        protected TofArColorManager colorMgr;
        protected TofArTofManager tofMgr;

        protected List<IDependManager> managersDependTof = new List<IDependManager>();
        protected List<IDependManager> managersDependColor = new List<IDependManager>();

        private IDependendStreamUIHandler[] subHandlers;

        private void Awake()
        {
            subHandlers = FindObjectsOfType<IDependendStreamUIHandler>();

            colorMgr = TofArColorManager.Instance;
            tofMgr = TofArTofManager.Instance;

            foreach (var subHandler in subHandlers)
            {
                if (subHandler != null)
                {
                    subHandler.AddColorDependendStreams(managersDependColor);
                    subHandler.AddTofDependendStreams(managersDependTof);
                    subHandler.OnStreamStatusChanged += UpdateDropdownInteractibility;
                }
            }
        }

        protected virtual void OnEnable()
        {
            TofArColorManager.OnStreamStarted += OnColorStreamStarted;
            TofArColorManager.OnStreamStopped += OnColorStreamStopped;

            TofArTofManager.OnStreamStarted += OnTofStreamStarted;
            TofArTofManager.OnStreamStopped += OnTofStreamStopped;
        }

        protected virtual void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
            TofArColorManager.OnStreamStopped -= OnColorStreamStopped;

            TofArTofManager.OnStreamStarted -= OnTofStreamStarted;
            TofArTofManager.OnStreamStopped -= OnTofStreamStopped;
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

        protected virtual void UpdateDropdownInteractibility()
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
