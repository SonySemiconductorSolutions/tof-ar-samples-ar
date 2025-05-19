/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2024 Sony Semiconductor Solutions Corporation.
 *
 */

using TofArSettings.Color;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class MultiCamSupportToggles : MonoBehaviour
    {
        [SerializeField]
        Toggle depth;

        [SerializeField]
        Toggle multiCam;

        private DropdownDialog dropdownDialog;
        private ColorManagerController colorMgrCtrl;

        // Start is called before the first frame update
        void Start()
        {
            dropdownDialog = GetComponentInParent<DropdownDialog>();
            colorMgrCtrl = FindAnyObjectByType<ColorManagerController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ResetIsOn()
        {
            depth.isOn = false;
            multiCam.isOn = false;
        }

        public void DepthOnValueChanged()
        {
            OnToggleValueChanged();
        }

        public void MultiCamOnValueChanged()
        {
            OnToggleValueChanged();
        }

        private void OnToggleValueChanged()
        {
            if (colorMgrCtrl != null)
            {
                dropdownDialog.ClearSelectActiveOptions();

                if (depth.isOn || multiCam.isOn)
                {
                    var indices = colorMgrCtrl.GetAvfSupportedResolutionPropertyIndices(depth.isOn, multiCam.isOn);
                    dropdownDialog.SelectActiveOptions(indices);
                }
            }
        }
    }
}
