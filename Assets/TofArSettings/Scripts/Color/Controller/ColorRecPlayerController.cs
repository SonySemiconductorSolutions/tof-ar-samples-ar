/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Linq;
using TofAr.V0;
using TofAr.V0.Color;
using TofArSettings.UI;
using UnityEngine;

namespace TofArSettings.Color
{
    public class ColorRecPlayerController : RecPlayerController
    {
        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Color;
        public override bool HasDropdown => true;
        public override bool IsPriority => true;


        protected override void Start()
        {
            recCtrl = FindObjectOfType<ColorRecordController>();
            base.Start();
        }

        protected override void PlayPrep_internal()
        {
            TofArColorManager.Instance.PauseStream();
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

                TofArColorManager.Instance.PauseStream();
            TofArColorManager.Instance.StartPlayback(file);
            return true;
        }

        protected override void Pause_internal()
        {
            Debug.Log("Color Pause");
            TofArColorManager.Instance.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArColorManager.Instance.UnpauseStream();
            Debug.Log("Color UnPause");
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

        protected override void Stop_internal()
        {
            TofArColorManager.Instance.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            TofArColorManager.Instance.UnpauseStream();
        }
    }
}
