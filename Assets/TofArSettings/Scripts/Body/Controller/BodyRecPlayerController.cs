/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Linq;
using TofAr.V0;
using TofAr.V0.Body;
using TofArSettings.UI;
using UnityEngine;

namespace TofArSettings.Body
{
    public class BodyRecPlayerController : RecPlayerController
    {
        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Body;

        public override bool HasDropdown => true;

        protected override void Start()
        {
            recCtrl = FindObjectOfType<BodyRecordController>();
            base.Start();
        }

        protected override void PlayPrep_internal()
        {
            TofArBodyManager.Instance.PauseStream();
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

                TofArBodyManager.Instance.PauseStream();
            TofArBodyManager.Instance.StartPlayback(file);
            return true;
        }

        protected override void Pause_internal()
        {
            Debug.Log("Body Pause");
            TofArBodyManager.Instance.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArBodyManager.Instance.UnpauseStream();
            Debug.Log("Body UnPause");
        }

        protected override void OnEndRecord(bool result, string filePath)
        {
            base.OnEndRecord(result, filePath);
        }

        protected override string[] GetFileNames(string dirPath)
        {
                var directoryListProp = TofArManager.Instance.GetProperty<DirectoryListProperty>();

                 var options = (directoryListProp.directoryList)
                    .Where(x => x.Contains(TofArBodyManager.StreamKey))
                    .OrderBy(x => x).ToArray();
                return options;
        }

        protected override void Stop_internal()
        {
            TofArBodyManager.Instance.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            TofArBodyManager.Instance.UnpauseStream();
        }
    }
}
