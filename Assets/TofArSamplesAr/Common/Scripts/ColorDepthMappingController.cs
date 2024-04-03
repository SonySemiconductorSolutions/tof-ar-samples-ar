/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofArSamples.TofComponent;
using TofArSettings;
using UnityEditor;
using UnityEngine;

namespace TofArSamples.ColorDepth
{
    public class ColorDepthMappingController : ControllerBase
    {
        public float Alpha = 0.5f;

        bool remap = false;
        public bool Remap
        {
            get { return remap; }
            set
            {
                if (remap != value)
                {
                    remap = value;
                }
            }
        }

        [SerializeField]
        QuadFitter fitterColor;
        [SerializeField]
        QuadFitter fitterDepth;
        [SerializeField]
        TofFovAdjuster fovAdjuster;

        void OnEnable()
        {
            fovAdjuster.OnChangeFov += OnChangeFov;
        }

        void OnDisable()
        {
            fovAdjuster.OnChangeFov -= OnChangeFov;
        }

        protected override void Start()
        {
            base.Start();
        }

        void OnChangeFov(float fov, float aspect)
        {
            fitterColor.Fitting();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor Debug
        /// </summary>
        [CustomEditor(typeof(ColorDepthMappingController))]
        class ColorDepthMappingControllerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                Instance.Remap = EditorGUILayout.Toggle("Remap", Instance.Remap);
            }

            ColorDepthMappingController Instance
            {
                get { return target as ColorDepthMappingController; }
            }
        }
#endif

    }
}
