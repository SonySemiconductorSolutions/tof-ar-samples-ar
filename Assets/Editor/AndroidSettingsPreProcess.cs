/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

#if UNITY_EDITOR && UNITY_ANDROID
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AndroidSettingsPreProcess : IPreprocessBuildWithReport
{
    private const string UNITY_PROJECT_SETTINGS_PATH = "/ProjectSettings/ProjectSettings.asset";
    private const string TARGET_SDK_VERSION_CHECK_NAME = "AndroidTargetSdkVersion";
    private const string TARGET_SDK_VERSION_OVERWRITE = "  AndroidTargetSdkVersion: ";

    private const string ANDROID_MANIFEST_PATH = "/Assets/Plugins/Android/AndroidManifest.xml";
    private const string USES_NATIVE_LIBRARY_CHECK_NAME = "libOpenCL";
    private const string APPLICATION_END_LINE = "</application>";

    private const string USES_NATIVE_LIBRARY_ADD = "<uses-native-library android:name=\"libOpenCL.so\" android:required=\"false\" />\r\n<uses-native-library android:name=\"libOpenCL-car.so\" android:required=\"false\" />\r\n<uses-native-library android:name=\"libOpenCL-pixel.so\" android:required=\"false\" />";

    string projectFolderPath = Application.dataPath.Replace("/Assets", "");

    char separater = '\n';

    public int callbackOrder
    {
        get { return 0; }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.Android)
        {
#if UNITY_2022_1_OR_NEWER
            AndroidProjectSettings(32);
            AndroidManifestSetting(true);

#else
            AndroidProjectSettings(30);
            AndroidManifestSetting(false);
#endif
        }

    }

    private void AndroidProjectSettings(int apiLevel)
    {
        string projectSettingsPath = projectFolderPath + UNITY_PROJECT_SETTINGS_PATH;
        string projectSettingsText = File.ReadAllText(projectSettingsPath);

        if (!string.IsNullOrEmpty(projectSettingsText))
        {
            string newProjectSettings = string.Empty;

            string[] splits = projectSettingsText.Split(separater);

            foreach (string text in splits)
            {
                if (text.Contains(TARGET_SDK_VERSION_CHECK_NAME))
                {
                    newProjectSettings += TARGET_SDK_VERSION_OVERWRITE + apiLevel + separater;
                }
                else
                {
                    newProjectSettings += text + separater;
                }
            }

            newProjectSettings = newProjectSettings[..^1];

            File.WriteAllText(projectSettingsPath, newProjectSettings);
            EditorUtility.RequestScriptReload();
        }
    }

    private void AndroidManifestSetting(bool apiLevel31Over)
    {
        string androidManifPath = projectFolderPath + ANDROID_MANIFEST_PATH;
        string androidManifText = File.ReadAllText(androidManifPath);

        if (!string.IsNullOrEmpty(androidManifText))
        {
            string newAndroidManifText = string.Empty;

            bool libraryCheckState = androidManifText.Contains(USES_NATIVE_LIBRARY_CHECK_NAME);
            
            if (!libraryCheckState && apiLevel31Over)
            {
                newAndroidManifText = androidManifText.Replace(APPLICATION_END_LINE, USES_NATIVE_LIBRARY_ADD + separater + APPLICATION_END_LINE);

            }
            else if (libraryCheckState && !apiLevel31Over)
            {
                string[] splits = androidManifText.Split(separater);

                foreach (string text in splits)
                {
                    if (!text.Contains(USES_NATIVE_LIBRARY_CHECK_NAME))
                    {
                        newAndroidManifText += text + separater;
                    }
                }

                newAndroidManifText = newAndroidManifText[..^1];
            }

            if (!string.IsNullOrEmpty(newAndroidManifText))
            {
                File.WriteAllText(androidManifPath, newAndroidManifText);
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif
