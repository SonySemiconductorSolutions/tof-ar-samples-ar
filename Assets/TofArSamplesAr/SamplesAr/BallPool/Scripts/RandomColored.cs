/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TofArARSamples.BallPool
{
    public class RandomColored : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer _mesh;
        
        [SerializeField]
        private Color[] _colorList;

        void Start()
        {
            var count = _colorList.Length;
            var random = Random.Range(0, count);
            var mat = _mesh.material;
            mat.color = _colorList[random];
            _mesh.material = mat;
        }

        void Update()
        {
            
        }
    }
}
