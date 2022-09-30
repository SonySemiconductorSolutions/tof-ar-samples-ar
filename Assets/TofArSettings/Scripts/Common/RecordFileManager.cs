/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections;
using TofAr.V0;
using UnityEngine;

namespace TofArSettings
{
    public class RecordFileManager
    {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        private static System.Text.StringBuilder sbOutput;
        private static System.Text.StringBuilder sbError;

        private static string CheckArguments(TofArManager.AdbCommand command, params string[] arguments)
        {
            switch (command)
            {
                case TofArManager.AdbCommand.Push:
                    if (arguments.Length != 2)
                    {
                        throw new ArgumentException("adb push needs two arguments (src, dst)");
                    }
                    // check arguments for absolute path
                    if (!System.IO.Path.IsPathRooted(arguments[0].Trim('"')))
                    {
                        throw new ArgumentException($"src must be absolute path");
                    }
                    if (!System.IO.Path.IsPathRooted(arguments[1].Trim('"')))
                    {
                        throw new ArgumentException($"dst must be absolute path");
                    }
                    return $"push {arguments[0]} {arguments[1]}";
                case TofArManager.AdbCommand.Pull:
                    if (arguments.Length != 2)
                    {
                        throw new ArgumentException("adb pull needs two arguments (src, dst)");
                    }
                    // check arguments for absolute path
                    if (!System.IO.Path.IsPathRooted(arguments[0].Trim('"')))
                    {
                        throw new ArgumentException($"src must be absolute path");
                    }
                    if (!System.IO.Path.IsPathRooted(arguments[1].Trim('"')))
                    {
                        throw new ArgumentException($"dst must be absolute path");
                    }
                    return $"pull {arguments[0]} {arguments[1]}";
                case TofArManager.AdbCommand.Remove:
                    if (arguments.Length != 1)
                    {
                        throw new ArgumentException("adb shell rm needs one argument");
                    }
                    if (!System.IO.Path.IsPathRooted(arguments[0].Trim('"')))
                    {
                        throw new ArgumentException("path must be absolute");
                    }
                    return $"shell \"rm {arguments[0]}\"";
                case TofArManager.AdbCommand.Forward:
                    if (arguments.Length != 2)
                    {
                        throw new ArgumentException("adb forward needs two arguments (src protocol:port, dst protocol:port)");
                    }
                    if (!System.Text.RegularExpressions.Regex.Match(arguments[0], @"^\b(tcp|udp|local)\b:\d{1,5}").Success ||
                        !System.Text.RegularExpressions.Regex.Match(arguments[1], @"^\b(tcp|udp|local)\b:\d{1,5}").Success)
                    {
                        throw new ArgumentException("Arguments must be like this: [tcp|udp|local]:[PORT_NUMBER]");
                    }
                    return $"forward {arguments[0]} {arguments[1]}";
                default:
                    throw new ArgumentException("Invalid adb command");
            }
        }

        /// <summary>
        /// Save copy to device
        /// </summary>
        /// <param name="src">Source</param>
        /// <param name="dst">Destination</param>
        /// <returns></returns>
        public static IEnumerator CopyToDevice(string src, string dst)
        {
            if (!System.IO.Path.IsPathRooted(src))
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, $"src must be absolute path");
                yield break;
            }
            if (!System.IO.Path.IsPathRooted(dst))
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, $"dst must be absolute path");
                yield break;
            }

            string[] arguments = { $"\"{src}\"", dst };


#if UNITY_EDITOR
            var adbPath = EditorUtils.GetAdbPath();
#else
            var adbPath = TofArManager.Instance.ServerConnectionSettingsForStandalone.fullPathToAdb;
#endif

            if (!string.IsNullOrEmpty(adbPath) && System.IO.Path.IsPathRooted(adbPath))
            {
                sbOutput = new System.Text.StringBuilder();

                sbError = new System.Text.StringBuilder();

                var psi = new System.Diagnostics.ProcessStartInfo();

#if UNITY_EDITOR_WIN
                if (!adbPath.EndsWith("adb.exe"))
#elif UNITY_STANDALONE_WIN
                if (!System.IO.Path.HasExtension(adbPath))
                {
                    adbPath += ".exe";
                }
                if (!adbPath.EndsWith("adb.exe"))
#else
                var fileInfo = new System.IO.FileInfo(adbPath);
                if (!adbPath.EndsWith("adb") || fileInfo.Attributes != System.IO.FileAttributes.Normal)
#endif
                {
                    throw new System.InvalidOperationException("Invalid adb file");
                }

                psi.FileName = adbPath;
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                try
                {
                    string psiArgs = CheckArguments(TofArManager.AdbCommand.Push, arguments);
                    psi.Arguments = psiArgs;
                }
                catch (ArgumentException e)
                {
                    TofArManager.Logger.WriteLog(LogLevel.Debug, $"Failed to copy to device. Reason: {e.Message}");
                    yield break;
                }
                bool threadFinished = false;
                bool success = true;
                string processMsg = null;
                string processError = null;

                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                {
                    var process = new System.Diagnostics.Process();
                    process.OutputDataReceived += Process_OutputDataReceived;
                    process.ErrorDataReceived += Process_ErrorDataReceived;
                    process.StartInfo = psi;
                    process.Start();
                    process.BeginOutputReadLine();
                    success = process.WaitForExit(300000);

                    if (!process.HasExited || process.ExitCode != 0)
                    {
                        success = false;
                    }

                    threadFinished = true;
                }));

                thread.Start();

                while (!threadFinished)
                {
                    yield return new WaitForSeconds(0.5f);
                }

                processError = sbError.ToString();
                processMsg = sbOutput.ToString();

                if (processMsg.StartsWith("adb: error:"))
                {
                    success = false;
                }

                if (!success)
                {
                    if (processMsg != null)
                    {
                        TofArManager.Logger.WriteLog(LogLevel.Debug, $"Process caused error: {processMsg}");
                    }
                    if (processError != null)
                    {
                        TofArManager.Logger.WriteLog(LogLevel.Debug, $"Details: {processError}");
                    }
                }
            }
        }

        private static void Process_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            sbError.Append(e.Data);
        }


        private static void Process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            sbOutput.Append(e.Data);
        }
#endif
    }
}
