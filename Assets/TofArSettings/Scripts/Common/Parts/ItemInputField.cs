/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class ItemInputField : Item
    {
        public override bool Interactable
        {
            set
            {
                base.Interactable = value;
                inputField.interactable = value;
            }
        }

        /// <summary>
        /// InputField width
        /// </summary>
        public float Width
        {
            get { return inputField.preferredWidth; }
            set
            {
                if (value > 0)
                {
                    inputRt.sizeDelta = new Vector2(value, inputRt.sizeDelta.y);
                }
            }
        }

        string val;
        public string Value
        {
            get { return val; }
            set
            {
                if (val != value && CheckRange(value))
                {
                    val = value;

                    OnChange?.Invoke(Value);
                    OnChangeInt?.Invoke(IntValue);
                    OnChangeFloat?.Invoke(FloatValue);
                }

                inputField.text = val;
            }
        }

        /// <summary>
        /// Value converted to an integer
        /// </summary>
        public int IntValue
        {
            get
            {
                if (int.TryParse(Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out int v))
                {
                    return v;
                }

                return 0;
            }
        }

        /// <summary>
        /// Value converted to a float
        /// </summary>
        public float FloatValue
        {
            get
            {
                if (float.TryParse(Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float v))
                {
                    return v;
                }

                return 0;
            }
        }

        float min = 0;
        public float Min
        {
            get { return min; }
            set
            {
                if (min != value)
                {
                    min = value;
                    SetRangeText();
                }
            }
        }

        [HideInInspector]
        float max = 0;
        public float Max
        {
            get { return max; }
            set
            {
                if (max != value)
                {
                    max = value;
                    SetRangeText();
                }
            }
        }

        /// <summary>
        /// Text displaying range
        /// </summary>
        public Text TxtRange { get; private set; }

        /// <summary>
        /// Event that is called when the content of input field is changed (string)
        /// </summary>
        /// <param name="val">Input field content</param>
        public delegate void ChangeTextEvent(string val);

        public event ChangeTextEvent OnChange;

        /// <summary>
        /// Event that is called when the content of input field is changed (integer)
        /// </summary>
        /// <param name="val">Input field content</param>
        public delegate void ChangeIntEvent(int val);

        public event ChangeIntEvent OnChangeInt;

        /// <summary>
        /// Event that is called when the content of input field is changed (float)
        /// </summary>
        /// <param name="val">Input field content</param>
        public delegate void ChangeFloatEvent(float val);

        public event ChangeFloatEvent OnChangeFloat;

        InputField inputField;
        RectTransform inputRt;
        bool isNum = false;

        protected override void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            base.Awake();

            // Get UI
            inputField = GetComponentInChildren<InputField>();
            inputRt = inputField.GetComponent<RectTransform>();
            foreach (var txt in GetComponentsInChildren<Text>())
            {
                if (txt.name.Contains("Range"))
                {
                    TxtRange = txt;
                    break;
                }
            }

            // Register input field event
            inputField.onEndEdit.AddListener((val) =>
            {
                Value = val;
            });
        }

        /// <summary>
        /// Intialize (string)
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="contentType">InputField input restriction</param>
        /// <param name="val">Initial value</param>
        /// <param name="onChange">Event that is called when InputField value is changed</param>
        /// <param name="width">InputField width</param>
        public void Init(string title, int relativeFontSize, float fixedTitleWidth,
            string val, ChangeTextEvent onChange, float width = 0)
        {
            base.Init(title, relativeFontSize, fixedTitleWidth);
            InitText(val, onChange, width);
        }

        /// <summary>
        /// Intialize (integer)
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <param name="val">Initial value</param>
        /// <param name="onChange">Event that is called when InputField value is changed</param>
        /// <param name="width">InputField width</param>
        public void Init(string title, int relativeFontSize, float fixedTitleWidth,
            int min, int max, int val,
            ChangeIntEvent onChange, float width = 0)
        {
            base.Init(title, relativeFontSize, fixedTitleWidth);

            InitNum(min, max);
            inputField.characterValidation = InputField.CharacterValidation.Integer;
            OnChangeInt = onChange;

            InitText(val.ToString(), null, width);
        }

        /// <summary>
        /// Intialize (float)
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="relativeFontSize">Title font size (relative)</param>
        /// <param name="fixedTitleWidth">Title fixed width</param>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <param name="val">Initial value</param>
        /// <param name="onChange">Event that is called when InputField value is changed</param>
        /// <param name="width">InputField width</param>
        public void Init(string title, int relativeFontSize, float fixedTitleWidth,
            float min, float max, float val,
            ChangeFloatEvent onChange, float width = 0)
        {
            base.Init(title, relativeFontSize, fixedTitleWidth);

            InitNum(min, max);
            inputField.characterValidation = InputField.CharacterValidation.Decimal;
            OnChangeFloat = onChange;

            InitText(val.ToString(), null, width);
        }

        /// <summary>
        /// Initialize (string)
        /// </summary>
        /// <param name="val">Initial value</param>
        /// <param name="onChange">Event that is called when InputField value is changed</param>
        /// <param name="width">InputField width</param>
        void InitText(string val, ChangeTextEvent onChange, float width = 0)
        {
            Value = val;
            OnChange = onChange;
            Width = width;

            // Remove range notation and extend vertically if not a number
            if (!isNum)
            {
                inputRt.sizeDelta = new Vector2(inputRt.sizeDelta.x,
                    inputRt.sizeDelta.y + TxtRange.preferredHeight);
                TxtRange.gameObject.SetActive(false);
            }

            // Calculate and set the overall size
            var rt = GetComponent<RectTransform>();
            var layout = GetComponent<HorizontalOrVerticalLayoutGroup>();
            float w = titleRt.sizeDelta.x + inputRt.sizeDelta.x + layout.spacing +
                layout.padding.left + layout.padding.right;
            rt.sizeDelta = new Vector2(w, rt.sizeDelta.y);
        }

        /// <summary>
        /// Numeric-related initialization
        /// </summary>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        void InitNum(float min, float max)
        {
            isNum = true;
            inputField.textComponent.alignment = TextAnchor.MiddleRight;
            Min = min;
            Max = max;
            SetRangeText();
        }

        /// <summary>
        /// Check range
        /// </summary>
        /// <param name="val">Input value</param>
        /// <returns>Within/Outside range</returns>
        bool CheckRange(string val)
        {
            // Do not check if not a number
            if (!isNum)
            {
                return true;
            }

            if (float.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float v))
            {
                return (Min <= v && v <= Max);
            }

            return false;
        }

        /// <summary>
        /// Update range display
        /// </summary>
        void SetRangeText()
        {
            TxtRange.text = $"{Min} ~ {Max}";
        }
    }
}
