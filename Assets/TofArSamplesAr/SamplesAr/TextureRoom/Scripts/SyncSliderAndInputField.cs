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
    /// This class is used to sync slider values with inputfield values.
    /// </summary>
    public class SyncSliderAndInputField : MonoBehaviour
    {
        public Slider slider;
        public InputField inputField;
        public int maxNumber;
        public int minNumber;

        void Start()
        {
            inputField.text = Common.map(slider.value, slider.minValue, slider.maxValue, minNumber, maxNumber).ToString();
        }

        public void SyncSliderChanged()
        {
            inputField.text = Common.map(slider.value, slider.minValue, slider.maxValue, minNumber, maxNumber).ToString();
        }

        public void SyncInputFieldChanged()
        {
            if (float.Parse(inputField.text) > maxNumber)
            {
                inputField.text = maxNumber.ToString();
            }
            else if (float.Parse(inputField.text) < minNumber)
            {
                inputField.text = minNumber.ToString();
            }
            slider.value = Common.map(float.Parse(inputField.text), minNumber, maxNumber, 0, 1f);
        }
    }
}
