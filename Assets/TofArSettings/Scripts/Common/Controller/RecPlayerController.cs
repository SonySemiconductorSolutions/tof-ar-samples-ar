/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TofAr.V0;
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

            if (FileNames != null && FileNames.Length > 0)
            {
                fnames.AddRange(FileNames);
            }
            else
            {
                //zero entry at the top
                fnames.Add("-");
            }

            var fileNames = GetFileNames(dirPath);
            foreach (string fileName in fileNames)
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    fnames.Add(fileName);
                }
            }

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
            if (!result || string.IsNullOrEmpty(filePath))
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

        protected string[] GetFileNames(string dirPath, string streamKey)
        {
            var runtimeMode = TofArManager.Instance.RuntimeSettings.runMode;

            if (runtimeMode == RunMode.MultiNode)
            {
                var directoryListProp = TofArManager.Instance.GetProperty<DirectoryListProperty>().directoryList
                    .Where(x => x.Contains(streamKey)).OrderBy(x => x);

                return directoryListProp.ToArray();
            }
            else
            {
                var directoryListProp = TofArManager.Instance.GetProperty<DirectoryListProperty>();

                string root = directoryListProp.path;

                List<string> fileNames = new List<string>();

                foreach (var dir in directoryListProp.directoryList)
                {
                    string currentPath = Path.Combine(root, dir);
                    string infoPath = Path.Combine(currentPath, "info.xml");
                    if (File.Exists(infoPath) && Path.IsPathRooted(infoPath))
                    {
                        string ext = new FileInfo(infoPath).Extension;
                        if (ext.Equals(".xml"))
                        {
                            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                            xmlDoc.Load(infoPath);

                            var streamNode = xmlDoc.SelectSingleNode("//record/stream");

                            string key = streamNode.Attributes["key"].Value;

                            if (streamKey.Equals(key))
                            {
                                fileNames.Add(dir);
                            }
                        }
                    }
                }
                return fileNames.OrderBy(x => x).ToArray();
            }
        }

        public void DeleteFile(string fileName, int idx)
        {
            var directoryListProp = TofArManager.Instance.GetProperty<DirectoryListProperty>();
            string fileRoot = directoryListProp.path;

            if (fileRoot == null)
            {
                return;
            }
            var directory = $"{fileRoot}{Path.DirectorySeparatorChar}{fileName}";

            // can delete only on Device
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);

                if (index == idx)
                {
                    // Currently selected index is deleted
                    index = idx > 0 ? idx - 1 : 0;
                }
                else if (idx < Index)
                {
                    // Lower index has been selected -> adjust currently selected index
                    index--;
                }

                var updatedFileNames = fileNames.ToList();
                updatedFileNames.Remove(fileName);
                FileNames = updatedFileNames.ToArray();
            }
        }

        public void RenameFile(string fileName, string fileNameNew, int index)
        {
            var directoryListProp = TofArManager.Instance.GetProperty<DirectoryListProperty>();
            string fileRoot = directoryListProp.path;

            if (fileRoot == null)
            {
                return;
            }
            var directory = $"{fileRoot}{Path.DirectorySeparatorChar}{fileName}";
            var directoryNew = $"{fileRoot}{Path.DirectorySeparatorChar}{fileNameNew}";

            // can rename only on Device
            if (Directory.Exists(directory) && !Directory.Exists(directoryNew))
            {
                Directory.Move(directory, directoryNew);

                fileNames[index] = fileNameNew;
                OnUpdateFileNames?.Invoke(FileNames, Index);
            }
        }
    }
}
