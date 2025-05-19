/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.UI
{
    public abstract class SettingsBase : MonoBehaviour
    {
        /// <summary>
        /// Supported component types
        /// </summary>
        public ComponentType CompoType;

        /// <summary>
        /// Title icon
        /// </summary>
        public Texture TitleIcon = null;

        /// <summary>
        /// Icon color
        /// </summary>
        public UnityEngine.Color IconColor;

        /// <summary>
        /// Title
        /// </summary>
        public string Title = string.Empty;

        /// <summary>
        /// Button to open/close
        /// </summary>
        [SerializeField]
        protected ToolButton toolBtn = null;

        /// <summary>
        /// Use top right back button or not
        /// </summary>
        [SerializeField]
        bool enableBack = true;

        /// <summary>
        /// Distance between toolbar and Panel
        /// </summary>
        [SerializeField]
        float offset = 20;

        /// <summary>
        /// Component type
        /// </summary>
        public enum ComponentType : int
        {
            None,
            Color,
            Tof,
            Hand,
            Segmentation,
            Body,
            Mesh,
            FingerTouch,
            Slam,
            Face,
            Plane
        }

        /// <summary>
        /// Event that is called when back button is pressed
        /// </summary>
        public UnityAction OnBack;

        public UnityAction OnOpened;
        public UnityAction OnClosed;

        protected SettingsPanel settings;
        protected List<ControllerBase> controllers = new List<ControllerBase>();

        /// <summary>
        /// Execution order of UI creation function
        /// </summary>
        protected UnityAction[] uiOrder;

        /// <summary>
        /// Alpha value when making the line color translucent
        /// </summary>
        protected const byte lineAlpha = 80;

        SettingsPrefabManager prefabMgr;

        /// <summary>
        /// Called after application startup (after Awake) (Unity standard function)
        /// </summary>
        protected virtual void Start()
        {
            prefabMgr = SettingsPrefabManager.Instance;

            // Create Settings Panel
            var panelObj = Instantiate(prefabMgr.PanelPrefab, transform);
            panelObj.name = $"Panel_{Title}";
            settings = panelObj.GetComponent<SettingsPanel>();

            // Create title
            settings.Init(TitleIcon, IconColor, Title, toolBtn,
                enableBack, offset, OnClickBack);

            StartCoroutine(WaitAndMakeUI());
        }

        /// <summary>
        /// Called when object is destroyed (Unity standard function)
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (settings)
            {
                Destroy(settings.gameObject);
                settings = null;
            }
        }

        /// <summary>
        /// Open Panel
        /// </summary>
        public void OpenPanel()
        {
            settings.OpenPanel();
            OnOpened?.Invoke();
        }

        /// <summary>
        /// Close Panel
        /// </summary>
        public void ClosePanel()
        {
            if (settings)
            {
                settings.ClosePanel();
                OnClosed?.Invoke();
            }
        }

        /// <summary>
        /// Set the parent-child relationship of the panel
        /// </summary>
        /// <param name="parentFunc">Palent's function (Register/UnregisterChildPanel)</param>
        public void LinkParent(UnityAction<Panel> parentFunc)
        {
            parentFunc?.Invoke(settings);
        }

        /// <summary>
        /// Event that is called when back button is pressed
        /// </summary>
        protected virtual void OnClickBack()
        {
            ClosePanel();
            OnBack?.Invoke();
        }

        /// <summary>
        /// Make UI
        /// </summary>
        protected virtual void MakeUI()
        {
            // Execute UI creation functions in defined order
            if (uiOrder != null)
            {
                for (int i = 0; i < uiOrder.Length; i++)
                {
                    uiOrder[i].Invoke();
                }
            }

            settings.AdjustUISize();
        }

        /// <summary>
        /// Wait until setup is complete and then make UI
        /// </summary>
        IEnumerator WaitAndMakeUI()
        {
            // Wait 1 frame to execute after other Start functions have completed
            yield return null;

            if (controllers.Count > 0)
            {
                while (true)
                {
                    bool isFinish = true;
                    for (int i = 0; i < controllers.Count; i++)
                    {
                        if (controllers[i] != null && !controllers[i].FinishedSetup)
                        {
                            isFinish = false;
                            break;
                        }
                    }

                    if (isFinish)
                    {
                        break;
                    }

                    yield return null;
                }
            }

            MakeUI();
        }
    }
}
