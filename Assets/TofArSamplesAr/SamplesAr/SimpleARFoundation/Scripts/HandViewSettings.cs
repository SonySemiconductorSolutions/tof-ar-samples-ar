/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofArSettings.UI;
using UnityEngine;
using UI = TofArSettings.UI;

namespace TofArARSamples.SimpleARFoundation
{
    public class HandViewSettings : MonoBehaviour
    {
        MeshViewSettings meshViewSettings;

        /// <summary>
        /// Toggle Skeleton Hand
        /// </summary>
        [SerializeField]
        bool skeleton = true;

        SkeletonHandController skeletonCtrl;

        UI.ItemToggle itemSkeleton;
        void Awake()
        {
            meshViewSettings = GetComponent<MeshViewSettings>();
            if (meshViewSettings != null)
            {
                meshViewSettings.AddListenerAddControllersEvent(SetController);

                skeletonCtrl = GetComponent<SkeletonHandController>();
                skeletonCtrl.enabled = skeleton;
            }
        }

        /// <summary>
        /// Setting Controller
        /// </summary>
        /// <param name="settingsPanel">SettingsPanel</param>
        public void SetController(SettingsPanel settingsPanel)
        {

            if (skeleton)
            {
                meshViewSettings.SetUIOrderList(MakeUISkeletonHand);
                meshViewSettings.SetController(skeletonCtrl);
            }
            
        }


        /// <summary>
        /// Make Skeleton Hand UI
        /// </summary>
        void MakeUISkeletonHand()
        {
            itemSkeleton = meshViewSettings.GetSettings().AddItem("Show Bones", skeletonCtrl.IsShow,
                ChangeSkeletonHand);

            skeletonCtrl.OnChangeShow += (onOff) =>
            {
                itemSkeleton.OnOff = onOff;
            };
        }

        /// <summary>
        /// Toggle Skeleton Hand display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangeSkeletonHand(bool onOff)
        {
            skeletonCtrl.IsShow = onOff;
        }

    }
}
