/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Mesh;
using TofArSettings.UI;
using UnityEngine;

namespace TofArSettings.Mesh
{
    public class MeshRecPlayerController : RecPlayerController
    {

        public override SettingsBase.ComponentType ComponentType => SettingsBase.ComponentType.Mesh;
        public override bool HasDropdown => false;
        
        private MeshManagerController managerController;

        private void Awake()
        {
            managerController = FindObjectOfType<MeshManagerController>();
        }

        protected override void PlayPrep_internal()
        {
            TofArMeshManager.Instance.StopStream();
            TofArMeshManager.Instance.StopPlayback();
        }

        protected override bool Play_internal(string fileName)
        {
            if (managerController.IsStreamActive())
            {
                TofArMeshManager.Instance.StartPlayback();
            }
            return true;
        }

        protected override void Pause_internal()
        {
            Debug.Log("Mesh Pause");
            TofArMeshManager.Instance.PauseStream();
        }

        protected override void UnPause_internal()
        {
            TofArMeshManager.Instance.UnpauseStream();
            Debug.Log("Mesh UnPause");
        }

        protected override void Stop_internal()
        {
            TofArMeshManager.Instance.StopPlayback();
        }

        protected override void StopCleanup_internal()
        {
            if (managerController.IsStreamActive())
            {
                TofArMeshManager.Instance.StartStream();
            }
        }

        protected override string[] GetFileNames(string dirPath)
        {
            return new string[] { "" };
        }
    }
}
