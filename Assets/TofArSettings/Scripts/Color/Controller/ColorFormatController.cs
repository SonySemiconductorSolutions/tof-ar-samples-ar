/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using TofAr.V0.Color;
using UnityEngine;

namespace TofArSettings.Color
{
    public class ColorFormatController : ControllerBase
    {
        int index;
        public int Index
        {
            get { return index; }
            set
            {
                if (value != index && 0 <= value &&
                    value < FormatList.Length)
                {
                    index = value;

                    // Restart Color and apply Property
                    colorMgrCtrl.Restart();

                    OnChange?.Invoke(Index);
                }
            }
        }

        public ColorFormat Format
        {
            get { return FormatList != null ? FormatList[Index] : ColorFormat.YUV420; }
            set
            {
                if (value != Format && FormatList != null)
                {
                    Index = Utils.Find(value, FormatList);
                }
            }
        }

        public ColorFormat[] FormatList { get; private set; }
        public string[] FormatNames { get; private set; }

        public event ChangeIndexEvent OnChange;

        ColorManagerController colorMgrCtrl;

        protected void Awake()
        {
            colorMgrCtrl = GetComponent<ColorManagerController>();
        }

        void OnEnable()
        {
            TofArColorManager.OnStreamStarted += OnStreamStarted;
            colorMgrCtrl.OnRestart += SetProperty;
        }

        void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnStreamStarted;
            colorMgrCtrl.OnRestart -= SetProperty;
        }

        protected override void Start()
        {
            // Get ColorFormat list
            FormatList = (ColorFormat[])Enum.GetValues(typeof(ColorFormat));
            FormatNames = new string[FormatList.Length];
            for (int i = 0; i < FormatList.Length; i++)
            {
                FormatNames[i] = FormatList[i].ToString();
            }

            GetProperty();

            base.Start();
        }

        /// <summary>
        /// Event that is called when Color stream is started
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        /// <param name="colorTexture">Color texture</param>
        void OnStreamStarted(object sender, Texture2D colorTexture)
        {
            GetProperty();
        }

        /// <summary>
        /// Get ColorFormat property
        /// </summary>
        void GetProperty()
        {
            var prop = TofArColorManager.Instance.GetProperty<FormatConvertProperty>();
            Format = prop.format;
        }

        /// <summary>
        /// Apply ColorFormat property to TofArColorManager
        /// </summary>
        void SetProperty()
        {
            // Called during the restart of Color stream after stopping
            var prop = new FormatConvertProperty
            {
                format = Format
            };

            TofArColorManager.Instance.SetProperty<FormatConvertProperty>(prop);
        }
    }
}
