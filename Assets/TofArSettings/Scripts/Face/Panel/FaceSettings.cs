/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Linq;
using TofAr.V0.Face;
using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Face
{
    public class FaceSettings : UI.SettingsBase
    {
        FaceRuntimeController runtimeController;
        FaceEstimatorController faceEstimatorController;
        FaceManagerController managerController;

        UI.ItemDropdown itemMode, itemRuntimeMode, itemProcessModeLandmark;
        UI.ItemSlider itemThread;
        UI.ItemToggle itemStartStream;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIStartStream,
                MakeUIDetectorType,
                MakeUIRuntime
            };

            runtimeController = FindObjectOfType<FaceRuntimeController>();
            controllers.Add(runtimeController);
            faceEstimatorController = FindObjectOfType<FaceEstimatorController>();
            controllers.Add(faceEstimatorController);
            managerController = FindObjectOfType<FaceManagerController>();
            controllers.Add(managerController);

            base.Start();

            settings.OnChangeStart += OnChangePanel;
        }

        /// <summary>
        /// Make Body dictionary UI
        /// </summary>
        void MakeUIDetectorType()
        {
            if (runtimeController.DetectorTypeNames.Length > 0)
            {
                itemMode = settings.AddItem("Detector Type", runtimeController.DetectorTypeNames,
                runtimeController.DetectorTypeIndex, ChangeMode);

                runtimeController.OnChangeDetectorType += (index) =>
                {
                    itemMode.Index = index;
                    SetExternalInteractability();
                };
            }
            else
            {
                itemMode = settings.AddItem("Detector Type", new string[1] { "-" }, 0, (idx) => { });
                itemMode.Interactable = false;
            }
        }

        /// <summary>
        /// Change Body dictionary
        /// </summary>
        /// <param name="index">Hand dictionary index</param>
        void ChangeMode(int index)
        {
            runtimeController.DetectorTypeIndex = index;
        }

        /// <summary>
        /// Make Runtime UI
        /// </summary>
        void MakeUIRuntime()
        {
            if (!runtimeController.DetectorTypeList.Contains(TofAr.V0.Face.FaceDetectorType.External))
            {
                return;
            }

            itemRuntimeMode = settings.AddItem("Face Runtime", faceEstimatorController.ExecModeNames,
                faceEstimatorController.ExecModeIndex, ChangeRuntimeMode);
            itemThread = settings.AddItem("Face Threads",
                FaceEstimatorController.ThreadMin, FaceEstimatorController.ThreadMax,
                FaceEstimatorController.ThreadStep, faceEstimatorController.ModeThreads,
                ChangeThreads);

            faceEstimatorController.OnChangeRuntimeMode += (index) =>
            {
                itemRuntimeMode.Index = index;
                itemThread.Interactable = faceEstimatorController.IsInteractableModeThreads;
            };

            faceEstimatorController.OnUpdateRuntimeModeList += (list, runtimeModeIndex) =>
            {
                itemRuntimeMode.Options = list;
                itemRuntimeMode.Index = runtimeModeIndex;
            };

            faceEstimatorController.OnChangeModeThreads += (val) =>
            {
                itemThread.Value = val;
            };

            itemProcessModeLandmark = settings.AddItem("Process Mode\nLandmark", faceEstimatorController.ProcessModeNames,
            faceEstimatorController.ProcessModeLandmarkIndex, ChangeProcessModeLandmark, -2);

            faceEstimatorController.OnUpdateProcessModeLandmarkList += (list, detectionTypeIndex) =>
            {
                itemProcessModeLandmark.Options = list;
                itemProcessModeLandmark.Index = detectionTypeIndex;
            };

            faceEstimatorController.OnChangeModelType += (index) =>
            {
                itemProcessModeLandmark.Index = index;
            };

            SetExternalInteractability();
        }

        /// <summary>
        /// Enable or disable the interactability of external UI based on the runtime controller
        /// </summary>
        void SetExternalInteractability()
        {
            var interactable = runtimeController.DetectorTypeNames.Length > 0 && runtimeController.DetectorType == TofAr.V0.Face.FaceDetectorType.External;
            itemRuntimeMode.Interactable = interactable;
            if (itemProcessModeLandmark)
            {
                itemProcessModeLandmark.Interactable = interactable;
            }
            itemThread.Interactable = interactable && faceEstimatorController.IsInteractableModeThreads;
        }

        /// <summary>
        /// Change RuntimeMode1
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeRuntimeMode(int index)
        {
            faceEstimatorController.ExecModeIndex = index;
        }

        /// <summary>
        /// Change ProcessMode
        /// </summary>
        /// <param name="index">ProcessMode index</param>
        void ChangeProcessModeLandmark(int index)
        {
            faceEstimatorController.ProcessModeLandmarkIndex = index;
        }

        /// <summary>
        /// Change value of Threads of RuntimeMode1
        /// </summary>
        /// <param name="val">Thread count</param>
        void ChangeThreads(float val)
        {
            faceEstimatorController.ModeThreads = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Make StartStream UI
        /// </summary>
        void MakeUIStartStream()
        {
            itemStartStream = settings.AddItem("Start Stream", TofArFaceManager.Instance.autoStart, ChangeStartStream);

            if (runtimeController.DetectorTypeNames.Length <= 0)
            {
                itemStartStream.Interactable = false;
            }

            managerController.OnStreamStartStatusChanged += (val) =>
            {
                itemStartStream.OnOff = val;
            };
        }

        /// <summary>
        /// If stream oocurs or not
        /// </summary>
        /// <param name="val">Stream started or not</param>
        void ChangeStartStream(bool val)
        {
            if (val)
            {
                managerController.StartStream();
            }
            else
            {
                managerController.StopStream();
            }
        }

        /// <summary>
        /// Event called when the state of the panel changes
        /// </summary>
        /// <param name="onOff">open/close</param>
        void OnChangePanel(bool onOff)
        {
            if (onOff)
            {
                itemStartStream.OnOff = TofArFaceManager.Instance.IsStreamActive;
            }
        }
    }
}
