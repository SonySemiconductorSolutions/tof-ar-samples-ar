/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class manages the score of juggling scene.
    /// </summary>
    public class JugglingScoreManager : MonoBehaviour
    {
        [SerializeField]
        private JugglingHand leftHand;

        [SerializeField]
        private JugglingHand rightHand;

        [SerializeField]
        private JugglingBallManager ballManager;

        private int ballCount;

        private int catchCount;

        public delegate void OnScoreUpdatedHandler(int ballCount, int catchCount);
        public event OnScoreUpdatedHandler OnScoreUpdated;

        private void Awake()
        {
            leftHand.OnBallCaught += OnBallCaught;
            rightHand.OnBallCaught += OnBallCaught;
        }

        private void OnDestroy()
        {
            leftHand.OnBallCaught -= OnBallCaught;
            rightHand.OnBallCaught -= OnBallCaught;
        }

        /// <summary>
        /// called when the player caught a ball.
        /// </summary>
        /// <param name="handSide"></param>
        /// <param name="ball"></param>
        private void OnBallCaught(HandSide handSide, JugglingBall ball)
        {
            IncreaseScore();
        }

        /// <summary>
        /// resets the score.
        /// </summary>
        public void ResetScore()
        {
            ballCount = 0;
            catchCount = 0;
        }

        /// <summary>
        /// resets count the player caught.
        /// </summary>
        public void ResetCatchCount()
        {
            ballCount = ballManager.GetActiveBallCount();
            catchCount = 0;
            OnScoreUpdated?.Invoke(ballCount, catchCount);
        }

        /// <summary>
        /// increases catch count score.
        /// </summary>
        private void IncreaseScore()
        {
            ballCount = ballManager.GetActiveBallCount();
            catchCount++;
            OnScoreUpdated?.Invoke(ballCount, catchCount);
        }

        /// <summary>
        /// sets the score directly. Currently this method is not called from any classes.
        /// </summary>
        /// <param name="ballCount"></param>
        /// <param name="catchCount"></param>
        private void UpdateScore(int ballCount, int catchCount)
        {
            this.ballCount = ballCount;
            this.catchCount = catchCount;
            OnScoreUpdated?.Invoke(ballCount, catchCount);
        }

        /// <summary>
        /// gets current score.
        /// </summary>
        /// <param name="ballCount"></param>
        /// <param name="catchCount"></param>
        public void GetScore(out int ballCount, out int catchCount)
        {
            ballCount = this.ballCount;
            catchCount = this.catchCount;
        }
    }
}
