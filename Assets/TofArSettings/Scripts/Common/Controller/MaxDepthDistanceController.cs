/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArSettings
{
    public class MaxDepthDistanceController : ControllerBase
    {
        private const string preferenceKey = "DepthDisplayDistance";

        public event ChangeIndexEvent OnChangeDistance;

        [SerializeField]
        private Material[] depthViewMaterials;

        [SerializeField]
        private int[] maxDepthDistanceList;

        int maxDepthDistanceIndex;
        public int MaxDepthDistanceIndex
        {
            get { return maxDepthDistanceIndex; }
            set
            {
                if (value != maxDepthDistanceIndex && 0 <= value && value < MaxDepthDistanceList.Length)
                {
                    maxDepthDistanceIndex = value;

                    PlayerPrefs.SetInt(preferenceKey, value);

                    MaxDepthDistance = maxDepthDistanceList[value];
                }
            }
        }

        public int MaxDepthDistance
        {
            get
            {

                return maxDepthDistanceList[maxDepthDistanceIndex];
            }

            private set
            {
                foreach (var d in depthViewMaterials)
                {
                    d.SetFloat("_DistanceMultiplier", 6.0f / value);
                }


                OnChangeDistance?.Invoke(maxDepthDistanceIndex);
                
            }
        }

        public string[] MaxDepthDistanceList {
            get
            {
                string[] distanceList = new string[maxDepthDistanceList.Length];
                for (int i = 0; i < maxDepthDistanceList.Length; i++)
                {
                    distanceList[i] = $"{maxDepthDistanceList[i]}m";
                }
                return distanceList;
            }
        }

        protected override void Start()
        {
            int index = PlayerPrefs.GetInt(preferenceKey, 0);

            MaxDepthDistanceIndex = index;

            base.Start();
        }
    }

}
