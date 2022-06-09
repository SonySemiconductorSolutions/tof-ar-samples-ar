/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Mesh;
using TofAr.V0.Tof;
using System.Threading;
using System.Collections;

namespace TofArSettings.Mesh
{
    public class MeshManagerController : ControllerBase
    {
        private SynchronizationContext context;

        private void Awake()
        {
            isStarted = TofArMeshManager.Instance.autoStart;

            context = SynchronizationContext.Current;
        }

        protected void OnEnable()
        {
            TofArTofManager.OnStreamStarted += OnTofStreamStarted;
            TofArTofManager.OnStreamStopped += OnTofStreamStopped;
        }

        protected void OnDisable()
        {
            TofArTofManager.OnStreamStarted -= OnTofStreamStarted;
            TofArTofManager.OnStreamStopped -= OnTofStreamStopped;
        }

        public bool IsStreamActive()
        {
            return isStarted && TofArTofManager.Instance.IsStreamActive;
        }

        private bool isStarted = false;

        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            isStarted = true;
            if (TofArTofManager.Instance.IsStreamActive)
            {
                TofArMeshManager.Instance.StartStream();
            }
            if (TofArTofManager.Instance.IsPlaying)
            {
                TofArMeshManager.Instance.StartPlayback();
            }
            OnStreamStartStatusChanged?.Invoke(isStarted);
        }

        /// <summary>
        /// Starts Mesh stream after a short delay
        /// </summary>
        private IEnumerator StartStreamCoroutine()
        {
            yield return new UnityEngine.WaitForEndOfFrame();
            if (TofArTofManager.Instance.IsPlaying)
            {
                TofArMeshManager.Instance.StartPlayback();
            }
            else
            {
                TofArMeshManager.Instance.StartStream();
            }
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        public void StopStream()
        {
            isStarted = false;
            if (TofArMeshManager.Instance.IsStreamActive)
            {
                TofArMeshManager.Instance.StopStream();
            }
            OnStreamStartStatusChanged?.Invoke(isStarted);
        }

        /// <summary>
        /// Event that is called when Tof stream is started and status is changed
        /// </summary>
        public event ChangeToggleEvent OnStreamStartStatusChanged;

        /// <summary>
        /// Event that is called when Tof stream is started
        /// </summary>
        /// <param name="sender">TofArMeshManager</param>
        private void OnTofStreamStarted(object sender, UnityEngine.Texture2D depth, UnityEngine.Texture2D conf, PointCloudData pc)
        {
            if (isStarted)
            {
                context.Post((s) =>
                {
                    StartCoroutine(StartStreamCoroutine());
                }, null);
            }
        }

        /// <summary>
        /// Event that is called when Tof stream is stopped
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        private void OnTofStreamStopped(object sender)
        {
            if (isStarted)
            {
                TofArMeshManager.Instance.StopStream();
            }
        }
    }
}
