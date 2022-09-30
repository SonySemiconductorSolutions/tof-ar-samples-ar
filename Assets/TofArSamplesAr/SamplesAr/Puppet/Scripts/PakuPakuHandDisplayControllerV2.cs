/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArARSamples.Puppet
{
    public class PakuPakuHandDisplayControllerV2 : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hand;
        [SerializeField]
        private SkinnedMeshRenderer _ren;
        [SerializeField]
        private Material _leftMat;
        [SerializeField]
        private Material _rightMat;

        private TofAr.V0.Hand.AbstractHandModel hbr;

        // Start is called before the first frame update
        void Start()
        {
            hbr = GetComponent<TofAr.V0.Hand.AbstractHandModel>();
            
            var mat = hbr.LRHand == TofAr.V0.Hand.HandStatus.LeftHand ? _leftMat : _rightMat;
            _ren.material = mat;
        }

        // Update is called once per frame
        void Update()
        {
            if (hbr.IsHandDetected)
            {
                show();
            }
            else
            {
                hide();
            }
        }
        
        private void show()
        {
            _hand.SetActive(true);
        }
        private void hide()
        {
            _hand.SetActive(false);
        }

    }

}
