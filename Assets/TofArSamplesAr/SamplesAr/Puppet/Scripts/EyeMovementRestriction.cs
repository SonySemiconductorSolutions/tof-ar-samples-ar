/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TofArARSamples.Puppet
{
    public class EyeMovementRestriction : MonoBehaviour
    {
        private readonly float LIMIT_LENGTH = 0.0075f;
        private Vector3 initPosition = default(Vector3);

        void Awake()
        {
            initPosition = gameObject.transform.localPosition;
        }
        void Start()
        {
        }

        void Update()
        {
            var p = gameObject.transform.localPosition - initPosition;
            p = new Vector3(p.x, p.y, 0.0f);
            if (p.magnitude > LIMIT_LENGTH)
            {
                p = p.normalized * LIMIT_LENGTH;
                gameObject.transform.localPosition = p + initPosition;
            }
        }
    }
}
