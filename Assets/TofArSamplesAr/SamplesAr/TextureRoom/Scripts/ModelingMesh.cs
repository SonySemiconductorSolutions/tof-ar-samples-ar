/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TofAr.V0.Modeling;
using TofAr.V0;

namespace TofArARSamples.TextureRoom
{
    public class ModelingMesh : MonoBehaviour
    {
        [SerializeField]
        private GameObject meshPrefab;

        private SynchronizationContext context;

        private string dataPath;

        private bool isStarted = false;

        private Matrix4x4 u2uLL;

        private ScreenOrientation ori;

        //public Text txtFps;

        private float realTime = 0;
        private int processedFrames = 0;
        private float fps = 0;

        private Matrix4x4 u2uLR;
        private Matrix4x4 u2uP;
        private Matrix4x4 u2uPUD;

        public GameObject savedMessagePanel;
        private Animator savedMessageAnimator;
        private Text savedMessageText;

        private void OnEnable()
        {
            // savedMessageAnimator = savedMessagePanel.GetComponent<Animator>();
            // savedMessageText = savedMessagePanel.GetComponentInChildren<Text>();

            TofArModelingManager.OnStreamStarted += StreamStarted;
            TofArModelingManager.OnFrameArrived += Instance_FrameArrived;
            TofArModelingManager.OnStreamStopped += StreamStopped;
            TofArManager.OnScreenOrientationUpdated += OnScreenOrientationChanged;

            Apply();

        }

        private void OnDisable()
        {
            TofArModelingManager.OnStreamStarted -= StreamStarted;
            TofArModelingManager.OnFrameArrived -= Instance_FrameArrived;
            TofArModelingManager.OnStreamStopped -= StreamStopped;
            TofArManager.OnScreenOrientationUpdated -= OnScreenOrientationChanged;
        }

        // Use this for initialization
        void Start()
        {
            this.context = SynchronizationContext.Current;


            dataPath = Application.persistentDataPath + "/meshData";
            if (!System.IO.Directory.Exists(dataPath))
            {
                System.IO.Directory.CreateDirectory(dataPath);
            }

            var settings = TofArModelingManager.Instance.GetProperty<ModelingSettingsProperty>();

            this.context.Post((s) =>
            {
                for (var i = 0; i < settings.numMaxBlocks; i++)
                {
                    var meshObj = GameObject.Instantiate(this.meshPrefab, this.transform);
                    var mesh = meshObj.GetComponent<MeshFilter>().mesh;
                    mesh.indexFormat = IndexFormat.UInt32;

                }

                isStarted = true;

            }, null);


            // TofArModelingManager.Instance.StartStream();

        }

        private void OnScreenOrientationChanged(ScreenOrientation old, ScreenOrientation newOri)
        {
            ori = newOri;
            RotateAccordingToOrientation();
        }

        private void StreamStarted(object sender)
        {
            StartCoroutine(MeasureFps());

            OnScreenOrientationChanged(ScreenOrientation.AutoRotation, TofArManager.Instance.GetProperty<DeviceOrientationsProperty>().screenOrientation);
        }

        private void StreamStopped(object sender)
        {
            StopCoroutine(MeasureFps());
            ClearMesh();
        }

        IEnumerator MeasureFps()
        {
            fps = 0;
            processedFrames = 0;
            realTime = 0;

            while (true)
            {
                /*if (txtFps != null)
                {
                    txtFps.text = string.Format("{0:0.0} fps", fps);
                }*/
                realTime += Time.deltaTime;

                yield return null;
            }
        }

        void OnDestroy()
        {
            TofArModelingManager.OnFrameArrived -= Instance_FrameArrived;

            TofArManager.OnScreenOrientationUpdated -= OnScreenOrientationChanged;
        }

        public void Apply()
        {
            //you have to swap the coordinates when the phone rotates, else the rotation is wrong
            Matrix4x4 c2uLL = new Matrix4x4();
            c2uLL[0, 0] = 1; c2uLL[0, 1] = 0; c2uLL[0, 2] = 0; c2uLL[0, 3] = 0;
            c2uLL[1, 0] = 0; c2uLL[1, 1] = -1; c2uLL[1, 2] = 0; c2uLL[1, 3] = 0;
            c2uLL[2, 0] = 0; c2uLL[2, 1] = 0; c2uLL[2, 2] = 1; c2uLL[2, 3] = 0;
            c2uLL[3, 0] = 0; c2uLL[3, 1] = 0; c2uLL[3, 2] = 0; c2uLL[3, 3] = 1;
            Matrix4x4 u2cLL = c2uLL.transpose;

            Matrix4x4 c2uLR = new Matrix4x4();
            c2uLR[0, 0] = -1; c2uLR[0, 1] = 0; c2uLR[0, 2] = 0; c2uLR[0, 3] = 0;
            c2uLR[1, 0] = 0; c2uLR[1, 1] = 1; c2uLR[1, 2] = 0; c2uLR[1, 3] = 0;
            c2uLR[2, 0] = 0; c2uLR[2, 1] = 0; c2uLR[2, 2] = 1; c2uLR[2, 3] = 0;
            c2uLR[3, 0] = 0; c2uLR[3, 1] = 0; c2uLR[3, 2] = 0; c2uLR[3, 3] = 1;
            Matrix4x4 u2cLR = c2uLR.transpose;

            Matrix4x4 c2uP = new Matrix4x4();
            c2uP[0, 0] = 0; c2uP[0, 1] = -1; c2uP[0, 2] = 0; c2uP[0, 3] = 0;
            c2uP[1, 0] = -1; c2uP[1, 1] = 0; c2uP[1, 2] = 0; c2uP[1, 3] = 0;
            c2uP[2, 0] = 0; c2uP[2, 1] = 0; c2uP[2, 2] = 1; c2uP[2, 3] = 0;
            c2uP[3, 0] = 0; c2uP[3, 1] = 0; c2uP[3, 2] = 0; c2uP[3, 3] = 1;
            Matrix4x4 u2cP = c2uP.transpose;

            Matrix4x4 c2uPUD = new Matrix4x4();
            c2uPUD[0, 0] = 0; c2uPUD[0, 1] = 1; c2uPUD[0, 2] = 0; c2uPUD[0, 3] = 0;
            c2uPUD[1, 0] = 1; c2uPUD[1, 1] = 0; c2uPUD[1, 2] = 0; c2uPUD[1, 3] = 0;
            c2uPUD[2, 0] = 0; c2uPUD[2, 1] = 0; c2uPUD[2, 2] = 1; c2uPUD[2, 3] = 0;
            c2uPUD[3, 0] = 0; c2uPUD[3, 1] = 0; c2uPUD[3, 2] = 0; c2uPUD[3, 3] = 1;
            Matrix4x4 u2cPUD = c2uPUD.transpose;

            u2uLL = c2uLL * u2cLL;
            u2uLR = c2uLR * u2cLR;
            u2uP = c2uP * u2cP;
            u2uPUD = c2uPUD * u2cPUD;

            ori = TofArManager.Instance.GetProperty<DeviceOrientationsProperty>().screenOrientation;
            RotateAccordingToOrientation();


        }

        void RotateAccordingToOrientation()
        {
            Matrix4x4 rotmat = u2uLL;
            if (TofArManager.Instance.RuntimeSettings.runMode == RunMode.Default)
            {

                var screenOrientation = ori;

                switch (screenOrientation)
                {
                    case ScreenOrientation.Portrait:
                        rotmat = u2uP; break;
                    case ScreenOrientation.PortraitUpsideDown:
                        rotmat = u2uPUD; break;
                    case ScreenOrientation.LandscapeLeft:
                        rotmat = u2uLL; break;
                    case ScreenOrientation.LandscapeRight:
                        rotmat = u2uLR; break;
                    default:
                        break;
                }
            }
            transform.localRotation = Quaternion.LookRotation(rotmat.MultiplyVector(Vector3.forward), rotmat.MultiplyVector(Vector3.up));
            transform.localPosition = rotmat.MultiplyPoint3x4(Vector3.zero);
        }


        private void Instance_FrameArrived(object sender)
        {
            if (!this.isStarted)
            {
                return;
            }

            var modelingData = TofArModelingManager.Instance.ModelingData;

            if (modelingData != null)
            {
                var data = modelingData.Data;

                var settings = TofArModelingManager.Instance.GetProperty<ModelingSettingsProperty>();

                context.Post((s) =>
                {

                    if (data != null)
                    {
                        for (int m = 0; m < settings.numMaxBlocks; m++)
                        {
                            var meshData = data[m];

                            int nVertices = meshData.vertices.Length / 3;
                            int nTriangles = meshData.triangles.Length;

                            if (nVertices > 0)
                            {
                                var childTransform = this.transform.GetChild(m);
                                var mesh = childTransform.GetComponent<MeshFilter>().mesh;

                                var vertices = new Vector3[nVertices];
                                for (int i = 0; i < nVertices; i++)
                                {
                                    vertices[i] = new Vector3(meshData.vertices[i * 3 + 0], meshData.vertices[i * 3 + 1], meshData.vertices[i * 3 + 2]);
                                }

                                mesh.Clear();
                                mesh.vertices = vertices;
                                mesh.triangles = meshData.triangles;
                                mesh.RecalculateNormals();

                                var meshCollider = childTransform.GetComponent<MeshCollider>();
                                /*if (meshCollider != null)
                                {
                                    Destroy(meshCollider);
                                }
                                meshCollider = childTransform.gameObject.AddComponent<MeshCollider>();*/
                                if (meshCollider != null)
                                {
                                    meshCollider.sharedMesh = mesh;
                                }
                            }

                        }

                    }
                }, null);

                processedFrames++;

                if (realTime > 1.0f)
                {
                    TofArManager.Logger.WriteLog(LogLevel.Debug, $"Processed frames {processedFrames} in {realTime}s");
                    fps = processedFrames / realTime;

                    realTime = 0;
                    processedFrames = 0;
                }

            }
        }

        public void SaveMesh()
        {
            savedMessageAnimator?.ResetTrigger("showMessageSlow");
            context.Post((s) =>
            {

                int nVertices = 0;
                int nFaces = 0;

                foreach (Transform child in this.transform)
                {
                    var mesh = child.GetComponent<MeshFilter>().mesh;

                    var vertices = mesh.vertices;

                    nVertices += vertices.Length;

                    var triangles = mesh.triangles;

                    nFaces += triangles.Length;
                }

                string plyPath = dataPath + "/" + System.DateTime.Now.ToString("yyyyMMdd-HHmmssfff") + ".ply";
                bool saveSuccess = false;
                try
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(plyPath))
                    {
                        {
                            string header = $"ply\nformat ascii 1.0\nelement vertex {nVertices}\nproperty float x\nproperty float y\nproperty float z\nelement face {nFaces / 3}\nproperty list uchar int vertex_index\nend_header";

                            writer.WriteLine(header);

                            foreach (Transform child in this.transform)
                            {
                                var mesh = child.GetComponent<MeshFilter>().mesh;

                                var vertices = mesh.vertices;

                                foreach (var vertex in vertices)
                                {
                                    writer.WriteLine($"{vertex.x} {vertex.z} {vertex.y}");
                                }
                            }

                            int triangleOffset = 0;

                            foreach (Transform child in this.transform)
                            {
                                var mesh = child.GetComponent<MeshFilter>().mesh;

                                var triangles = mesh.triangles;

                                for (int i = 0; i < triangles.Length; i += 3)
                                {
                                    writer.WriteLine($"3 {triangles[i + 0] + triangleOffset} {triangles[i + 1] + triangleOffset} {triangles[i + 2] + triangleOffset}");
                                }

                                triangleOffset += mesh.vertices.Length;

                            }
                        }
                    }

                    saveSuccess = true;
                }
                finally
                {
                    if (savedMessageText != null)
                    {
                        savedMessageText.text = saveSuccess ? "successfully saved data to\n" + plyPath : "failed to save data";
                    }
                    savedMessageAnimator?.SetTrigger("showMessageSlow");
                }

            }, null);
        }

        public void ToggleVisibility(bool visible)
        {
            foreach (Transform childTransform in this.transform)
            {
                var renderer = childTransform.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material.SetColor("_MainColor", visible ? new UnityEngine.Color(1f, 1f, 1f, 1f) : new UnityEngine.Color(1f, 1f, 1f, 0f));
                    //renderer.enabled = visible;
                }

            }
        }
        public void ClearMesh()
        {
            if (context == null)
            {
                return;
            }
            context.Post((s) =>
            {

                foreach (Transform childTransform in this.transform)
                {
                    var meshFilter = childTransform.GetComponent<MeshFilter>();
                    if (meshFilter != null)
                    {
                        meshFilter.mesh.Clear();
                    }

                    var meshCollider = childTransform.GetComponent<MeshCollider>();
                    if (meshCollider != null)
                    {
                        meshCollider.sharedMesh = null;
                    }
                }

            }, null);



        }


    }

}

