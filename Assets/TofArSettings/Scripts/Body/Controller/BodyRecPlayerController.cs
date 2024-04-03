/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using System.IO;
using TofAr.V0;
using TofAr.V0.Body;
using TofAr.V0.Tof;
using TofArSettings.UI;

namespace TofArSettings.Body
{
    public class BodyRecPlayerController : RecPlayerController
    {
        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Body;
        public override bool HasDropdown => true;

        private bool isStreamActive = false;

        protected override void Start()
        {
            var fnames = new List<string>();
            fnames.Add("-");
            fnames.Add(RecPlayerSettings.fnamePlayBackEstimate_tof);
            FileNames = fnames.ToArray();

            recCtrl = FindObjectOfType<BodyRecordController>();
            base.Start();
        }

        protected override void PlayPrep_internal()
        {
            TofArBodyManager.Instance?.PauseStream();
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

            var bodyMgr = TofArBodyManager.Instance;
            if (!bodyMgr)
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

                bodyMgr.UnpauseStream();

                if (bodyMgr.IsStreamActive)
                {
                    isStreamActive = true;
                    bodyMgr.StopStream();
                }

                bodyMgr.StartPlayback();

                return true;
            }
            else
            {

                var file = $"{fileRoot}{Path.AltDirectorySeparatorChar}{fileName}";
                bodyMgr.PauseStream();
                bodyMgr.StartPlayback(file);

                return true;
            }
        }

        protected override void Pause_internal()
        {
            TofArBodyManager.Instance?.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArBodyManager.Instance?.UnpauseStream();
        }

        protected override void Stop_internal()
        {
            TofArBodyManager.Instance?.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            if (isStreamActive)
            {
                TofArBodyManager.Instance?.StartStream();
                TofArBodyManager.Instance?.PauseStream();
            }

            TofArBodyManager.Instance?.UnpauseStream();
        }

        protected override void OnEndRecord(bool result, string filePath)
        {
            base.OnEndRecord(result, filePath);
        }

        protected override string[] GetFileNames(string dirPath)
        {
            var options = GetFileNames(dirPath, TofArBodyManager.StreamKey);

            return options;
        }
    }
}
