/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofArSettings.UI;
using UnityEngine;

namespace TofArSettings.Hand
{
    public class HandRecPlayerSettings : MonoBehaviour
    {
        RecPlayerSettings recPlayerSettings;

        void Awake()
        {
            recPlayerSettings = GetComponent<RecPlayerSettings>();
            if (recPlayerSettings != null)
            {
                recPlayerSettings.AddListenerAddControllersAndPlayerSetsEvent(SetControllerAndPlayerSet);
            }
        }

        /// <summary>
        /// Set controller and playerSet
        /// </summary>
        public void SetControllerAndPlayerSet()
        {
            var handCtrl = FindObjectOfType<HandRecPlayerController>();
            recPlayerSettings.SetController(handCtrl);
            recPlayerSettings.SetPlayerSets(SettingsBase.ComponentType.Hand, handCtrl);
        }
    }
}
