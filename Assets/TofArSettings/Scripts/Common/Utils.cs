/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;

namespace TofArSettings
{
    public static class Utils
    {
        /// <summary>
        /// Search for value in array and return it's index
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="val">Value to search for</param>
        /// <param name="array">Array to search</param>
        /// <param name="startIndex">Index to start searching from</param>
        /// <returns>Index of value</returns>
        public static int Find<T>(T val, T[] array, int startIndex = 0)
            where T : Enum
        {
            int index = -1;
            for (int i = startIndex; i < array.Length; i++)
            {
                if (val.CompareTo(array[i]) == 0)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }
    }
}
