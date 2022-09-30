/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using TofAr.V0.Body;
using TofAr.V0.Color;
using TofAr.V0.Coordinate;
using TofAr.V0.Face;
using TofAr.V0.Mesh;
using TofAr.V0.Modeling;
using TofAr.V0.Plane;
using TofAr.V0.Segmentation;
using TofAr.V0.Slam;
using TofAr.V0.Tof;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace TofArSamples.Startup
{
    public class Startup : Singleton<Startup>
    {
        string startSceneName;
        SceneSelector sceneSelector;

        UnityEvent stopTofArEvent = new UnityEvent();
        UnityEvent destroyTofArEvent = new UnityEvent();

        void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            // Get scene name (used for screen transitions)
            startSceneName = SceneManager.GetActiveScene().name;

            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;

            sceneSelector = FindObjectOfType<SceneSelector>();
            sceneSelector.OnActivated += DestroyTofAr;
        }

        private void SceneUnloaded(Scene arg0)
        {
            LoaderUtility.Deinitialize();
            LoaderUtility.Initialize();
        }

        /// <summary>
        /// Event that is called when scene load is completed
        /// </summary>
        /// <param name="nextScene">Scene</param>
        /// <param name="mode">Scene mode</param>
        void SceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            // Destroy TofArManager if Startup scene
            if (nextScene.name == startSceneName)
            {
                sceneSelector = FindObjectOfType<SceneSelector>();
                sceneSelector.OnActivated += DestroyTofAr;
            }
        }

        void Update()
        {
            if ((Application.platform == RuntimePlatform.WindowsEditor) ||
                (Application.platform == RuntimePlatform.WindowsPlayer) ||
                (Application.platform == RuntimePlatform.OSXEditor) ||
                (Application.platform == RuntimePlatform.OSXPlayer))
            {
                // If running on PC
                // Detect right click
                if (Input.GetMouseButtonDown(1))
                {
                    BackToStartScene();
                }
            }
#if UNITY_EDITOR
            else if (EditorApplication.isPlaying)
            {
                // If running on editor
                // Detect right click
                if (Input.GetMouseButtonDown(1))
                {
                    BackToStartScene();
                }
            }
#endif
            else
            {
                // If running on mobile
                var fingerCount = 0;
                var touches = Input.touches;
                foreach (Touch touch in touches)
                {
                    if (touch.phase != TouchPhase.Ended &&
                        touch.phase != TouchPhase.Canceled)
                    {
                        fingerCount++;
                    }
                }

                // 4 finger touch or press back key
                if ((fingerCount >= 4 || Input.GetKeyDown(KeyCode.Escape)))
                {
                    BackToStartScene();
                }
            }
        }

        /// <summary>
        /// Destroy existing TofArXManager in the Startup scene
        /// </summary>
        void DestroyTofAr()
        {
            // Update info text
            AppInfoPanel appInfoPanel = FindObjectOfType<AppInfoPanel>();
            if (appInfoPanel != null)
            {
                appInfoPanel.SetInfo();
            }

            // Destroy each component
            var mgr = TofArManager.Instance;
            if (mgr)
            {
                Destroy(mgr.gameObject);
            }
            
            var modelingMgr = TofArModelingManager.Instance;
            if (modelingMgr)
            {
                Destroy(modelingMgr.gameObject);
            }

            var bodyMgr = TofArBodyManager.Instance;
            if (bodyMgr)
            {
                Destroy(bodyMgr.gameObject);
            }

            var segmanager = TofArSegmentationManager.Instance;
            if (segmanager)
            {
                Destroy(segmanager.gameObject);
            }

            var colorMgr = TofArColorManager.Instance;
            if (colorMgr)
            {
                Destroy(colorMgr.gameObject);
            }

            var cooMgr = TofArCoordinateManager.Instance;
            if (cooMgr)
            {
                Destroy(cooMgr.gameObject);
            }

            var meshMgr = TofArMeshManager.Instance;
            if (meshMgr)
            {
                Destroy(meshMgr.gameObject);
            }

            var planeMgr = TofArPlaneManager.Instance;
            if (planeMgr)
            {
                Destroy(planeMgr.gameObject);
            }

            var slamMgr = TofArSlamManager.Instance;
            if (slamMgr)
            {
                Destroy(slamMgr.gameObject);
            }

            var faceMgr = TofArFaceManager.Instance;
            if (faceMgr)
            {
                Destroy(faceMgr.gameObject);
            }

            var tofMgr = TofArTofManager.Instance;
            if (tofMgr)
            {
                Destroy(tofMgr.gameObject);
            }

            destroyTofArEvent.Invoke();
        }

        /// <summary>
        /// Return to startup screen
        /// </summary>
        void BackToStartScene()
        {
            if (SceneManager.GetActiveScene().name == startSceneName)
            {
                return;
            }

            // Stop each component
            TofArMeshManager.Instance?.StopStream();
            TofArPlaneManager.Instance?.StopStream();
            TofArBodyManager.Instance?.StopStream();
            TofArModelingManager.Instance?.StopStream();
            TofArSlamManager.Instance?.StopStream();
            TofArFaceManager.Instance?.StopStream();
            TofArSegmentationManager.Instance?.StopStream();

            stopTofArEvent.Invoke();

            var colorMgr = TofArColorManager.Instance;
            var tofMgr = TofArTofManager.Instance;
            if (colorMgr != null && tofMgr != null)
            {
                TofArTofManager.Instance?.StopStreamWithColor();
            }
            else
            {
                TofArColorManager.Instance?.StopStream();
                TofArTofManager.Instance?.StopStream();
            }

            // Transition screens
            SceneManager.LoadSceneAsync(startSceneName);
        }

        /// <summary>
        /// Register event for when TofAr stops
        /// </summary>
        /// <param name="unityAction"></param>
        public void AddListenerStopTofArEventEvent(UnityAction unityAction)
        {
            stopTofArEvent.AddListener(unityAction);
        }

        /// <summary>
        /// Register event for when TofAr is destroyed
        /// </summary>
        /// <param name="unityAction">Argumentless delegate</param>
        public void AddListenerDestroyTofArEvent(UnityAction unityAction)
        {
            destroyTofArEvent.AddListener(unityAction);
        }
    }
}
