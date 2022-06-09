/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using TofAr.V0.Hand;
using UnityEngine;

namespace TofArSamples.HandComponent
{
    public class HandStartup : MonoBehaviour
    {
        TofArSamples.Startup.Startup startup;

        void Awake()
        {
            startup = GetComponent<TofArSamples.Startup.Startup>();
            if (startup != null)
            {
                startup.AddListenerStopTofArEventEvent(StopTofAr);
                startup.AddListenerDestroyTofArEvent(DestroyTofAr);
            }
        }

        public void StopTofAr()
        {
            TofArHandManager.Instance?.StopStream();
        }

        public void DestroyTofAr()
        {
            var handMgr = TofArHandManager.Instance;
            if (handMgr)
            {
                Destroy(handMgr.gameObject);
            }
        }
    }
}
