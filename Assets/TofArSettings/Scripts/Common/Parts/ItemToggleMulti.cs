/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System;

namespace TofArSettings.UI
{
    public class ItemToggleMulti : Item
    {
        public override bool Interactable
        {
            get
            {
                return GetInteractable(0);
            }

            set
            {
                for (int i = 0; i < itemTgls.Length; i++)
                {
                    SetInteractable(i, value);
                }
            }
        }

        public bool OnOff
        {
            set
            {
                for (int i = 0; i < itemTgls.Length; i++)
                {
                    SetOnOff(i, value);
                }
            }
        }

        /// <summary>
        /// Event that is called when toggle status is changed
        /// </summary>
        /// <param name="index">Toggle group index</param>
        /// <param name="onOff">On/Off</param>
        public delegate void ChangeEvent(int index, bool onOff);

        public event ChangeEvent OnChange;

        ItemToggle[] itemTgls = new ItemToggle[1];

        protected override void Awake()
        {
            if (finishedSetup)
            {
                return;
            }

            base.Awake();

            // Get UI
            itemTgls[0] = GetComponentInChildren<ItemToggle>();
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="title">Title of each toggle</param>
        /// <param name="relativeFontSize">Title font size (relative) of each toggle</param>
        /// <param name="fixedTitleWidth">Title fixed width of each toggle</param>
        /// <param name="onOff">Status of each toggle</param>
        /// <param name="onChange">Event that is called when any of the toggles are pressed</param>
        public void Init(string[] title, int[] relativeFontSize,
            float[] fixedTitleWidth, bool[] onOff, ChangeEvent onChange)
        {
            base.Init(string.Empty, 0, 0);

            if (title.Length != itemTgls.Length)
            {
                // Remove excess if the number has been reduced
                for (int i = title.Length; i < itemTgls.Length; i++)
                {
                    Destroy(itemTgls[i].gameObject);
                    itemTgls[i] = null;
                }

                Array.Resize(ref itemTgls, title.Length);
            }

            int baseRelativeFontSize = 0;
            for (int i = 0; i < itemTgls.Length; i++)
            {
                if (!itemTgls[i])
                {
                    var obj = Instantiate(itemTgls[0].gameObject, transform);
                    itemTgls[i] = obj.GetComponent<ItemToggle>();
                }

                // To prevent the second and subsequent sizes from changing, the first value is stored
                int font = (relativeFontSize == null) ? 0 : relativeFontSize[i];
                if (i <= 0)
                {
                    baseRelativeFontSize = font;
                }
                else
                {
                    font = baseRelativeFontSize - font;
                }

                float fix = (fixedTitleWidth == null) ? 0 : fixedTitleWidth[i];

                itemTgls[i].Init(title[i], font, fix, onOff[i], i,
                    (index, onOff) =>
                    {
                        OnChange?.Invoke(index, onOff);
                    });
            }

            OnChange = onChange;
        }

        /// <summary>
        /// Toggle button status (individual)
        /// </summary>
        /// <param name="index">Button index</param>
        /// <param name="onOff">Button status</param>
        public void SetOnOff(int index, bool onOff)
        {
            if (CheckRange(index))
            {
                itemTgls[index].OnOff = onOff;
            }
        }

        /// <summary>
        /// Get button status (individual)
        /// </summary>
        /// <param name="index">Button index</param>
        /// <returns>Button status</returns>
        public bool GetOnOff(int index)
        {
            if (CheckRange(index))
            {
                return itemTgls[index].OnOff;
            }

            return false;
        }

        /// <summary>
        /// Set UI interactability (individual)
        /// </summary>
        /// <param name="index">Button index</param>
        /// <param name="onOff">Interactable/non-interactable</param>
        public void SetInteractable(int index, bool onOff)
        {
            if (CheckRange(index))
            {
                itemTgls[index].Interactable = onOff;
            }
        }

        /// <summary>
        /// Get UI interactability status (individual)
        /// </summary>
        /// <param name="index">Button index</param>
        /// <returns>Interactable/non-interactable</returns>
        public bool GetInteractable(int index)
        {
            if (CheckRange(index))
            {
                return itemTgls[index].Interactable;
            }

            return false;
        }

        /// <summary>
        /// Check index range
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Within/Outside range</returns>
        bool CheckRange(int index)
        {
            return (0 <= index && index < itemTgls.Length);
        }
    }
}
