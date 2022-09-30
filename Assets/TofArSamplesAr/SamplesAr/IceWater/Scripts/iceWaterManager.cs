/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace TofArARSamples.IceWater
{
    public class iceWaterManager : MonoBehaviour
    {
        #region AR settings
        [Header("AR settings")]

        [SerializeField]
        [Tooltip("Generated in the beginning")]
        private GameObject anchor;

        [SerializeField]
        private GameObject anchorDestory;

        private GameObject startPoint = null;

        private bool check2create = false;

        [SerializeField]
        [Tooltip("Generated while the CREATE button is clicked")]
        private GameObject scanEffect;

        [SerializeField]
        [Tooltip("Generated while the CREATE button is clicked, the Y axis of it depends on the Raycast position")]
        private GameObject SurfaceWaterPrefab = null;

        private GameObject startPrefab = null;

        private float waterLevel = 0.0f;

        [Range(.01f, .1f)]
        [SerializeField]
        [Tooltip("At each click, the final height of water surface rises to")]
        private float surfaceSpeed = 0.05f;

        [SerializeField]
        private Camera arCamera = null;

        [SerializeField]
        private ARMeshManager arMeshManager = null;

        [SerializeField]
        [Tooltip("At each click, the generated water is saved as the child of this gameoject")]
        private GameObject waterContainer;

        [SerializeField]
        [Tooltip("The ice water prefabs should be attached here, and if multiple forms of water need to be used, you should also edit the variable selector in this script")]
        private GameObject[] waterPrefabs;

        [SerializeField]
        private Slider waterSpeed;

        [SerializeField]
        private LayerMask layersToInclude;

        bool checkMesh;
        #endregion

        #region UI settings
        [Header("UI settings")]
        [SerializeField]
        private GameObject CreateButton;

        [SerializeField]
        private GameObject RestartButton;

        [SerializeField]
        private GameObject SettingOpenButton;

        [SerializeField]
        private GameObject SettingCloseButton;

        [SerializeField]
        private GameObject WelcomePanel;

        [SerializeField]
        private GameObject SettingPanel;

        [SerializeField]
        private GameObject InfoPanel;

        [SerializeField]
        private Toggle iceCubeNumber0;
        [SerializeField]
        private Toggle iceCubeNumber5;
        [SerializeField]
        private Toggle iceCubeNumber10;
        [SerializeField]
        private Toggle iceCubeNumber15;
        [SerializeField]
        private Toggle iceCubeNumber20;
        [SerializeField]
        private Toggle iceCubeNumber30;
        #endregion

        /// <summary>
        /// URPの設定ファイル
        /// </summary>
        public RenderPipelineAsset UrpAsset;

        private void Awake()
        {
#if !UNITY_EDITOR
            SetRenderPipeline(UrpAsset);
#endif
        }

        private void Start()
        {
            SettingPanel.GetComponent<CanvasGroup>().alpha = 0;
            SettingPanel.GetComponent<CanvasGroup>().interactable = false;
            SettingPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;

            InfoPanel.GetComponent<CanvasGroup>().alpha = 0;
            InfoPanel.GetComponent<CanvasGroup>().interactable = false;
            InfoPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;

            CreateButton.GetComponent<Button>().interactable = false;
            CreateButton.GetComponentInChildren<Text>().text = "Computing";

            RestartButton.SetActive(false);
            SettingOpenButton.SetActive(false);
            SettingCloseButton.SetActive(false);

            checkMesh = false;
            startPrefab = null;

            iceCubeNumber0.onValueChanged.AddListener((bool ice) => { onIceCubeNumberChanged(ice); });
            iceCubeNumber5.onValueChanged.AddListener((bool ice) => { onIceCubeNumberChanged(ice); });
            iceCubeNumber10.onValueChanged.AddListener((bool ice) => { onIceCubeNumberChanged(ice); });
            iceCubeNumber15.onValueChanged.AddListener((bool ice) => { onIceCubeNumberChanged(ice); });
            iceCubeNumber20.onValueChanged.AddListener((bool ice) => { onIceCubeNumberChanged(ice); });
            iceCubeNumber30.onValueChanged.AddListener((bool ice) => { onIceCubeNumberChanged(ice); });
        }

        private void Update()
        {
            if (startPrefab != null)
            {
                bool newObjectInstantiated = false;
                waterSpeed.onValueChanged.AddListener((float waterSpeedNumber) => { onWaterSpeedChanged(waterSpeed, waterSpeedNumber); });
                for (int i = 0; i < Input.touchCount; i++)
                {
                    var touch = Input.GetTouch(i);
                    var touchPhase = touch.phase;

                    if (touchPhase == TouchPhase.Began || touchPhase == TouchPhase.Moved)
                    {
                        var ray = arCamera.ScreenPointToRay(touch.position);
                        var hasHit = Physics.Raycast(ray, out var hit, float.PositiveInfinity, layersToInclude);

                        InfoPanel.GetComponent<CanvasGroup>().alpha = 0;
                        InfoPanel.GetComponent<CanvasGroup>().interactable = false;
                        InfoPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;

                        bool isOverUI = IsPointOverUIObject(touch.position);

                        if (!isOverUI && hasHit && waterPrefabs.Length > 0)
                        {
                            GameObject newObject = null;
                            Quaternion newObjectRoation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                            //for multiple water prefabs setting
                            int selector = 0;
                            //int selector = Random.Range(0, waterPrefabs.Length);

                            newObject = GameObject.Instantiate(waterPrefabs[selector], hit.point, newObjectRoation, waterContainer.transform);
                            newObject.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

                            newObjectInstantiated = true;

                            waterLevel += surfaceSpeed;
                            startPrefab.SetActive(true);
                            startPrefab.GetComponent<SliderTransform>().waterLevel += waterLevel;

                            Destroy(newObject, 30);
                        }
                    }
                }
                if (newObjectInstantiated)
                {
                    onIceCubeNumberChanged(true);
                }
            }
            else
            {
                eventStart();
            }
        }

        private bool IsPointOverUIObject(Vector2 touchPoint)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = touchPoint;

            List<RaycastResult> raycastResult = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerEventData, raycastResult);

            return raycastResult.Count > 0;
        }

        private void OnDisable()
        {
            SetRenderPipeline(null);
        }

        private void eventStart()
        {
            var ray = arCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            var hasHit = Physics.Raycast(ray, out var hit, float.PositiveInfinity, layersToInclude);
            Quaternion newObjectRoation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            if (startPrefab == null)
            {
                if (!checkMesh)
                {
                    if (hasHit == false)
                    {
                        CreateButton.GetComponent<Button>().interactable = false;
                        CreateButton.GetComponentInChildren<Text>().text = "Computing";
                    }
                    else
                    {
                        CreateButton.GetComponent<Button>().interactable = true;
                        CreateButton.GetComponentInChildren<Text>().text = "CREATE";
                        checkMesh = true;
                    }
                }
                else
                {
                    if (startPoint == null)
                    {
                        startPoint = Instantiate(anchor, hit.point, newObjectRoation);
                        startPoint.layer = LayerMask.NameToLayer("Ignore Raycast");
                    }
                    else
                    {
                        startPoint.transform.position = hit.point;
                        startPoint.transform.rotation = newObjectRoation;
                    }
                }

            }
            if (check2create)
            {
                GameObject scanStart = Instantiate(scanEffect, startPoint.transform.position, Quaternion.Euler(0, 0, 0));
                startPrefab = Instantiate(SurfaceWaterPrefab, startPoint.transform.position, Quaternion.Euler(0, 0, 0));
                startPrefab.SetActive(false);
                Destroy(startPoint);
                //Destroy(scanEffect, 30);
                GameObject destoryAnchor = Instantiate(anchorDestory, startPoint.transform.position, Quaternion.Euler(0, 0, 0));
                Destroy(CreateButton);
            }
        }

        public void createStartPrefab()
        {
            check2create = true;
            //RestartButton.SetActive(true);
            SettingOpenButton.SetActive(true);
            InfoPanel.GetComponent<CanvasGroup>().alpha = 1;
            InfoPanel.GetComponent<CanvasGroup>().interactable = true;
            InfoPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        public void DestoryWelcomePanel()
        {
            Destroy(WelcomePanel);
        }

        public void SettingPanelOpen()
        {
            SettingPanel.GetComponent<CanvasGroup>().alpha = 1;
            SettingPanel.GetComponent<CanvasGroup>().interactable = true;
            SettingPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
            SettingCloseButton.SetActive(true);
            SettingOpenButton.SetActive(false);


        }
        public void SettingPanelClose()
        {
            SettingPanel.GetComponent<CanvasGroup>().alpha = 0;
            SettingPanel.GetComponent<CanvasGroup>().interactable = false;
            SettingPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
            SettingOpenButton.SetActive(true);
            SettingCloseButton.SetActive(false);
        }

        public void onWaterSpeedChanged(Slider slider, float waterSpeedNumber)
        {
            waterContainer.GetComponentInChildren<ParticleSystem>().gravityModifier = waterSpeed.value;
        }

        public void onIceCubeNumberChanged(bool ice)
        {
            if (iceCubeNumber0.isOn)
            {
                ParticleSystem[] theseParticleSystem;
                theseParticleSystem = waterContainer.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem thisParticleSystem in theseParticleSystem)
                {
                    thisParticleSystem.emissionRate = 0;
                }
            }
            if (iceCubeNumber5.isOn)
            {
                ParticleSystem[] theseParticleSystem;
                theseParticleSystem = waterContainer.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem thisParticleSystem in theseParticleSystem)
                {
                    thisParticleSystem.emissionRate = 5;
                }
            }
            if (iceCubeNumber10.isOn)
            {
                ParticleSystem[] theseParticleSystem;
                theseParticleSystem = waterContainer.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem thisParticleSystem in theseParticleSystem)
                {
                    thisParticleSystem.emissionRate = 10;
                }
            }
            if (iceCubeNumber15.isOn)
            {
                ParticleSystem[] theseParticleSystem;
                theseParticleSystem = waterContainer.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem thisParticleSystem in theseParticleSystem)
                {
                    thisParticleSystem.emissionRate = 15;
                }
            }
            if (iceCubeNumber20.isOn)
            {
                ParticleSystem[] theseParticleSystem;
                theseParticleSystem = waterContainer.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem thisParticleSystem in theseParticleSystem)
                {
                    thisParticleSystem.emissionRate = 20;
                }
            }
            if (iceCubeNumber30.isOn)
            {
                ParticleSystem[] theseParticleSystem;
                theseParticleSystem = waterContainer.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem thisParticleSystem in theseParticleSystem)
                {
                    thisParticleSystem.emissionRate = 30;
                }
            }
        }

        private void SetRenderPipeline(RenderPipelineAsset renderPipelineAsset)
        {
            GraphicsSettings.renderPipelineAsset = renderPipelineAsset;
        }
    }
}
