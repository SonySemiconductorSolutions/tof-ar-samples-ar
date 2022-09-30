/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using TofArSettings;
using TofArSettings.UI;
using UnityEngine;
using UnityEngine.Events;
using UI = TofArSettings.UI;

namespace TofArARSamples.SimpleARFoundation
{
    public class MeshViewSettings : UI.SettingsBase
    {
        //MeshMaterialController meshMaterialController;
        OcclusionObjectController occlusionObjectController;

        [SerializeField]
        bool material = true;

        /// <summary>
        /// Toggle occlusion
        /// </summary>
        [SerializeField]
        bool occlusion = true;

        UI.ItemToggle itemCube, itemPlane, itemHandOcclusion;
        UI.ItemSlider itemClippingDistance, itemPlaneDistance;
        UI.ItemDropdown itemMaterials;

        List<UnityAction> list;

        [System.Serializable]
        public class AddControllersEvent : UnityEvent<SettingsPanel> { }
        AddControllersEvent addControllersEvent = new AddControllersEvent();

        private void Awake()
        {
           /*meshMaterialController = GetComponent<MeshMaterialController>();
            meshMaterialController.enabled = material;*/
            occlusionObjectController = GetComponent<OcclusionObjectController>();
            occlusionObjectController.enabled = occlusion;
        }

        protected override void Start()
        {
            PrepareUI();
            base.Start();
        }

        private void PrepareUI()
        {
            list = new List<UnityAction>();

            if (occlusion)
            {
                list.Add(MakeUIOcclusion);
                controllers.Add(occlusionObjectController);
            }

            // Set UI order
            
            addControllersEvent.Invoke(settings);

            uiOrder = list.ToArray();
        }

        /// <summary>
        /// Make MeshMaterial UI
        /// </summary>
        /*void MakeUIMaterial()
        {
            itemMaterials = settings.AddItem("Mesh material", meshMaterialController.MaterialNames,
                meshMaterialController.Index, ChangeMeshMaterial);

            meshMaterialController.OnChangeIndex += (index) =>
            {
                itemMaterials.Index = index;
            };
        }*/

        /// <summary>
        /// Change material of mesh
        /// </summary>
        /// <param name="index"></param>
        /*void ChangeMeshMaterial(int index)
        {
            meshMaterialController.Index = index;
        }*/

        /// <summary>
        /// Make UI for occlusion object
        /// </summary>
        void MakeUIOcclusion()
        {
            itemCube = settings.AddItem("Occlusion Test Cube", occlusionObjectController.IsCube,
                ChangeCube);

            occlusionObjectController.OnChangeCube += (onOff) =>
            {
                itemCube.OnOff = onOff;
            };

            ChangeCube(occlusionObjectController.IsCube);

            itemPlane = settings.AddItem("Occlusion Test Plane", occlusionObjectController.IsPlane,
                ChangePlane);
            occlusionObjectController.OnChangePlane += (onOff) =>
            {
                itemPlane.OnOff = onOff;
                itemPlaneDistance.Interactable = onOff;
            };

            itemPlaneDistance = settings.AddItem("Occlusion Plane\nDistance", 0.5f, 3.0f, 0.1f, occlusionObjectController.PlaneDistance,
                ChangePlaneDistance, -1);
            occlusionObjectController.OnChangePlaneDistance += (val) =>
            {
                itemPlaneDistance.Value = val;
            };

            itemPlaneDistance.Interactable = occlusionObjectController.IsPlane;
        }

        /// <summary>
        /// Toggle cube display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangeCube(bool onOff)
        {
            occlusionObjectController.IsCube = onOff;
        }

        /// <summary>
        /// Toggle plane display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangePlane(bool onOff)
        {
            occlusionObjectController.IsPlane = onOff;
        }

        /// <summary>
        /// Toggle plane display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangePlaneDistance(float dist)
        {
            occlusionObjectController.PlaneDistance = dist;
        }

        /// <summary>
        /// Controller registration event
        /// </summary>
        /// <param name="unityAction">Argumentless delegate</param>
        public void AddListenerAddControllersEvent(UnityAction<SettingsPanel> unityAction)
        {
            addControllersEvent.AddListener(unityAction);
        }

        /// <summary>
        /// Register to UIOrderList
        /// </summary>
        /// <param name="controller">RecordController</param>
        public void SetUIOrderList(UnityAction unityAction)
        {
            list.Add(unityAction);
        }

        /// <summary>
        /// Register to Controller
        /// </summary>
        /// <param name="controller">ControllerBase</param>
        public void SetController(ControllerBase controller)
        {
            controllers.Add(controller);
        }

        /// <summary>
        /// Get settings
        /// </summary>
        /// <returns></returns>
        public SettingsPanel GetSettings()
        {
            return settings;
        }
    }
}
