/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2024 Sony Semiconductor Solutions Corporation.
 *
 */

#if UNITY_EDITOR && UNITY_ANDROID
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class AndroidSettingsPreProcess : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.Android)
        {
            var libCppSharedSoPlugin = AssetImporter.GetAtPath("Assets/TofAr/TofAr/Plugins/Android/arm64-v8a/libc++_shared.so") as PluginImporter;
            if (libCppSharedSoPlugin != null)
            {
#if UNITY_6000_0_OR_NEWER
                if (libCppSharedSoPlugin.GetAndroidSharedLibraryType() != AndroidSharedLibraryType.Symbol)
                {
                    libCppSharedSoPlugin.SetAndroidSharedLibraryType(AndroidSharedLibraryType.Symbol);
                }
#else
            if (libCppSharedSoPlugin.GetAndroidSharedLibraryType() != AndroidSharedLibraryType.Executable)
            {
                libCppSharedSoPlugin.SetAndroidSharedLibraryType(AndroidSharedLibraryType.Executable);
            }
#endif
            }
        }
    }
}

#endif
