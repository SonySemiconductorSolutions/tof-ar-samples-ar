/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

namespace TofArARSamples.IceWater
{
    public class SystemManager : MonoBehaviour
    {
        int frameCount = -1;
        float startTime = 0;
        float fps = 0;
        public Text FPStext;
        public ARSession arSession;
        public AROcclusionManager arOcclusion;
        public ARMeshManager arMeshManager;

        public Material arMeshVisualize;

        public Toggle MeshVisualization;
        public Toggle MeshFunction;


        void Start()
        {
            MeshVisualization.onValueChanged.AddListener((bool isOn) => { OnVisualizationClick(MeshVisualization, isOn); });
            MeshFunction.onValueChanged.AddListener((bool isOn) => { OnFunctionClick(MeshFunction, isOn); });
            arMeshVisualize.shader = Shader.Find("Shader Graphs/arMeshVisualizer");
            arMeshVisualize.SetFloat("Vector1_64b2089f80b24320a36b2ad23b19dc14", 0.0f);

            arMeshManager.meshesChanged += MeshesChanged;
        }

        private void OnDisable()
        {
            arMeshManager.meshesChanged -= MeshesChanged;
        }

        private void Update()
        {
            if (frameCount == -1)
            {
                startTime = Time.realtimeSinceStartup;
            }
            if (++frameCount == 100)
            {
                fps = 100.0f / (Time.realtimeSinceStartup - startTime);
                frameCount = -1;
                int n = Mathf.FloorToInt(100 * fps);
                FPStext.text = "FPS: " + Mathf.FloorToInt(n / 100) + "." + (n % 100);
                FPStext.SetAllDirty();
            }
        }

        private void OnVisualizationClick(Toggle toggle, bool isOn)
        {
            ChangeAllMeshsMaterial();
        }

        public void MeshesChanged(ARMeshesChangedEventArgs eventArgs)
        {
            ChangeAllMeshsMaterial();
        }

        private void ChangeAllMeshsMaterial()
        {
            float setvalue = 0.0f;

            if (MeshVisualization.isOn)
            {
                setvalue = 1.0f;
                MeshFunction.isOn = true;
            }
            else
            {
                setvalue = 0.0f;
            }

            #if UNITY_IOS
            foreach (MeshFilter mesh in arMeshManager.meshes)
            {
                mesh.GetComponent<MeshRenderer>().material.SetFloat("Vector1_64b2089f80b24320a36b2ad23b19dc14", setvalue);
            }
            #else
            arMeshVisualize.SetFloat("Vector1_64b2089f80b24320a36b2ad23b19dc14", setvalue);
            #endif
        }

        private void OnFunctionClick(Toggle toggle, bool isOn)
        {
            if (isOn)
            {
                arOcclusion.requestedEnvironmentDepthMode = UnityEngine.XR.ARSubsystems.EnvironmentDepthMode.Medium;
            }
            else
            {
                arOcclusion.requestedEnvironmentDepthMode = UnityEngine.XR.ARSubsystems.EnvironmentDepthMode.Disabled;
                MeshVisualization.isOn = false;
            }

        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            arSession.Reset();
        }

    }
}
