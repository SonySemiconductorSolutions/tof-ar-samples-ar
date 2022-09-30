/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class ItemSlider : Item
    {
        private const long MIN_VAL = -9999999;
        private const long MAX_VAL = 9999999;

        public override bool Interactable
        {
            set
            {
                base.Interactable = value;
                itemInput.Interactable = value;
                slider.interactable = value;
                adjuster.Interactable = value;
            }
        }

        public bool IsNotifyImmediately = false;

        float val;
        public float Value
        {
            get { return val; }
            set
            {
                if (val != value && CheckRange(value))
                {
                    val = value;
                    itemInput.Value = val.ToString();
                    adjuster.Value = val;
                    slider.value = adjuster.PlainValue;

                    if (IsNotifyImmediately)
                    {
                        OnChange?.Invoke(Value);
                    }
                    else if (!operating)
                    {
                        // If Slider if being used, notify when done
                        OnChange?.Invoke(Value);
                    }
                }
            }
        }

        /// <summary>
        /// Minimum possible input value
        /// </summary>
        public float Min
        {
            get
            {
                return adjuster.ConvertFromPlain(adjuster.Min);
            }

            set
            {
                adjuster.Min = adjuster.ConvertToPlain(Mathf.Min(MAX_VAL, Mathf.Max(MIN_VAL, value)));
                slider.minValue = adjuster.Min;
                itemInput.Min = Min;
                SetRangeText();
            }
        }

        /// <summary>
        /// Maximum possible input value
        /// </summary>
        public float Max
        {
            get
            {
                return adjuster.ConvertFromPlain(adjuster.Max);
            }

            set
            {
                adjuster.Max = adjuster.ConvertToPlain(Mathf.Min(MAX_VAL, Mathf.Max(MIN_VAL, value)));
                slider.maxValue = adjuster.Max;
                itemInput.Max = Max;
                SetRangeText();
            }
        }

        /// <summary>
        /// Step value
        /// </summary>
        public float Step
        {
            get
            {
                return adjuster.ConvertFromPlain(adjuster.AdjustValue);
            }
            set
            {
                // Count the number of decimal places and set them in Adjuster
                string str = value.ToString();
                string[] split = str.Split('.');
                adjuster.DecimalDigit = (split.Length > 1) ?
                    split[1].Length : 0;
                adjuster.AdjustValue = adjuster.ConvertToPlain(value);
                adjuster.AdjustValueLong = adjuster.AdjustValue * 3;
                SetRangeText();
            }
        }

        /// <summary>
        /// Event that is called when value is changed
        /// </summary>
        /// <param name="val">Value</param>
        public delegate void ChangeEvent(float val);

        /// <summary>
        /// Event that is called when value is changed
        /// </summary>
        public event ChangeEvent OnChange;

        ItemInputField itemInput;
        Slider slider;
        SliderExt sliderExt;
        RectTransform sliderRt;
        Adjuster adjuster;

        bool operating = false;

        protected override void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            base.Awake();

            // Get UI
            itemInput = GetComponentInChildren<ItemInputField>();
            slider = GetComponentInChildren<Slider>();
            sliderExt = slider.GetComponent<SliderExt>();
            sliderRt = slider.GetComponent<RectTransform>();
            adjuster = GetComponentInChildren<Adjuster>();

            // Register event
            slider.onValueChanged.AddListener(OnChangeSlider);
            sliderExt.OnDown += () =>
            {
                operating = true;
            };

            sliderExt.OnUp += () =>
            {
                operating = false;
                OnChange?.Invoke(Value);
            };

            adjuster.OnChange += (val) =>
            {
                Value = val;
            };
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <param name="step">Step value</param>
        /// <param name="val">Initial value</param>
        /// <param name="onChange">Event that is called when value is changed</param>
        public void Init(string title, int relativeFontSize, float fixedTitleWidth,
            float min, float max, float step, float val, ChangeEvent onChange)
        {
            base.Init(title, relativeFontSize, fixedTitleWidth);

            itemInput.Init(title, relativeFontSize, fixedTitleWidth, min, max,
                val, (val) =>
            {
                // Reset if value not divisible by Step is entered
                /*if (val % Step == 0)
                {
                    Value = val;
                }
                else
                {
                    itemInput.Value = Value.ToString();
                }*/

                Value = val;
            });

            // Call step before min and max to setup Adjuster
            Step = step;
            Min = min;
            Max = max;
            Value = val;
            OnChange = onChange;

            SetRangeText();

            adjuster.DefaultValue = adjuster.ConvertToPlain(Value);

            // Adjust size of Slider to fit the size of ItemInput
            sliderRt.sizeDelta = new Vector2(sliderRt.sizeDelta.x - WidthDiff, sliderRt.sizeDelta.y);
        }

        /// <summary>
        /// Event that is called when Slider is used
        /// </summary>
        /// <param name="v">Slider value</param>
        void OnChangeSlider(float v)
        {
            // Calculate to move by Step
            int val = Mathf.RoundToInt(v);
            if (adjuster.AdjustValue != 0)
            {
                while (true)
                {
                    int rem = val % adjuster.AdjustValue;
                    if (rem == 0)
                    {
                        break;
                    }
                    else if (rem <= adjuster.AdjustValue / 2)
                    {
                        val--;
                    }
                    else
                    {
                        val++;
                    }
                }
            }

            adjuster.PlainValue = val;
        }

        /// <summary>
        /// Check range
        /// </summary>
        /// <param name="val">Input value</param>
        /// <returns>Within/Outside range</returns>
        bool CheckRange(float val)
        {
            return (Min <= val && val <= Max);
        }

        /// <summary>
        /// Update range display
        /// </summary>
        void SetRangeText()
        {
            itemInput.TxtRange.text = $"{Min} ~ {Max} , step = {Step}";
        }
    }
}
