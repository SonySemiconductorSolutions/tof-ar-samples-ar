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
    public class HumanCollider : MonoBehaviour
    {
        private Renderer r;
            
        public delegate void HumanColliderDelegate(HumanCollider humanCollider);
        public HumanColliderDelegate humanColliderDelegate;
        
        const int planeLayer = 7;
        
        private void Awake() 
        {
            r = GetComponent<Renderer>();
        }
        
        private void OnTriggerStay(Collider other) 
        {
            if (other.gameObject.layer != planeLayer) { return; }
            humanColliderDelegate?.Invoke(this);
        }
        
        public void Show(bool visible)
        {
            r.enabled = visible;
        }
    }
}
