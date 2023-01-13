/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using TofAr.V0;
using UnityEngine;

namespace TofArSettings.UI
{
    public class GeneralSettings : SettingsBase
    {
        [Header("Use Component")]

        /// <summary>
        /// Use/do not use Color component
        /// </summary>
        [SerializeField]
        bool color = true;

        /// <summary>
        /// Use/do not use Tof component
        /// </summary>
        [SerializeField]
        bool tof = true;

        /// <summary>
        /// Use/do not use Hand component
        /// </summary>
        [SerializeField]
        bool hand = true;

        /// <summary>
        /// Use/do not use Body component
        /// </summary>
        [SerializeField]
        bool body = true;

        /// <summary>
        /// Use/do not use Segmentation component
        /// </summary>
        [SerializeField]
        bool segmentation = true;

        /// <summary>
        /// Use/do not use Mesh component
        /// </summary>
        [SerializeField]
        bool mesh = true;

        /// <summary>
        /// Use/do not use Fingertouch component
        /// </summary>
        [SerializeField]
        bool fingerTouch = true;

        /// <summary>
        /// Use/do not use Slam component
        /// </summary>
        [SerializeField]
        bool slam = false;

        /// <summary>
        /// Use/do not use Face component
        /// </summary>
        [SerializeField]
        bool face = false;

        List<SettingsBase> menus = new List<SettingsBase>();

        /// <summary>
        /// Called after application startup (Unity standard function)
        /// </summary>
        void Awake()
        {
            var ms = GetComponentsInChildren<SettingsBase>();

            // add GeneralChild Panel if using iOS
            bool addGeneralChild = (TofArManager.Instance.UsingIos);

            for (int i = 0; i < ms.Length; i++)
            {
                var menu = ms[i];
                if (menu == this)
                {
                    continue;
                }

                if ((addGeneralChild && menu is General.GeneralSettingsChild) || 
                    IsCompoTypeAvailable(menu))
                {
                    menus.Add(menu);
                }
                else
                {
                    menu.gameObject.SetActive(false);
                }
            }
        }

        private bool IsCompoTypeAvailable(SettingsBase menu)
        {
            return menu.CompoType == ComponentType.Color && color ||
                    menu.CompoType == ComponentType.Tof && tof ||
                    menu.CompoType == ComponentType.Hand && hand ||
                    menu.CompoType == ComponentType.Segmentation && segmentation ||
                    menu.CompoType == ComponentType.Body && body ||
                    menu.CompoType == ComponentType.Mesh && mesh ||
                    menu.CompoType == ComponentType.FingerTouch && fingerTouch ||
                    menu.CompoType == ComponentType.Slam && slam ||
                    menu.CompoType == ComponentType.Face && face;
        }

        /// <summary>
        /// Make UI
        /// </summary>
        protected override void MakeUI()
        {
            for (int i = 0; i < menus.Count; i++)
            {
                var menu = menus[i];

                // Set sub menu panel to open when selected
                settings.AddItem(menu.Title, menu.TitleIcon, menu.IconColor, () =>
                {
                    menu.OpenPanel();
                });

                // Return back to main menu after pressing back button on sub menu
                menu.OnBack += () =>
                {
                    settings.OpenPanel();
                };
            }

            base.MakeUI();
        }
    }
}
