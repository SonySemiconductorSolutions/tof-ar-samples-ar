/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Modeling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

namespace TofArARSamples.SimpleARFoundation
{
    public class ModelingSettings : TofArSettings.UI.SettingsBase
    {
        ModelingManagerController managerController;
        ModelingRuntimeController runtimeController;

        TofArSettings.UI.ItemToggle itemStartStream, itemEstimateSurface, itemConfidenceCorrection;

        TofArSettings.UI.ItemSlider itemEstimateInterval, itemConfidenceCorrectionThresh, itemDepthScale, itemUpdateInterval;

        ARMeshManager arMeshManager;

        protected override void Start()
        {
            arMeshManager = FindObjectOfType<ARMeshManager>(true);

            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIStartStream,
                MakeUIRuntimeSettings
            };
            managerController = FindObjectOfType<ModelingManagerController>();
            controllers.Add(managerController);

            runtimeController = FindObjectOfType<ModelingRuntimeController>();
            controllers.Add(runtimeController);

            base.Start();
        }

        void MakeUIStartStream()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                itemStartStream = settings.AddItem("Start Stream", arMeshManager.enabled, ChangeStartStream);
            } 
            else
            {
                itemStartStream = settings.AddItem("Start Stream", TofArModelingManager.Instance.autoStart, ChangeStartStream);
            }
            managerController.OnStreamStartStatusChanged += (val) =>
            {
                itemStartStream.OnOff = val;
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
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

        void MakeUIRuntimeSettings()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return;
            }
            
            itemEstimateInterval = settings.AddItem("Estimate Interval", 1, 10, 1, TofArModelingManager.Instance.EstimateInterval, ChangeEstimateInterval, -2);
            runtimeController.OnChangeEstimateInterval += (val) =>
            {
                itemEstimateInterval.Value = val;
            };

            itemUpdateInterval = settings.AddItem("Update Interval", 1, 10, 1, TofArModelingManager.Instance.UpdateInterval, ChangeUpdateInterval, -2);
            runtimeController.OnChangeUpdateInterval += (val) =>
            {
                itemUpdateInterval.Value = val;
            };

            itemDepthScale = settings.AddItem("Depth Scale", 0.001f, 1f, 0.001f, TofArModelingManager.Instance.DepthScale, ChangeDepthScale, -1);
            runtimeController.OnChangeDepthScale += (val) =>
            {
                itemDepthScale.Value = val;
            };

            itemEstimateSurface = settings.AddItem("Estimate Surface", TofArModelingManager.Instance.EstimateUpdatedSurface, ChangeEstimateUpdatedSurface);
            runtimeController.OnChangeEstimateUpdatedSurface += (val) =>
            {
                itemEstimateSurface.OnOff = val;
            };

            itemConfidenceCorrection = settings.AddItem("Confidence Correction", TofArModelingManager.Instance.EnableConfidenceCorrection, ChangeConfidenceCorrection);
            runtimeController.OnChangeEnableConfidenceCorrection += (val) =>
            {
                itemConfidenceCorrection.OnOff = val;
                itemConfidenceCorrectionThresh.Interactable = val;
            };


            itemConfidenceCorrectionThresh = settings.AddItem("Conf. Corr.\nThreshold", 0, 255, 1, TofArModelingManager.Instance.ConfidenceCorrectionThreshold, ChangeConfidenceCorrectionThresh, -1);
            runtimeController.OnChangeConfidenceCorrectionThreshold += (val) =>
            {
                itemConfidenceCorrectionThresh.Value = val;
            };

            itemConfidenceCorrectionThresh.Interactable = TofArModelingManager.Instance.EnableConfidenceCorrection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        void ChangeEstimateInterval(float val)
        {
            runtimeController.EstimateInterval = (uint) val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        void ChangeUpdateInterval(float val)
        {
            runtimeController.UpdateInterval = (uint)val;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        void ChangeConfidenceCorrectionThresh(float val)
        {
            runtimeController.ConfidenceCorrectionThreshold = (ushort)val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        void ChangeDepthScale(float val)
        {
            runtimeController.DepthScale = val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        void ChangeEstimateUpdatedSurface(bool val)
        {
            runtimeController.EstimateUpdatedSurface = val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        void ChangeConfidenceCorrection(bool val)
        {
            runtimeController.EnableConfidenceCorrection = val;
        }

    }
}
