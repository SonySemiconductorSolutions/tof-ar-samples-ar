/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TofArARSamples.TextureRoom
{
    /// <summary>
    /// Scriptable object to set texture for animation
    /// </summary>
    [CreateAssetMenu(menuName = "TofArSettings/Create Mapping Setting")]
    public class MappingSettingsScriptableObject : ScriptableObject
    {
        public List<Texture2D> animationTextures;
    }
}
