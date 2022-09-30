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
    public class ARBall : MonoBehaviour
    {
        public Rigidbody rb;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.position.z < -2)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
