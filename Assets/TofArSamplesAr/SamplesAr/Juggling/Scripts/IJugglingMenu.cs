/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This interface defines methods of GUI menu in juggling scene.
    /// </summary>
    public interface IJugglingMenu
    {
        /// <summary>
        /// displays menu on screen.
        /// </summary>
        public void OpenMenu();

        /// <summary>
        /// hides menu on screen.
        /// </summary>
        public void CloseMenu();
    }
}
