/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.IO;
using TofAr.V0;
using TofAr.V0.Tof;
using TofArSettings.UI;

namespace TofArSettings.Tof
{
    public class TofRecPlayerController : RecPlayerController
    {
        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Tof;
        public override bool HasDropdown => true;
        public override bool IsPriority => true;

        protected override void Start()
        {
            recCtrl = FindObjectOfType<TofRecordController>();
            base.Start();
        }

        protected override void PlayPrep_internal()
        {
            TofArTofManager.Instance?.PauseStream();
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

            var tofMgr = TofArTofManager.Instance;
            if (!tofMgr)
            {
                return false;
            }

            var file = $"{fileRoot}{Path.AltDirectorySeparatorChar}{fileName}";
            tofMgr.PauseStream();
            tofMgr.StartPlayback(file);

            return true;
        }

        protected override void Pause_internal()
        {
            TofArTofManager.Instance?.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArTofManager.Instance?.UnpauseStream();
        }

        protected override void Stop_internal()
        {
            TofArTofManager.Instance?.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            TofArTofManager.Instance?.UnpauseStream();
        }

        protected override void OnEndRecord(bool result, string filePath)
        {
            base.OnEndRecord(result, filePath);
        }

        protected override string[] GetFileNames(string dirPath)
        {
            var options = GetFileNames(dirPath, TofArTofManager.StreamKey);

            return options;
        }
    }
}
