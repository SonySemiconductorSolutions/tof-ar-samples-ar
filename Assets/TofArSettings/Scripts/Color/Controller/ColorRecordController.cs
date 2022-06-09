/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Linq;
using TofAr.V0;
using TofAr.V0.Color;
using SensCord;
using UnityEngine;

namespace TofArSettings.Color
{
    public class ColorRecordController : RecordController
    {
        private bool isRecording = false;
        string recordingPath = "";

        private bool isCopyingBackgroundFiles = false;

        public event ChangeToggleEvent OnIsCopyingBackgroundFilesChanged;

        public bool IsCopyingBackgroundFiles
        {
            get => isCopyingBackgroundFiles;
            private set
            {
                if(value != isCopyingBackgroundFiles)
                {
                    isCopyingBackgroundFiles = value;
                    OnIsCopyingBackgroundFilesChanged?.Invoke(value);
                }
            }
        }

        public override bool IsMultiple => true;

        protected override bool IsRecord()
        {
            var instance = TofArColorManager.Instance;

            if (instance.IsStreamActive)
            {
                return true;
            }

            return false;
        }

        protected override bool Record()
        {
            var instance = TofArColorManager.Instance;

            if (instance.IsStreamActive)
            {
                var recordProperty = instance.GetProperty<RecordProperty>();
                var directoryListProp = TofArManager.Instance.GetProperty<DirectoryListProperty>();

                var runTime = TofArManager.Instance.RuntimeSettings;

                if (runTime.runMode == RunMode.MultiNode)
                {
                    recordingPath = System.IO.Path.Combine(Application.persistentDataPath, "recordings");
                    if (!System.IO.Directory.Exists(recordingPath))
                    {
                        try
                        {
                            System.IO.Directory.CreateDirectory(recordingPath);
                        }
                        catch (System.IO.IOException e)
                        {
                            TofArManager.Logger.WriteLog(LogLevel.Debug, $"Failed to create directory {recordingPath}. Reason: {e.Message}");
                            return false;
                        }
                        catch (System.ArgumentException e)
                        {
                            TofArManager.Logger.WriteLog(LogLevel.Debug, $"Failed to create directory {recordingPath}. Reason: {e.Message}");
                            return false;
                        }
                    }
                }
                else
                {
                    recordingPath = directoryListProp.path;
                }


                this.isRecording = true;


                var channelInfo = instance.GetProperty<ChannelInfoProperty>();

                recordProperty = new RecordProperty()
                {
                    Enabled = this.isRecording,
                    Path = recordingPath,
                    BufferNum = 5
                };

                foreach (var channel in channelInfo.Channels)
                {
                    if (channel.Key >= 0x80000000)
                    {
                        continue;
                    }

                    recordProperty.Formats[channel.Key] = "raw";
                }
                instance.SetProperty(recordProperty);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void StopRecording()
        {
            var instance = TofArColorManager.Instance;
            var runTime = TofArManager.Instance.RuntimeSettings;
            this.isRecording = false;

            var channelInfo = instance.GetProperty<ChannelInfoProperty>();

            var recordProperty = new RecordProperty()
            {
                Enabled = this.isRecording,
                Path = recordingPath,
                BufferNum = 5
            };

            foreach (var channel in channelInfo.Channels)
            {
                if (channel.Key >= 0x80000000)
                {
                    continue;
                }
                recordProperty.Formats[channel.Key] = "raw";
            }
            instance.SetProperty(recordProperty);

            if (!recordProperty.Enabled) // stopped
            {
                // Copy to device (for DebugServer)

                if (runTime.runMode == RunMode.MultiNode)
                {
                    StartCoroutine(CopyToDevice());
                }
            }
        }

        protected override string Output()
        {
            var directoryListProp = TofArManager.Instance.GetProperty<DirectoryListProperty>().directoryList
                    .Where(x => x.Contains(TofArColorManager.StreamKey)).OrderBy(x => x);

            return directoryListProp.Last();
        }

        private IEnumerator CopyToDevice()
        {
#if UNITY_EDITOR
            var recordings = new System.IO.DirectoryInfo($"{Application.persistentDataPath}/recordings/").EnumerateDirectories();

            string filePath = System.IO.Path.GetFullPath(recordings.Last().FullName);

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

            IsCopyingBackgroundFiles = true;

            yield return RecordFileManager.CopyToDevice(filePath, path);

            IsCopyingBackgroundFiles = false;

#endif
            yield return null;

        }
    }
}
