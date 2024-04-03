/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using System.Linq;
using TofAr.V0;
using TofAr.V0.Tof;
using TofArSettings.Color;
using UnityEngine;

namespace TofArSettings.Tof
{
    public class TofManagerController : CameraManagerController
    {
        private ColorManagerController colorManagerController;

        public CameraConfigurationProperty[] Configs { get; private set; }

        public CameraConfigurationProperty CurrentConfig
        {
            get
            {
                return (Configs == null) ? null : Configs[Index];
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

        Camera2DefaultConfigurationProperty defaultConf;

        protected void Awake()
        {
            colorManagerController = FindObjectOfType<ColorManagerController>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            TofArTofManager.OnAvailableConfigurationsChanged += MakeConfigOptions;
            TofArTofManager.OnStreamStarted += OnTofStreamStarted;
            TofArTofManager.OnStreamStopped += OnTofStreamStopped;

            TofArManager.Instance?.postInternalSessionStart.AddListener(OnInternalSessionStarted);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            TofArTofManager.OnAvailableConfigurationsChanged -= MakeConfigOptions;
            TofArTofManager.OnStreamStarted -= OnTofStreamStarted;
            TofArTofManager.OnStreamStopped -= OnTofStreamStopped;

            TofArManager.Instance?.postInternalSessionStart.RemoveListener(OnInternalSessionStarted);
        }

        protected override void Start()
        {
            var mgr = TofArTofManager.Instance;
            isProcessTexture = (mgr && mgr.ProcessTexture);

            // Get CameraConfig list
            var props = mgr?.GetProperty<CameraConfigurationsProperty>();
            MakeConfigOptions(props);

            base.Start();
        }

        protected override bool CheckIndexRange(int newIndex)
        {
            return (0 <= newIndex && newIndex < Configs.Length);
        }

        public override bool IsStreamActive()
        {
            var mgr = TofArTofManager.Instance;
            return (mgr && mgr.IsStreamActive);
        }

        protected override void StartStream()
        {
            base.StartStream();

            if (colorManagerController?.IsStreamActive() == true)
            {
                int colorIndex = colorManagerController.Index;
                colorManagerController.Index = 0;
                var conf = Configs[index];
                var currentColor = colorManagerController.Resolutions[colorIndex];
                var mgr = TofArManager.Instance;
                if (mgr && mgr.UsingIos)
                {
                    float ratioTof = (float)conf.width / (float)conf.height;
                    float ratioColor = (float)currentColor.width / (float)currentColor.height;
                    if (ratioTof != ratioColor || conf.cameraId != currentColor.cameraId)
                    {
                        var resolutionsFiltered = colorManagerController.Resolutions.Where(x =>
                        {
                            ratioColor = (float)x.width / (float)x.height;
                            return ratioColor == ratioTof && x.cameraId == conf.cameraId;
                        });
                        if (resolutionsFiltered.Count() > 0)
                        {
                            currentColor = resolutionsFiltered.First();
                            colorIndex = colorManagerController.FindIndex(currentColor);
                        }
                    }
                }

                TofArTofManager.Instance?.StartStreamWithColor(Configs[index], colorManagerController.Resolutions[colorIndex], isProcessTexture, colorManagerController.IsProcessTexture);
            }
            else
            {
                TofArTofManager.Instance?.StartStream(Configs[Index],
                    isProcessTexture);
            }
        }

        protected override void StopStream()
        {
            base.StopStream();

            TofArTofManager.Instance?.StopStream();
        }

        protected override string GetApplyText()
        {
            return (Configs == null) ? string.Empty :
                $"ToF mode {Configs[Index].name} has been selected.";
        }

        /// <summary>
        /// Event that is called when Tof stream is started
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        /// <param name="depthTexture">Depth texture</param>
        /// <param name="confidenceTexture">Confidence texture</param>
        /// <param name="pointCloudData">PointCloud data</param>
        void OnTofStreamStarted(object sender, Texture2D depthTexture,
            Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            var mgr = sender as TofArTofManager;
            if (!mgr)
            {
                return;
            }

            var prop = mgr.GetProperty<CameraConfigurationProperty>();
            index = FindIndex(prop);

            OnStreamStarted();
        }

        /// <summary>
        /// Event that is called when Tof stream is stopped
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        void OnTofStreamStopped(object sender)
        {
            OnStreamStopped();
        }

        /// <summary>
        /// Make CameraConfig option list
        /// </summary>
        /// <param name="properties">CameraConfig list</param>
        void MakeConfigOptions(CameraConfigurationsProperty properties)
        {
            defaultConf = TofArTofManager.Instance?.GetProperty<Camera2DefaultConfigurationProperty>();
            if (defaultConf != null)
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, $"Defaulut ToF Configuration - uid:{defaultConf.uid} cameraId:{defaultConf.cameraId} width:{defaultConf.width} height:{defaultConf.height} frameRate:{defaultConf.frameRate}");
            }

            var props = (properties == null) ? new List<CameraConfigurationProperty>() :
                properties.configurations.ToList();

            // Make options
            var propTexts = new List<string>();
            int defaultIndex = 0;
            if (defaultConf != null)
            {
                var mgr = TofArManager.Instance;
                for (int i = 0; i < props.Count; i++)
                {
                    var prop = props[i];
                    string text = MakeText(prop);

                    // Use recommended values as initial values
                    if (prop.cameraId == defaultConf.cameraId &&
                        prop.width == defaultConf.width &&
                        prop.height == defaultConf.height &&
                        prop.lensFacing == defaultConf.lensFacing)
                    {
                        if (mgr && mgr.UsingIos)
                        {
                            if (prop.frameRate == defaultConf.frameRate)
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
            }

            if (propTexts.Count > 0)
            {
                // Highlight recommended values in color and move to the top of the list
                string defaultText = $"<color=red>{propTexts[defaultIndex]}</color>";
                props.RemoveAt(defaultIndex);
                propTexts.RemoveAt(defaultIndex);
                props.Insert(0, defaultConf);
                propTexts.Insert(0, defaultText);
            }

            // Add empty option at the top (for StopStream)
            var blank = new CameraConfigurationProperty
            {
                uid = int.MinValue
            };

            props.Insert(0, blank);
            propTexts.Insert(0, "-");

            Configs = props.ToArray();
            Options = propTexts.ToArray();

            // If stream is already running, set to current config
            var tofMgr = TofArTofManager.Instance;
            if (tofMgr && tofMgr.IsStreamActive && props.Count > 1)
            {
                var prop = tofMgr.GetProperty<CameraConfigurationProperty>();
                index = FindIndex(prop);
            }

            OnMadeOptions?.Invoke();
        }

        /// <summary>
        /// Find index
        /// </summary>
        /// <param name="prop">CameraConfig</param>
        /// <returns>CameraConfig index</returns>
        public int FindIndex(CameraConfigurationProperty prop)
        {
            if (Configs == null)
            {
                return 0;
            }

            var mgr = TofArManager.Instance;
            if (!mgr)
            {
                return 0;
            }

            int pIndex = 0;
            for (int i = 0; i < Configs.Length; i++)
            {
                if (prop.cameraId == Configs[i].cameraId &&
                    prop.width == Configs[i].width &&
                    prop.height == Configs[i].height &&
                    prop.lensFacing == Configs[i].lensFacing)
                {
                    if (mgr.UsingIos)
                    {
                        if (prop.frameRate == Configs[i].frameRate)
                        {
                            pIndex = i;
                            break;
                        }
                    }
                    else
                    {
                        pIndex = i;
                        break;
                    }
                }
            }

            return pIndex;
        }

        /// <summary>
        /// Make display text
        /// </summary>
        /// <param name="prop">CameraConfig</param>
        /// <returns>Text</returns>
        string MakeText(CameraConfigurationProperty prop)
        {
            var mgr = TofArManager.Instance;
            if (mgr && mgr.UsingIos)
            {
                var platformConfigProperty = mgr.GetProperty<PlatformConfigurationProperty>();
                if (platformConfigProperty?.platformConfigurationIos?.cameraApi == IosCameraApi.AvFoundation)
                {
                    return $"{prop.cameraId} {(LensFacing)prop.lensFacing} {prop.width}x{prop.height} ({(int)prop.frameRate}FPS)";
                }
            }

            return $"{prop.cameraId} {(LensFacing)prop.lensFacing} {prop.name} {prop.width}x{prop.height}";
        }

        private void OnInternalSessionStarted()
        {
            var props = TofArTofManager.Instance?.GetProperty<CameraConfigurationsProperty>();
            MakeConfigOptions(props);
        }
    }
}
