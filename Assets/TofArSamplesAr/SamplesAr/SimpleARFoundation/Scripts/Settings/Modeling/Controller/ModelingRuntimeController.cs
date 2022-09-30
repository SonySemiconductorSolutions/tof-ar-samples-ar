/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Modeling;
using TofArSettings;

namespace TofArARSamples.SimpleARFoundation
{
    public class ModelingRuntimeController : ControllerBase
    {
        public bool EnableConfidenceCorrection
        {
            get
            {
                return TofArModelingManager.Instance.EnableConfidenceCorrection;
            }

            set
            {
                if (TofArModelingManager.Instance.EnableConfidenceCorrection != value)
                {
                    TofArModelingManager.Instance.EnableConfidenceCorrection = value;
                    OnChangeEnableConfidenceCorrection?.Invoke(value);
                }
            }
        }

        public bool EstimateUpdatedSurface
        {
            get
            {
                return TofArModelingManager.Instance.EstimateUpdatedSurface;
            }

            set
            {
                if (TofArModelingManager.Instance.EstimateUpdatedSurface != value)
                {
                    TofArModelingManager.Instance.EstimateUpdatedSurface = value;
                    OnChangeEstimateUpdatedSurface?.Invoke(value);
                }
            }
        }

        public uint UpdateInterval
        {
            get
            {
                return TofArModelingManager.Instance.UpdateInterval;
            }
            set
            {
                if (TofArModelingManager.Instance.UpdateInterval != value)
                {
                    TofArModelingManager.Instance.UpdateInterval = value;
                    OnChangeUpdateInterval?.Invoke(value);
                }
            }
        }

        public uint EstimateInterval
        {
            get
            {
                return TofArModelingManager.Instance.EstimateInterval;
            }
            set
            {
                if (TofArModelingManager.Instance.EstimateInterval != value)
                {
                    TofArModelingManager.Instance.EstimateInterval = value;
                    OnChangeEstimateInterval?.Invoke(value);
                }
            }
        }

        public float DepthScale
        {
            get
            {
                return TofArModelingManager.Instance.DepthScale;
            }
            set
            {
                if (TofArModelingManager.Instance.DepthScale != value)
                {
                    TofArModelingManager.Instance.DepthScale = value;
                    OnChangeDepthScale?.Invoke(value);
                }
            }
        }

        public ushort ConfidenceCorrectionThreshold
        {
            get
            {
                return TofArModelingManager.Instance.ConfidenceCorrectionThreshold;
            }
            set
            {
                if (TofArModelingManager.Instance.ConfidenceCorrectionThreshold != value)
                {
                    TofArModelingManager.Instance.ConfidenceCorrectionThreshold = value;
                    OnChangeConfidenceCorrectionThreshold?.Invoke(value);
                }
            }
        }


        public event ChangeToggleEvent OnChangeEstimateUpdatedSurface;
        public event ChangeToggleEvent OnChangeEnableConfidenceCorrection;
        public event ChangeValueEvent OnChangeUpdateInterval;
        public event ChangeValueEvent OnChangeEstimateInterval;
        public event ChangeValueEvent OnChangeDepthScale;
        public event ChangeValueEvent OnChangeConfidenceCorrectionThreshold;
    }
}
