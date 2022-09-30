/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using UnityEngine.Events;

namespace TofArSettings.General
{
    public class TofArFramerateController : ControllerBase
    {
        /// <summary>
        /// ToFAR Session Framerate
        /// </summary>
        public int Framerate
        {
            get
            {
                return framerate;
            }

            set
            {
                var mgr = TofArManager.Instance;
                if (value != framerate)
                {
                    framerate = value;
                    SetProperty(Framerate);
                    OnChangeFramerate?.Invoke(Framerate);
                }
            }
        }

        int framerate = FramerateDefault;

        public const int FramerateMin = 2;
        public const int FramerateMax = 60;
        public const int FramerateStep = 1;
        public const int FramerateDefault = 30;

        public UnityAction<int> OnChangeFramerate;

        protected override void Start()
        {
            SetProperty(Framerate);
            base.Start();
        }

        /// <summary>
        /// Apply Framerate
        /// </summary>
        /// <param name="rate">Framerate</param>
        void SetProperty(int rate)
        {
            var mgr = TofArManager.Instance;

            // Set only when on iOS
            if (!mgr || mgr.Stream == null || !mgr.UsingIos)
            {
                return;
            }

            mgr.Stream.SetProperty(new PlatformConfigurationProperty()
            {
                platformConfigurationIos = new PlatformConfigurationIos()
                {
                    sessionFramerate = rate
                }
            });
        }
    }
}
