/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace TofArARSamples.TextureRoom
{
    /// <summary>
    /// This class is about "Stamp".
    /// To register events to the slider gui and adjust the properties of stamp.
    /// To get the collision between the tapped point and the stamp.
    /// Ability to retrieve photos for stamps from the camera roll
    /// </summary>
    public class StampController : MonoBehaviour
    {
        [SerializeField]
        private GameObject stampObj;

        [SerializeField]
        private Camera stampCamera;

        [SerializeField]
        private Transform parent;

        [SerializeField]
        private GameObject stampUI;

        private GameObject position_x_Slider;
        private GameObject position_x_InputField;
        private GameObject position_y_Slider;
        private GameObject position_y_InputField;
        private GameObject scaleSlider;
        private GameObject scaleInputField;
        private GameObject rotationSlider;
        private GameObject rotationInputField;
        private GameObject panel;
        private List<GameObject> stamps;
        private float curret_x = 0;
        private float curret_y = 0;
        private float curret_scale = 1f;
        private float curret_rotation = 0;
        private Transform changingStamp;
        private Vector3 originalPosition;

        static int stampEffectNumber = 0;

        void Start()
        {
            stamps = new List<GameObject>();

            position_x_Slider = stampUI.transform.Find("position_X_Slider").gameObject;
            position_x_InputField = stampUI.transform.Find("position_X_InputField").gameObject;
            position_y_Slider = stampUI.transform.Find("position_Y_Slider").gameObject;
            position_y_InputField = stampUI.transform.Find("position_Y_InputField").gameObject;
            scaleSlider = stampUI.transform.Find("scale_Slider").gameObject;
            scaleInputField = stampUI.transform.Find("scale_InputField").gameObject;
            rotationSlider = stampUI.transform.Find("rotation_Slider").gameObject;
            rotationInputField = stampUI.transform.Find("rotation_InputField").gameObject;
        }

        void UpdatePositionX(float value)
        {
            if (curret_x != value)
            {
                value = Common.map(value, 0, 1f, -20f, 20f);
                var pos = changingStamp.localPosition;
                pos.x = value;
                changingStamp.localPosition = pos;
                curret_x = value;
            }
            else
            {
                return;
            }
        }

        void UpdatePositionY(float value)
        {
            if (curret_y != value)
            {
                value = Common.map(value, 0, 1f, -4f, 4f);
                var pos = changingStamp.localPosition;
                pos.y = value;
                changingStamp.localPosition = pos;
                curret_y = value;
            }
            else
            {
                return;
            }
        }

        void UpdateScale(float value)
        {
            if (curret_scale != value)
            {
                value *= 3f;
                changingStamp.localScale = new Vector3(value, value, value);
                curret_scale = value;
            }
            else
            {
                return;
            }
        }

        void UpdateRotation(float value)
        {
            if (curret_rotation != value)
            {
                value *= 360f;
                var rotation = changingStamp.localRotation;
                var v = value - curret_rotation;
                rotation = Quaternion.AngleAxis(value, changingStamp.forward);
                changingStamp.localRotation = rotation;
                curret_scale = value;
            }
            else
            {
                return;
            }
        }

        public void Init()
        {
            for (int i = 0; i < stamps.Count; i++)
            {
                Destroy(stamps[i]);
            }
            stamps.Clear();
        }

        public void PickImage(int maxSize)
        {
            NativeGallery.Permission permission = NativeGallery.GetImagesFromGallery((paths) =>
           {
               if (paths != null)
               {
                   for (int i = 0; i < paths.Length; i++)
                   {
                       var obj = GameObject.Instantiate(stampObj);
                       obj.transform.parent = parent;
                       if (stamps.Count == 0)
                       {
                           obj.transform.localPosition = Vector3.zero;
                       }
                       else
                       {
                           obj.transform.localPosition = new Vector3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-4f, 4f), 0);
                       }

                       stamps.Add(obj);

                       Texture2D texture = NativeGallery.LoadImageAtPath(paths[i], maxSize);
                       if (texture == null)
                       {
                           return;
                       }

                       obj.GetComponent<StampEffectController>().InitStamp(texture, stampEffectNumber);
                   }
               }
           });
        }

        public void CheckCollisionToStamp(Vector2 pos)
        {
            var viewpos = new Vector3(pos.x, pos.y, 0);
            var ray = stampCamera.ViewportPointToRay(viewpos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                stampUI.SetActive(true);

                changingStamp = hit.transform;

                position_x_Slider.GetComponent<Slider>().value = Common.map(changingStamp.localPosition.x, -20f, 20f, 0, 1f);
                position_y_Slider.GetComponent<Slider>().value = Common.map(changingStamp.localPosition.y, -4f, 4f, 0, 1f);
                scaleSlider.GetComponent<Slider>().value = 0.5f;
                curret_x = changingStamp.localPosition.x;
                curret_y = changingStamp.localPosition.y;
                curret_scale = changingStamp.localScale.x;

                position_x_Slider.GetComponent<Slider>().onValueChanged.RemoveAllListeners();
                position_y_Slider.GetComponent<Slider>().onValueChanged.RemoveAllListeners();
                scaleSlider.GetComponent<Slider>().onValueChanged.RemoveAllListeners();
                rotationSlider.GetComponent<Slider>().onValueChanged.RemoveAllListeners();

                position_x_Slider.GetComponent<Slider>().onValueChanged.AddListener(UpdatePositionX);
                position_y_Slider.GetComponent<Slider>().onValueChanged.AddListener(UpdatePositionY);
                scaleSlider.GetComponent<Slider>().onValueChanged.AddListener(UpdateScale);
                rotationSlider.GetComponent<Slider>().onValueChanged.AddListener(UpdateRotation);
            }
            else
            {
                stampUI.SetActive(false);
            }
        }
    }
}
