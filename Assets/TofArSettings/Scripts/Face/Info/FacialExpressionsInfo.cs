/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Text;
using System.Threading;
using TofAr.V0.Face;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.Face
{
    public class FacialExpressionsInfo : FaceInfo
    {
        Text txtExpression;
        SynchronizationContext context;

        protected override void Awake()
        {
            base.Awake();

            txtExpression = GetComponent<Text>();
        }

        void OnEnable()
        {
            TofArFacialExpressionEstimator.OnFacialExpressionEstimated += OnFacialExpressionEstimated;
            ShowDisableText();
        }

        void OnDisable()
        {
            TofArFacialExpressionEstimator.OnFacialExpressionEstimated -= OnFacialExpressionEstimated;
        }

        void Start()
        {
            context = SynchronizationContext.Current;

            ShowDisableText();
        }

        /// <summary>
        /// Facial expression estimation result
        /// </summary>
        /// <param name="result"></param>
        private void OnFacialExpressionEstimated(float[] result)
        {
            // Show
            context.Post((s) =>
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Facial Expressions");
                sb.AppendLine($"A: {string.Format("{0:0.000}", result[(int)FacialExpression.Japanese_A])}");
                sb.AppendLine($"I: {string.Format("{0:0.000}", result[(int)FacialExpression.Japanese_I])}");
                sb.AppendLine($"U: {string.Format("{0:0.000}", result[(int)FacialExpression.Japanese_U])}");
                sb.AppendLine($"E: {string.Format("{0:0.000}", result[(int)FacialExpression.Japanese_E])}");
                sb.AppendLine($"O: {string.Format("{0:0.000}", result[(int)FacialExpression.Japanese_O])}");
                sb.AppendLine($"EyeOpen: {string.Format("{0:0.000}", result[(int)FacialExpression.EyeOpen])}");
                sb.Append($"BrowUp: {string.Format("{0:0.000}", result[(int)FacialExpression.BrowUp])}");

                txtExpression.text = sb.ToString();
            }, null);
        }

        void ShowDisableText()
        {
            /*if (facialCtrl.OnOff)
            {
                return;
            }*/

        }
    }
}
