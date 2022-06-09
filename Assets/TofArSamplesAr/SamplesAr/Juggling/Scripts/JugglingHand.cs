/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TofAr.V0.Tof;
using TofAr.V0.Color;
using TofAr.V0.Hand;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class defines the hand object in juggling scene.
    /// </summary>
    public class JugglingHand : MonoBehaviour
    {
        private float catchBallRange = 0.1f;

        private float throwRange = 0.05f;

        [SerializeField]
        private JugglingBallManager ballManager;

        [SerializeField]
        private HandSide handSide;

        [SerializeField]
        private JugglingHand anotherHand;

        [SerializeField]
        JugglingHandController handController;

        private List<Vector3> positions;

        private Vector3 currentPos;

        List<JugglingBall> balls;

        public delegate void OnBallThrewHandler(float throwPower);
        public event OnBallThrewHandler OnBallThrew;

        public delegate void OnBallCaughtHandler(HandSide handSide, JugglingBall ball);
        public event OnBallCaughtHandler OnBallCaught;

        private int currentFrameCount;
        private int frameCount;

        private void OnEnable()
        {
            currentFrameCount = 0;
            frameCount = 0;

            balls = new List<JugglingBall>();

            positions = new List<Vector3>();
        }

        private void LateUpdate()
        {
            CheckBalls();

            SetBallPos();
        }

        /// <summary>
        /// sets coordinate of the hand. 
        /// </summary>
        public void SetHandPos(Vector3 pos)
        {
            if (positions == null)
            {
                return;
            }

            currentPos = GetTranslatedPosition(pos);
            positions.Add(currentPos);

            AddFrame();
        }

        /// <summary>
        /// add the frame data of TofArHandManager.OnFrameArrived.
        /// </summary>
        private void AddFrame()
        {
            if (frameCount == 0)
            {
                float fps = handController.GetHandFrameRate();

                float i = fps * 0.2f;

                frameCount = Mathf.FloorToInt(i);
            }

            currentFrameCount++;

            if (currentFrameCount >= frameCount)
            {
                CheckPositions();
                positions.Clear();
                currentFrameCount = 0;
                frameCount = 0;
            }
        }

        /// <summary>
        /// tracks the coordinates of the ball being grasped to the center of the hand.
        /// </summary>
        private void SetBallPos()
        {
            if (balls.Count == 1)
            {
                balls[0].SetPosition(currentPos);
            }
            else
            {
                for (int i = 0; i < balls.Count; i++)
                {
                    balls[i].SetPosition(currentPos, i);
                }
            }
        }

        /// <summary>
        /// checks the contents of the most recently acquired array of coordinates.
        /// </summary>
        private void CheckPositions()
        {
            Vector3 v = Vector3.zero;

            if (positions.Count == 0)
            {
                return;
            }

            float firstY = positions[0].y;

            float maxY = firstY;

            foreach (Vector3 pos in positions)
            {
                if (pos.y > maxY)
                {
                    maxY = pos.y;
                }
            }

            float diffY = maxY - firstY;

            //throw the ball If the direction of the vector is positive to the Y-axis and within the threshold 
            if (diffY >= throwRange)
            {
                ThrowBall();
                OnBallThrew?.Invoke(diffY);
            }
        }

        /// <summary>
        /// checks to see if there is a ball that can be grabbed.
        /// </summary>
        private void CheckBalls()
        {
            List<JugglingBall> allBalls = ballManager.GetBalls();

            foreach (JugglingBall ball in allBalls)
            {
                if (ball.GetStatus() == JugglingBallStatus.Falling)
                {
                    Vector3 ballPos = ball.GetPosition();

                    float distance = Vector3.Distance(currentPos, ballPos);

                    if (distance <= catchBallRange)
                    {
                        CatchBall(ball);
                        ball.Caught();
                    }

                }
            }
        }

        /// <summary>
        /// throws the ball.
        /// </summary>
        public void ThrowBall()
        {
            if (balls.Count == 0)
            {
                return;
            }

            //the ball is not thrown if both hand coordinates are not taken
            if (handController.GetHandStatus() != HandStatus.BothHands)
            {
                return;
            }

            balls[0].Thrown(anotherHand.GetCurrentPosition());
            balls.RemoveAt(0);

            positions.Clear();
            currentFrameCount = 0;
            frameCount = 0;
        }

        /// <summary>
        /// grabs the ball.
        /// </summary>
        /// <param name="ball"></param>
        public void CatchBall(JugglingBall ball)
        {
            balls.Add(ball);
            ball.Caught();
            OnBallCaught?.Invoke(this.handSide, ball);

            //Reset the coordinate information that had been acquired to prevent throwing at the moment of grabbing
            positions.Clear();
            currentFrameCount = 0;
            frameCount = 0;
        }

        /// <summary>
        /// releases the balls in the hand when the juggling is finished.
        /// </summary>
        public void ReleaseBalls()
        {
            balls.Clear();
        }

        /// <summary>
        /// returns number of balls holding.
        /// </summary>
        /// <returns></returns>
        public int GetBallsCount()
        {
            if (balls == null)
            {
                return 0;
            }

            return balls.Count;
        }

        /// <summary>
        /// returns current handCenter position.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCurrentPosition()
        {
            return currentPos;
        }

        /// <summary>
        /// returns the conversion from local to world coordinates.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private Vector3 GetTranslatedPosition(Vector3 pos)
        {
            Vector3 translatedPosition = Vector3.zero;

            var localMatrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
            var worldMatrix = transform.localToWorldMatrix * localMatrix;
            translatedPosition = new Vector3(worldMatrix[0, 3], worldMatrix[1, 3], worldMatrix[2, 3]);

            return translatedPosition;
        }

        /// <summary>
        /// sets a threshold for the distance at which the ball can be caught.
        /// </summary>
        /// <param name="catchBallRange"></param>
        public void SetCatchBallRange(float catchBallRange)
        {
            this.catchBallRange = catchBallRange;
        }

        /// <summary>
        /// sets the threshold at which the ball can be thrown.
        /// </summary>
        public void SetThrowRange(float throwRange)
        {
            this.throwRange = throwRange;
        }
    }

    /// <summary>
    /// defines which side, right hand-side or left.
    /// </summary>
    public enum HandSide
    {
        Left, Right
    }
}
