/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TofArSettings.UI
{
    [CreateAssetMenu(menuName = "TofArSettings/Create BaseColor")]
    public class BaseColor : ScriptableObject
    {
        public UnityEngine.Color color;
    }
}
