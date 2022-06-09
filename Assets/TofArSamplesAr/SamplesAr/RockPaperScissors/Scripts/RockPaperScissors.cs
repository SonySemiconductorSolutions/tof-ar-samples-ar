/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using TofArSettings.Hand;
using UnityEngine;

namespace TofArARSamples.RockPaperScissors
{
    /// <summary>
    /// Connection class with SDK of rock-paper-scissors app
    /// </summary>
    public class RockPaperScissors : MonoBehaviour
    {
        private RecogModeController recogModeController = null;

        private RpsAppProgressManager rpsAppProgressManager = null;

        private PoseIndex poseLeft = PoseIndex.None;

        private PoseIndex poseRight = PoseIndex.None;

        void Awake()
        {
            recogModeController = FindObjectOfType<RecogModeController>();
            rpsAppProgressManager = FindObjectOfType<RpsAppProgressManager>();
        }

        void OnEnable()
        {
            TofArHandManager.OnStreamStarted += OnStreamStarted;
            TofArHandManager.OnFrameArrived += HandFrameArrived;
        }

        void OnDisable()
        {
            TofArHandManager.OnStreamStarted -= OnStreamStarted;
            TofArHandManager.OnFrameArrived -= HandFrameArrived;
        }

        /// <summary>
        /// Event called at the start of the stream
        /// </summary>
        /// <param name="sender"></param>
        void OnStreamStarted(object sender) {}

        /// <summary>
        /// Event called when Hand data is updated
        /// </summary>
        /// <param name="sender">TofArHandManager</param>
        private void HandFrameArrived(object sender)
        {
            var manager = sender as TofArHandManager;
            if (manager == null)
            {
                return;
            }

            manager.HandData.GetPoseIndex(out poseLeft, out poseRight);
            rpsAppProgressManager.PoseReflection(poseLeft, poseRight);
        }
    }
}
