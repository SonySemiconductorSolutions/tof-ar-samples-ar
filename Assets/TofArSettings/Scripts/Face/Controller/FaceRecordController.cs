﻿/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Linq;
using TofAr.V0;
using TofAr.V0.Face;
using SensCord;
using UnityEngine;

namespace TofArSettings.Face
{
    public class FaceRecordController : RecordController
    {
        private bool isRecording = false;
        string recordingPath = "";

        public override bool IsMultiple => true;

        protected override bool IsRecord()
        {
            var instance = TofArFaceManager.Instance;

            if (instance.IsStreamActive)
            {
                return true;
            }

            return false;
        }

        protected override bool Record()
        {
            var instance = TofArFaceManager.Instance;

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
            var instance = TofArFaceManager.Instance;
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
                    .Where(x => x.Contains(TofArFaceManager.StreamKey)).OrderBy(x => x);
            if (directoryListProp.Count() == 0)
            {
                return string.Empty;
            }
            return directoryListProp.Last();
        }

        protected override string GetLastRecording()
        {
            var recordings = new System.IO.DirectoryInfo($"{Application.persistentDataPath}/recordings/").EnumerateDirectories()
                .Where(x => x.Name.Contains(TofArFaceManager.StreamKey));
            if (recordings.Count() == 0)
            {
                return null;
            }

            return recordings.Last().FullName;
        }
    }
}
