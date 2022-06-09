/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using UnityEngine;

namespace TofArSettings.UI
{
    public class Adjuster : MonoBehaviour
    {
        /// <summary>
        /// Initial value
        /// </summary>
        public int DefaultValue = 0;

        /// <summary>
        /// Number of decimal places
        /// </summary>
        public int DecimalDigit = 1;

        /// <summary>
        /// Adjustment value range
        /// </summary>
        public int AdjustValue = 5;

        /// <summary>
        /// Adjustment value range (long tap)
        /// </summary>
        public int AdjustValueLong = 5;

        /// <summary>
        /// Time in order to recognize as long press (Unit: s)
        /// </summary>
        public float JudgeLongTapTime = 1.0f;

        /// <summary>
        /// Intervals at which calculations are performed on long pressed (Unit: s)
        /// </summary>
        public float LongTapInterval = 0.15f;

        /// <summary>
        /// Minimum possible input value
        /// </summary>
        public int Min = 0;

        /// <summary>
        /// Maximum possible input value
        /// </summary>
        public int Max = 10;

        /// <summary>
        /// Real value
        /// </summary>
        public float Value
        {
            get
            {
                return ConvertFromPlain(PlainValue);
            }

            set
            {
                PlainValue = ConvertToPlain(value);
            }
        }

        /// <summary>
        /// Value used for calculation
        /// </summary>
        int val;
        public int PlainValue
        {
            get { return val; }
            set
            {
                if (val != value && Min <= value && value <= Max)
                {
                    val = value;
                    OnChange?.Invoke(Value);
                }
            }
        }

        /// <summary>
        /// Interactable/non-interactable
        /// </summary>
        bool interactable = true;
        public bool Interactable
        {
            get { return interactable; }
            set
            {
                if (interactable != value)
                {
                    if (!finishedSetup)
                    {
                        Awake();
                    }

                    interactable = value;
                    for (int i = 0; i < triggers.Length; i++)
                    {
                        triggers[i].Interactable = value;
                    }
                }
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

        ImageButtonTrigger[] triggers;
        bool isTap = false;
        bool isRunning = false;

        bool finishedSetup = false;

        void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            // Get UI
            triggers = GetComponentsInChildren<ImageButtonTrigger>();
            for (int i = 0; i < triggers.Length; i++)
            {
                var trigger = triggers[i];
                if (trigger.name.Contains("Minus"))
                {
                    trigger.OnTouchDown += () =>
                    {
                        OnTouchDown(false);
                    };
                }
                else if (trigger.name.Contains("Plus"))
                {
                    trigger.OnTouchDown += () =>
                    {
                        OnTouchDown(true);
                    };
                }

                trigger.OnTouchUp += OnTouchUp;
            }

            val = DefaultValue;

            finishedSetup = true;
        }

        /// <summary>
        /// Convert calculation value to real value
        /// </summary>
        /// <param name="plainVal">Calculation value</param>
        /// <returns>Real value</returns>
        public float ConvertFromPlain(int plainVal)
        {
            return plainVal / Mathf.Pow(10, DecimalDigit);
        }

        /// <summary>
        /// Convert real value to calculation value
        /// </summary>
        /// <param name="realVal">Real value</param>
        /// <returns>Calculation value</returns>
        public int ConvertToPlain(float realVal)
        {
            float v = realVal * Mathf.Pow(10, DecimalDigit);
            return Mathf.RoundToInt(v);
        }

        /// <summary>
        /// Event that is called when plus or minus button is pressed
        /// </summary>
        /// <param name="plusOrMinus">Plus or minus button</param>
        void OnTouchDown(bool plusOrMinus)
        {
            isTap = true;
            StartCoroutine(LongTap(plusOrMinus));
        }

        /// <summary>
        /// Event that is called when plus or minus button is released
        /// </summary>
        /// <param name="plusOrMinus">Plus or minus button</param>
        void OnTouchUp()
        {
            isTap = false;
        }

        /// <summary>
        /// Recognize long press and change value
        /// </summary>
        /// <param name="plusOrMinus">Plus or minus button</param>
        IEnumerator LongTap(bool plusOrMinus)
        {
            if (isRunning)
            {
                yield break;
            }

            isRunning = true;

            // Perform one calculation
            Calc(plusOrMinus);

            // Long press recognition
            float time = 0.0f;
            while (time < JudgeLongTapTime)
            {
                time += Time.deltaTime;

                // Stop when released
                if (!isTap)
                {
                    isRunning = false;
                    yield break;
                }

                yield return null;
            }

            // When long press, changes value at regular intervals
            int count = 0;
            while (isTap)
            {
                if (count < 5)
                {
                    // Change first five times as usual
                    Calc(plusOrMinus);
                }
                else
                {
                    // Change more afterwards
                    Calc(plusOrMinus, true);
                }

                count++;
                yield return new WaitForSeconds(LongTapInterval);
            }

            isRunning = false;
        }

        /// <summary>
        /// Calculate value
        /// </summary>
        /// <param name="plusOrMinus">Plus or minus button</param>
        /// <param name="isLongTap">Long press or not</param>
        void Calc(bool plusOrMinus, bool isLongTap = false)
        {
            int adjust = (isLongTap) ? AdjustValueLong : AdjustValue;
            PlainValue = (plusOrMinus) ?
                PlainValue + adjust : PlainValue - adjust;
        }
    }
}
