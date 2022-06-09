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
    /// This class defines the instance of juggling ball.
    /// </summary>
    public class JugglingBall : MonoBehaviour
    {
        [SerializeField]
        private float flightTime = 1.0f;

        [SerializeField]
        private float speedRate = 1.0f;

        [SerializeField]
        private float fallLimit = -2.0f;

        private const float g = -9.8f;

        private float time;

        private IEnumerator coroutine;

        private JugglingBallStatus status;

        public delegate void OnDroppedBallHandler(JugglingBall ball);
        public event OnDroppedBallHandler OnDroppedBall;

        private Material material;

        private Color[] colors = new Color[] {
            Color.white,
            Color.red,
            Color.blue,
            Color.yellow,
            Color.green,
            new Color32(255,165,0,255), //orange
            new Color32(255,102,204,255),//pink
            new Color32(51,0,102,255), //purple
            Color.cyan,
            Color.magenta
        };

        //the offset when the player holds single ball
        private Vector3 singleOffset = new Vector3(0.0f, 0.03f, 0.0f);

        private Vector3[] offsets = new Vector3[]
        {
            new Vector3(0.03f,0.03f,0.0f),
            new Vector3(-0.03f,0.03f,0.0f),
            new Vector3(0.0f,0.06f,0.0f),
            new Vector3(0.0f,0.03f,-0.03f),
            new Vector3(0.0f,0.03f,0.03f)
        };

        /// <summary>
        /// called when the ball is thrown.
        /// </summary>
        public void Thrown(Vector3 endPos)
        {
            gameObject.SetActive(true);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            time = 0f;

            coroutine = Jump(endPos, flightTime, speedRate, g);
            StartCoroutine(coroutine);
        }

        /// <summary>
        /// called when the ball is caught.
        /// </summary>
        public void Caught()
        {
            gameObject.SetActive(true);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            time = 0f;

            status = JugglingBallStatus.Holding;
        }

        /// <summary>
        /// initializes instance values.
        /// </summary>
        public void ResetBall()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            time = 0f;

            transform.position = Vector3.zero;
            status = JugglingBallStatus.Idle;

            gameObject.SetActive(false);
        }

        /// <summary>
        /// moves the ball in a parabolic pattern.
        /// </summary>
        /// <param name="endPos">terminus</param>
        /// <param name="flightTime">duration of a flight</param>
        /// <param name="speedRate">speed multiplier</param>
        /// <param name="gravity">coefficient of gravity</param>
        /// <returns></returns>
        private IEnumerator Jump(Vector3 endPos, float flightTime, float speedRate, float gravity)
        {
            status = JugglingBallStatus.Thrown;

            var startPos = transform.position;

            //Z coordinate is fixed
            startPos.z = endPos.z;

            var diffY = (endPos - startPos).y;
            var vn = (diffY - gravity * 0.5f * flightTime * flightTime) / flightTime;

            while (time < flightTime)
            {
                var p = Vector3.Lerp(startPos, endPos, time / flightTime);
                p.y = startPos.y + vn * time + 0.5f * gravity * time * time;
                transform.position = p;

                //The status will be changed "Falling" when the ball reaches the vertices.
                if (time / flightTime >= 0.5f)
                {
                    status = JugglingBallStatus.Falling;
                }

                time += (Time.deltaTime * speedRate);

                yield return null;
            }

            transform.position = endPos;

            //After reaching the endpoint, it will start free-falling unless it is grabbed.
            Vector3 origin = endPos;
            origin.y = startPos.y;

            coroutine = Fall(vn, origin);
            StartCoroutine(coroutine);
        }

        /// <summary>
        /// makes the ball free-falling.
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator Fall(float vn, Vector3 origin)
        {
            status = JugglingBallStatus.Falling;

            while (transform.position.y >= fallLimit)
            {
                Vector3 pos = transform.position;
                pos.y = origin.y + (vn * time) + (0.5f * g * Mathf.Pow(time, 2.0f));
                transform.position = pos;

                time += (Time.deltaTime * speedRate);
                yield return null;
            }

            OnDroppedBall?.Invoke(this);
        }

        /// <summary>
        /// sets the position of the ball if the player is holding ONLY ONE BALL.
        /// </summary>
        /// <param name="pos">World position of ball</param>
        public void SetPosition(Vector3 pos)
        {
            transform.position = pos + singleOffset;
        }

        /// <summary>
        /// sets the position of the ball if the player is holding SEVERAL BALLS.
        /// </summary>
        public void SetPosition(Vector3 pos, int index)
        {
            if (index >= offsets.Length)
            {
                SetPosition(pos);
                return;
            }

            transform.position = pos + offsets[index];
        }

        /// <summary>
        /// returns current position of the ball.
        /// </summary>
        /// <returns>World position of ball</returns>
        public Vector3 GetPosition()
        {
            return transform.position;
        }

        /// <summary>
        /// sets the status of the ball.
        /// </summary>
        /// <param name="status"></param>
        public void SetStatus(JugglingBallStatus status)
        {
            this.status = status;
        }

        /// <summary>
        /// returns the status of the ball.
        /// </summary>
        /// <returns>current status of the ball</returns>
        public JugglingBallStatus GetStatus()
        {
            return status;
        }

        /// <summary>
        /// This method can be called externally and makes the ball free-falling,
        /// mainly used to start falling above from the player's hand.
        /// </summary>
        public void Drop()
        {
            gameObject.SetActive(true);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            time = 0f;

            coroutine = Fall(0f, transform.position);
            StartCoroutine(coroutine);
        }

        /// <summary>
        /// sets the balls color depends on the index.
        /// </summary>
        /// <param name="index"></param>
        public void SetColor(int index)
        {
            if (material == null)
            {
                material = GetComponent<MeshRenderer>().material;
            }

            if (index < colors.Length)
            {
                material.color = colors[index];
            }

            if (colors.Length <= index)
            {
                material.color = colors[index % colors.Length];
            }
        }
    }

    /// <summary>
    /// defines the ball status.
    /// </summary>
    public enum JugglingBallStatus
    {
        Idle,
        Holding,
        Thrown,
        Falling
    }
}
