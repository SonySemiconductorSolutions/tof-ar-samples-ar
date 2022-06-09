/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TofAr.V0.Face;
using TofAr.V0.Hand;
using TofAr.V0.Color;
using System.Threading;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class manages UI displayed when camera distance from face and hands.
    /// </summary>
    public class JugglingMeasureMenu : MonoBehaviour, IJugglingMenu
    {
        [SerializeField]
        private RectTransform panel;

        [SerializeField]
        private Text measureText;

        [SerializeField]
        private JugglingDistanceManager distanceManager;

        private SynchronizationContext context;

        private bool usingFrontCamera = false;

        private void Awake()
        {
            context = SynchronizationContext.Current;

#if UNITY_ANDROID
            TofArFaceManager.OnFrameArrived += OnFrameArrived;
#endif

            TofArHandManager.OnFrameArrived += OnFrameArrived;

            TofArColorManager.OnStreamStarted += OnColorStreamStarted;
        }

        private void OnDestroy()
        {

#if UNITY_ANDROID
            TofArFaceManager.OnFrameArrived -= OnFrameArrived;
#endif

            TofArHandManager.OnFrameArrived -= OnFrameArrived;


            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
        }

        /// <summary>
        /// Called when TofArColorManager has started a stream
        /// </summary>
        /// <param name="sender">TofArColorManager</param>
        /// <param name="colorTexture">Color texture</param>
        private void OnColorStreamStarted(object sender, Texture2D colorTexture)
        {
            var currentConfig = TofArColorManager.Instance.GetProperty<ResolutionProperty>();
            if (currentConfig != null)
            {
                usingFrontCamera = currentConfig.lensFacing == 0;
            }
        }

        /// <summary>
        /// called when a TofArHandManager frame arrived that includes hand data.
        /// </summary>
        /// <param name="sender">TofArHandManager</param>
        private void OnFrameArrived(object sender)
        {
            context.Post((d) =>
            {
                ReloadText();
            }, null);
        }

        /// <summary>
        /// displays the menu.
        /// </summary>
        public void OpenMenu()
        {
            panel.gameObject.SetActive(true);
        }

        /// <summary>
        /// hides the menu.
        /// </summary>
        public void CloseMenu()
        {
            panel.gameObject.SetActive(false);
        }

        /// <summary>
        /// updates the text description.
        /// </summary>
        private void ReloadText()
        {
            string description = "";

            if (distanceManager.IsOk())
            {
                description = "Please open your hands.";
            }
            else
            {
                description = GetRequiredDistanceMessage();
            }

            measureText.text = description;
        }

        /// <summary>
        /// returns the description how much more or less distance is needed to start.
        /// </summary>
        /// <returns></returns>
        private string GetRequiredDistanceMessage()
        {
            StringBuilder sb = new StringBuilder();

            distanceManager.GetRequiredDistance(out float requiredFace, out float requiredHandLeft, out float requiredHandRight);

#if UNITY_IOS
            if (usingFrontCamera)
            {
                //face
                sb.Append(GetMessageText(requiredFace, "face"));
                sb.AppendLine();
            }
#endif

            //left hand
            sb.Append(GetMessageText(requiredHandLeft, "left hand"));
            sb.AppendLine();

            //right hand
            sb.Append(GetMessageText(requiredHandRight, "right hand"));

            return sb.ToString();
        }

        /// <summary>
        /// returns text of hand and face status.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private string GetMessageText(float val, string target)
        {
            string msg = "";

            if (val == 0)
            {
                msg = string.Format("The {0} is no tracking.", target);
            }

            if (val > 0)
            {
                msg = string.Format("Keep your {0} {1, -4:0.00}cm away.", target, GetCentimiter(val));
            }

            if (val < 0)
            {
                msg = string.Format("Bring your {0} {1, -4:0.00}cm closer.", target, GetCentimiter(val));
            }

            //the value returned is 100 if it is tracked successfully.
            if (val == 100f)
            {
                msg = string.Format("The {0} is OK.", target);
            }

            return msg;
        }

        /// <summary>
        /// returns value converted to centimeters.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private float GetCentimiter(float val)
        {
            return Mathf.Floor((Mathf.Abs(val) * 100f));
        }
    }
}
