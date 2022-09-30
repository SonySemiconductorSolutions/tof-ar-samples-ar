/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using TofAr.V0.Mesh;
using TofArSettings.Mesh;

namespace TofArARSamples.SimpleARFoundation
{
    public class MeshVisibilityController : MonoBehaviour
    {
        MeshManagerController mgrCtrl;
        DynamicMesh dynamicMesh;

        private void Awake()
        {
            mgrCtrl = FindObjectOfType<MeshManagerController>();
            dynamicMesh = FindObjectOfType<DynamicMesh>();
        }

        private void OnEnable()
        {
            mgrCtrl.OnStreamStartStatusChanged += OnMeshStreamStartStatusChanged;
        }

        private void OnDisable()
        {
            mgrCtrl.OnStreamStartStatusChanged -= OnMeshStreamStartStatusChanged;
        }

        private void OnMeshStreamStartStatusChanged(bool onOff)
        {
            dynamicMesh.GetComponent<MeshRenderer>().enabled = onOff;
        }
    }
}
