/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using System.IO;
using TofAr.V0;
using TofAr.V0.Face;
using TofArSettings.UI;

namespace TofArSettings.Face
{
    public class FaceRecPlayerController : RecPlayerController
    {
        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Face;
        public override bool HasDropdown => true;

        protected override void Start()
        {
            recCtrl = FindAnyObjectByType<FaceRecordController>();
            base.Start();
        }

        protected override void PlayPrep_internal()
        {
            TofArFaceManager.Instance?.PauseStream();
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

            var faceMgr = TofArFaceManager.Instance;
            if (!faceMgr)
            {
                return false;
            }

            var file = $"{fileRoot}{Path.AltDirectorySeparatorChar}{fileName}";
            faceMgr.PauseStream();
            faceMgr.StartPlayback(file);

            return true;
        }

        protected override void Pause_internal()
        {
            TofArFaceManager.Instance?.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArFaceManager.Instance?.UnpauseStream();
        }

        protected override void Stop_internal()
        {
            TofArFaceManager.Instance?.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            TofArFaceManager.Instance?.UnpauseStream();
        }

        protected override void OnEndRecord(bool result, string filePath)
        {
            base.OnEndRecord(result, filePath);
        }

        protected override string[] GetFileNames(string dirPath)
        {
            var options = GetFileNames(dirPath, TofArFaceManager.StreamKey);

            return options;
        }
    }
}
