/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TofAr.V0;
using TofAr.V0.Color;
using TofAr.V0.Tof;
using TofArSettings.Tof;
using LensFacing = TofAr.V0.Color.LensFacing;

namespace TofArSettings.Color
{
    public class ColorManagerController : CameraManagerController
    {
        private TofManagerController tofManagerController;

        public ResolutionProperty[] Resolutions { get; private set; }

        public ResolutionProperty CurrentResolution
        {
            get
            {
                return (Resolutions == null) ? null : Resolutions[Index];
            }
        }

        bool isProcessTexture = true;
        public bool IsProcessTexture
        {
            get { return isProcessTexture; }
            set
            {
                if (IsProcessTexture != value)
                {
                    isProcessTexture = value;
                    Apply(true);
                }
            }
        }

        ResolutionProperty defaultReso;

        protected void Awake()
        {
            tofManagerController = FindObjectOfType<TofManagerController>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            TofArColorManager.OnAvailableResolutionsChanged += MakeResoOptions;
            TofArColorManager.OnStreamStarted += OnColorStreamStarted;
            TofArColorManager.OnStreamStopped += OnColorStreamStopped;

            TofArManager.Instance?.postInternalSessionStart.AddListener(OnInternalSessionStarted);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            TofArColorManager.OnAvailableResolutionsChanged -= MakeResoOptions;
            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
            TofArColorManager.OnStreamStopped -= OnColorStreamStopped;

            TofArManager.Instance?.postInternalSessionStart.RemoveListener(OnInternalSessionStarted);
        }

        protected override void Start()
        {
            // Get CameraResolution list
            var props = TofArColorManager.Instance?.GetProperty<AvailableResolutionsProperty>();
            MakeResoOptions(props);

            base.Start();
        }

        protected override bool CheckIndexRange(int newIndex)
        {
            return (0 <= newIndex && newIndex < Resolutions.Length);
        }

        public override bool IsStreamActive()
        {
            var mgr = TofArColorManager.Instance;
            return (mgr && mgr.IsStreamActive);
        }

        protected override void StartStream()
        {
            base.StartStream();

            if (tofManagerController?.IsStreamActive() == true)
            {
                int tofIndex = tofManagerController.Index;
                tofManagerController.Index = 0;
                var conf = tofManagerController.Configs[tofIndex];
                var currentColor = Resolutions[Index];

                var mgr = TofArManager.Instance;
                var tofMgr = TofArTofManager.Instance;
                if (mgr && mgr.UsingIos)
                {
                    float ratioTof = (float)conf.width / (float)conf.height;
                    float ratioColor = (float)currentColor.width / (float)currentColor.height;
                    if (ratioTof != ratioColor || conf.cameraId != currentColor.cameraId)
                    {
                        tofMgr?.StopStream();
                        var resolutionsFiltered = tofManagerController.Configs.Where(x =>
                        {
                            ratioTof = (float)x.width / (float)x.height;
                            return ratioColor == ratioTof && x.cameraId == currentColor.cameraId;
                        });
                        if (resolutionsFiltered.Count() > 0)
                        {
                            conf = resolutionsFiltered.First();
                            tofIndex = tofManagerController.FindIndex(conf);
                        }
                    }
                }

                tofMgr?.StartStreamWithColor(
                    tofManagerController.Configs[tofIndex], Resolutions[index], tofManagerController.IsProcessTexture, IsProcessTexture);
            }
            else
            {
                TofArColorManager.Instance?.StartStream(Resolutions[Index], IsProcessTexture);
            }
        }

        protected override void StopStream()
        {
            base.StopStream();

            TofArColorManager.Instance?.StopStream();
        }

        protected override string GetApplyText()
        {
            if (Resolutions == null)
            {
                return string.Empty;
            }

            string text = MakeText(Resolutions[Index]);
            return $"Color mode {text} has been selected.";
        }

        /// <summary>
        /// Event that is called when Color stream is started
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        /// <param name="colorTexture">Color texture</param>
        void OnColorStreamStarted(object sender, Texture2D colorTexture)
        {
            var mgr = sender as TofArColorManager;
            if (!mgr)
            {
                return;
            }

            var prop = mgr.GetProperty<ResolutionProperty>();
            index = FindIndex(prop);

            OnStreamStarted();
        }

        /// <summary>
        /// Event that is called when Color stream is stopped
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        void OnColorStreamStopped(object sender)
        {
            OnStreamStopped();
        }

        /// <summary>
        /// Make list of CameraResolution options
        /// </summary>
        /// <param name="properties">CameraResolution list</param>
        void MakeResoOptions(AvailableResolutionsProperty properties)
        {
            defaultReso = TofArColorManager.Instance?.GetProperty<ResolutionProperty>();
            if (defaultReso == null)
            {
                return;
            }

            TofArManager.Logger.WriteLog(LogLevel.Debug, $"Defaulut Color Resolution - cameraId:{defaultReso.cameraId} width:{defaultReso.width} height:{defaultReso.height} frameRate:{defaultReso.frameRate}");

            var props = (properties == null) ? new List<ResolutionProperty>() :
                properties.resolutions.ToList();

            // Make option
            var propTexts = new List<string>();
            int defaultIndex = 0;
            var mgr = TofArManager.Instance;
            for (int i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                string text = MakeText(prop);

                // Use recommended values as intial values
                if (prop.cameraId == defaultReso.cameraId &&
                    prop.width == defaultReso.width &&
                    prop.height == defaultReso.height &&
                    prop.lensFacing == defaultReso.lensFacing)
                {
                    if (mgr && mgr.UsingIos)
                    {
                        if (prop.frameRate == defaultReso.frameRate)
                        {
                            defaultIndex = i;
                        }
                    }
                    else
                    {
                        defaultIndex = i;
                    }
                }

                propTexts.Add(text);
            }

            if (propTexts.Count > 0)
            {
                // Highlight recommended values in color and move to the top of the list
                string defaultText = $"<color=red>{propTexts[defaultIndex]}</color>";
                props.RemoveAt(defaultIndex);
                propTexts.RemoveAt(defaultIndex);
                props.Insert(0, defaultReso);
                propTexts.Insert(0, defaultText);
            }

            // Add an empty option at the top (for StopStream)
            var blank = new ResolutionProperty();
            props.Insert(0, blank);
            propTexts.Insert(0, "-");

            Resolutions = props.ToArray();
            Options = propTexts.ToArray();

            // If stream is already running, set to current config
            var colorMgr = TofArColorManager.Instance;
            if (colorMgr && colorMgr.IsStreamActive && props.Count > 1)
            {
                var prop = colorMgr.GetProperty<ResolutionProperty>();
                index = FindIndex(prop);
            }

            OnMadeOptions?.Invoke();
        }

        /// <summary>
        /// Find index
        /// </summary>
        /// <param name="prop">CameraResolution</param>
        /// <returns>CameraResolution index</returns>
        public int FindIndex(ResolutionProperty prop)
        {
            if (Resolutions == null)
            {
                return 0;
            }

            var mgr = TofArManager.Instance;
            if (!mgr)
            {
                return 0;
            }

            int pIndex = 0;
            for (int i = 0; i < Resolutions.Length; i++)
            {
                if (prop.cameraId == Resolutions[i].cameraId &&
                    prop.width == Resolutions[i].width &&
                    prop.height == Resolutions[i].height &&
                    prop.lensFacing == Resolutions[i].lensFacing)
                {
                    if (mgr.UsingIos)
                    {
                        if (prop.frameRate == Resolutions[i].frameRate &&
                            prop.enablePhoto == Resolutions[i].enablePhoto)
                        {
                            pIndex = i;
                            break;
                        }
                    }
                    else
                    {
                        pIndex = i;
                        break;
                    };
                }
            }

            return pIndex;
        }

        /// <summary>
        /// Make display text
        /// </summary>
        /// <param name="prop">CameraResolution</param>
        /// <returns>String</returns>
        string MakeText(ResolutionProperty prop)
        {
            var mgr = TofArManager.Instance;
            if (mgr && mgr.UsingIos)
            {
                var platformConfigProperty = mgr.GetProperty<PlatformConfigurationProperty>();
                if (platformConfigProperty?.platformConfigurationIos?.cameraApi == IosCameraApi.AvFoundation)
                {
                    if (prop.enablePhoto)
                    {
                        return $"{prop.cameraId} {(LensFacing)prop.lensFacing} {prop.width}x{prop.height} (Photo)";
                    }
                    else
                    {
                        return $"{prop.cameraId} {(LensFacing)prop.lensFacing} {prop.width}x{prop.height} ({(int)prop.frameRate}FPS)";
                    }
                }
            }

            return $"{prop.cameraId} {(LensFacing)prop.lensFacing} {prop.width}x{prop.height}";
        }

        private void OnInternalSessionStarted()
        {
            var props = TofArColorManager.Instance?.GetProperty<AvailableResolutionsProperty>();
            MakeResoOptions(props);
        }
    }
}
