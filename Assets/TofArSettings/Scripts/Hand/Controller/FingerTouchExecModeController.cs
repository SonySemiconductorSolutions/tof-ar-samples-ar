/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using TofAr.V0.Hand;
using TensorFlowLite.Runtime;

namespace TofArSettings.Hand
{
    public class FingerTouchExecModeController : ControllerBase
    {
        int index;
        public int Index
        {
            get { return index; }
            set
            {
                if (value != index && 0 <= value && value < ExecModeList.Length)
                {
                    Mode = ExecModeList[value];
                }
            }
        }

        public TFLiteRuntime.ExecMode Mode
        {
            get { return detector.ExecMode; }
            set
            {
                if (value != Mode)
                {
                    index = Utils.Find(value, ExecModeList);
                    detector.ExecMode = value;

                    OnChange?.Invoke(Index);
                }
            }
        }

        public TFLiteRuntime.ExecMode[] ExecModeList { get; private set; }

        public string[] ExecModeNames { get; private set; }

        public event ChangeIndexEvent OnChange;

        FingerTouchDetector detector;

        protected void Awake()
        {
            detector = FindObjectOfType<FingerTouchDetector>();
        }

        protected override void Start()
        {
            // Get Exec Mode list
            ExecModeList = (TFLiteRuntime.ExecMode[])Enum.GetValues(
                typeof(TFLiteRuntime.ExecMode));
            ExecModeNames = new string[ExecModeList.Length];
            for (int i = 0; i < ExecModeList.Length; i++)
            {
                string modeName = ExecModeList[i].ToString();
                ExecModeNames[i] = modeName.Substring(modeName.LastIndexOf('_') + 1);

                // Get initial values of Exec Mode
                if (Mode == ExecModeList[i])
                {
                    index = i;
                }
            }

            base.Start();
        }
    }
}
