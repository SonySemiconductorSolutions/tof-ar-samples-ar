/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArARSamples.SimpleARFoundation
{
    public class ARMeshController : MonoBehaviour
    {
        ModelingManagerController mgrCtrl;

        private void Awake()
        {
            mgrCtrl = FindObjectOfType<ModelingManagerController>();
        }

        private void OnEnable()
        {
            mgrCtrl.OnStreamStartStatusChanged += MgrCtrl_OnStreamStartStatusChanged;
        }

        private void OnDisable()
        {
            mgrCtrl.OnStreamStartStatusChanged -= MgrCtrl_OnStreamStartStatusChanged;
        }

        private void MgrCtrl_OnStreamStartStatusChanged(bool onOff)
        {
            if (!onOff)
            {
                var trackables = GameObject.Find("Trackables");
                if (trackables)
                {
                    foreach (Transform trackable in trackables.transform)
                    {
                        var meshFilter = trackable.GetComponent<MeshFilter>();
                        if (meshFilter)
                        {
                            meshFilter.sharedMesh.Clear();
                        }
                    }
                }
            }
        }

    }
}