/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Linq;
using TofAr.V0;
using TofAr.V0.Face;
using TofArSettings.UI;
using UnityEngine;

namespace TofArSettings.Face
{
    public class FaceRecPlayerController : RecPlayerController
    {
        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Face;
        public override bool HasDropdown => true;

        protected override void Start()
        {
            recCtrl = FindObjectOfType<FaceRecordController>();
            base.Start();
        }

        protected override void PlayPrep_internal()
        {
            TofArFaceManager.Instance.PauseStream();
        }

        protected override bool Play_internal(string fileName)
        {

            var directoryListProp = TofArManager.Instance.GetProperty<DirectoryListProperty>();
            string fileRoot = directoryListProp.path;

            if (fileRoot == null)
            {
                return false;
            }
            var file = $"{fileRoot}/{fileName}";

            TofArFaceManager.Instance.PauseStream();
            TofArFaceManager.Instance.StartPlayback(file);
            return true;
        }

        protected override void Pause_internal()
        {
            Debug.Log("Face Pause");
            TofArFaceManager.Instance.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArFaceManager.Instance.UnpauseStream();
            Debug.Log("Face UnPause");
        }

        protected override void OnEndRecord(bool result, string filePath)
        {
            base.OnEndRecord(result, filePath);
        }

        protected override string[] GetFileNames(string dirPath)
        {
            var directoryListProp = TofArManager.Instance.GetProperty<DirectoryListProperty>();

            var options = (directoryListProp.directoryList)
               .Where(x => x.Contains(TofArFaceManager.StreamKey))
               .OrderBy(x => x).ToArray();
            return options;
        }

        protected override void Stop_internal()
        {
            TofArFaceManager.Instance.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            TofArFaceManager.Instance.UnpauseStream();
        }
    }
}
