/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArARSamples.Puppet
{
    public class PakuPakuHandDisplayController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hand;
        [SerializeField]
        private SkinnedMeshRenderer _ren;
        [SerializeField]
        private Color _leftColor = Color.white;
        [SerializeField]
        private Color _rightColor = Color.white;

        private TofAr.V0.Hand.AbstractHandModel hbr;

        // Start is called before the first frame update
        void Start()
        {
            hbr = GetComponent<TofAr.V0.Hand.AbstractHandModel>();
            
            var color = hbr.LRHand == TofAr.V0.Hand.HandStatus.LeftHand ? _leftColor : _rightColor;
            var mat = _ren.material;
            mat.SetColor("_Color", color);
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
