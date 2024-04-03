/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Threading;
using TofAr.V0.Color;
using UnityEngine;

namespace TofArSettings.Color
{
    public class ColorCapturePhotoController : ControllerBase
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
                    OnChange?.Invoke(Index);
                }
            }
        }

        public PhotoFormat Format
        {
            get { return FormatList != null ? FormatList[Index] : PhotoFormat.HEVC; }
            set
            {
                if (value != Format && FormatList != null)
                {
                    Index = Utils.Find(value, FormatList);
                }
            }
        }

        public PhotoFormat[] FormatList { get; private set; }
        public string[] FormatNames { get; private set; }

        public event ChangeIndexEvent OnChange;

        private SynchronizationContext context;

        void OnEnable()
        {
            TofArColorManager.OnPhotoFrameArrived += PhotoFrameArrived;
        }

        void OnDisable()
        {
            TofArColorManager.OnPhotoFrameArrived -= PhotoFrameArrived;
        }

        protected override void Start()
        {
            this.context = SynchronizationContext.Current;

            // Get ColorFormat list
            FormatList = (PhotoFormat[])Enum.GetValues(typeof(PhotoFormat));
            FormatNames = new string[FormatList.Length];
            for (int i = 0; i < FormatList.Length; i++)
            {
                FormatNames[i] = FormatList[i].ToString();
            }

            base.Start();
        }

        /// <summary>
        /// Set CapturePhoto property
        /// </summary>
        public void SetProperty()
        {
            var capturePhotoProperty = new CapturePhotoProperty();
            capturePhotoProperty.format = FormatList[index];

            TofArColorManager.Instance.SetProperty(capturePhotoProperty);
        }

        /// <summary>
        /// Get Take Photo frame Event
        /// </summary>
        /// <param name="sender"></param>
        public void PhotoFrameArrived(object sender)
        {
            context.Post((s) =>
            {
                string dataPath = Application.persistentDataPath + "/photodata/";
                string timeStamp = DateTime.Now.ToString("yyyyMMdd-HHmmssfff");

                if (!System.IO.Directory.Exists(dataPath))
                {
                    System.IO.Directory.CreateDirectory(dataPath);
                }

                System.IO.Directory.CreateDirectory(dataPath + timeStamp);

                string photoColorDataFileName = "/photo_color" + GetFormatExtension();
                System.IO.File.WriteAllBytes(dataPath + timeStamp + photoColorDataFileName, TofArColorManager.Instance.PhotoColorData.Data);
                if (TofArColorManager.Instance.PhotoDepthData != null)
                {
                    var depthData = TofArColorManager.Instance.PhotoDepthData;
                    string photoDepthDataFileName = $"/photo_depth_{depthData.Width}x{depthData.Height}.bytes";
                    System.IO.File.WriteAllBytes(dataPath + timeStamp + photoDepthDataFileName, TofArColorManager.Instance.PhotoDepthData.Data);
                }
            }, null);
        }

        private string GetFormatExtension()
        {
            switch (FormatList[index])
            {
                case PhotoFormat.HEVC:
                    return ".heic";
                case PhotoFormat.JPEG:
                    return ".jpeg";
            }

            return ".jpeg";
        }
    }
}
