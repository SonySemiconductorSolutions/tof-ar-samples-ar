/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022, 2023 Sony Semiconductor Solutions Corporation.
 *
 */

namespace TofArARSamples.SimpleARFoundation
{
    public static class BlurSettings
    {
        private const float defaultValue = 150.0f;

        public const float Min = 0.0f;
        public const float Max = 300.0f;
        public const float Step = 1.0f;

        public static float BlurStrength = defaultValue;

        public static void Init()
        {
            BlurStrength = defaultValue;
        }
    }
}