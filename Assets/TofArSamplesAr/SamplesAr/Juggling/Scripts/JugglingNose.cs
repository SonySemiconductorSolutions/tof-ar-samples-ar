/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TofAr.V0.Face;

namespace TofArARSamples.Juggling
{
    /// <summary>
    /// This class defines the nose object that follows the FaceModel.
    /// </summary>
    public class JugglingNose : MonoBehaviour
    {
        [SerializeField]
        private JugglingFaceController faceController;

        [SerializeField]
        private GameObject noseSphere;

        private Vector3 position;
        private Quaternion rotation;
        private Vector3 noseVertice;

        private MeshRenderer meshRenderer;

        private void LateUpdate()
        {
            UpdateVisibility(faceController.GetFaceResult());

            transform.localPosition = position;
            transform.localRotation = rotation;

            noseSphere.transform.localPosition = noseVertice;
        }

        /// <summary>
        /// displays or hides this object depending on the FaceResult.
        /// </summary>
        /// <param name="result"></param>
        private void UpdateVisibility(FaceResult result)
        {
            if (result == null)
            {
                return;
            }

            var visible = enabled && (result.trackingState != TrackingState.None);

            SetVisible(visible);
        }

        /// <summary>
        /// enables or disables the MeshRenderer.
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
        {
            meshRenderer = noseSphere.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                return;
            }
            meshRenderer.enabled = visible;
        }

        /// <summary>
        /// sets the coordinates of this object.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="noseVertice"></param>
        public void SetPosition(Vector3 position, Quaternion rotation, Vector3 noseVertice)
        {
            this.position = position;
            this.rotation = rotation;
            this.noseVertice = noseVertice;
        }
    }
}
