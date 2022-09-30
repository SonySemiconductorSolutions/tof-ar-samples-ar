/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class RecordStatusPanel : Panel
    {
        /// <summary>
        /// Font size when displaying numbers
        /// </summary>
        [SerializeField]
        int numFontSize = 96;

        [SerializeField]
        float resultMsgTime = 1.5f;

        [SerializeField]
        float iconBlinkTime = 1;

        Text txt;
        RawImage rawImg;

        RecordController[] recCtrls;

        int fontSize;
        UnityEngine.Color fontColor;

        IEnumerator countDown, recording, end;

        protected override void Awake()
        {
            base.Awake();
            recCtrls = FindObjectsOfType<RecordController>();
        }

        void OnEnable()
        {
            foreach (RecordController recCtrl in recCtrls)
            {
                recCtrl.OnChangeStatus += OnChangeRecStatus;
                recCtrl.OnEndExec += OnEndRec;
            }
        }

        void OnDisable()
        {
            if (recCtrls.Length > 0)
            {
                foreach (RecordController recCtrl in recCtrls)
                {
                    recCtrl.OnChangeStatus -= OnChangeRecStatus;
                    recCtrl.OnEndExec -= OnEndRec;
                }
            }
        }
        protected override void Start()
        {
            txt = GetComponentInChildren<Text>();
            fontSize = txt.fontSize;
            fontColor = txt.color;
            rawImg = GetComponentInChildren<RawImage>();
            rawImg.enabled = false;

            base.Start();
        }

        /// <summary>
        /// Event that is caled when recording status is changed
        /// </summary>
        /// <param name="status">Recording status</param>
        void OnChangeRecStatus(RecordController.RecStatus status)
        {
            switch (status)
            {
                case RecordController.RecStatus.Start:
                    // Show panel
                    if (!IsOpen)
                    {
                        OpenPanel(false);
                    }
                    break;

                case RecordController.RecStatus.Waiting:
                    if (countDown == null)
                    {
                        countDown = CountDown();
                        StartCoroutine(countDown);
                    }
                    break;

                case RecordController.RecStatus.Recording:
                    if (recording == null)
                    {
                        recording = Recording();
                        StartCoroutine(recording);
                    }
                    break;

                case RecordController.RecStatus.Stop:

                    if (countDown != null)
                    {
                        StopCoroutine(countDown);
                        countDown = null;
                    }

                    if (recording != null)
                    {
                        StopCoroutine(recording);
                        recording = null;
                    }

                    ResetDesign();
                    break;
            }
        }

        /// <summary>
        /// Event that is called when recording is finished
        /// </summary>
        /// <param name="result">Recording success/fail</param>
        /// <returns>Save path</returns>
        void OnEndRec(bool result, string filePath)
        {
            if (end != null)
            {
                StopCoroutine(end);
                end = null;

            }

            end = End(result);
            StartCoroutine(end);
            
        }

        /// <summary>
        /// Show countdown
        /// </summary>
        IEnumerator CountDown()
        {
            // Enlarge font size
            txt.fontSize = numFontSize;

            while (true)
            {
                bool recStateWaite = false;

                foreach (RecordController recCtrl in recCtrls)
                {
                    if (recCtrl.Status == RecordController.RecStatus.Waiting)
                    {
                        // Show countdown
                        int time = Mathf.CeilToInt(recCtrl.TimeRemaining);
                        txt.text = time.ToString();

                        fontColor.a = recCtrl.TimeRemaining - (time - 1);
                        txt.color = fontColor;

                        recStateWaite = true;

                        break;
                    }
                }

                yield return null;

                if (!recStateWaite)
                {
                    break;
                }
            }

            countDown = null;
        }

        /// <summary>
        /// Show recording
        /// </summary>
        IEnumerator Recording()
        {
            txt.text = string.Empty;

            while (true)
            {
                bool recStateRecord = false;

                foreach (RecordController recCtrl in recCtrls)
                {
                    if (recCtrl.Status == RecordController.RecStatus.Recording)
                    {
                        // Make icons flash/blink
                        rawImg.enabled = !rawImg.enabled;
                        yield return new WaitForSeconds(iconBlinkTime);

                        recStateRecord = true;

                        break;
                    }
                }

                if (!recStateRecord)
                {
                    break;
                }
            }

            rawImg.enabled = false;

            recording = null;
        }

        /// <summary>
        /// Show recording finished
        /// </summary>
        /// <param name="result">Recording success/fail</param>
        IEnumerator End(bool result)
        {
            ResetDesign();

            yield return new WaitForEndOfFrame();

            txt.text = (result) ? "Succeed in save." : "Failed in save.";

            // Close after waiting for a certain period of time
            float delTime = 0;
            while (delTime < resultMsgTime)
            {
                bool recStateChange = false;

                foreach (RecordController recCtrl in recCtrls)
                {
                    if (recCtrl.Status != RecordController.RecStatus.Stop)
                    {
                        recStateChange = true;
                        break;
                    }
                }

                // Do not close if next recording is started
                if (recStateChange)
                {
                    yield break;
                }

                delTime += Time.deltaTime;
                yield return null;
            }

            ClosePanel();

            end = null;
        }

        /// <summary>
        /// Reset appearance
        /// </summary>
        void ResetDesign()
        {
            txt.text = string.Empty;
            fontColor.a = 1;
            txt.color = fontColor;
            txt.fontSize = fontSize;

            rawImg.enabled = false;
        }
    }
}
