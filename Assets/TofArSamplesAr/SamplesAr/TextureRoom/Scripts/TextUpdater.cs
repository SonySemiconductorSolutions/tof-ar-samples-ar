/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;

namespace TofArARSamples.TextureRoom
{
    /// <summary>
    /// Class for retrieving the text entered in the inputfield
    /// </summary>
    public class TextUpdater : MonoBehaviour
    {
        public InputField InputField;
        public Text Text;

        void Update()
        {
            Text.text = InputField.text;
        }
    }
}
