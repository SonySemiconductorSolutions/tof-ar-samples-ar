/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;
using TofAr.V0.Tof;
using System.Text;
using System.Linq;

namespace TofArSamples.Startup
{
    public class SupportChecker : MonoBehaviour
    {
        [SerializeField]
        private GameObject uiPanel;

        private static SupportChecker instance;

        string[] supportedDevicesIphone =
        {
            "X",
            "XS",
            "XR",
            "11",
            "12",
            "13",
            "14"
        };

        string[] supportedDevicesIpadPro=
        {
            "11-inch",
            "12.9-inch (3rd Gen or later)",
        };


        private void Awake()
        {
            DontDestroyOnLoad(this);

            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            if (uiPanel != null)
            {
                var messageBox = uiPanel.transform.GetChild(0);
                if (messageBox != null)
                {
                    var messageBoxRectTransform = messageBox.GetComponent<RectTransform>();
                    var txtUnsupportedMessage = messageBox.transform.Find("MessageText").gameObject.GetComponent<Text>();
                    var buttonOk = messageBox.GetComponentInChildren<Button>();
                    
                    SetUnsupportedMessage(txtUnsupportedMessage);

                    AdjustHeight(messageBoxRectTransform, txtUnsupportedMessage, buttonOk);
                }
            }

        }

        private void SetUnsupportedMessage(Text txtUnsupportedMessage)
        {
            if (txtUnsupportedMessage != null)
            {
                var sb = new StringBuilder();
                sb.Append("This model is not supported.");

                if (TofAr.V0.TofArManager.Instance.UsingIos)
                {
                    sb.Append($"\n\n<size={(txtUnsupportedMessage.fontSize - 4)}>Supported models are");
                    if (supportedDevicesIphone.Length > 0)
                    {
                        sb.Append("\niPhone");
                        foreach (var iphone in supportedDevicesIphone)
                        {
                            string delimiter = iphone != supportedDevicesIphone.Last() ? "," : "";
                            sb.Append($" {iphone}{delimiter}");
                        }
                    }
                    if (supportedDevicesIpadPro.Length > 0)
                    {
                        sb.Append(",\niPad Pro");
                        foreach (var ipad in supportedDevicesIpadPro)
                        {
                            string delimiter = ipad != supportedDevicesIpadPro.Last() ? "," : "";
                            sb.Append($" {ipad}{delimiter}");
                        }
                    }
                    sb.Append(".</size>");
                }

                txtUnsupportedMessage.text = sb.ToString();
            }
        }

        private void AdjustHeight(RectTransform messageBoxRectTransform, Text txtUnsupportedMessage, Button buttonOk)
        {
            if (txtUnsupportedMessage != null && buttonOk != null && messageBoxRectTransform != null)
            {
                var height = txtUnsupportedMessage.preferredHeight;
                var textRectTransform = txtUnsupportedMessage.GetComponent<RectTransform>();

                const int paddingText = 40;
                var textSizeDelta = textRectTransform.sizeDelta;
                textSizeDelta.y = height + paddingText + paddingText;
                textRectTransform.sizeDelta = textSizeDelta;

                var buttonRectTransform = buttonOk.GetComponent<RectTransform>();

                const int gap = 10;
                float posTop = -textRectTransform.anchoredPosition.y;
                float posBottom = buttonRectTransform.anchoredPosition.y;
                float buttonHeight = buttonRectTransform.sizeDelta.y;
                float padding = posTop + posBottom + gap + buttonHeight;
                var uiPanelSizeDelta = messageBoxRectTransform.sizeDelta;
                uiPanelSizeDelta.y = textSizeDelta.y + padding;
                messageBoxRectTransform.sizeDelta = uiPanelSizeDelta;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (uiPanel != null)
            {
                if (!TofArTofManager.Instance.CheckDevice())
                {
                    // show unsupported message
                    uiPanel.SetActive(true);
                }
            }
        }
    }
}
