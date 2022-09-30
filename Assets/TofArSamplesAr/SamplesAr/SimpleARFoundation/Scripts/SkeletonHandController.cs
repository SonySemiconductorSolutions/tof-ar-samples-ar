/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using TofArSettings;

namespace TofArARSamples.SimpleARFoundation
{
    public class SkeletonHandController : ControllerBase
    {
        public bool IsShow
        {
            get
            {
                bool isShow = false;
                for (int i = 0; i < handModels.Length; i++)
                {
                    if (handModels[i].enabled)
                    {
                        isShow = true;
                        break;
                    }
                }

                return isShow;
            }

            set
            {
                if (IsShow != value)
                {
                    for (int i = 0; i < handModels.Length; i++)
                    {
                        handModels[i].enabled = value;
                    }

                    OnChangeShow?.Invoke(IsShow);
                }
            }
        }

        public event ChangeToggleEvent OnChangeShow;

        HandModel[] handModels;

        protected override void Start()
        {
            handModels = FindObjectsOfType<HandModel>();

            base.Start();
        }


    }
}
