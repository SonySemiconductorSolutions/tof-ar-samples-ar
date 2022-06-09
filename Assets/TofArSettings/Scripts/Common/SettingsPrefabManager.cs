/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using TofAr.V0;

namespace TofArSettings.UI
{
    public class SettingsPrefabManager : Singleton<SettingsPrefabManager>
    {
        /// <summary>
        /// SettingsPanel Prefab
        /// </summary>
        public GameObject PanelPrefab;

        /// <summary>
        /// Line Prefab
        /// </summary>
        public GameObject LinePrefab;

        /// <summary>
        /// Nest Menu Prefab
        /// </summary>
        public GameObject ItemNestMenuPrefab;

        /// <summary>
        /// Button Prefab
        /// </summary>
        public GameObject ItemButtonPrefab;

        /// <summary>
        /// Dropdown Prefab
        /// </summary>
        public GameObject ItemDropdownPrefab;

        /// <summary>
        /// InputField Prefab
        /// </summary>
        public GameObject ItemInputFieldPrefab;

        /// <summary>
        /// Slider Prefab
        /// </summary>
        public GameObject ItemSliderPrefab;

        /// <summary>
        /// Toggle Prefab
        /// </summary>
        public GameObject ItemTogglePrefab;

        /// <summary>
        /// Multiple Toggle Prefab
        /// </summary>
        public GameObject ItemToggleMultiPrefab;

        /// <summary>
        /// Dropdown Selection Dialog Prefab
        /// </summary>
        public GameObject DropdownDialogPrefab;

        /// <summary>
        /// Dropdown Option Prefab
        /// </summary>
        public GameObject DropdownOptionPrefab;
    }
}
