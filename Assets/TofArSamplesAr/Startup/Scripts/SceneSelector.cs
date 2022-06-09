/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections;
using TofAr.V0.Tof;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TofArSamples.Startup
{
    public class SceneSelector : MonoBehaviour
    {
        [SerializeField]
        GameObject btnPrefab = null;

        /// <summary>
        /// Transform of target to display
        /// </summary>
        [SerializeField]
        Transform panel = null;

        [Serializable]
        class SceneData
        {
            /// <summary>
            /// Scene name
            /// </summary>
            public string SceneName;

            /// <summary>
            /// Button display name
            /// </summary>
            public string DisplayName;

            /// <summary>
            /// If the scene requires Tof or not
            /// </summary>
            public bool RequireTof;
        }

        /// <summary>
        /// Scene list
        /// </summary>
        [SerializeField]
        SceneData[] sceneData = new SceneData[0];

        /// <summary>
        /// Event that is called when the scene selection button is available for use
        /// </summary>
        public event UnityAction OnActivated;

        Button[] btns;

        void Start()
        {
            // Create scene button list
            btns = new Button[sceneData.Length];
            for (int i = 0; i < sceneData.Length; i++)
            {
                var data = sceneData[i];

                // Use scene name if display name is empty
                if (data.DisplayName.Length <= 0)
                {
                    data.DisplayName = data.SceneName;
                }

                // Create button
                var obj = Instantiate(btnPrefab, panel);
                var btn = obj.GetComponent<Button>();

                btn.name = i.ToString();
                btn.onClick.AddListener(() =>
                {
                    if (int.TryParse(btn.name, out int index))
                    {
                        SceneManager.LoadScene(sceneData[index].SceneName);
                    }
                });

                var txt = btn.GetComponentInChildren<Text>();
                txt.text = data.DisplayName;

                btns[i] = btn;
            }

            // Check if Tof can be used
            bool tofCheck = TofArTofManager.Instance.CheckDevice();

            StartCoroutine(WaitAndActivate(tofCheck, 0.5f));
        }

        void OnDestroy()
        {
            if (btns != null)
            {
                for (int i = 0; i < btns.Length; i++)
                {
                    if (btns[i])
                    {
                        Destroy(btns[i].gameObject);
                        btns[i] = null;
                    }
                }

                btns = null;
            }
        }

        /// <summary>
        /// Activate buttons after a certain period of time
        /// </summary>
        /// <param name="tofCheck">If Tof can be used or not</param>
        /// <param name="time">Wait time (Unit: s)</param>
        IEnumerator WaitAndActivate(bool tofCheck, float time)
        {
            // Disable immediately after startup
            for (int i = 0; i < sceneData.Length; i++)
            {
                btns[i].interactable = false;
            }

            yield return new WaitForSeconds(time);

            // Reactivate
            for (int i = 0; i < sceneData.Length; i++)
            {
                // Keep disabled if Tof is unavailable
                if (!tofCheck && sceneData[i].RequireTof)
                {
                    continue;
                }

                btns[i].interactable = true;
            }

            OnActivated?.Invoke();
        }
    }
}
