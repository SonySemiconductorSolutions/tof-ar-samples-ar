/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Linq;
using TofAr.V0;
using TofAr.V0.Slam;
using UnityEngine;

namespace TofArSettings.Slam
{
    public class SlamManagerController : ControllerBase
    {
        private void Awake()
        {
            var ctrl = FindAnyObjectByType<General.CameraApiController>();
            ctrl.OnChangeApi += (idx) =>
            {
                if (ctrl.CameraApi == TofAr.V0.IosCameraApi.AvFoundation)
                {
                    lastPoseSource = this.CameraPoseSource;
                    this.CameraPoseSource = CameraPoseSource.Gyro;
                }
                else
                {
                    this.CameraPoseSource = lastPoseSource;
                }
            };

            var mgr = TofAr.V0.TofArManager.Instance;
            if (mgr && mgr.UsingIos && ctrl.CameraApi == TofAr.V0.IosCameraApi.AvFoundation)
            {
                this.CameraPoseSource = CameraPoseSource.Gyro;
            }

            SetupPoseSourceLists();
            SetupRotationCalculateTypeLists();
        }

        protected override void Start()
        {
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

            // change back to gyro when debugging
            var mgr = TofAr.V0.TofArManager.Instance;
            if (!mgr)
            {
                return;
            }

            if ((mgr.UsingIos && !mgr.RuntimeSettings.isRemoteServer) ||
                mgr.RuntimeSettings.runMode == TofAr.V0.RunMode.MultiNode)
            {
                StopStream();
                CameraPoseSource = CameraPoseSource.Gyro;
                StartStream();
            }

        }

        private void OnSlamStreamStarted(object sender)
        {
            OnStreamStartStatusChanged?.Invoke(true);

            var mgr = TofArSlamManager.Instance;
            if (!mgr)
            {
                return;
            }

            int idx = Utils.Find(mgr.CameraPoseSource, PoseSources);
            if (idx != index)
            {
                Index = idx;
            }
        }

        private void OnSlamStreamStopped(object sender)
        {
            OnStreamStartStatusChanged?.Invoke(false);
        }

        public bool IsStreamActive()
        {
            var mgr = TofArSlamManager.Instance;
            return (mgr && mgr.IsStreamActive);
        }

        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            TofArSlamManager.Instance?.StartStream();
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        public void StopStream()
        {
            TofArSlamManager.Instance?.StopStream();
        }

        public event ChangeToggleEvent OnStreamStartStatusChanged;

        public CameraPoseSource CameraPoseSource
        {
            get
            {
                var mgr = TofArSlamManager.Instance;
                return (mgr) ? mgr.CameraPoseSource : CameraPoseSource.Gyro;
            }

            set
            {
                var mgr = TofArSlamManager.Instance;
                if (mgr && value != mgr.CameraPoseSource)
                {
                    mgr.CameraPoseSource = value;
                    Index = Utils.Find(value, PoseSources);
                }
            }
        }

        private CameraPoseSource lastPoseSource;

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

        public event ChangeIndexEvent OnChangeIndex, OnChangeRotationCalculateType;

        private void SetupPoseSourceLists()
        {
            var mgr = TofAr.V0.TofArManager.Instance;
            if (!mgr)
            {
                return;
            }

            PoseSources = ((CameraPoseSource[])Enum.GetValues(typeof(CameraPoseSource)));
            if (!mgr.UsingIos)
            {
                var internalTracker = Resources.Load("Prefabs/KPoseTracker");
                var internalTracker2 = Resources.Load("Prefabs/SdsPoseTracker");

                var poseSources = PoseSources.Distinct();
                if (internalTracker == null)
                {
                    poseSources = poseSources.Where(x => x != CameraPoseSource.InternalEngine).ToArray();
                }

                if (internalTracker2 == null)
                {
                    poseSources = poseSources.Where(x => x != CameraPoseSource.InternalEngine02).ToArray();
                }

                var platformConfig = TofArManager.Instance.GetProperty<PlatformConfigurationProperty>();
                if (platformConfig?.platformConfigurationPC?.customData?.Contains("k4a") == true)
                {
                    poseSources = poseSources.Where(x => x != CameraPoseSource.ARKitOrARFoundation).ToArray();
                }

                PoseSources = poseSources.ToArray();
            }
            else
            {
                PoseSources = PoseSources.Distinct().Where(x => x != CameraPoseSource.InternalEngine && x != CameraPoseSource.InternalEngine02).ToArray();
            }

            PoseSourceNames = PoseSources.Select((s) => s.ToString()).ToArray();
        }

        private int rotationCalculateTypeIndex = 0;
        public int RotationCalculateTypeIndex
        {
            get { return rotationCalculateTypeIndex; }
            set
            {
                if (value != rotationCalculateTypeIndex && 0 <= value &&
                    value < RotationCalculateTypeList.Length)
                {
                    RotationCalculateType = RotationCalculateTypeList[value];
                }
            }
        }

        public RotationCalculateType RotationCalculateType
        {
            get
            {
                return TofArSlamManager.Instance.RotationCalculateType;
            }
            set
            {
                if (value != TofArSlamManager.Instance.RotationCalculateType)
                {
                    rotationCalculateTypeIndex = Utils.Find(value, RotationCalculateTypeList);
                    TofArSlamManager.Instance.RotationCalculateType = value;
                    OnChangeRotationCalculateType?.Invoke(RotationCalculateTypeIndex);
                }
            }
        }

        public RotationCalculateType[] RotationCalculateTypeList { get; private set; }

        string[] rotationCalculateTypeListNames = new string[0];
        public string[] RotationCalculateTypeNames
        {
            get { return rotationCalculateTypeListNames; }
        }

        void SetupRotationCalculateTypeLists()
        {
            // Get RotationCalculateType list
            RotationCalculateTypeList = (RotationCalculateType[])Enum.GetValues(typeof(RotationCalculateType));
            if (RotationCalculateTypeList.Length != rotationCalculateTypeListNames.Length)
            {
                Array.Resize(ref rotationCalculateTypeListNames, RotationCalculateTypeList.Length);
            }

            for (int i = 0; i < RotationCalculateTypeList.Length; i++)
            {
                rotationCalculateTypeListNames[i] = RotationCalculateTypeList[i].ToString();
            }

            // Get intial values of RotationCalculateType
            if (TofArSlamManager.Instantiated)
            {
                rotationCalculateTypeIndex = Utils.Find(TofArSlamManager.Instance.RotationCalculateType, RotationCalculateTypeList);
                if (rotationCalculateTypeIndex < 0)
                {
                    rotationCalculateTypeIndex = 0;
                }
            }
        }
    }
}
