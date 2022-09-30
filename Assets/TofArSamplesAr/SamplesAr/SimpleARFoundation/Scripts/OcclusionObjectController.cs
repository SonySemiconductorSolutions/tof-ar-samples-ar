/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Text;
using TofArSettings;
using UnityEngine;

namespace TofArARSamples.SimpleARFoundation
{
    public class OcclusionObjectController : ControllerBase
    {

        [SerializeField]
        GameObject occlusionCube, occlusionPlane;

        public event ChangeToggleEvent OnChangeCube, OnChangePlane;
        public event ChangeValueEvent OnChangePlaneDistance;

        public bool IsCube
        {
            get => occlusionCube.activeInHierarchy;
            set
            {
                if (value != IsCube)
                {
                    occlusionCube.SetActive(value);
                    OnChangeCube?.Invoke(value);
                }
            }
        }

        public bool IsPlane
        {
            get => occlusionPlane.activeInHierarchy;
            set
            {
                if (value != IsPlane)
                {
                    occlusionPlane.SetActive(value);
                    OnChangePlane?.Invoke(value);
                }
            }
        }

        public float PlaneDistance
        {
            get => occlusionPlane.transform.localPosition.z;
            set
            {
                if (value != occlusionPlane.transform.localPosition.z)
                {
                    var localPos = occlusionPlane.transform.localPosition;
                    localPos.z = value;
                    occlusionPlane.transform.localPosition = localPos;
                    OnChangePlaneDistance?.Invoke(value);
                }
            }
        }
    }
}
