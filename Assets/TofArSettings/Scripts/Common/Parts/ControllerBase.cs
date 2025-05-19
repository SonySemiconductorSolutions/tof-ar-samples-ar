/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings
{
    public class ControllerBase : MonoBehaviour
    {
        public bool FinishedSetup { get; private set; } = false;

        /// <summary>
        /// Event that is called when setup is finished
        /// </summary>
        public UnityAction OnFinishedSetup;

        /// <summary>
        /// Event that is called when array index is changed
        /// </summary>
        /// <param name="index">Index</param>
        public delegate void ChangeIndexEvent(int index);

        /// <summary>
        /// Event that is called when value is changed
        /// </summary>
        /// <param name="val">Value</param>
        public delegate void ChangeValueEvent(float val);

        /// <summary>
        /// Event that is called when array is updated
        /// </summary>
        /// <param name="array">Name list</param>
        /// <param name="index">Index</param>
        public delegate void UpdateArrayEvent(string[] list, int index);

        /// <summary>
        /// Event that is called when toggled
        /// </summary>
        /// <param name="onOff">On/Off</param>
        public delegate void ChangeToggleEvent(bool onOff);

        /// <summary>
        /// Event that is called when vector is changed
        /// </summary>
        /// <param name="vector">Vector</param>
        public delegate void ChangeVectorEvent(Vector3 vector);

        /// <summary>
        /// Event that is called stream error
        /// </summary>
        /// <param name="msg">Message string</param>
        public delegate void StreamErrorEvent(string msg);

        protected virtual void Start()
        {
            FinishedSetup = true;
            OnFinishedSetup?.Invoke();
        }
    }
}
