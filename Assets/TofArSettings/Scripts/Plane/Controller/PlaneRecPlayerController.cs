/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using System.IO;
using TofAr.V0;
using TofAr.V0.Plane;
using TofAr.V0.Tof;
using TofArSettings.UI;
using UnityEngine;

namespace TofArSettings.Plane
{
    public class PlaneRecPlayerController : RecPlayerController
    {
        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Plane;
        public override bool HasDropdown => true;

        private bool isStreamActive = false;

        protected override void Start()
        {
            var fnames = new List<string>();
            fnames.Add("-");
            fnames.Add(RecPlayerSettings.fnamePlayBackEstimate_tof);
            FileNames = fnames.ToArray();

            recCtrl = FindObjectOfType<PlaneRecordController>();
            base.Start();
        }

        protected override void PlayPrep_internal()
        {
            TofArPlaneManager.Instance.PauseStream();
        }

        protected override bool Play_internal(string fileName)
        {
            var directoryListProp = TofArManager.Instance?.GetProperty<DirectoryListProperty>();
            if (directoryListProp == null)
            {
                return false;
            }

            string fileRoot = directoryListProp.path;
            if (fileRoot.Length <= 0)
            {
                return false;
            }

            var planeMgr = TofArPlaneManager.Instance;
            if (!planeMgr)
            {
                return false;
            }

            if (fileName.Equals(RecPlayerSettings.fnamePlayBackEstimate_tof))
            {
                var tofMgr = TofArTofManager.Instance;
                if (!tofMgr)
                {
                    return false;
                }

                if (!TofArTofManager.Instance.IsPlaying)
                {
                    return false;
                }

                planeMgr.UnpauseStream();

                if (planeMgr.IsStreamActive)
                {
                    isStreamActive = true;
                    planeMgr.StopStream();
                }

                planeMgr.StartPlayback();

                return true;
            }
            else
            {
                var file = $"{fileRoot}{Path.AltDirectorySeparatorChar}{fileName}";
                planeMgr.PauseStream();
                planeMgr.StartPlayback(file);
                return true;
            }
        }

        protected override void Pause_internal()
        {
            TofArPlaneManager.Instance?.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArPlaneManager.Instance?.UnpauseStream();
        }

        protected override void Stop_internal()
        {
            TofArPlaneManager.Instance?.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            if (isStreamActive)
            {
                TofArPlaneManager.Instance?.StartStream();
                TofArPlaneManager.Instance?.PauseStream();
            }

            TofArPlaneManager.Instance?.UnpauseStream();
        }

        protected override void OnEndRecord(bool result, string filePath)
        {
            base.OnEndRecord(result, filePath);
        }

        protected override string[] GetFileNames(string dirPath)
        {
            var options = GetFileNames(dirPath, TofArPlaneManager.StreamKey);

            return options;
        }
    }
}

