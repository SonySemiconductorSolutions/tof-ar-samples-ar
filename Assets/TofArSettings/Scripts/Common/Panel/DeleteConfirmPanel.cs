/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class DeleteConfirmPanel : Panel
    {
        [SerializeField]
        Button btnOk;

        [SerializeField]
        Button btnCancel;

        public UnityEngine.Events.UnityAction<bool> OnConfirmDelete;

        void OnEnable()
        {
            btnOk.onClick.AddListener(Apply);
            btnCancel.onClick.AddListener(Cancel);
        }

        void OnDisable()
        {
            btnOk.onClick.RemoveListener(Apply);
            btnCancel.onClick.RemoveListener(Cancel);
        }

        public void Apply()
        {
            OnConfirmDelete?.Invoke(true);
        }

        public void Cancel()
        {
            OnConfirmDelete?.Invoke(false);
        }
    }
}
