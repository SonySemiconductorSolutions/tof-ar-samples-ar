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
    ///  This class manages the balls in Juggling Scene.
    /// </summary>
    public class JugglingBallManager : MonoBehaviour
    {
        private List<JugglingBall> balls;

        [SerializeField]
        private JugglingBall ballPrefab;

        [SerializeField]
        private Transform parent;

        [SerializeField]
        private int maxBallCount;

        [SerializeField]
        private JugglingHand rightHand;

        [SerializeField]
        private JugglingHand leftHand;

        public delegate void OnBallDroppedHandler();
        public event OnBallDroppedHandler OnBallDropped;

        private void Start()
        {
            CreateBalls();
        }

        /// <summary>
        /// creates a pool of juggling ball objects.
        /// </summary>
        private void CreateBalls()
        {
            balls = new List<JugglingBall>();

            for (int i = 0; i <= maxBallCount; i++)
            {
                AddBall();
            }
        }

        /// <summary>
        /// instantiates juggling ball and adds on the pool.
        /// </summary>
        private void AddBall()
        {
            JugglingBall ball = Instantiate<JugglingBall>(ballPrefab, parent);
            ball.ResetBall();
            ball.OnDroppedBall += OnDroppedBall;
            ball.SetColor(balls.Count);
            balls.Add(ball);
        }

        /// <summary>
        /// called when the ball was fallen.
        /// </summary>
        /// <param name="ball"></param>
        private void OnDroppedBall(JugglingBall ball)
        {
            ball.ResetBall();
            OnBallDropped?.Invoke();
        }

        /// <summary>
        /// returns juggling ball instance in the pool  that is not used.
        /// </summary>
        /// <returns>JugglingBall instance</returns>
        private JugglingBall GetCurrentBall()
        {
            JugglingBall target = null;

            foreach (JugglingBall ball in balls)
            {
                if (ball.GetStatus() == JugglingBallStatus.Idle)
                {
                    target = ball;
                    break;
                }
            }

            return target;
        }

        /// <summary>
        /// sets a ball on the player's hand when the juggling is started.
        /// </summary>
        public void AddFirstBall()
        {
            IncreaseBall(HandSide.Left);
        }

        /// <summary>
        /// increase number of balls in the screen.
        /// </summary>
        public void IncreaseBall()
        {
            if (leftHand.GetBallsCount() <= rightHand.GetBallsCount())
            {
                IncreaseBall(HandSide.Left);
            }
            else
            {
                IncreaseBall(HandSide.Right);
            }
        }

        /// <summary>
        /// increase number of balls in the screen.
        /// </summary>
        /// <param name="handSide">which side of hands</param>
        public void IncreaseBall(HandSide handSide)
        {
            JugglingBall ball = GetCurrentBall();

            Vector3 pos = Vector3.zero;

            if (handSide == HandSide.Left)
            {
                pos = leftHand.GetCurrentPosition();
            }
            else
            {
                pos = rightHand.GetCurrentPosition();
            }

            //drop the ball from 0.5 meters above
            pos.y += 0.5f;

            if (ball != null)
            {
                DropBall(ball, pos);
            }
        }

        /// <summary>
        /// returns the list of balls.
        /// </summary>
        /// <returns>a List of JugglingBall objects</returns>
        public List<JugglingBall> GetBalls()
        {
            return balls;
        }

        /// <summary>
        /// returns the number of JugglingBall instances in the screen.
        /// </summary>
        /// <returns>number of balls</returns>
        public int GetActiveBallCount()
        {
            int count = 0;

            foreach (JugglingBall ball in balls)
            {
                if (ball.GetStatus() != JugglingBallStatus.Idle)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// initializes values and status of JugglingBall instances.
        /// </summary>
        public void ResetBalls()
        {
            //release balls held on both hands.
            leftHand.ReleaseBalls();
            rightHand.ReleaseBalls();

            foreach (JugglingBall ball in balls)
            {
                ball.ResetBall();
            }
        }

        /// <summary>
        /// makes a ball free falling from specified position.
        /// </summary>
        /// <param name="ball">JugglingBall instance</param>
        /// <param name="pos">specified position</param>
        private void DropBall(JugglingBall ball, Vector3 pos)
        {
            ball.gameObject.SetActive(true);
            ball.SetPosition(pos);
            ball.Drop();
        }
    }
}
