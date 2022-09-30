/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */
using TofAr.V0.Color;
using UnityEngine;
using System;

namespace TofArSettings.Color
{
    public class ColorExposureController : ImageExposureController
    {
        long frameDuration;
        public long FrameDuration
        {
            get { return frameDuration; }
            set
            {
                if (frameDuration != value && FrameDurationMin <= value && value <= FrameDurationMax)
                {
                    frameDuration = value;
                    Apply();

                    OnChangeFrameDuration?.Invoke(FrameDuration);
                }
            }
        }

        public long FrameDurationMin { get; private set; }
        public long FrameDurationMax { get; private set; }
        public const long FrameDurationStep = 1;

        int sensitivity;
        public int Sensitivity
        {
            get { return sensitivity; }
            set
            {
                if (sensitivity != value && SensitivityMin <= value && value <= SensitivityMax)
                {
                    sensitivity = value;
                    Apply();

                    OnChangeSensitivity?.Invoke(Sensitivity);
                }
            }
        }


        public int SensitivityMin { get; private set; }
        public int SensitivityMax { get; private set; }
        public const int SensitivityStep = 1;

        int flashIndex;
        public int FlashIndex
        {
            get { return flashIndex; }
            set
            {
                if (value != flashIndex && 0 <= value &&
                    value < FlashList.Length)
                {
                    flashIndex = value;
                    Apply();

                    OnChangeFlashMode?.Invoke(FlashIndex);
                }
            }
        }

        FlashMode Flash
        {
            get { return FlashList[FlashIndex]; }
            set
            {
                if (value != Flash)
                {
                    FlashIndex = Utils.Find(value, FlashList);
                }
            }
        }

        public FlashMode[] FlashList { get; private set; }

        public string[] FlashNames { get; private set; }


        public event ChangeLongEvent OnChangeFrameDuration;

        public event ChangeIndexEvent OnChangeSensitivity;

        public event ChangeIndexEvent OnChangeFlashMode;

        private const long scaleExposure = 1000;
        private const long scaleFrameDuration = 1000;

        protected override void OnEnable()
        {
            base.OnEnable();
            TofArColorManager.OnStreamStarted += OnStreamStarted;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TofArColorManager.OnStreamStarted -= OnStreamStarted;
        }

        protected override void Start()
        {
            // Get FlashMode list
            var flashList = (FlashMode[])Enum.GetValues(typeof(FlashMode));

            // Remove Single from the list when using iPhone as it it not configurable
            bool isIPhone = (Application.platform == RuntimePlatform.IPhonePlayer);
            int length = (isIPhone) ? flashList.Length - 1 : flashList.Length;

            FlashList = new FlashMode[length];
            FlashNames = new string[length];

            int index = 0;
            for (int i = 0; i < flashList.Length; i++)
            {
                var flash = flashList[i];
                if (isIPhone && flash == FlashMode.Single)
                {
                    continue;
                }

                FlashList[index] = flash;
                FlashNames[index] = flash.ToString();

                index++;
            }

            base.Start();
        }

        /// <summary>
        /// Event that is called when Color stream is started
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        /// <param name="colorTexture"></param>
        void OnStreamStarted(object sender, Texture2D colorTexture)
        {
            GetProperty();
        }

        protected override void GetProperty()
        {
            base.GetProperty();

            var prop = TofArColorManager.Instance.GetProperty<ExposureModeProperty>();
            bool isChange = (autoExposure != prop.autoExposure ||
                exposureTime != prop.exposureTime ||
                TimeMin != prop.minExposureTime || TimeMax != prop.maxExposureTime ||
                frameDuration != prop.frameDurarion ||
                FrameDurationMin != prop.minFrameDurationForCurrentResolution ||
                FrameDurationMax != prop.maxFrameDuration ||
                sensitivity != prop.sensitibity ||
                SensitivityMin != prop.minSensitivity ||
                SensitivityMax != prop.maxSensitivity ||
                Flash != prop.flash);

            autoExposure = prop.autoExposure;

            // Carry forward the unit of measure as the value of ExposureTime is large 
            TimeMin = (long)Mathf.Ceil(prop.minExposureTime / (float)scaleExposure);
            TimeMax = (long)Mathf.Floor(prop.maxExposureTime / (float)scaleExposure);
            exposureTime = prop.exposureTime / scaleExposure;
            if (exposureTime < TimeMin)
            {
                exposureTime = TimeMin;
            }
            else if (TimeMax < exposureTime)
            {
                exposureTime = TimeMax;
            }

            

            FrameDurationMin = (long)Mathf.Ceil(prop.minFrameDurationForCurrentResolution / (float)scaleFrameDuration);
            FrameDurationMax = (long)Mathf.Floor(prop.maxFrameDuration / (float)scaleFrameDuration);
            frameDuration = prop.frameDurarion / scaleFrameDuration;
            if (frameDuration < FrameDurationMin)
            {
                frameDuration = FrameDurationMin;
            }
            else if (FrameDurationMax < frameDuration)
            {
                frameDuration = FrameDurationMax;
            }

            SensitivityMin = prop.minSensitivity;
            SensitivityMax = prop.maxSensitivity;
            sensitivity = prop.sensitibity;
            if (sensitivity < SensitivityMin)
            {
                sensitivity = SensitivityMin;
            }
            else if (SensitivityMax < sensitivity)
            {
                sensitivity = SensitivityMax;
            }

            Flash = prop.flash;

            // Notify when properties are changed
            if (isChange)
            {
                OnChangeProperty?.Invoke();
            }
        }

        protected override void Apply()
        {
            var prop = new ExposureModeProperty
            {
                autoExposure = AutoExposure,
                // Undo the unit of the measure that has been carried forward
                exposureTime = ExposureTime * scaleExposure,
                frameDurarion = FrameDuration * scaleFrameDuration,
                sensitibity = Sensitivity,
                flash = Flash
            };

            try
            {
                TofArColorManager.Instance.SetProperty(prop);
            }
            catch (SensCord.ApiException e)
            {
                TofAr.V0.TofArManager.Logger.WriteLog(TofAr.V0.LogLevel.Debug, $"Failed to set exposure mode. {e.Message}");
            }
            base.Apply();
        }
    }
}
