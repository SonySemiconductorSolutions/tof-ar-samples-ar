/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TofArSettings
{
    public abstract class RecPlayerController : ControllerBase
    {
        /// <summary>
        /// Playback status
        /// </summary>
        public enum PlayStatus : int
        {
            Idle,
            Playing,
            Pause,
            None
        }

        /// <summary>
        /// With/without individual dropdown
        /// </summary>
        public virtual bool HasDropdown { get; } = false;

        /// <summary>
        /// If required to playback before others
        /// </summary>
        public virtual bool IsPriority { get; } = false;

        /// <summary>
        /// Component Type
        /// </summary>
        public virtual UI.SettingsBase.ComponentType ComponentType { get; }

        int index = 0;
        public int Index
        {
            get
            {
                if (HasDropdown)
                {
                    return index;
                }
                else
                {
                    return 1;
                }
            }

            set
            {
                if (value != index && 0 <= value &&
                    value < FileNames.Length)
                {
                    index = value;

                    // Stop playback
                    Status = PlayStatus.Idle;

                    OnChangeIndex?.Invoke(Index);
                }
            }
        }

        protected string[] fileNames;
        public string[] FileNames
        {
            get { return fileNames; }

            protected set
            {
                if (value != fileNames)
                {
                    fileNames = value;
                    OnUpdateFileNames?.Invoke(FileNames, Index);
                }
            }
        }

        PlayStatus status = PlayStatus.Idle;

        /// <summary>
        /// Playback status
        /// </summary>
        public PlayStatus Status
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
        /// Event that is called when index is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeIndex;

        /// <summary>
        /// Event that is called when playback status is changed
        /// </summary>
        /// <param name="status">Playback status</param>
        public delegate void ChangeStatusEvent(PlayStatus status);

        /// <summary>
        /// Event that is called when playback status is changed
        /// </summary>
        public event ChangeStatusEvent OnChangeStatus;

        /// <summary>
        /// Event that is called when file name list is updated
        /// </summary>
        public event UpdateArrayEvent OnUpdateFileNames;

        protected RecordController recCtrl;

        /// <summary>
        /// Called after application startup (after Awake) (Unity standard function)
        /// </summary>
        protected override void Start()
        {
            // Check folder
            string dirPath = Path.Combine(Application.persistentDataPath, recCtrl?.DirPath ?? "data");

            var fnames = new List<string>();
            //zero entry at the top
            fnames.Add("-");
            fnames.AddRange(GetFileNames(dirPath));
            FileNames = fnames.ToArray();

            if (recCtrl != null)
            {
                recCtrl.OnEndExec += OnEndRecord;
            }
            base.Start();
        }

        /// <summary>
        /// Executed when script is disabled (Unity standard function)
        /// </summary>
        private void OnDisable()
        {
            Stop();
            StopCleanup();
        }

        /// <summary>
        /// Playback preparation - 
        /// For synchronization
        /// </summary>
        public void PlayPrep()
        {
            if (Status == PlayStatus.Idle)
            {
                PlayPrep_internal();
            }
        }

        protected abstract void PlayPrep_internal();

        /// <summary>
        /// For playback -
        /// For synchronization
        /// </summary>
        public void StopCleanup()
        {
            if (Status == PlayStatus.Idle)
            {
                StopCleanup_internal();
            }
        }

        protected abstract void StopCleanup_internal();

        /// <summary>
        /// Start playback
        /// </summary>
        /// <returns>Playback success/fail</returns>
        public bool PlayPause()
        {
            bool result = true;
            switch (Status)
            {
                case PlayStatus.Idle:
                    // Execute if selected
                    if (Index > 0)
                    {
                        result = Play(FileNames[Index]);
                    }
                    else
                    {
                        Status = PlayStatus.None;
                    }
                    break;

                case PlayStatus.Playing:
                    Stop();
                    break;

                case PlayStatus.None:
                    Status = PlayStatus.Idle;
                    break;
                    //case PlayStatus.Pause:
                    //    UnPause();
                    //    break;
            }

            return result;
        }

        /// <summary>
        /// Start playback
        /// </summary>
        /// <param name="fileName">Name of file to playback</param>
        /// <returns>Playback success/fail</returns>
        public bool Play(string fileName)
        {
            Status = PlayStatus.Playing;

            return Play_internal(fileName);
        }

        /// <summary>
        /// Start playback
        /// </summary>
        /// <param name="fileName">Name of file to playback</param>
        /// <returns>Playback success/fail</returns>
        protected abstract bool Play_internal(string fileName);

        /// <summary>
        /// Pause
        /// </summary>
        public void Pause()
        {
            Status = PlayStatus.Pause;
            Pause_internal();
        }

        protected abstract void Pause_internal();

        /// <summary>
        /// Unpause
        /// </summary>
        public void UnPause()
        {
            Status = PlayStatus.Playing;
            UnPause_internal();
        }
        protected abstract void UnPause_internal();

        /// <summary>
        /// Stop
        /// </summary>
        public void Stop()
        {
            Status = PlayStatus.Idle;
            Stop_internal();
        }

        protected abstract void Stop_internal();

        /// <summary>
        /// Event that is called when recording is finished
        /// </summary>
        /// <param name="result">Recording result</param>
        /// <param name="filePath">Path of saved file</param>
        protected virtual void OnEndRecord(bool result, string filePath)
        {
            // Do not do anything if not saved
            if (!result || filePath.Length <= 0)
            {
                return;
            }

            var files = new List<string>(FileNames);
            files.Add(filePath);
            FileNames = files.ToArray();
        }

        /// <summary>
        /// Get file names
        /// </summary>
        /// <param name="dirPath">Folder path</param>
        /// <returns>List of file names</returns>
        protected abstract string[] GetFileNames(string dirPath);

    }
}
