/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class manages the UI of the Juggling scene.
    /// </summary>
    public class JugglingUIManager : MonoBehaviour
    {
        [SerializeField]
        private JugglingAppController appController;

        [SerializeField]
        private JugglingMeasureMenu measureMenu;

        [SerializeField]
        private JugglingReadyMenu readyMenu;

        [SerializeField]
        private JugglingScoreMenu scoreMenu;

        [SerializeField]
        private JugglingResultMenu resultMenu;

        private void Awake()
        {
            appController.OnJugglingReady += OnJugglingReady;
            appController.OnJugglingStarted += OnJugglingStarted;
            appController.OnJugglingFinished += OnJugglingFinihed;
        }

        private void OnDestroy()
        {
            appController.OnJugglingReady -= OnJugglingReady;
            appController.OnJugglingStarted -= OnJugglingStarted;
            appController.OnJugglingFinished -= OnJugglingFinihed;
        }

        private void Start()
        {
            measureMenu.OpenMenu();
        }

        /// <summary>
        /// called when the juggling is ready.
        /// </summary>
        private void OnJugglingReady()
        {
            measureMenu.CloseMenu();

            StartCoroutine(ShowReadyMenu());
        }

        /// <summary>
        /// called when the juggling is started.
        /// </summary>
        private void OnJugglingStarted()
        {
            scoreMenu.OpenMenu();
        }

        /// <summary>
        /// called when the juggling is finished.
        /// </summary>
        private void OnJugglingFinihed()
        {
            scoreMenu.CloseMenu();
            resultMenu.OpenMenu();
        }

        /// <summary>
        /// retries the juggling.
        /// </summary>
        public void Retry()
        {
            resultMenu.CloseMenu();
            measureMenu.OpenMenu();

            appController.Retry();
        }

        /// <summary>
        /// displays "READY" text during a second.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowReadyMenu()
        {
            readyMenu.OpenMenu();

            yield return new WaitForSeconds(1.0f);

            readyMenu.CloseMenu();

            appController.StartJuggling();
        }
    }
}
