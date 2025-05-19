/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using System.IO;
using TofAr.V0;
using TofAr.V0.Color;
using TofArSettings.UI;

namespace TofArSettings.Color
{
    public class ColorRecPlayerController : RecPlayerController
    {
        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Color;
        public override bool HasDropdown => true;
        public override bool IsPriority => true;


        protected override void Start()
        {
            recCtrl = FindAnyObjectByType<ColorRecordController>();
            base.Start();
        }

        protected override void PlayPrep_internal()
        {
            TofArColorManager.Instance?.PauseStream();
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

            var colorMgr = TofArColorManager.Instance;
            if (!colorMgr)
            {
                return false;
            }

            var file = $"{fileRoot}{Path.AltDirectorySeparatorChar}{fileName}";
            colorMgr.PauseStream();
            colorMgr.StartPlayback(file);

            return true;
        }

        protected override void Pause_internal()
        {
            TofArColorManager.Instance?.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArColorManager.Instance?.UnpauseStream();
        }

        protected override void Stop_internal()
        {
            TofArColorManager.Instance?.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            TofArColorManager.Instance?.UnpauseStream();
        }

        protected override void OnEndRecord(bool result, string filePath)
        {
            base.OnEndRecord(result, filePath);
        }

        protected override string[] GetFileNames(string dirPath)
        {
            var options = GetFileNames(dirPath, TofArColorManager.StreamKey);

            return options;
        }
    }
}
