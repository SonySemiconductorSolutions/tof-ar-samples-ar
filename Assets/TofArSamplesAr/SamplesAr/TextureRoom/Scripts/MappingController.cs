/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using TofAr.V0.Segmentation.Human;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;

namespace TofArARSamples.TextureRoom
{
    public enum Mode
    {
        TextureAnimation = 0,
        TextInput = 1,
        Stamp = 2
    }

    /// <summary>
    /// Class for configuring things related to mapping
    /// </summary>
    public class MappingController : MonoBehaviour
    {
        [SerializeField]
        private HumanSegmentationDetector humaSegemetationDetector;

        /// <summary>
        /// For mapping materials.
        /// </summary>
        [SerializeField]
        private Material material;

        /// <summary>
        /// rendertexture for inputText.
        /// </summary>
        [SerializeField]
        private RenderTexture textRT;

        /// <summary>
        /// stamp was taken with a different camera and burned into rendertexture
        /// </summary>
        [SerializeField]
        private RenderTexture stampRT;

        [SerializeField]
        private GameObject textInpuUI;

        [SerializeField]
        private GameObject stampUI;

        [SerializeField]
        private StampController stampController;

        private List<Texture2D> textAnimations;
        private Vector4 screenSize;
        private int textureIdx;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private Mode currentMode = Mode.TextureAnimation;

        void Start()
        {
            screenSize = new Vector4(Screen.width, Screen.height, 0.0f, 0.0f);
            Application.targetFrameRate = 20;

            textAnimations = Resources.Load<MappingSettingsScriptableObject>("MappingSettingsValue").animationTextures;
        }

        void Update()
        {
            if (currentMode == Mode.TextureAnimation)
            {
                Texture2D maskTexture = humaSegemetationDetector.MaskTexture;
                material.SetTexture("_OcclusionStencil", maskTexture);
                material.SetVector("_DisplaySize", screenSize);
                material.SetTexture("_MainTexture", textAnimations[textureIdx]);
                material.SetInt("_CurrentMode", 0);
                textureIdx += 1;

                if (textureIdx > textAnimations.Count - 1)
                {
                    textureIdx = 0;
                }
            }
            else if (currentMode == Mode.TextInput)
            {
                material.SetTexture("_MainTexture", textRT);
                material.SetInt("_CurrentMode", 1);
            }
            else if (currentMode == Mode.Stamp)
            {
                material.SetInt("_CurrentMode", 2);
                material.SetTexture("_StampTexture", stampRT);

                /// <summary>
                /// The process detects that the stamp has been tapped
                /// </summary>
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            var hitPos = hit.point;

                            Vector3 pos = hitPos.normalized;
                            float lon = Mathf.Atan2(pos.z, pos.x);
                            float u = lon * (1.0f / 3.14f);
                            u = u * 0.5f + 0.5f;
                            float v = Common.map(hitPos.y, -1.25f, 1.25f, 0, 1f);
                            Vector2 p = new Vector2(u, v);

                            if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                            {
                                stampController.CheckCollisionToStamp(p);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Change mode.Hides unnecessary UI when changing.
        /// </summary>
        public void changeCurrentMode(Mode mode)
        {
            currentMode = mode;
            if (mode == Mode.TextInput)
            {
                textInpuUI.SetActive(true);
            }
            else
            {
                textInpuUI.SetActive(false);
            }
        }

        public Mode getCurrentMode()
        {
            return currentMode;
        }
    }

}
