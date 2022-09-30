/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TofArSettings.UI;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class adjusts the threshold with the GUI.
    /// </summary>
    public class JugglingSettings : SettingsBase
    {
        ItemSlider throwRangeSlider;
        ItemSlider catchRangeSlider;
        ItemToggle handToggle;
        ItemToggle faceToggle;
        ItemToggle showTextToggle;

        float throwRangeIndex = 0.05f;
        float catchRangeIndex = 0.1f;

        [SerializeField]
        private JugglingHandController handController;

        [SerializeField]
        private JugglingFaceController faceController;

        [SerializeField]
        private JugglingInformationText informationText;

        protected override void Start()
        {
            PrepareUI();
            base.Start();
        }

        /// <summary>
        /// prepares a GUI object for the setting menu.
        /// </summary>
        protected void PrepareUI()
        {
            var list = new List<UnityAction>();

            list.Add(() =>
            {
                throwRangeSlider = settings.AddItem("Throw Range", 0.01f, 0.5f, 0.01f, throwRangeIndex, ChangeThrowRange);
            });

            list.Add(() =>
            {
                catchRangeSlider = settings.AddItem("Catch Ball Range", 0.01f, 0.5f, 0.01f, catchRangeIndex, ChangeCatchBallRange,-3);
            });

            list.Add(() =>
            {
                handToggle = settings.AddItem("Show Hand Bones", true, ChangeHandToggle);
            });

            list.Add(() =>
            {
                faceToggle = settings.AddItem("Show Face Model", true, ChangeFaceToggle);
            });

            list.Add(() =>
            {
                showTextToggle = settings.AddItem("Show Information Text", informationText.IsTextShowing(), ChangeTextToggle);
            });

            uiOrder = list.ToArray();
        }

        /// <summary>
        /// sets a HandController's catchRange from GUI.
        /// </summary>
        /// <param name="index"></param>
        public void ChangeThrowRange(float index)
        {
            throwRangeIndex = index;

            handController.SetThrowRange(index);
        }

        /// <summary>
        /// sets a HandController's throwRange from GUI.
        /// </summary>
        /// <param name="index"></param>
        public void ChangeCatchBallRange(float index)
        {
            catchRangeIndex = index;

            handController.SetCatchBallRange(index);
        }

        /// <summary>
        /// displays or hides HandModel on screen.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeHandToggle(bool state)
        {
            handController.SetVisible(state);
        }

        /// <summary>
        /// displays or hides FaceModel on screen.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeFaceToggle(bool state)
        {
            faceController.SetVisible(state);
        }

        /// <summary>
        /// displays or hides information text on screen.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeTextToggle(bool state)
        {
            informationText.ShowText(state);
        }

        /// <summary>
        /// creates toggles and sliders instance.
        /// </summary>
        protected override void MakeUI()
        {
            base.MakeUI();
        }
    }
}
