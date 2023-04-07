/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Linq;
using TofAr.V0.Slam;
using TofAr.V0.Face;

namespace TofArSettings
{
    public class DependendStreamCameraApiHandler : DependendStreamUIHandler
    {

        protected override void OnEnable()
        {
            base.OnEnable();

            TofArFaceManager.OnStreamStarted += OnFaceStreamStarted;
            TofArSlamManager.OnStreamStarted += OnSlamStreamStarted;

            TofArFaceManager.OnStreamStopped += OnFaceStreamStopped;
            TofArSlamManager.OnStreamStopped += OnSlamStreamStopped;

            UpdateDropdownInteractibility();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            TofArFaceManager.OnStreamStarted -= OnFaceStreamStarted;
            TofArSlamManager.OnStreamStarted -= OnSlamStreamStarted;

            TofArFaceManager.OnStreamStopped -= OnFaceStreamStopped;
            TofArSlamManager.OnStreamStopped -= OnSlamStreamStopped;
        }

        private void OnFaceStreamStarted(object sender)
        {
            UpdateDropdownInteractibility();
        }

        private void OnSlamStreamStarted(object sender)
        {
            UpdateDropdownInteractibility();
        }

        private void OnFaceStreamStopped(object sender)
        {
            UpdateDropdownInteractibility();
        }

        private void OnSlamStreamStopped(object sender)
        {
            UpdateDropdownInteractibility();
        }

        protected override void UpdateDropdownInteractibility()
        {
            UpdateUIForManager();
        }

        private void UpdateUIForManager()
        {
            var mgrs = managersDependColor.Union(managersDependTof);

            var faceMgr = TofArFaceManager.Instance;
            var slamMgr = TofArSlamManager.Instance;

            foreach (var dropdowns in listDropdowns.Values)
            {
                foreach (var dropdown in dropdowns)
                {
                    if (dropdown != null)
                    {
                        bool setInteractable = (colorMgr == null || !colorMgr.IsStreamActive)
                            && (tofMgr == null || !tofMgr.IsStreamActive)
                            && (faceMgr == null || !faceMgr.IsStreamActive)
                            && (slamMgr == null || !slamMgr.IsStreamActive);

                        if (mgrs != null && setInteractable)
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

    }
}
