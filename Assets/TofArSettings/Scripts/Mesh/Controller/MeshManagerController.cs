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
        private bool isStarted = false;
        private bool isPlaying = false;
        private bool restartStream = false;

        private void Awake()
        {
            context = SynchronizationContext.Current;
            isStarted = TofArMeshManager.Instance.autoStart;
        }

        protected void OnEnable()
        {
            TofArTofManager.OnStreamStarted += OnTofPlaybackStreamStarted;
            TofArTofManager.OnStreamStopped += OnTofPlaybackStreamStopped;
        }

        protected void OnDisable()
        {
            TofArTofManager.OnStreamStarted -= OnTofPlaybackStreamStarted;
            TofArTofManager.OnStreamStopped -= OnTofPlaybackStreamStopped;
        }

        public bool IsStreamActive()
        {
            return isStarted && TofArTofManager.Instance.IsStreamActive;
        }

        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            if (!isStarted)
            {
                isStarted = true;
                if (TofArTofManager.Instance.IsPlaying)
                {
                    TofArMeshManager.Instance.StartPlayback();
                }
                else
                {
                    TofArMeshManager.Instance.StartStream();
                }
                OnStreamStartStatusChanged?.Invoke(isStarted);
            }
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        public void StopStream()
        {
            if (isStarted)
            {
                isStarted = false;
                TofArMeshManager.Instance.StopStream();
                OnStreamStartStatusChanged?.Invoke(isStarted);
            }
        }

        /// <summary>
        /// Starts Mesh stream after a short delay
        /// </summary>
        private IEnumerator StartStreamCoroutine()
        {
            // Wait 1 frame when executing OnStreamStarted directly because it does not execute for only the first time
            yield return new UnityEngine.WaitForEndOfFrame();

            StartStream();
        }

        private IEnumerator StartPlaybackStreamCoroutine()
        {
            yield return new UnityEngine.WaitForEndOfFrame();

            TofArMeshManager.Instance.StartPlayback();
            OnStreamStartStatusChanged?.Invoke(true);
            isPlaying = true;
        }

        /// <summary>
        /// Event that is called when Tof stream is started and status is changed
        /// </summary>
        public event ChangeToggleEvent OnStreamStartStatusChanged;

        /// <summary>
        /// Event that is called when Tof stream is started
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        public void OnTofStreamStarted(object sender, UnityEngine.Texture2D depth, UnityEngine.Texture2D conf, PointCloudData pc)
        {
            context.Post((s) =>
            {
                StartCoroutine(StartStreamCoroutine());
            }, null);
        }

        /// <summary>
        /// Event that is called when Tof stream is stopped
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        public void OnTofStreamStopped(object sender)
        {
            StopStream();
        }

        /// <summary>
        /// Event that is called when Tof playback stream is started
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        private void OnTofPlaybackStreamStarted(object sender, UnityEngine.Texture2D depth, UnityEngine.Texture2D conf, PointCloudData pc)
        {
            if (TofArTofManager.Instance.IsPlaying)
            {
                if (isStarted)
                {
                    restartStream = true;
                }

                context.Post((s) =>
                {
                    StartCoroutine(StartPlaybackStreamCoroutine());
                }, null);
            }
        }

        /// <summary>
        /// Event that is called when Tof stream is stopped
        /// </summary>
        /// <param name="sender">TofArTofManager</param>
        private void OnTofPlaybackStreamStopped(object sender)
        {
            if (isPlaying)
            {
                isPlaying = false;
                OnTofStreamStopped(sender);
            }

            // may have to restart mesh stream
            if (restartStream)
            {
                restartStream = false;
                OnTofStreamStarted(sender, null, null, null);
            }
        }
    }
}
