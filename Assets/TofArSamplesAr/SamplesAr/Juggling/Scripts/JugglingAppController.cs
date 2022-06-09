/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TofAr.V0.Hand;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class manages the whole progress in Juggling scene.
    /// </summary>
    public class JugglingAppController : MonoBehaviour
    {
        [SerializeField]
        private JugglingHandController handController;

        [SerializeField]
        private JugglingDistanceManager distanceManager;

        [SerializeField]
        private JugglingBallManager ballManager;

        [SerializeField]
        private JugglingScoreManager scoreManager;

        private JugglingMode mode;

        public delegate void OnJugglingReadyHandler();
        public event OnJugglingReadyHandler OnJugglingReady;

        public delegate void OnJugglingStartedHandler();
        public event OnJugglingStartedHandler OnJugglingStarted;

        public delegate void OnJuggingFinihedHandler();
        public event OnJuggingFinihedHandler OnJugglingFinished;

        private void Awake()
        {
            mode = JugglingMode.Waiting;

            ballManager.OnBallDropped += OnBallDropped;
            scoreManager.OnScoreUpdated += OnScoreUpdated;
        }

        private void OnDestroy()
        {
            ballManager.OnBallDropped -= OnBallDropped;
            scoreManager.OnScoreUpdated -= OnScoreUpdated;
        }

        private void LateUpdate()
        {
            if (mode != JugglingMode.Waiting)
            {
                return;
            }

            if (distanceManager.IsOk() && handController.PalmHands())
            {
                mode = JugglingMode.Ready;
                OnJugglingReady?.Invoke();
            }
        }

        /// <summary>
        /// called when the ball was fallen without catching.
        /// </summary>
        private void OnBallDropped()
        {
            FinishJuggling();
        }

        /// <summary>
        /// called when the score was updated.
        /// </summary>
        /// <param name="ballCount">number of balls in screen</param>
        /// <param name="catchCount">number of times the ball was caught</param>
        private void OnScoreUpdated(int ballCount, int catchCount)
        {
            if (catchCount > 5 && ballCount < ballManager.GetBalls().Count)
            {
                ballManager.IncreaseBall();
                scoreManager.ResetCatchCount();
            }
        }

        /// <summary>
        /// starts juggling.
        /// </summary>
        public void StartJuggling()
        {
            mode = JugglingMode.Juggling;

            ballManager.AddFirstBall();

            OnJugglingStarted?.Invoke();
        }

        /// <summary>
        /// finishes juggling.
        /// </summary>
        private void FinishJuggling()
        {
            ballManager.ResetBalls();

            mode = JugglingMode.Finished;

            OnJugglingFinished?.Invoke();
        }

        /// <summary>
        /// retries juggling.
        /// </summary>
        public void Retry()
        {
            scoreManager.ResetScore();
            mode = JugglingMode.Waiting;
        }

        /// <summary>
        /// returns current scene mode.
        /// </summary>
        /// <returns>Juggling Mode</returns>
        public JugglingMode GetCurrentMode()
        {
            return mode;
        }
    }

    /// <summary>
    /// The scene status
    /// </summary>
    public enum JugglingMode
    {
        Waiting,
        Ready,
        Juggling,
        Finished
    }
}
