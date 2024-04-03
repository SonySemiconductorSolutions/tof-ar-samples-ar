/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using System.IO;
using TofAr.V0;
using TofAr.V0.Hand;
using TofAr.V0.Tof;
using TofArSettings.UI;

namespace TofArSettings.Hand
{
    public class HandRecPlayerController : RecPlayerController
    {
        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Hand;
        public override bool HasDropdown => true;

        private bool isStreamActive = false;

        protected override void Start()
        {
            var fnames = new List<string>();
            fnames.Add("-");
            fnames.Add(RecPlayerSettings.fnamePlayBackEstimate_tof);
            FileNames = fnames.ToArray();

            recCtrl = FindObjectOfType<HandRecordController>();
            base.Start();
        }

        protected override void PlayPrep_internal()
        {
            isStreamActive = false;
            TofArHandManager.Instance?.PauseStream();
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

            var handMgr = TofArHandManager.Instance;
            if (!handMgr)
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

                handMgr.UnpauseStream();

                if (handMgr.IsStreamActive)
                {
                    isStreamActive = true;
                    handMgr.StopStream();
                }

                handMgr.StartPlayback();

                return true;
            }
            else
            {
                var file = $"{fileRoot}{Path.AltDirectorySeparatorChar}{fileName}";
                handMgr.PauseStream();
                handMgr.StartPlayback(file);

                return true;
            }
        }

        protected override void Pause_internal()
        {
            TofArHandManager.Instance?.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArHandManager.Instance?.UnpauseStream();
        }

        protected override void Stop_internal()
        {
            TofArHandManager.Instance?.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            if (isStreamActive)
            {
                TofArHandManager.Instance?.StartStream();
                TofArHandManager.Instance?.PauseStream();
            }

            TofArHandManager.Instance?.UnpauseStream();
        }

        protected override void OnEndRecord(bool result, string filePath)
        {
            base.OnEndRecord(result, filePath);
        }

        protected override string[] GetFileNames(string dirPath)
        {
            var options = GetFileNames(dirPath, TofArHandManager.StreamKeyTFLite);

            return options;
        }
    }
}
