/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Mesh;

namespace TofArSettings.Mesh
{
    public class ReductionLevelController : ControllerBase
    {
        public int ReductionLevel
        {
            get
            {
                return TofArMeshManager.Instance.MeshReductionLevel;
            }

            set
            {
                if (ReductionLevel != value &&
                    Min <= value && value <= Max)
                {
                    TofArMeshManager.Instance.MeshReductionLevel = value;
                    OnChange?.Invoke(ReductionLevel);
                }
            }
        }

        public const int Min = 0;
        public const int Max = 10;
        public const int Step = 1;

        public event ChangeValueEvent OnChange;
    }
}
