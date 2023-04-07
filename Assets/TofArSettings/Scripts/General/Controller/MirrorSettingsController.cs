/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using UnityEngine.Events;

namespace TofArSettings.General
{
    public class MirrorSettingsController : ControllerBase
    {
        /// <summary>
        /// Enable/Disable Mirroring
        /// </summary>
        public bool OnOff
        {
            get
            {
                return onOff;
            }

            set
            {
                if (value != onOff)
                {
                    onOff = value;
                    SetProperty();
                    OnChangeMirrorSettings?.Invoke(onOff);
                }
            }
        }

        bool onOff = false;

        public UnityAction<bool> OnChangeMirrorSettings;


        protected override void Start()
        {
            OnOff = TofArManager.Instance?.IsMirroring == true;
            base.Start();
        }

        /// <summary>
        /// Apply mirroring property
        /// </summary>
        void SetProperty()
        {
            var mgr = TofArManager.Instance;
            if (!mgr)
            {
                return;
            }

            mgr.IsMirroring = onOff;
        }
    }
}
