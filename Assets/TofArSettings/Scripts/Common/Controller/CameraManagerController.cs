/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using UnityEngine.Events;

namespace TofArSettings
{
    public class CameraManagerController : ControllerBase
    {
        public string[] Options { get; protected set; }

        protected int index = 0;
        public int Index
        {
            get { return index; }
            set
            {
                if (index != value && CheckIndexRange(value))
                {
                    index = value;
                    Apply(true);
                }
            }
        }

        public UnityAction OnMadeOptions;

        public event ChangeIndexEvent OnChangeBefore, OnChangeAfter;

        public event UnityAction OnRestart;

        bool restart = false;

        /// <summary>
        /// Executed when script is enabled (Unity standard function)
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// Executed when script is disabled (Unity standard function)
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// Called after application startup (after Awake) (Unity standard function)
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Restart stream
        /// </summary>
        public void Restart()
        {
            if (IsStreamActive())
            {
                Apply(true);
            }
        }

        /// <summary>
        /// Change index range
        /// </summary>
        /// <param name="newIndex">index</param>
        /// <returns>Whether in range or not</returns>
        protected virtual bool CheckIndexRange(int newIndex)
        {
            return false;
        }

        /// <summary>
        /// Check if stream is active
        /// </summary>
        /// <returns>Active/Inactive</returns>
        public virtual bool IsStreamActive()
        {
            return false;
        }

        /// <summary>
        /// Start stream
        /// </summary>
        protected virtual void StartStream()
        {
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        protected virtual void StopStream()
        {
        }

        /// <summary>
        /// Generate text that is displayed in log when Apply is called
        /// </summary>
        /// <returns>String</returns>
        protected virtual string GetApplyText()
        {
            return string.Empty;
        }

        /// <summary>
        /// Event that is called when Tof stream is started
        /// </summary>
        protected void OnStreamStarted()
        {
            Apply(false);
        }

        /// <summary>
        /// Event that is called when Tof stream is stopped
        /// </summary>
        protected void OnStreamStopped()
        {
            if (!restart)
            {
                index = 0;
                Apply(false);
            }
        }

        /// <summary>
        /// Apply CameraConfig/Resolution
        /// </summary>
        /// <param name="execRestart">Execute/do not execute stream restart</param>
        protected void Apply(bool execRestart)
        {
            OnChangeBefore?.Invoke(Index);

            if (execRestart)
            {
                restart = true;

                if (Index > 0)
                {
                    string text = GetApplyText();
                    TofArManager.Logger.WriteLog(LogLevel.Debug, text);
                }

                // Stop stream
                StopStream();

                // Notify classes that will execute SetProperty
                OnRestart?.Invoke();

                // Start stream and apply Config
                if (Index > 0)
                {
                    StartStream();
                }

                restart = false;
            }

            OnChangeAfter?.Invoke(Index);
        }
    }
}
