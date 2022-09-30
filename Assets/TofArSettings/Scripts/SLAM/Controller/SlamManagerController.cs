/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Slam;
using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace TofArSettings.Slam
{
    public class SlamManagerController : ControllerBase
    {
        private void Awake()
        {
            isStarted = TofArSlamManager.Instance.autoStart;

            SetupPoseSourceLists();
        }

        protected override void Start()
        {
            var defaultCameraPoseSource = (TofAr.V0.TofArManager.Instance.UsingIos || TofAr.V0.TofArManager.Instance.RuntimeSettings.runMode == TofAr.V0.RunMode.MultiNode) ? 
                CameraPoseSource.MainCamera : CameraPoseSource.Gyro;

            CameraPoseSource = defaultCameraPoseSource;

            base.Start();
        }

        private void OnEnable()
        {
            TofArSlamManager.OnStreamStarted += OnSlamStreamStarted;
            TofArSlamManager.OnStreamStopped += OnSlamStreamStopped;
        }

        private void OnDisable()
        {
            TofArSlamManager.OnStreamStopped -= OnSlamStreamStopped;
            TofArSlamManager.OnStreamStarted -= OnSlamStreamStarted;
        }

        private void OnSlamStreamStarted(object sender)
        {
            isStarted = true;
            OnStreamStartStatusChanged?.Invoke(isStarted);
        }

        private void OnSlamStreamStopped(object sender)
        {
            isStarted = false;
            OnStreamStartStatusChanged?.Invoke(isStarted);
        }

        public bool IsStreamActive()
        {
            return TofArSlamManager.Instance.IsStreamActive;
        }

        private bool isStarted = false;

        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            TofArSlamManager.Instance.StartStream();
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        public void StopStream()
        {
            TofArSlamManager.Instance.StopStream();
        }

        public event ChangeToggleEvent OnStreamStartStatusChanged;

        public CameraPoseSource CameraPoseSource
        {
            get => TofArSlamManager.Instance.CameraPoseSource;
            set
            {
                if (value != TofArSlamManager.Instance.CameraPoseSource)
                {
                    TofArSlamManager.Instance.CameraPoseSource = value;
                    Index = Utils.Find(value, PoseSources);
                }
            }
        }

        private int index = 0;
        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                if (value != Index &&
                    0 <= value && value < PoseSources.Length)
                {
                    index = value;
                    CameraPoseSource = PoseSources[index];
                    OnChangeIndex?.Invoke(Index);
                }
            }
        }

        public string[] PoseSourceNames { get; private set; }

        public CameraPoseSource[] PoseSources { get; private set; }

        public event ChangeIndexEvent OnChangeIndex;

        private void SetupPoseSourceLists()
        {
            PoseSources = ((CameraPoseSource[])Enum.GetValues(typeof(CameraPoseSource)));
            if (!TofAr.V0.TofArManager.Instance.UsingIos)
            {
                var internalTracker = Resources.Load("Prefabs/KudanPoseTracker");
                var internalTracker2 = Resources.Load("Prefabs/SdsPoseTracker");

                var poseSources = PoseSources;
                if (internalTracker == null)
                {
                    poseSources = poseSources.Where(x => x != CameraPoseSource.InternalEngine).ToArray(); 
                }

                if (internalTracker2 == null)
                {
                    poseSources = poseSources.Where(x => x != CameraPoseSource.InternalEngine02).ToArray();
                }

                PoseSources = poseSources.ToArray();
            }
            else
            {
                PoseSources = PoseSources.Where(x => x != CameraPoseSource.InternalEngine && x != CameraPoseSource.InternalEngine02).ToArray();
            }
            PoseSourceNames = PoseSources.Select((s) => s.ToString()).ToArray();
        }

    }
}
