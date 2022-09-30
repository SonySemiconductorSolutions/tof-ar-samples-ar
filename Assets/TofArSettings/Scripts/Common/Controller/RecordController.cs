/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using TofAr.V0;
using UnityEngine;

namespace TofArSettings
{
    public abstract class RecordController : ControllerBase
    {
        protected bool isCopyingBackgroundFiles = false;

        public event ChangeToggleEvent OnIsCopyingBackgroundFilesChanged;

        public bool IsCopyingBackgroundFiles
        {
            get => isCopyingBackgroundFiles;
            protected set
            {
                if (value != isCopyingBackgroundFiles)
                {
                    isCopyingBackgroundFiles = value;
                    OnIsCopyingBackgroundFilesChanged?.Invoke(value);
                }
            }
        }

        /// <summary>
        /// Save directory path
        /// </summary>
        public string DirPath;

        /// <summary>
        /// Recording status
        /// </summary>
        public enum RecStatus : int
        {
            Start,
            Waiting,
            Recording,
            Stop
        }

        /// <summary>
        /// Recording mode (Single/Multiple)
        /// </summary>
        public abstract bool IsMultiple
        {
            get;
        }

        int timerIndex = 0;

        /// <summary>
        /// Timer index for recording
        /// </summary>
        public int TimerIndex
        {
            get { return timerIndex; }

            set
            {
                if (value != timerIndex && 0 <= value && value < TimerCounts.Length)
                {
                    timerIndex = value;
                    OnChangeTimerIndex?.Invoke(TimerIndex);
                }
            }
        }

        /// <summary>
        /// Timer count for recording (Unit: s)
        /// </summary>
        public int TimerCount
        {
            get
            {
                return TimerCounts[timerIndex];
            }
        }

        /// <summary>
        /// Remining time (Unit: s)
        /// </summary>
        public float TimeRemaining { get; private set; } = 0;

        RecStatus status = RecStatus.Stop;

        /// <summary>
        /// Recording status
        /// </summary>
        public RecStatus Status
        {
            get { return status; }

            protected set
            {
                if (value != status)
                {
                    status = value;
                    OnChangeStatus?.Invoke(Status);
                }
            }
        }

        /// <summary>
        /// Recording timer options (Unit: s) 
        /// </summary>
        public static int[] TimerCounts = new int[]
        {
            0, 3, 5, 10
        };

        /// <summary>
        /// Event that is called when recording timer is changed (Get option index)
        /// </summary>
        public event ChangeIndexEvent OnChangeTimerIndex;

        /// <summary>
        /// Event that is called when recording status is changed
        /// </summary>
        /// <param name="status">Recording status</param>
        public delegate void ChangeStatusEvent(RecStatus status);

        /// <summary>
        /// Event that is called when recording status is changed
        /// </summary>
        public event ChangeStatusEvent OnChangeStatus;

        /// <summary>
        /// Event that is called when recording is finished
        /// </summary>
        /// <param name="result">Recording success/failure</param>
        /// <param name="filePath">Path of saved file</param>
        public delegate void ResultEvent(bool result, string filePath);

        /// <summary>
        /// Event that is called when recording is finished
        /// </summary>
        public event ResultEvent OnEndExec;

        private IEnumerator waitAndExec;

        /// <summary>
        /// Start recording (with timer)
        /// </summary>
        public void Exec()
        {
            if (IsRecord())
            {
                if (IsMultiple)
                {
                    ExecMulti(TimerCount);
                }
                else
                {
                    ExecSingle(TimerCount);
                }
            }
        }

        /// <summary>
        /// Start single recording (with timer)
        /// </summary>
        /// <param name="seconds">Timer</param>
        public void ExecSingle(float seconds)
        {
            if (Status == RecStatus.Stop)
            {
                // Start if not recording
                waitAndExec = WaitAndExec(seconds, false);
                StartCoroutine(waitAndExec);
            }
            else
            {
                if (Status == RecStatus.Waiting && waitAndExec != null)
                {
                    StopCoroutine(waitAndExec);
                    waitAndExec = null;
                }

                if (Status == RecStatus.Recording)
                {
                    StopRecording();
                }

                // Stop
                Status = RecStatus.Stop;

            }
        }

        /// <summary>
        /// Start multiple recording (with timer)
        /// </summary>
        /// <param name="seconds">Timer</param>
        public void ExecMulti(float seconds)
        {
            if (Status == RecStatus.Stop)
            {
                // Start if not recording
                waitAndExec = WaitAndExec(seconds, true);
                StartCoroutine(waitAndExec);
            }
            else
            {
                if (Status == RecStatus.Waiting && waitAndExec != null)
                {
                    StopCoroutine(waitAndExec);
                    waitAndExec = null;
                }

                if (Status == RecStatus.Recording)
                {
                    StopRecording();
                }

                // Stop
                Status = RecStatus.Stop;
            }
        }

        /// <summary>
        /// Stop recording
        /// </summary>
        protected abstract void StopRecording();

        /// <summary>
        /// Check if can record
        /// </summary>
        protected abstract bool IsRecord();

        /// <summary>
        /// Start recording
        /// </summary>
        /// <returns>Success/Fail</returns>
        protected abstract bool Record();

        /// <summary>
        /// Save
        /// </summary>
        /// <returns>Save path</returns>
        protected abstract string Output();

        protected abstract string GetLastRecording();

        private static bool copyInProcess = false;

        protected IEnumerator CopyToDevice()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN

            var lastRecording = GetLastRecording();

            if (lastRecording == null)
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, $"Invalid path");
                yield break;
            }

            string filePath = System.IO.Path.GetFullPath(lastRecording);

            var directoryListProp = TofArManager.Instance.GetProperty<DirectoryListProperty>();
            string path = directoryListProp.path;

            if (!System.IO.Path.IsPathRooted(filePath))
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, $"src {filePath} must be absolute path");
                yield break;
            }
            if (!System.IO.Path.IsPathRooted(path))
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, $"src {path} must be absolute path");
                yield break;
            }

            const string rawIndexFile = "raw_index.dat";
            string rawIndexPath = System.IO.Path.Combine(filePath, rawIndexFile);
            if (!System.IO.File.Exists(rawIndexPath))
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, $"couldn't find raw_index.dat file");
                yield break;
            }

            IsCopyingBackgroundFiles = true;

            while (copyInProcess)
            {
                yield return null;
            }

            copyInProcess = true;

            yield return RecordFileManager.CopyToDevice(filePath, path);

            copyInProcess = false;

            IsCopyingBackgroundFiles = false;

#endif
            yield return null;
        }

        /// <summary>
        /// Start recording after specified time
        /// </summary>
        /// <param name="time">Wait time (Unit: s)</param>
        /// <param name="isMulti">Single/multiple</param>
        IEnumerator WaitAndExec(float time, bool isMulti)
        {
            // Prevent multiple execution
            if (Status != RecStatus.Stop)
            {
                yield break;
            }

            TimeRemaining = time;

            // Notify start
            Status = RecStatus.Start;
            yield return null;

            // Wait for specified time
            Status = RecStatus.Waiting;
            while (TimeRemaining > 0)
            {
                // If stopped, no further processing is performed
                if (Status == RecStatus.Stop)
                {
                    yield break;
                }

                TimeRemaining -= Time.deltaTime;
                yield return null;
            }

            // Start recording
            Status = RecStatus.Recording;

            bool result = false;
            if (isMulti)
            {
                result = Record();
                // Keep recording until stopped
                while (Status == RecStatus.Recording)
                {

                    // Stop if fail
                    if (!result)
                    {
                        break;
                    }

                    yield return null;
                }
            }
            else
            {
                // Single recording
                result = Record();
            }

            Status = RecStatus.Stop;

            string filePath = string.Empty;
            if (result)
            {
                // wait until copy is finished
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
                while (IsCopyingBackgroundFiles)
                {
                    yield return null;
                }
#endif
                filePath = Output();
            }

            OnEndExec?.Invoke(result, filePath);
        }
    }
}
