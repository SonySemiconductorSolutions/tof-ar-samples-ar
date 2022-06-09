/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArSettings.MarkRecog
{
    public abstract class MarkRecogInfo : MonoBehaviour
    {
        protected MarkRecogManagerController managerController;

        public Vector2 Size
        {
            get { return rt.sizeDelta; }
            set
            {
                rt.sizeDelta = value;
            }
        }

        RectTransform rt;

        protected virtual void Awake()
        {
            rt = GetComponent<RectTransform>();
            managerController = FindObjectOfType<MarkRecogManagerController>();
        }
    }
}
