/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Modeling;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace TofArARSamples.SimpleARFoundation
{
    public class FpsModeling : MonoBehaviour
    {
        Text txt;

        ARMeshManager meshManager;

        private int frameCount = 0;
        private float timeElapsed = 0;
        private float fps = 0;

        void Start()
        {
            meshManager = FindObjectOfType<ARMeshManager>();
            
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (!meshManager)
                {
                    Debug.LogError("ARMeshManager is not found.");
                    enabled = false;
                    return;
                }

                meshManager.meshesChanged += MeshManager_meshesChanged;
            }
            else
            {
                if (!TofArModelingManager.Instance)
                {
                    Debug.LogError("TofArModelingManager is not found.");
                    enabled = false;
                    return;
                }
            }
            
            // Get UI
            foreach (var ui in GetComponentsInChildren<Text>())
            {
                if (ui.name.Contains("Fps"))
                {
                    txt = ui;
                    break;
                }
            }
        }

        private void OnDestroy()
        {
            if (meshManager)
            {
                meshManager.meshesChanged -= MeshManager_meshesChanged;
            }
        }

        private void MeshManager_meshesChanged(ARMeshesChangedEventArgs obj)
        {
            frameCount++;
        }

        void Update()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                timeElapsed += Time.unscaledDeltaTime;
                if (timeElapsed > 1f)
                {
                    fps = frameCount / timeElapsed;
                    timeElapsed = 0f;
                    frameCount = 0;
                }

                txt.text = $"{fps:0.0}";
            }
            else
            {
                txt.text = $"{TofArModelingManager.Instance.FrameRate:0.0}";
            }

            
        }
    }
}
