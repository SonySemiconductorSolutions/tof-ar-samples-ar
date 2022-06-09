/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

namespace TofArSettings
{
    public class OutputBuildDateTime : IPreprocessBuildWithReport
    {
        const string resouceDirPath = "Assets/Resources";
        const string fileName = "BuildDateTime.txt";

        /// <summary>
        /// Execution order
        /// </summary>
        public int callbackOrder
        {
            get { return 0; }
        }

        /// <summary>
        /// Actions performed before build
        /// </summary>
        /// <param name="buildReport">Build information</param>
        public void OnPreprocessBuild(BuildReport buildReport)
        {
            try
            {
                if (!Directory.Exists(resouceDirPath))
                {
                    Directory.CreateDirectory(resouceDirPath);
                }

                string filePath = Path.Combine(resouceDirPath, fileName);
                using (var stream = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    var text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                    stream.Write(text);
                    stream.Close();
                }
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e);
            }
            catch (IOException e)
            {
                Debug.LogError(e);
            }
            finally
            {
                AssetDatabase.Refresh();
            }
        }
    }
}
