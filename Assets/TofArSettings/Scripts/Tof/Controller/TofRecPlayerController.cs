/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Linq;
using TofAr.V0;
using TofAr.V0.Tof;
using TofArSettings.UI;
using UnityEngine;

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

        protected override bool Play_internal(string fileName)
        {

            var directoryListProp = TofArManager.Instance.GetProperty<DirectoryListProperty>();
            string fileRoot = directoryListProp.path;

            if (fileRoot == null)
            {
                return false;
            }
            var file = $"{fileRoot}/{fileName}";

            TofArTofManager.Instance.StartPlayback(file);
            return true;
        }

        protected override void Pause_internal()
        {
            Debug.Log("Tof Pause");
            TofArTofManager.Instance.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArTofManager.Instance.UnpauseStream();
            Debug.Log("tof UnPause");
        }

        protected override void OnEndRecord(bool result, string filePath)
        {
            base.OnEndRecord(result, filePath);
        }

        protected override string[] GetFileNames(string dirPath)
        {
                var directoryListProp = TofArManager.Instance.GetProperty<DirectoryListProperty>();

                 var options = (directoryListProp.directoryList)
                    .Where(x => x.Contains(TofArTofManager.StreamKey))
                    .OrderBy(x => x).ToArray();
                return options;
        }

        protected override void Stop_internal()
        {
            TofArTofManager.Instance.StopPlayback();
        }

        protected override void PlayPrep_internal()
        {
            TofArTofManager.Instance.PauseStream();
        }

        protected override void StopCleanup_internal()
        {
            TofArTofManager.Instance.UnpauseStream();
        }
    }
}
