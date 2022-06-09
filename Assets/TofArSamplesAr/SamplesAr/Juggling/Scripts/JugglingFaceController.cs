/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TofAr.V0.Face;
using TofAr.V0;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class manages values of face data in juggling scene.
    /// </summary>
    public class JugglingFaceController : MonoBehaviour
    {
        private FaceResults faceResults;
        private FaceResult faceResultMain;

        [SerializeField]
        private FaceModel faceModel;

        [SerializeField]
        private JugglingNose jugglingNose;

        private int frameCount;
        private float faceFrameRate = 60.0f;
        private float prevTime;

        private const int noseIndexARKit = 9;
        private const int noseIndexTFLite = 4;

        private void OnEnable()
        {
            //Add Listener
            TofArFaceManager.OnFaceEstimated += OnFaceEstimated;

            TofArFaceManager.OnStreamStarted += TofArFaceManager_OnStreamStarted;
            TofArFaceManager.OnStreamStopped += OnStreamStopped;

            
        }

        private void OnDisable()
        {
            //Remove Listener
            TofArFaceManager.OnFaceEstimated -= OnFaceEstimated;

            TofArFaceManager.OnStreamStarted -= TofArFaceManager_OnStreamStarted;
            TofArFaceManager.OnStreamStopped -= OnStreamStopped;
        }

        private void TofArFaceManager_OnStreamStarted(object sender)
        {
            SetVisible(true);
        }

        private void OnStreamStopped(object sender)
        {
            jugglingNose.SetVisible(false);
            SetVisible(false);
            faceResultMain = null;
        }

        private void Update()
        {
            float time = Time.realtimeSinceStartup - prevTime;

            //measure framerate per 0.5 seconds
            if (time >= 0.5f)
            {
                faceFrameRate = frameCount / time;
                frameCount = 0;
                prevTime = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// called when a TofArFaceManager frame arrived that includes face data.
        /// </summary>
        /// <param name="sender">TofArFaceManager</param>
        private void OnFaceEstimated(FaceResults faceResults)
        {
            frameCount++;

            var faceManager = TofArFaceManager.Instance;

            if (faceManager == null)
            {
                return;
            }

            faceResults = faceManager.FaceData.Data;

            if (faceResults.results == null)
            {
                jugglingNose.SetVisible(false);
                return;
            }

            if (faceResults.results.Length > 0)
            {
                faceResultMain = faceResults.results[0];
            }

            if (faceResultMain == null)
            {
                return;
            }

            if (faceResultMain.pose != null && faceResultMain.vertices != null && faceResultMain.vertices.Length > 0)
            {
                int index = TofArFaceManager.Instance.DetectorType == FaceDetectorType.Internal_ARKit ? noseIndexARKit : noseIndexTFLite;
                jugglingNose.SetPosition(faceResultMain.pose.position.GetVector3(), faceResultMain.pose.rotation.GetQuaternion(), faceResultMain.vertices[index].GetVector3());
            }
        }

        /// <summary>
        /// returns recent face data.
        /// </summary>
        /// <returns>FaceResult</returns>
        public FaceResult GetFaceResult()
        {
            return faceResultMain;
        }

        /// <summary>
        /// returns current face tracking state.
        /// </summary>
        /// <returns>TrackingState</returns>
        public TrackingState GetTrackingState()
        {
            if (faceResultMain == null)
            {
                return TrackingState.None;
            }

            return faceResultMain.trackingState;
        }

        /// <summary>
        /// returns the distance between face and camrera.
        /// </summary>
        /// <param name="distance"></param>
        public void GetDistance(out float distance)
        {
            //Initialize parameter
            distance = 0f;

            if (faceResultMain == null)
            {
                return;
            }

            Vector3 pos = faceResultMain.pose.position.GetVector3();

            distance = pos.magnitude;
        }

        /// <summary>
        /// sets visibillity of face mesh data.
        /// </summary>
        /// <param name="enable"></param>
        public void SetVisible(bool enable)
        {
            faceModel.gameObject.SetActive(enable);
            jugglingNose.gameObject.SetActive(enable);
        }

        /// <summary>
        /// returns how much face frames arrived in a second.
        /// </summary>
        /// <returns></returns>
        public float GetFaceFrameRate()
        {
            return faceFrameRate;
        }
    }
}
