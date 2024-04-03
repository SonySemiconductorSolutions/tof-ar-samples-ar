/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofArSettings.UI;
using UnityEngine;

namespace TofArSettings.Hand
{
    public class HandRecordSettings : MonoBehaviour
    {
        RecordSettings recordSettings;

        void Awake()
        {
            recordSettings = GetComponent<RecordSettings>();
            if (recordSettings != null)
            {
                recordSettings.AddListenerAddControllersEvent(SetController);
            }
        }

        /// <summary>
        /// Set Controller
        /// </summary>
        public void SetController()
        {
            var handSnapshotCtrl = FindObjectOfType<HandSnapshotController>();
            recordSettings.SetController(handSnapshotCtrl);

            var handRecCtrl = FindObjectOfType<HandRecordController>();
            recordSettings.SetController(handRecCtrl);
        }
    }
}
